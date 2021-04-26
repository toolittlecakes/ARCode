using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

namespace ARLens
{

    // В будущем можнос делать это класс не MonoBehaviour, и уж точно ему не нужен отдельный GameObject.
    //  Пока не получается изза StartCourutine
    public class ARSceneManager : MonoBehaviour
    {
        public GameObject waitScene;
        public GameObject sceneHolderPrefab;

        private GameObject sceneHolder = null;
        private ARCodeTracker arCodeTracker;

        void Start()
        {
            arCodeTracker = GameObject.FindObjectOfType<ARCodeTracker>();
            arCodeTracker.arCodeFound.AddListener(OnARCodeFound);
            arCodeTracker.arCodeLost.AddListener(OnARCodeLost);
        }

        void OnARCodeFound()
        {
            var arcode = arCodeTracker.arCode;
            string code = arcode.GetName();

            if (!code.ToLower().StartsWith("https://arcode"))
            {
                return;
            }

         
            sceneHolder = CreateSceneHolder(Instantiate(waitScene));
            StartCoroutine(DownloadScene(code));

        }

        void OnARCodeLost()
        {
            Debug.Log("DELETE HOLDER");
            // arCodeTracker.arCodeUpdate.RemoveListener(sceneHolder.GetComponent<ARSceneHolder>().SetPose);

            Destroy(sceneHolder);
            sceneHolder = null;
        }

        IEnumerator DownloadScene(string code)
        {
            string[] args = code.Split();

            var downloader = ScriptableObject.CreateInstance<ARSceneDownloader>();
            yield return downloader.Download(args);
            if (sceneHolder)
            {
                Destroy(sceneHolder);
                sceneHolder = CreateSceneHolder(downloader.result);
            }
            Destroy(downloader);
        }

        GameObject CreateSceneHolder(GameObject scene, Transform parent = null)
        {
            var pose = arCodeTracker.arCode.GetPose();
            var holder = Instantiate(sceneHolderPrefab, parent);
            scene.transform.SetParent(holder.transform);
            scene.SetActive(true);

            holder.transform.SetPositionAndRotation(
                pose.GetColumn(3),
                pose.rotation
            );
            
            holder.GetComponent<ARSceneHolder>().SetPose(pose);
       
            arCodeTracker.arCodeUpdate.AddListener(holder.GetComponent<ARSceneHolder>().SetPose);
            return holder;
        }
    }
}
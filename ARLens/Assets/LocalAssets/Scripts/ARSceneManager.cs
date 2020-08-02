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

        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

        private ARCodeTracker arCodeTracker;

        void Start()
        {
            arCodeTracker = GameObject.FindObjectOfType<ARCodeTracker>();
            arCodeTracker.ARCodeFound.AddListener(OnARCodeFound);
            arCodeTracker.ARCodeLost.AddListener(OnARCodeLost);
        }



        void OnARCodeFound()
        {
            var arcode = arCodeTracker.ARCode();
            string code = arcode.GetName();

            // if (!code.ToLower().StartsWith("https://arcode"))
            // {
            // return;
            // }

            // if (sceneInstant == null)
            // {
                if (cache.ContainsKey(code))
                {
                    Debug.Log("get from cache");
                    var cached = GameObject.Instantiate(cache[code]);
                    cached.SetActive(true);
                    sceneHolder = CreateSceneHolder(cached);
                }
                else
                {
                    sceneHolder = CreateSceneHolder(GameObject.Instantiate(waitScene));
                    // StartCoroutine(DownloadScene(code));
                    StartCoroutine(GetAssetBundle(code, code));
                }
            // }

         }


        void OnARCodeLost()
        {
            Debug.Log("DELETE HOLDER");
            // arCodeTracker.ARCodeUpdate.RemoveListener(sceneHolder.SetPose);
           
            GameObject.Destroy(sceneHolder.gameObject);
            sceneHolder = null;
        }


        IEnumerator DownloadScene(string code)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(code))
            {
                yield return webRequest.SendWebRequest();

                string realUrl = webRequest.downloadHandler.text;


                Debug.Log("code: " + code);
                Debug.Log("url: " + realUrl);

                yield return GetAssetBundle(realUrl, code);
            }
        }

        IEnumerator GetAssetBundle(string realUrl, string code)
        {
            ARSceneDownloader downloader = ScriptableObject.CreateInstance<ARSceneDownloader>();
            yield return downloader.Download(realUrl);
            yield return new WaitForSeconds(3f);

            GameObject.Destroy(sceneHolder);

            var scene = downloader.result;
            
            sceneHolder = CreateSceneHolder(GameObject.Instantiate(scene));
            
            cache[code] = scene;
            cache[code].SetActive(false);

            GameObject.Destroy(downloader);
        }

        GameObject CreateSceneHolder(GameObject scene, Transform parent = null)
        {
            var holder = GameObject.Instantiate(sceneHolderPrefab, parent);
            scene.transform.SetParent(holder.transform);

            arCodeTracker.ARCodeUpdate.AddListener(holder.GetComponent<ARSceneHolder>().SetPose);

            return holder;
            
        }
    }
}
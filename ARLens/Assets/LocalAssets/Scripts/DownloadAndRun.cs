using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ARLens
{
    public class DownloadAndRun : MonoBehaviour
    {
        public bool isNoisy;
        public bool isStabilizated;
        public float wait;
        public string url;
        
        private GameObject scene = null;
        private Vector3 lastPosition;
        private Vector3 speed = new Vector3(0,0,0);        
        IEnumerator Start()
        {
            if (wait != 0f)
            {
                yield return new WaitForSeconds(wait);                
            }
            var downloader = ScriptableObject.CreateInstance<ARSceneDownloader>();
            yield return downloader.Download(url);

            scene = downloader.result;
            scene.transform.SetParent(transform);

            Destroy(downloader);

            lastPosition = scene.transform.position;
            Debug.Log(lastPosition);
        }

        void Update()
        {

            // SetFromToRotation(rndVector, new Vector3(-1,0,0));

            if (scene)
            {

                
                scene.transform.position = transform.TransformPoint(new Vector3(Mathf.Sin(Time.time),Mathf.Sin(Time.time),Mathf.Cos(Time.time)));
                var s = new Quaternion();
                s.SetLookRotation(new Vector3(Mathf.Sin(Time.time),Mathf.Sin(Time.time) ,Mathf.Cos(Time.time)));
                
                scene.transform.rotation = s;
                if (isNoisy)
                {
                    ApplyNoise();
                }
                if(isStabilizated)
                {
                    var newPosition = lastPosition + speed * Time.deltaTime;
                    speed =  speed + (scene.transform.position - lastPosition) * 0.01f;
                    scene.transform.position = (newPosition);
                    lastPosition = scene.transform.position;

                }
            }
        }

        void ApplyNoise()
        {
            float scale = 0.05f;
            var rndVector = new Vector3(Random.value, Random.value, Random.value) * scale;
            var rndVector2 = new Vector3(Random.value, Random.value, Random.value ) * scale;
            Debug.Log(rndVector2);
            var rndQuaternion = new Quaternion();
            rndQuaternion.SetLookRotation(rndVector2);
            

            scene.transform.Translate(rndVector);
            // scene.transform.rotation = Quaternion.Slerp(scene.transform.rotation, rndQuaternion, 0.02f);

        }
        // void Start()
        // {
        //     var bytes = System.IO.File.ReadAllBytes("C:/Users/sne/UnityProjects/ExternalCodeLauncher/Assets/Prefabs/SinMotion.bytes");
        //     var assembly = System.Reflection.Assembly.Load(bytes);
        //     var type = assembly.GetType("SinMotion");
        //     Debug.Log(type);
        //     // Instantiate a GameObject and add a component with the loaded class
        //     gameObject.AddComponent(type);
        // }

    }
}
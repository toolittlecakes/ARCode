using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

using maxstAR;

namespace LensAR
{
    public class ArCodeTrackerSample : ARBehaviour
    {
        public GameObject WaitScene;

        private GameObject sceneInstant = null;

        private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;

        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

        void Awake()
        {
            Init();

            cameraBackgroundBehaviour = FindObjectOfType<CameraBackgroundBehaviour>();
            if (cameraBackgroundBehaviour == null)
            {
                Debug.LogError("Can't find CameraBackgroundBehaviour.");
                return;
            }

        }

        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            // TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.JITTER_REDUCTION_DEACTIVATION);
            // TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.MULTI_TRACKING);
            TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_QR_TRACKER);
            StartCamera();
        }


        void Update()
        {

            TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

            if (state == null)
            {
                return;
            }
            cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

            TrackingResult trackingResult = state.GetTrackingResult();

            if (trackingResult.GetCount() > 0)
            {
                OnArCodeFound(trackingResult.GetTrackable(0)); // max 1 qrcode
            }
            else if (sceneInstant != null)
            {
                OnArCodeLost();
            }
        }


        void OnArCodeFound(Trackable marker)
        {

            string code = marker.GetName();

            if (!code.ToLower().StartsWith("https://arcode"))
            {
                return;
            }

            if (sceneInstant == null)
            {
                if (cache.ContainsKey(code))
                {
                    sceneInstant = Instantiate(cache[code], transform);
                    sceneInstant.SetActive(true);
                }
                else
                {
                    sceneInstant = Instantiate(WaitScene, transform);
                    StartCoroutine(DownloadScene(code));
                }
            }

            var poseMatrix = marker.GetPose();
            var instantiatedTransfrom = sceneInstant.GetComponent<Transform>();
            instantiatedTransfrom.position = MatrixUtils.PositionFromMatrix(poseMatrix);
            instantiatedTransfrom.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);
        }


        void OnArCodeLost()
        {
            Destroy(sceneInstant);
            sceneInstant = null;
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
            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(realUrl))
            {
                yield return webRequest.SendWebRequest();
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                var loadMapAsset = bundle.LoadAssetAsync("Map.json");
                var loadScriptsAsset = bundle.LoadAssetAsync("Scripts.bytes");
                var loadSceneAsset = bundle.LoadAssetAsync("Scene.prefab");
                yield return loadMapAsset;
                yield return loadScriptsAsset;
                yield return loadSceneAsset;
                bundle.Unload(false);

                TextAsset map = loadMapAsset.allAssets[0] as TextAsset;
                TextAsset scripts = loadScriptsAsset.allAssets[0] as TextAsset;
                GameObject scene = loadSceneAsset.allAssets[0] as GameObject;

                var assembly = System.Reflection.Assembly.Load(scripts.bytes);

                var types = assembly.GetTypes();
                var Map = JsonConvert.DeserializeObject<Dictionary<string, string>>(map.text);
               
                Destroy(sceneInstant);
                sceneInstant = Instantiate(scene, transform) as GameObject;

                GameObject ARCodePlaceholder = GameObject.Find("ARCodePlaceholder");
                ARCodePlaceholder.transform.SetParent(transform);
                sceneInstant.transform.SetParent(ARCodePlaceholder.transform);
                sceneInstant = ARCodePlaceholder;



                foreach(var item in Map) 
                {
                    Debug.Log(assembly.GetTypes()[0]);
                    Debug.Log(item.Key);
                    Debug.Log(item.Value);
                    GameObject obj = GameObject.Find(item.Key);
                    obj.AddComponent(assembly.GetType(item.Value));
                }
                cache[code] = Instantiate(sceneInstant);
                cache[code].SetActive(false);

                // var cached = Instantiate(sceneInstant);
                // cached.GetComponent<Renderer>().;
            }
        }

        void SetScene(Object scene)
        {
            sceneInstant = Instantiate(scene, transform) as GameObject;

            GameObject ARCodePlaceholder = GameObject.Find("ARCodePlaceholder");
            ARCodePlaceholder.transform.SetParent(transform);

            sceneInstant.transform.SetParent(ARCodePlaceholder.transform);
            sceneInstant = ARCodePlaceholder;
            sceneInstant.SetActive(false);

        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                TrackerManager.GetInstance().StopTracker();
                StopCamera();
            }
            else
            {
                StartCamera();
                TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_QR_TRACKER);
            }
        }


        void OnDestroy()
        {
            TrackerManager.GetInstance().StopTracker();
            TrackerManager.GetInstance().DestroyTracker();
            StopCamera();

        }
    }
}
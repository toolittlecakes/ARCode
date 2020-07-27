using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

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
            sceneInstant = Instantiate(WaitScene, transform);
            Destroy(sceneInstant);
            sceneInstant = null;
            StartCoroutine(Warmup());

        }
        private IEnumerator Warmup()
        {
            yield return new WaitForSeconds(0.1f);
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

            // if (!code.ToLower().StartsWith("https://arcode"))
            // {
                // return;
            // }

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
                    // StartCoroutine(DownloadScene(code));
                    StartCoroutine(GetAssetBundle(code, code));
                }
            }

            var instantiatedTransfrom = sceneInstant.GetComponent<Transform>();
            var poseMatrix = marker.GetPose();
            instantiatedTransfrom.position = (MatrixUtils.PositionFromMatrix(poseMatrix) + instantiatedTransfrom.position) / 2;
            instantiatedTransfrom.rotation = Quaternion.Lerp(MatrixUtils.QuaternionFromMatrix(poseMatrix), 
            instantiatedTransfrom.rotation, 0.5f);

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
                Debug.Log("Getting asset from: "+ realUrl);
                yield return webRequest.SendWebRequest();
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                var loadDlls = bundle.LoadAllAssetsAsync<TextAsset>();
                yield return loadDlls;

                foreach (TextAsset dll in loadDlls.allAssets)
                {
                    System.Reflection.Assembly.Load(dll.bytes);
                }


                var loadScenes = bundle.LoadAllAssetsAsync<GameObject>();
                yield return loadScenes;

                bundle.Unload(true);

                Destroy(sceneInstant);
                foreach (GameObject scene in loadScenes.allAssets) // TODO: fix (works only with one)
                {
                    sceneInstant = Instantiate(scene, transform);
                    Transform ARCodePlaceholder = sceneInstant.transform.Find("ARCodePlaceholder");
                    ARCodePlaceholder.SetParent(transform);
                    sceneInstant.transform.SetParent(ARCodePlaceholder);
                    sceneInstant = ARCodePlaceholder.gameObject;
                }


                cache[code] = Instantiate(sceneInstant);
                cache[code].SetActive(false);

            }
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
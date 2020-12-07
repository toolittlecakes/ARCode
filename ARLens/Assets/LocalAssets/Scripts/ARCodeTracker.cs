using UnityEngine;
using UnityEngine.Events;


using maxstAR;

namespace ARLens
{

    [System.Serializable]
    public class ARCodeFoundEvent : UnityEvent { }

    [System.Serializable]
    public class ARCodeLostEvent : UnityEvent { }

    [System.Serializable]
    public class ARCodeUpdateEvent : UnityEvent<Matrix4x4> { }

    public class ARCodeTracker : ARBehaviour
    {
        private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;
        private bool arCodeTracked = false;

        public Trackable arCode { get; private set; } = null;
        public ARCodeFoundEvent arCodeFound { get; private set; }
        public ARCodeLostEvent arCodeLost { get; private set; }
        public ARCodeUpdateEvent arCodeUpdate { get; private set; }

        void Awake()
        {
            Init();

            cameraBackgroundBehaviour = FindObjectOfType<CameraBackgroundBehaviour>();
            if (cameraBackgroundBehaviour == null)
            {
                Debug.LogError("Can't find CameraBackgroundBehaviour.");
                return;
            }
            arCodeFound = new ARCodeFoundEvent();
            arCodeLost = new ARCodeLostEvent();
            arCodeUpdate = new ARCodeUpdateEvent();
        }

        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            // TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.JITTER_REDUCTION_DEACTIVATION);
            TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.JITTER_REDUCTION_ACTIVATION);
            TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_QR_TRACKER);
            StartCamera();
        }

        void FixedUpdate()
        {
            TrackingState trackingState = TrackerManager.GetInstance().UpdateTrackingState();

            if (trackingState == null)
            {
                return;
            }
            cameraBackgroundBehaviour.UpdateCameraBackgroundImage(trackingState);

            TrackingResult trackingResult = trackingState.GetTrackingResult();

            // state flag
            bool arCodeTracked = trackingResult.GetCount() > 0;

            // events flags (state changing)
            bool arCodeFound = arCode == null && arCodeTracked;
            bool arCodeLost = arCode != null && !arCodeTracked;
            // Debug.Log("tracked + " + arCodeFound);

            // update
            if (arCodeTracked)
            {
                arCode = trackingResult.GetTrackable(0);
                arCodeUpdate.Invoke(arCode.GetPose());
            }

            // call events handlers
            if (arCodeFound)
            {
                this.arCodeFound.Invoke();
            }
            else if (arCodeLost)
            {
                Debug.Log("LOST");
                arCode = null;
                this.arCodeLost.Invoke();
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
} // namespace ARCode
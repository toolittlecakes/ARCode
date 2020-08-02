using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


using maxstAR;

namespace ARLens
{

    [System.Serializable]
    public class ARCodeFoundEvent : UnityEvent{}    
    
    [System.Serializable]
    public class ARCodeLostEvent : UnityEvent{}    
    
    [System.Serializable]
    public class ARCodeUpdateEvent : UnityEvent<Matrix4x4>{}

    public class ARCodeTracker : ARBehaviour
    {
        private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;

        private bool arCodeTracked = false;

        private Trackable arCode = null;
        public ARCodeFoundEvent ARCodeFound {get; private set;}
        public ARCodeLostEvent ARCodeLost {get; private set;}
        public ARCodeUpdateEvent ARCodeUpdate {get; private set;}


        void Awake()
        {
            Init();

            cameraBackgroundBehaviour = FindObjectOfType<CameraBackgroundBehaviour>();
            if (cameraBackgroundBehaviour == null)
            {
                Debug.LogError("Can't find CameraBackgroundBehaviour.");
                return;
            }
            ARCodeFound = new ARCodeFoundEvent();
            ARCodeLost = new ARCodeLostEvent();
            ARCodeUpdate = new ARCodeUpdateEvent();
        }

        void Start()
        {

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            // TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.JITTER_REDUCTION_DEACTIVATION);
            TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_QR_TRACKER);
            StartCamera();
        }

        void Update()
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
                ARCodeUpdate.Invoke(arCode.GetPose());
            }
            

            // call events handlers
            if (arCodeFound)
            {
                ARCodeFound.Invoke();
            }
            else if (arCodeLost)
            {
                Debug.Log("LOST");
                arCode = null;
                ARCodeLost.Invoke();
            }

        }


        public Trackable ARCode() // TODO change to property
        {
            return arCode;
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
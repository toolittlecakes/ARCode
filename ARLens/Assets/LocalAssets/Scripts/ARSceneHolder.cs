using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSceneHolder : MonoBehaviour
{
    private Matrix4x4 pose_;

    void Start()
    {
    }

    // void Update()
    // {
    //     var trackerPosition = maxstAR.MatrixUtils.PositionFromMatrix(pose_);
    //     var trackerRotation = maxstAR.MatrixUtils.QuaternionFromMatrix(pose_);


    //     transform.position = Vector3.Lerp(transform.position, trackerPosition, 0.5f);
    //     // transform.rotation = Quaternion.Slerp(transform.rotation, trackerRotation, 0.5f);
    //     Rigidbody rb = GetComponent<Rigidbody>();
    //     // var force = trackerPosition - transform.position;
    //     // rb.AddForce(force);

        
    //     var q_ω = trackerRotation * Quaternion.Inverse(transform.rotation);
    //     q_ω.ToAngleAxis(out float angle, out Vector3 axis);

    //     var ω = (angle )*axis;

    //     // rb.angularVelocity = rb.angularVelocity * factor + ω * Time.deltaTime;
    //     rb.AddTorque(ω);
    // }


    public void SetPose(Matrix4x4 pose)
    {
        pose_ = pose;
    }
}

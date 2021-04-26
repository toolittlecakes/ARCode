using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSceneHolder : MonoBehaviour
{
    private Matrix4x4 pose_;

    void Start()
    {
        var trackerPosition = maxstAR.MatrixUtils.PositionFromMatrix(pose_);
        var trackerRotation = maxstAR.MatrixUtils.QuaternionFromMatrix(pose_);

        transform.position = trackerPosition;
        transform.rotation = trackerRotation;
    }

    // Стабилизация работает хорошо, но есть баг с поворотом на 180 градусов в определнном положении.
    // Скорее всего дело в матричных преобразованиях квартернионов
    void Update()
    {

        var trackerPosition = maxstAR.MatrixUtils.PositionFromMatrix(pose_);
        var trackerRotation = maxstAR.MatrixUtils.QuaternionFromMatrix(pose_);


        transform.position = Vector3.Lerp(transform.position, trackerPosition, 0.5f);
        // transform.rotation = Quaternion.Slerp(transform.rotation, trackerRotation, 0.5f);
        Rigidbody rb = GetComponent<Rigidbody>();
        // var force = trackerPosition - transform.position;
        // rb.AddForce(force);


        transform.rotation = Quaternion.Lerp(transform.rotation, trackerRotation, 0.5f);

        // var q_ω = trackerRotation * Quaternion.Inverse(transform.rotation);
        // q_ω.ToAngleAxis(out float angle, out Vector3 axis);

        // var ω = angle * axis;
        // // rb.AddTorque(ω);
    }


    public void SetPose(Matrix4x4 pose)
    {
        pose_ = pose;

        // transform.SetPositionAndRotation(
        //         pose.GetColumn(3),
        //         pose.rotation
        //     );
    }
}

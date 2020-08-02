using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSceneHolder : MonoBehaviour
{
    private Matrix4x4 pose_;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = maxstAR.MatrixUtils.PositionFromMatrix(pose_);
        transform.rotation = maxstAR.MatrixUtils.QuaternionFromMatrix(pose_);
    }
    public void SetPose(Matrix4x4 pose)
    {
        pose_ = pose;
    }
}

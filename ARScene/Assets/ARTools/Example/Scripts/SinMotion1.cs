using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMotion1 : MonoBehaviour
{
    private Transform startTransform;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startTransform = transform;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var newPosition = new Vector3(startPosition.x,
                                  startPosition.y,
                                  startPosition.z + Mathf.Sin(Time.time) * 0.1f);
        transform.SetPositionAndRotation(newPosition, startTransform.rotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorGenerator
{
    public static Vector3 GenerateSin()
    {
        return new Vector3(0, 0, Mathf.Sin(Time.time / 0.5f) * 0.5f);
    }
}

public class SinMotion : MonoBehaviour
{
    private Transform startTransform;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var newPosition = VectorGenerator.GenerateSin();
        transform.position = startPosition;
        transform.Translate(newPosition);

    }
}

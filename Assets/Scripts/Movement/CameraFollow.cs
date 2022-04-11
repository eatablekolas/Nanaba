using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject objectToFollow;

    Transform CameraTransform;
    float z;
    Transform ObjectTransform;

    void Awake()
    {
        CameraTransform = transform;
        z = CameraTransform.position.z;
        ObjectTransform = objectToFollow.transform;
    }

    void Update()
    {
        float x = ObjectTransform.position.x;
        float y = ObjectTransform.position.y;

        if (x != CameraTransform.position.x || y != CameraTransform.position.y)
        {
            CameraTransform.position = new Vector3(x, y, z);
        }
    }
}

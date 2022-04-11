using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 0.05f;
    Transform CameraTransform;

    void Awake()
    {
        CameraTransform = transform;
    }

    void Update()
    {
        float horizontalDirection = Input.GetAxis("Horizontal") * cameraSpeed;
        float verticalDirection = Input.GetAxis("Vertical") * cameraSpeed;

        CameraTransform.position = CameraTransform.position + new Vector3(horizontalDirection, verticalDirection, 0);
    }
}

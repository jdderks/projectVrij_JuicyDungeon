using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 10;
    [SerializeField] private Transform target;
    [SerializeField] private float dstFromTarget = 2;

    private float yaw;
    private float pitch;

    
    private void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;

        Vector3 targetRotation = new Vector3(pitch,yaw);
        transform.eulerAngles = targetRotation;

        transform.position = target.position - transform.forward * dstFromTarget;
    }
}

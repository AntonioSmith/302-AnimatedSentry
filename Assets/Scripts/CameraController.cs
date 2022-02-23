using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;

    public Transform target;

    public Vector3 targetOffset; // Offset the camera position from the inspector

    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = -5;
    public float mouseSensitivityScroll = -5;

    private float pitch = 0;
    private float yaw = 0;
    private float dollyDis = 10;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Eased Movement
        if (target)
        {
            transform.position = AnimMath.Ease(transform.position, target.position + targetOffset, .01f);
        }

        // Camera Rotation
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * mouseSensitivityX;
        pitch += my * mouseSensitivityY;

        pitch = Mathf.Clamp(pitch, -10, 89); // Clamps how much the camera can move up/down

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Camera Zoom
        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDis = Mathf.Clamp(dollyDis, 3, 20); // Clamp zoom range

        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -dollyDis), .2f);
            new Vector3(0, 0, -dollyDis);
    }

    private void OnDrawGizmos()
    {
        if(!cam) cam = GetComponentInChildren<Camera>();
        if (!cam) return;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}

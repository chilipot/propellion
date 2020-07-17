using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 200;

    public static bool FreeCam = true;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!FreeCam) return;
        
        var moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * -1;
        var qX = Quaternion.AngleAxis(moveX, Vector3.up);
        var qY = Quaternion.AngleAxis(moveY, Vector3.right);
        transform.rotation *= qX * qY;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PhysicsCameraController : MonoBehaviour
{
    public float mouseSensitivity = 200;

    public static bool FreeCam = true;

    private Vector3 rotationForce;
    private Rigidbody rb;
    
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!FreeCam) return;
        
        var moveX = Input.GetAxis("Mouse X") * mouseSensitivity;
        var moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * -1;
        rotationForce = new Vector3(moveY, moveX, 0);
    }

    private void FixedUpdate()
    {
        rb.AddRelativeTorque(rotationForce, ForceMode.Acceleration);
    }
}

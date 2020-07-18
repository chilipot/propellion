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

    public static bool FreeCam = true; // TODO: make this an instance variable (see TODO below)

    private LevelManager _levelManager;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FreeCam = true;
        _levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        // TODO: instead of checking LevelIsOver here, encapsulate that logic into FreeCam by making it a
        // a private instance variable w/ public setter, which refuses to set it to true if LevelIsOver
        if (!FreeCam || _levelManager.LevelIsOver()) return;
        
        var moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * -1;
        var qX = Quaternion.AngleAxis(moveX, Vector3.up);
        var qY = Quaternion.AngleAxis(moveY, Vector3.right);
        transform.rotation *= qX * qY;
    }

}

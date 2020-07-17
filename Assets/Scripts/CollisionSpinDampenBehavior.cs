using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSpinDampenBehavior : MonoBehaviour
{
    public float dampenDuration = 1f;
    public float dampenOffset = 1f;
    public Rigidbody rb;

    private float? madeImpact = null;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        if (!madeImpact.HasValue) return;
        
        var impactOffset = Time.fixedTime - madeImpact.Value;
        if (rb.angularVelocity.Equals(Vector3.zero))
        {
            madeImpact = null;
            CameraController.FreeCam = true;
        }
        else if (impactOffset > dampenOffset)
        {
            var spinDampenLerp = impactOffset / dampenDuration;
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, spinDampenLerp);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CameraController.FreeCam = false;
    }

    private void OnCollisionExit(Collision other)
    {
        madeImpact = Time.fixedTime;
    }
}

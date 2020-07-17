using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustBehavior : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private ThrusterManager _thruster;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _thruster = FindObjectOfType<ThrusterManager>();
    }

    private void FixedUpdate()
    {
        if (_thruster.IsEngaged())
        {
            _rigidbody.AddForce(Time.fixedDeltaTime * _thruster.power * transform.forward);
        }
        
    }
}

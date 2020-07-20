using System.Collections.Generic;
using UnityEngine;

public class BlackHolePull : MonoBehaviour
{
    
    public float gravitationalForce = 1000f;
    
    private List<Rigidbody> pulledBodies;
    private SphereCollider pullCollider;
    
    private void Start()
    {
        pulledBodies = new List<Rigidbody>();
        pullCollider = GetComponent<SphereCollider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var pulledBody = other.gameObject.GetComponent<Rigidbody>();
        if (pulledBody) pulledBodies.Add(pulledBody);
    }

    private void OnTriggerExit(Collider other)
    {
        var pulledBody = other.gameObject.GetComponent<Rigidbody>();
        RemovePulledBody(pulledBody);
    }

    private void FixedUpdate()
    {
        foreach (var pulledBody in pulledBodies)
        {
            var singularityPosition = transform.position;
            var pulledBodyPosition = pulledBody.transform.position;
            var pullDirection = (singularityPosition - pulledBodyPosition).normalized;
            // TODO: optimize the pull force equation
            pulledBody.AddForce(Time.fixedDeltaTime * gravitationalForce * pullDirection, ForceMode.Acceleration);
        }
    }

    public void RemovePulledBody(Rigidbody pulledBody)
    {
        if (pulledBody && pulledBodies.Contains(pulledBody)) pulledBodies.Remove(pulledBody);
    }
    
}

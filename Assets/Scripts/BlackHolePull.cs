using System.Collections.Generic;
using UnityEngine;

public class BlackHolePull : MonoBehaviour
{
    
    public float gravitationalForce = 10;
    
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
        if (pulledBody != null)
        {
            pulledBodies.Add(pulledBody);
        }
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
            var direction = (singularityPosition - pulledBodyPosition).normalized;
            
            // TODO: fix this gravity force equation to make it more realistic to a black hole, and a greater force the closer to the singularity the object is
            // var proximityMultiplier = pullCollider.radius - Vector3.Distance(singularityPosition, pulledBodyPosition);
            pulledBody.AddForce(Time.fixedDeltaTime * gravitationalForce /* * proximityMultiplier */ * direction);
        }
    }

    public void RemovePulledBody(Rigidbody pulledBody)
    {
        if (pulledBody != null && pulledBodies.Contains(pulledBody))
        {
            pulledBodies.Remove(pulledBody);
        } 
    }
    
}

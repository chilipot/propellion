using System.Collections.Generic;
using UnityEngine;

public class BlackHolePull : MonoBehaviour
{
    
    public float gravitationalForce = 1000f;
    
    private Dictionary<int, Rigidbody> pulledBodies; // key is the ID of rigidbody's gameobject
    
    private void Start()
    {
        pulledBodies = new Dictionary<int, Rigidbody>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var pulledBody = other.gameObject.GetComponent<Rigidbody>();
        if (pulledBody) pulledBodies.Add(other.gameObject.GetInstanceID(), pulledBody);
    }

    private void OnTriggerExit(Collider other)
    {
        var pulledBody = other.gameObject.GetComponent<Rigidbody>();
        RemovePulledBody(pulledBody);
    }

    private void FixedUpdate()
    {
        foreach (var pulledBody in pulledBodies.Values)
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
        var pulledBodyId = pulledBody.gameObject.GetInstanceID();
        if (pulledBody && pulledBodies.ContainsKey(pulledBodyId)) pulledBodies.Remove(pulledBodyId);
    }
    
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackHolePull : MonoBehaviour
{
    
    public float gravitationalForce = 20f;
    public Collider blackHoleCenter;

    private const float GravForceMultiplier = 100_000f;
    private Dictionary<int, Rigidbody> pulledBodies; // key is the ID of rigidbody's gameobject

    private void Start()
    {
        pulledBodies = new Dictionary<int, Rigidbody>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var pulledBody = other.gameObject.GetComponent<Rigidbody>();
        var pulledBodyId = other.gameObject.GetInstanceID();
        if (pulledBody && !pulledBodies.ContainsKey(pulledBodyId)) pulledBodies.Add(pulledBodyId, pulledBody);
    }

    private void OnTriggerExit(Collider other)
    {
        var pulledBody = other.gameObject.GetComponent<Rigidbody>();
        RemovePulledBody(pulledBody);
    }

    private void FixedUpdate()
    {
        // first filter out any pulled bodies that may have been destroyed since the last update
        pulledBodies = pulledBodies.Where(pb => pb.Value).ToDictionary(pb => pb.Key, pb => pb.Value);
        // then pull each remaining pulled body
        foreach (var pulledBody in pulledBodies.Values)
        {
            var singularityPosition = transform.position;
            var pulledBodyPosition = pulledBody.transform.position;
            var pullDirection = (singularityPosition - pulledBodyPosition).normalized;
            var pullDistance = Vector3.Distance(pulledBodyPosition, blackHoleCenter.ClosestPoint(pulledBodyPosition));
            var pullForce = Time.fixedDeltaTime * gravitationalForce * GravForceMultiplier / Mathf.Pow(pullDistance, 2);
            if (!float.IsNaN(pullForce) && !float.IsInfinity(pullForce)) pulledBody.AddForce(pullForce * pullDirection, ForceMode.Acceleration);
        }
    }

    public void RemovePulledBody(Rigidbody pulledBody)
    {
        var pulledBodyId = pulledBody.gameObject.GetInstanceID();
        if (pulledBody && pulledBodies.ContainsKey(pulledBodyId)) pulledBodies.Remove(pulledBodyId);
    }
    
}

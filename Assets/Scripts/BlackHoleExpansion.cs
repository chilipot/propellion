using System.Collections.Generic;
using UnityEngine;

public class BlackHoleExpansion : MonoBehaviour
{
    
    public float expansionRate = 0.5f; // the fraction of current size to increase by per second
    public float gravitationalForce = 10;

    private List<Rigidbody> containedBodies;
    private SphereCollider pullCollider;

    private void Start()
    {
        containedBodies = new List<Rigidbody>();
        pullCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        transform.localScale *= expansionRate * Time.deltaTime + 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        var triggeredBody = other.gameObject.GetComponent<Rigidbody>();
        if (triggeredBody != null)
        {
            containedBodies.Add(triggeredBody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var triggeredBody = other.gameObject.GetComponent<Rigidbody>();
        if (triggeredBody != null && containedBodies.Contains(triggeredBody))
        {
            containedBodies.Remove(triggeredBody);
        }
    }

    private void FixedUpdate()
    {
        foreach (var containedBody in containedBodies)
        {
            var singularityPosition = transform.position;
            var containedBodyPosition = containedBody.transform.position;
            var direction = (singularityPosition - containedBodyPosition).normalized;
            
            // TODO: fix this gravity force equation to make it more realistic to a black hole, and a greater force the closer to the singularity the object is
            // var proximityMultiplier = pullCollider.radius - Vector3.Distance(singularityPosition, containedBodyPosition);
            containedBody.AddForce(Time.fixedDeltaTime * gravitationalForce /* * proximityMultiplier */ * direction);
        }
    }
}

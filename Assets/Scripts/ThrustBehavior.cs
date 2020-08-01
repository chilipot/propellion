using UnityEngine;

public class ThrustBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private ThrusterManager thruster;
    private bool adjustedDirection;
    private float? propelTime = null;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        thruster = GetComponentInChildren<ThrusterManager>();
        adjustedDirection = false;
    }

    private void FixedUpdate()
    {
        if (thruster.IsEngaged())
        {
            if (!propelTime.HasValue)
            {
                // Makes direction deltas more powerful
                propelTime = Time.fixedTime;
                var facingDirection = transform.forward;
                var directionMultiplier = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, facingDirection.normalized) - 1);
                var burstMultiplier = 35f * directionMultiplier;
                rb.AddForce(facingDirection * (thruster.power * burstMultiplier), ForceMode.Impulse);
                thruster.Burst(directionMultiplier / 2);
            }
            else
            {
                var thrustStrength = thruster.power;
                rb.AddForce(transform.forward * thrustStrength);
            }
        }
        else propelTime = null;
    }
}

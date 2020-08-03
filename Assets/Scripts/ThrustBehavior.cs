using UnityEngine;

public class ThrustBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private ThrusterManager thruster;
    private float? propelTime;
    
    private void Start()
    {
        rb = LevelManager.PlayerRb;
        thruster = GetComponentInChildren<ThrusterManager>();
        propelTime = null;
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

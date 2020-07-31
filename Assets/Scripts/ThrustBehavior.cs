using UnityEngine;

public class ThrustBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private ThrusterManager thruster;
    private bool adjustedDirection;
    
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
            if (!adjustedDirection)
            {
                rb.velocity = transform.forward * rb.velocity.magnitude;
                adjustedDirection = true;
            }
            rb.AddForce(Time.fixedDeltaTime * thruster.power * transform.forward);
        }
        else adjustedDirection = false;
    }
}

using UnityEngine;

public class ThrustBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private ThrusterManager thruster;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        thruster = GetComponentInChildren<ThrusterManager>();
    }

    private void FixedUpdate()
    {
        if (thruster.IsEngaged())
        {
            rb.AddForce(Time.fixedDeltaTime * thruster.power * transform.forward);
        }
    }
}

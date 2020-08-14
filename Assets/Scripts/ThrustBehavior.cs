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
            var thrustDirection = (Input.GetAxis("Horizontal") * transform.right +
                                  Input.GetAxis("Vertical") * transform.forward).normalized; //new Vector3(, 0, Input.GetAxis("Vertical") * transform.forward);
                
                //(new Vector3(Input.GetAxis("Horizontal"), 
                //0, Input.GetAxis("Vertical")) - transform.rotation.eulerAngles).normalized;
            
            if (!propelTime.HasValue)
            {
                // Makes direction deltas more powerful
                propelTime = Time.fixedTime;
                var directionMultiplier = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, thrustDirection) - 1);
                var burstMultiplier = 25f * directionMultiplier;
                rb.AddForce(thrustDirection * (thruster.power * burstMultiplier), ForceMode.Impulse);
                thruster.Burst(directionMultiplier / 2);
            }
            else
            {
                var thrustStrength = thruster.power;
                rb.AddForce(thrustDirection * thrustStrength);
            }
        }
        else propelTime = null;
    }
}

using UnityEngine;

public class CollisionPhysicsBehavior : MonoBehaviour
{
    public float spinDampenDelay = 1f;
    public float spinDampenDuration = 1f;

    private Rigidbody rb;
    private LevelManager levelManager;
    private ThrusterManager thruster;
    private float? impactTime;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        levelManager = FindObjectOfType<LevelManager>();
        thruster = GetComponentInChildren<ThrusterManager>();
        impactTime = null;
    }
    
    private void FixedUpdate()
    {
        if (!impactTime.HasValue || levelManager.LevelIsOver()) return; // if level is over, continue spinning indefinitely for dramatic effect
        var timeSinceImpact = Time.fixedTime - impactTime.Value;
        if (rb.angularVelocity.Equals(Vector3.zero))
        {
            impactTime = null;
            CameraController.FreeCam = true;
        }
        else if (timeSinceImpact > spinDampenDelay)
        {
            var spinDampenLerpT = timeSinceImpact / spinDampenDuration;
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, spinDampenLerpT);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CameraController.FreeCam = false;
        thruster.Disengage();
    }

    private void OnCollisionExit(Collision other)
    {
        impactTime = Time.fixedTime;
    }
}

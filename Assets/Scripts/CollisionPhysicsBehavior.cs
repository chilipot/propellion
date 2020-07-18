using UnityEngine;

public class CollisionPhysicsBehavior : MonoBehaviour
{
    public float spinDampenDelay = 0.5f;
    public float spinDampenDuration = 0.5f;

    private Rigidbody rb;
    private LevelManager levelManager;
    private ThrusterManager thruster;
    private float? impactTime;
    private Vector3? spinDampenStartVelocity;
    private float camDrag;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        levelManager = FindObjectOfType<LevelManager>();
        thruster = GetComponentInChildren<ThrusterManager>();
        impactTime = null;
        spinDampenStartVelocity = null;
        camDrag = rb.angularDrag;
    }
    
    private void FixedUpdate()
    {
        if (!impactTime.HasValue || levelManager.LevelIsOver()) return; // if level is over, continue spinning indefinitely for dramatic effect
        var timeSinceImpact = Time.fixedTime - impactTime.Value;
        if (timeSinceImpact > spinDampenDelay)
        {
            if (!spinDampenStartVelocity.HasValue) spinDampenStartVelocity = rb.angularVelocity;
            var spinDampenOffset = Mathf.Clamp((timeSinceImpact - spinDampenDelay) / spinDampenDuration, 0f, 1f);
            rb.angularVelocity = Vector3.Lerp(spinDampenStartVelocity.Value, Vector3.zero, spinDampenOffset);
        }
        if (rb.angularVelocity == Vector3.zero)
        {
            rb.angularDrag = camDrag;
            impactTime = null;
            spinDampenStartVelocity = null;
            CameraController.FreeCam = true;
            PhysicsCameraController.FreeCam = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CameraController.FreeCam = false;
        PhysicsCameraController.FreeCam = false;
        rb.angularDrag = 0;
        thruster.Disengage();
    }

    private void OnCollisionExit(Collision other)
    {
        impactTime = Time.fixedTime;
    }
}

using UnityEngine;

public class CollisionPhysicsBehavior : MonoBehaviour
{
    public float stabilizationDelay = 0.5f;
    public float stabilizationDuration = 0.5f;

    private Rigidbody rb;
    private LevelManager levelManager;
    private ThrusterManager thruster;
    private float? impactTime;
    private Vector3? stabilizationStartVelocity;
    private float camDrag;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        levelManager = FindObjectOfType<LevelManager>();
        thruster = GetComponentInChildren<ThrusterManager>();
        impactTime = null;
        stabilizationStartVelocity = null;
        camDrag = rb.angularDrag;
    }
    
    private void FixedUpdate()
    {
        if (!impactTime.HasValue || levelManager.LevelIsOver()) return; // if level is over, continue spinning indefinitely for dramatic effect
        var timeSinceImpact = Time.fixedTime - impactTime.Value;
        if (timeSinceImpact > stabilizationDelay) Stabilize(timeSinceImpact);
        if (rb.angularVelocity == Vector3.zero) FinishStabilizing();
    }

    private void Stabilize(float timeSinceImpact)
    {
        if (!stabilizationStartVelocity.HasValue) stabilizationStartVelocity = rb.angularVelocity;
        var spinDampenOffset = Mathf.Clamp((timeSinceImpact - stabilizationDelay) / stabilizationDuration, 0f, 1f);
        rb.angularVelocity = Vector3.Lerp(stabilizationStartVelocity.Value, Vector3.zero, spinDampenOffset);
    }

    private void FinishStabilizing()
    {
        rb.angularDrag = camDrag;
        impactTime = null;
        stabilizationStartVelocity = null;
        CameraController.FreeCam = true;
        PhysicsCameraController.FreeCam = true;
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

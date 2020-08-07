using System;
using UnityEngine;
using UnityEngine.UI;

public class GrappleGunBehavior : MonoBehaviour
{
    public bool breakable = false;
    public float maxRetractionForce = 150f;
    public float maxGrappleLength = 250f;
    public LayerMask grappleableStuff;
    public Transform gunTip;

    private LineRenderer lineRenderer;
    private Transform grappledObj; // TODO: make this explicitly nullable (maybe by abstracting grappledObj, grappledObjOffset, grappledRetractable, and grappling out into an optional struct)
    private Vector3? grappledObjOffset;
    private Retractable grappledRetractable; // TODO: make this explicitly nullable (maybe by abstracting grappledObj, grappledObjOffset, grappledRetractable, and grappling out into an optional struct)
    private Transform mainCamera;
    private GameObject player;
    private Rigidbody playerRb;
    private SpringJoint joint;
    private AudioSource shootGrappleSfx;
    private bool grappling;

    private UIManager ui;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        grappledObj = null;
        grappledObjOffset = null;
        grappledRetractable = null;
        mainCamera = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody>();
        shootGrappleSfx = GetComponent<AudioSource>();
        grappling = false;
        ui = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !LevelManager.LevelInactive) StartGrapple();
        else if (Input.GetMouseButtonUp(0) || GrappleTargetDestroyed()) StopGrapple();
    }

    private bool GrappleTargetDestroyed()
    {
        return grappling && !grappledObj;
    }

    private void BuildGrapple(RaycastHit hit)
    {
        joint = player.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedBody = hit.rigidbody;
        if (grappledObjOffset.HasValue)
        {
            joint.connectedAnchor = grappledObjOffset.Value.normalized;
        }

        joint.spring = 10f;
        joint.damper = 3f;
        joint.maxDistance = Vector3.Distance(player.transform.position, GrapplePoint());
        joint.minDistance = 0f;
        joint.enableCollision = true;
    }

    private void StartGrapple()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit, maxGrappleLength, grappleableStuff))
        {
            shootGrappleSfx.Play();
            grappledObj = hit.transform;

            grappledObjOffset = hit.point - grappledObj.position;
            grappling = true;
            lineRenderer.positionCount = 2;
            var retractable = hit.collider.GetComponent<Retractable>();
            if (retractable)
            {
                grappledRetractable = retractable; 
                grappledRetractable.Retract(player.transform);
            }
            BuildGrapple(hit);
        }
    }

    private void ReticleEffect()
    {
        var focusedReticle = Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit, maxGrappleLength,
            grappleableStuff);
        ui.SetReticleFocus(focusedReticle);
    }

    public void StopGrapple()
    {
        grappledObj = null;
        grappledObjOffset = null;
        if (grappledRetractable)
        {
            grappledRetractable.CancelRetraction();
            grappledRetractable = null;
        }
        grappling = false;
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void LateUpdate()
    {
        DrawGrappleRope();
    }

    private void DrawGrappleRope()
    {
        if (!grappling) return;
        Vector3[] renderPositions = {gunTip.position, GrapplePoint()};
        lineRenderer.SetPositions(renderPositions);
    }

    private void FixedUpdate()
    {
        ReticleEffect();
        if (!grappling) return;
        if (breakable && GrappleBroken()) StopGrapple(); // TODO: add particle effects, SFX, and/or animation to indicate what happened
        else
        {
            joint.maxDistance *= 0.975f;
            // Apply an extra tug - as if the rope stopped stretching
            var tugForce = ComputeTugForce();
            playerRb.AddForce(tugForce, ForceMode.Impulse);
        }
    }

    private bool GrappleBroken()
    {
        var grappleStart = lineRenderer.GetPosition(0);
        var grappleEnd = lineRenderer.GetPosition(1);
        var grappleDirection = grappleEnd - grappleStart;
        if (Physics.Raycast(grappleStart, grappleDirection, out var hit, grappleDirection.magnitude, grappleableStuff))
        {
            if (grappledRetractable && hit.collider.transform == grappledObj) return false;
            return Vector3.Distance(hit.point, GrapplePoint()) > 0.1f;
        }
        return false;
    }

    private Vector3 ComputeTugForce()
    {
        var grappleDirection = GrapplePoint() - player.transform.position;
        var distFromGrapplePoint = grappleDirection.magnitude;
        
        var directionMultiplier = Mathf.Abs(Vector3.Dot(playerRb.velocity.normalized, grappleDirection.normalized) - 1);
        var tensionMultiplier = 35f * directionMultiplier * Mathf.Clamp(distFromGrapplePoint / maxRetractionForce, 0.5f, 1);
        var tugForce = grappleDirection.normalized * tensionMultiplier;
        return tugForce;
    }

    // TODO: make this private if nothing outside the class uses it
    public Vector3 GrapplePoint()
    {
        if (!grappledObjOffset.HasValue) throw new InvalidOperationException("There is no active grapple point.");
        return grappledObj.position + grappledObjOffset.Value;
    }
}

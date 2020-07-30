using System;
using UnityEngine;

public class GrappleGunBehavior : MonoBehaviour
{
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
    private AudioSource shootGrappleSfx;
    private bool grappling;

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) StartGrapple();
        else if (Input.GetMouseButtonUp(0) || GrappleTargetDestroyed()) StopGrapple();
    }

    private bool GrappleTargetDestroyed()
    {
        return grappling && !grappledObj;
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
        }
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
        if (!grappling) return;
        if (GrappleBroken()) StopGrapple(); // TODO: add particle effects, SFX, and/or animation to indicate what happened
        else
        {
            var tugForce = ComputeTugForce();
            playerRb.AddForce(tugForce, ForceMode.Force);
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
        var tugStrength = grappleDirection.magnitude / maxGrappleLength * maxRetractionForce;
        var tugForce = grappleDirection.normalized * tugStrength;
        return tugForce;
    }

    // TODO: delete this if nothing uses it
    public bool IsGrappling()
    {
        return grappling;
    }

    // TODO: make this private if nothing outside the class uses it
    public Vector3 GrapplePoint()
    {
        if (!grappledObjOffset.HasValue) throw new InvalidOperationException("There is no active grapple point.");
        return grappledObj.position + grappledObjOffset.Value;
    }
}

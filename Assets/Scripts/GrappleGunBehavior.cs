using System;
using UnityEngine;
using UnityEngine.UI;

public class GrappleGunBehavior : MonoBehaviour
{
    public float maxRetractionForce = 150f;
    public float maxGrappleLength = 250f;
    public LayerMask grappleableStuff;
    public Transform gunTip;
    public Image reticleImage;
    public Color reticleImageHitColor = new Color(255, 92, 94);

    private LineRenderer lineRenderer;
    private Transform grappleObj; // TODO: make this explicitly nullable
    private Vector3? grappleObjOffset;
    private Transform mainCamera;
    private GameObject player;
    private Rigidbody playerRb;
    private bool grappling;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        grappleObj = null;
        grappleObjOffset = null;
        mainCamera = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody>();
        grappling = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) StartGrapple();
        else if (Input.GetMouseButtonUp(0) || GrappleTargetDestroyed()) StopGrapple();
    }

    private bool GrappleTargetDestroyed()
    {
        return grappling && !grappleObj;
    }

    private void StartGrapple()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit, maxGrappleLength, grappleableStuff))
        {
            grappling = true;
            grappleObj = hit.transform;
            grappleObjOffset = hit.point - grappleObj.position;
            lineRenderer.positionCount = 2;
        }
    }

    private void ReticleEffect()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit, maxGrappleLength, grappleableStuff))
        {
            reticleImage.color = reticleImageHitColor;
            reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale,
                new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 4);
        }
        else
        {
            reticleImage.color = Color.white;
            reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale,
                new Vector3(1f, 1f, 1f), Time.deltaTime * 4);
        }
    }

    public void StopGrapple()
    {
        grappling = false;
        grappleObj = null;
        grappleObjOffset = null;
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
        ReticleEffect();
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
        if (Physics.Raycast(grappleStart, grappleEnd - grappleStart, out var hit, Mathf.Infinity, grappleableStuff))
        {
            return Vector3.Distance(hit.point, GrapplePoint()) > 0.1f;
        }
        return true;
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
        if (grappleObjOffset == null) throw new InvalidOperationException("There is no active grapple point.");
        return grappleObj.position + grappleObjOffset.Value;
    }
}

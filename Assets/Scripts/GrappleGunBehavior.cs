using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class GrappleGunBehavior : MonoBehaviour
{
    public float maxRetractionForce = 150f;
    public float maxGrappleLength = 250f;
    public LayerMask grappleableStuff;
    public Transform gunTip;

    private LineRenderer lineRenderer;
    [CanBeNull] private GrappleTarget currentTarget;
    private Transform mainCamera;
    private AudioSource shootGrappleSfx;
    private UIManager ui;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        currentTarget = null;
        mainCamera = LevelManager.MainCamera.transform;
        shootGrappleSfx = GetComponent<AudioSource>();
        ui = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && LevelManager.LevelIsActive()) StartGrapple();
        else if (Input.GetMouseButtonUp(0) || GrappleTargetDestroyed()) StopGrapple();
    }

    private void StartGrapple()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit, maxGrappleLength, grappleableStuff))
        {
            currentTarget = new GrappleTarget(hit.transform, hit.point);
            lineRenderer.positionCount = 2;
            shootGrappleSfx.PlayOneShot(shootGrappleSfx.clip);
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
        if (currentTarget == null) return;
        currentTarget.Release();
        currentTarget = null;
        lineRenderer.positionCount = 0;
    }
    
    private bool GrappleTargetDestroyed()
    {
        return currentTarget != null && !currentTarget.Transform;
    }

    private void LateUpdate()
    {
        DrawGrappleRope();
    }

    private void DrawGrappleRope()
    {
        if (currentTarget == null) return;
        Vector3[] renderPositions = {gunTip.position, currentTarget.GetGrapplePoint()};
        lineRenderer.SetPositions(renderPositions);
    }

    private void FixedUpdate()
    {
        ReticleEffect();
        if (currentTarget == null) return;
        if (GrappleBroken()) StopGrapple(); // TODO: add particle effects, SFX, and/or animation to indicate what happened
        else
        {
            var tugForce = ComputeTugForce();
            LevelManager.PlayerRb.AddForce(tugForce, ForceMode.Force);
        }
    }

    private bool GrappleBroken()
    {
        if (currentTarget == null) throw new InvalidOperationException("Not currently grappling.");
        var grappleStart = lineRenderer.GetPosition(0);
        var grappleEnd = lineRenderer.GetPosition(1);
        var grappleDirection = grappleEnd - grappleStart;
        if (Physics.Raycast(grappleStart, grappleDirection, out var hit, grappleDirection.magnitude, grappleableStuff))
        {
            if (currentTarget.IsRetractable && hit.collider.transform == currentTarget.Transform) return false;
            return Vector3.Distance(hit.point, currentTarget.GetGrapplePoint()) > 0.1f;
        }
        return false;
    }

    private Vector3 ComputeTugForce()
    {
        if (currentTarget == null) throw new InvalidOperationException("Not currently grappling.");
        var grappleDirection = currentTarget.GetGrapplePoint() - LevelManager.Player.position;
        var tugStrength = grappleDirection.magnitude / maxGrappleLength * maxRetractionForce;
        var tugForce = grappleDirection.normalized * tugStrength;
        return tugForce;
    }

    private class GrappleTarget
    {
        public Transform Transform { get; }
        public bool IsRetractable { get; }
        
        private Vector3 Offset { get; }
        private IGrappleResponse[] Responses { get; }

        public GrappleTarget(Transform target, Vector3 grapplePoint)
        {
            Transform = target;
            Offset = grapplePoint - Transform.position;
            Responses = Transform.GetComponentsInChildren<IGrappleResponse>();
            foreach (var response in Responses) response.OnGrappleStart();
            IsRetractable = Responses.Any(res => res is Retractable);
        }

        public Vector3 GetGrapplePoint()
        {
            return Transform.position + Offset;
        }
        
        public void Release()
        {
            foreach (var response in Responses) response.OnGrappleStop();
        }
    }
}

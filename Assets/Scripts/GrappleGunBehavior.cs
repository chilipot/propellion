using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGunBehavior : MonoBehaviour
{
    public float maxRetractionForce = 150f;
    public float maxGrappleLength = 250f;
    public LayerMask grappleableStuff;
    public Transform _gunTip;

    private LineRenderer _lineRenderer;
    private Transform _grapplePoint;
    private bool grappling = false;
    private Transform _camera;
    private GameObject _player;
    private Rigidbody _player_rb;

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GrapplePoint()
    {
        return _grapplePoint.position;
    }
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _player_rb = _player.GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawGrappleRope();
    }

    private Vector3 ComputeTugForce()
    {
        var towardsGrapplePoint = _grapplePoint.position - _player.transform.position;
        var tugStrength = (towardsGrapplePoint.magnitude / maxGrappleLength) * maxRetractionForce;
        var tugForce = towardsGrapplePoint.normalized * tugStrength;
        return tugForce;
    }
    private void FixedUpdate()
    {
        if (!grappling) return;
        var tugForce = ComputeTugForce();
        _player_rb.AddForce(tugForce, ForceMode.Force);
    }

    private void DrawGrappleRope()
    {
        if (!grappling) return;
        Vector3[] renderPositions = {_gunTip.position, _grapplePoint.position};
        _lineRenderer.SetPositions(renderPositions);
    }
    
    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.position, _camera.forward, out hit, maxGrappleLength, grappleableStuff))
        {
            grappling = true;
            _grapplePoint = hit.transform;
            _lineRenderer.positionCount = 2;
        }
    }

    private void StopGrapple()
    {
        _lineRenderer.positionCount = 0;
        grappling = false;
    }
}

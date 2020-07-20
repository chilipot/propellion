using UnityEngine;

public class GrappleGunBehavior : MonoBehaviour
{
    public float maxRetractionForce = 150f;
    public float maxGrappleLength = 250f;
    public LayerMask grappleableStuff;
    public Transform gunTip;

    private LineRenderer lineRenderer;
    private Transform grapplePoint; // TODO: make this explicitly nullable
    private Transform mainCamera;
    private GameObject player;
    private Rigidbody playerRb;
    private bool grappling;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        grapplePoint = null;
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
        return grappling && !grapplePoint;
    }
    
    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, maxGrappleLength, grappleableStuff))
        {
            grappling = true;
            grapplePoint = hit.transform;
            lineRenderer.positionCount = 2;
        }
    }
    
    public void StopGrapple()
    {
        grappling = false;
        grapplePoint = null;
        lineRenderer.positionCount = 0;
    }

    private void LateUpdate()
    {
        DrawGrappleRope();
    }
    
    private void DrawGrappleRope()
    {
        if (!grappling) return;
        Vector3[] renderPositions = {gunTip.position, grapplePoint.position};
        lineRenderer.SetPositions(renderPositions);
    }
    
    private void FixedUpdate()
    {
        if (!grappling) return;
        var tugForce = ComputeTugForce();
        playerRb.AddForce(tugForce, ForceMode.Force);
    }

    private Vector3 ComputeTugForce()
    {
        var grappleDirection = grapplePoint.position - player.transform.position;
        var tugStrength = grappleDirection.magnitude / maxGrappleLength * maxRetractionForce;
        var tugForce = grappleDirection.normalized * tugStrength;
        return tugForce;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GrapplePoint()
    {
        return grapplePoint.position;
    }
}

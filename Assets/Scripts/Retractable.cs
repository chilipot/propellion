using JetBrains.Annotations;
using UnityEngine;

public class Retractable : MonoBehaviour, IGrappleResponse
{

    public float retractSpeed = 10f; // units per second
    
    private static Transform destination;

    [CanBeNull] private Retraction currentRetraction;

    private void Start()
    {
        destination = LevelManager.Player;
        currentRetraction = null;
    }

    private void Update()
    {
        if (currentRetraction != null)
        {
            transform.position = Vector3.Lerp(currentRetraction.StartPosition, destination.position,
                (Time.time - currentRetraction.RetractStartTime) / currentRetraction.RetractDuration);
        }
    }

    public void OnGrappleStart()
    {
        currentRetraction = new Retraction(transform.position, retractSpeed);
    }

    public void OnGrappleStop()
    {
        currentRetraction = null;
    }
    
    private class Retraction
    {
        public float RetractStartTime { get; }
        public float RetractDuration { get; }
        public Vector3 StartPosition { get; }
        
        public Retraction(Vector3 startPosition, float retractSpeed)
        {
            StartPosition = startPosition;
            RetractStartTime = Time.time;
            RetractDuration = Vector3.Distance(startPosition, destination.position) / retractSpeed;
        }
    }
}
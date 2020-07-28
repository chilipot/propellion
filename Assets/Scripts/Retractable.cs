using UnityEngine;

public class Retractable : MonoBehaviour
{

    public float retractSpeed = 10f; // units per second

    private Retraction? retraction;

    private void Update()
    {
        if (retraction.HasValue)
        {
            transform.position = Vector3.Lerp(retraction.Value.StartPosition, retraction.Value.Destination.position,
                (Time.time - retraction.Value.RetractStartTime) / retraction.Value.RetractDuration);
        }
    }

    public void Retract(Transform dest)
    {
        retraction = new Retraction(transform.position, dest, retractSpeed);
    }

    public void CancelRetraction()
    {
        retraction = null;
    }
    
}

public readonly struct Retraction
{
    public Retraction(Vector3 startPosition, Transform destination, float retractSpeed)
    {
        StartPosition = startPosition;
        Destination = destination;
        RetractStartTime = Time.time;
        RetractDuration = Vector3.Distance(startPosition, destination.position) / retractSpeed;
    }

    public readonly float RetractStartTime;
    public readonly float RetractDuration;
    public readonly Vector3 StartPosition;
    public readonly Transform Destination;
}
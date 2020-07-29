using System;
using JetBrains.Annotations;
using UnityEngine;

public class Retractable : MonoBehaviour
{

    public float retractSpeed = 10f; // units per second

    [CanBeNull] private Retraction currentRetraction;

    private void Start()
    {
        currentRetraction = null;
    }

    private void Update()
    {
        if (currentRetraction != null)
        {
            transform.position = Vector3.Lerp(currentRetraction.StartPosition, currentRetraction.Destination.position,
                (Time.time - currentRetraction.RetractStartTime) / currentRetraction.RetractDuration);
        }
    }

    public void Retract(Transform dest)
    {
        currentRetraction = new Retraction(transform.position, dest, retractSpeed);
    }

    public void CancelRetraction()
    {
        currentRetraction = null;
    }
    
    private class Retraction
    {
        public float RetractStartTime { get; }
        public float RetractDuration { get; }
        public Vector3 StartPosition { get; }
        public Transform Destination { get; }
        
        public Retraction(Vector3 startPosition, Transform destination, float retractSpeed)
        {
            StartPosition = startPosition;
            Destination = destination;
            RetractStartTime = Time.time;
            RetractDuration = Vector3.Distance(startPosition, destination.position) / retractSpeed;
        }
    }
}
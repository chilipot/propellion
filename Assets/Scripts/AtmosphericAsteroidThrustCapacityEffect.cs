using System;
using UnityEngine;

public class AtmosphericAsteroidThrustCapacityEffect : MonoBehaviour
{
    [Tooltip("Effect Interaction Delay")]
    public float thrustCapacityEffectInteractionDelay = 1f;
    public float refuelAmount = 2f;
    
    private float lastThrustCapacityEffect;

    private void Start()
    {
        lastThrustCapacityEffect = -thrustCapacityEffectInteractionDelay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || Time.time - lastThrustCapacityEffect <= thrustCapacityEffectInteractionDelay)
        {
            return;
        }
        var thrusterManager = other.GetComponentInChildren<ThrusterManager>();
        thrusterManager.Refuel(refuelAmount);
        lastThrustCapacityEffect = Time.time;
    }
}
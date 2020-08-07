using System;
using UnityEngine;

public class AtmosphericAsteroidThrustCapacityEffect : MonoBehaviour
{
    public float refuelAmount = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var thrusterManager = other.GetComponentInChildren<ThrusterManager>();
        thrusterManager.Refuel(refuelAmount);
    }
}
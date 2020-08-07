using System;
using UnityEngine;

public class AtmosphericAsteroidThrustCapacityEffect : ThrustCapacityEffectBehavior
{
    public override ThrustCapacityEffect Effect => ThrustCapacityEffect.Fill;
    public override float ComputeStrength() => refuelAmount;
    public float refuelAmount = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var thrusterManager = other.GetComponentInChildren<ThrusterManager>();
        thrusterManager.ProcessThrustCapacityEffect(gameObject);
    }
}
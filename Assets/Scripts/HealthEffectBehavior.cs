using JetBrains.Annotations;
using UnityEngine;

public enum HealthEffect
{
    Heal = 1,
    Damage = -1
}

public enum HealthEffectSource
{
    AlienProjectile,
    AlienFiringSquadProjectile,
    Asteroid,
    MedicalCanister
}

public abstract class HealthEffectBehavior : MonoBehaviour
{
    public abstract HealthEffectSource Source { get; }
    public abstract HealthEffect Effect { get; }

    public abstract int ComputeStrength([CanBeNull] Collision collision = null);
}

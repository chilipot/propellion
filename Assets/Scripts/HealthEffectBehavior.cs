using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthEffect
{
    Heal = 1,
    Damage = -1
}

public abstract class HealthEffectBehavior : MonoBehaviour
{
    public abstract HealthEffect Effect { get; }

    public abstract int ComputeStrength(Collision collision = null);
}

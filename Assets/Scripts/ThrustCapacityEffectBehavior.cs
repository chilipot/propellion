using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThrustCapacityEffect
{
    Fill = 1,
    Empty = -1
}

public abstract class ThrustCapacityEffectBehavior : MonoBehaviour
{
    public abstract ThrustCapacityEffect Effect { get; }

    public abstract float ComputeStrength();
}

﻿using JetBrains.Annotations;
using UnityEngine;

public class AsteroidCollisionHealthEffect : HealthEffectBehavior
{
    public override int ComputeStrength(Collision collision = null) => collision == null
        ? 0
        : Mathf.RoundToInt(Mathf.Clamp(collision.impulse.magnitude / Time.fixedDeltaTime / 1000 * damageMultiplier,1f, maxDmg));

    public override HealthEffect Effect => HealthEffect.Damage;
    public override HealthEffectSource Source => HealthEffectSource.Asteroid;

    public float damageMultiplier = 1.5f;
    public float maxDmg = 30f;
}
using UnityEngine;

public class AsteroidCollisionHealthEffect : HealthEffectBehavior
{
    public override int ComputeStrength(Collision collision = null) => collision == null
        ? 0
        : Mathf.RoundToInt(Mathf.Clamp(collision.impulse.magnitude / Time.fixedDeltaTime * (damageMultiplier / 1000),
            1f,
            maxDmg));

public override HealthEffect Effect => HealthEffect.Damage;

    public float damageMultiplier = 1f;
    public float maxDmg = 30f;

}
using UnityEngine;

public class AsteroidCollisionHealthEffect : HealthEffectBehavior
{
    public override int ComputeStrength(Collision collision = null) => collision == null
        ? 0
        : Mathf.RoundToInt(Mathf.Clamp(collision.impulse.magnitude * damageMultiplier, 0f, maxDmg));

    public override HealthEffect Effect => HealthEffect.Damage;

    public float damageMultiplier = 0.05f;
    public float maxDmg = 30f;

}
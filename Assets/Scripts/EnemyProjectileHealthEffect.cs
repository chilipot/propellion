using UnityEngine;

public class EnemyProjectileHealthEffect : HealthEffectBehavior
{
    public override HealthEffect Effect => HealthEffect.Damage;
    public override int ComputeStrength(Collision collision = null) => damage;

    public int damage = 30;
}

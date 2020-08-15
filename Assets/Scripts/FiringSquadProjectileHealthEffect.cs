using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringSquadProjectileHealthEffect  : HealthEffectBehavior
{
    public override HealthEffect Effect => HealthEffect.Damage;
    public override HealthEffectSource Source => HealthEffectSource.AlienFiringSquadProjectile;
    public override int ComputeStrength(Collision collision = null) => damage;

    public int damage = 9000;
}

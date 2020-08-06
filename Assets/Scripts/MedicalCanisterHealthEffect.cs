using JetBrains.Annotations;
using UnityEngine;

public class MedicalCanisterHealthEffect : HealthEffectBehavior
{
    public override int ComputeStrength([CanBeNull] Collision collision = null) => healAmount;
    
    public override HealthEffect Effect => HealthEffect.Heal;

    public int healAmount = 40;
}

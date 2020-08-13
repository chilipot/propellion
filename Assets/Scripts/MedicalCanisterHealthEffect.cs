using UnityEngine;

public class MedicalCanisterHealthEffect : HealthEffectBehavior
{
    public override int ComputeStrength(Collision collision = null) => healAmount;
    
    public override HealthEffect Effect => HealthEffect.Heal;
    public override HealthEffectSource Source => HealthEffectSource.MedicalCanister;
    
    public int healAmount = 40;
}

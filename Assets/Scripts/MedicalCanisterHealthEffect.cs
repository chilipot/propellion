public class MedicalCanisterHealthEffect : HealthEffectBehavior
{
    public override int Strength => healAmount;
    public override HealthEffect Effect => HealthEffect.Heal;

    public int healAmount = 20;
}

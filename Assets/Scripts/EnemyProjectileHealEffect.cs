public class EnemyProjectileHealEffect : HealthEffectBehavior
{
    public override int Strength => damage;
    public override HealthEffect Effect => HealthEffect.Damage;

    public int damage = 30;
}

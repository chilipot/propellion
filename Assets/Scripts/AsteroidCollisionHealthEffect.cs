using UnityEngine;

public class AsteroidCollisionHealthEffect : HealthEffectBehavior
{
    public override int Strength => Mathf.RoundToInt(size);
    public override HealthEffect Effect => HealthEffect.Damage;
    
    private float size = 0f;
    private void Start()
    {
        size = GetComponent<Renderer>().bounds.size.magnitude / 2;
    }
}
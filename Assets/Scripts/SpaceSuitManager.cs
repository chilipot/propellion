using UnityEngine;

public class SpaceSuitManager : MonoBehaviour
{
    public float healthEffectInteractionDelay = 0.5f;
    public float maxHealth = 100f;
    
    public static readonly StatEvent DamageDeathEvent = new StatEvent(StatEventType.SpaceSuitDamageDeath);
    
    private float health;
    private float lastHealthEffect;
    private LevelManager levelManager;
    private UIManager ui;

    private void Start()
    {
        health = 0f;
        lastHealthEffect = -healthEffectInteractionDelay;
        levelManager = FindObjectOfType<LevelManager>();
        ui = FindObjectOfType<UIManager>();
        SetHealth(maxHealth);
    }

    private void OnCollisionEnter(Collision other)
    {
        ProcessHealthEffect(other.gameObject, other);
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcessHealthEffect(other.gameObject);
    }

    private void ProcessHealthEffect(GameObject healthEffectObj, Collision collision = null)
    {
        if (!LevelManager.LevelIsActive) return; // Ignore stuff when the level isn't playing
        if (Time.fixedTime - lastHealthEffect <= healthEffectInteractionDelay) return;
        var healthEffectBehavior = healthEffectObj.GetComponent<HealthEffectBehavior>();
        if (!healthEffectBehavior) return;
        switch (healthEffectBehavior.Effect)
        {
            case HealthEffect.Heal:
                Heal(healthEffectBehavior.ComputeStrength(collision), healthEffectBehavior.Source);
                break;
            case HealthEffect.Damage:
                Damage(healthEffectBehavior.ComputeStrength(collision), healthEffectBehavior.Source);
                break;
        }
        lastHealthEffect = Time.fixedTime;
    }

    private void SetHealth(float newHealth, HealthEffectSource? healthEffectSource = null)
    {
        health = Mathf.Clamp(newHealth, 0f, maxHealth);
        ui.healthGauge.SetVal(health, maxHealth);
        if (health <= 0)
        {
            DamageDeathEvent.Trigger(healthEffectSource);
            levelManager.Lose();
        }
    }
    
    private void Heal(int heal, HealthEffectSource source)
    {
        SetHealth(health + heal, source);
    }
    
    private void Damage(int dmg, HealthEffectSource source)
    {
        SetHealth(health - dmg, source);
    }
    
}

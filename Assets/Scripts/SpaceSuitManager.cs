using UnityEngine;

public class SpaceSuitManager : MonoBehaviour
{
    public float healthEffectInteractionDelay = 0.5f;
    public float maxHealth = 100f;
    
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
        // TODO: Check what the player is colliding with
        if (Time.fixedTime - lastHealthEffect <= healthEffectInteractionDelay) return;
        var healthEffectBehavior = other.gameObject.GetComponent<HealthEffectBehavior>();
        var healthEffectStrength = healthEffectBehavior.Strength;
        switch (healthEffectBehavior.Effect)
        {
            case HealthEffect.Heal:
                Heal(healthEffectStrength);
                break;
            case HealthEffect.Damage:
                Damage(healthEffectStrength);
                break;
        }
        lastHealthEffect = Time.fixedTime;
    }
    
    private void SetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
        ui.SetHealthBar(health, maxHealth);
        if (health <= 0) levelManager.Lose();
    }
    
    private void Heal(int heal)
    {
        SetHealth(health + heal);
    }
    
    private void Damage(int dmg)
    {
        SetHealth(health - dmg);
    }
    
}

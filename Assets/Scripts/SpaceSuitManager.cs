using UnityEngine;

public class SpaceSuitManager : MonoBehaviour
{
    public float healthEffectInteractionDelay = 2f;
    public float maxHealth = 100f;

    public UIManager UI;

    private float health = 0f;
    private float lastHealthEffect = -1f;
    private LevelManager _levelManager;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        SetHealth(maxHealth);
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO: Check what the player is colliding with
        if (Time.fixedTime - lastHealthEffect <= healthEffectInteractionDelay)
        {
            return;
        }
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
    }
    
    private void SetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
        UI.SetHealthBar(health, maxHealth);
    }
    
    private void Heal(int heal)
    {
        SetHealth(health + heal);
    }
    
    private void Damage(int dmg)
    {
        SetHealth(health - dmg);
        if (health <= 0)
        {
            _levelManager.Lose();
        }
    }
    
}

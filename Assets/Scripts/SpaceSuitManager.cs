using UnityEngine;

public class SpaceSuitManager : MonoBehaviour
{
    public float healthEffectInteractionDelay = 0.5f;
    public float maxHealth = 100f;
    
    private float health;
    private float lastHealthEffect;
    private LevelManager levelManager;
    private UIManager ui;
    private bool killOnNextHit;
    
    private void Start()
    {
        killOnNextHit = false;
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
        if (Time.fixedTime - lastHealthEffect <= healthEffectInteractionDelay) return;
        var healthEffectBehavior = healthEffectObj.GetComponent<HealthEffectBehavior>();
        if (!healthEffectBehavior) return;
        switch (healthEffectBehavior.Effect)
        {
            case HealthEffect.Heal:
                Heal(healthEffectBehavior.ComputeStrength(collision));
                break;
            case HealthEffect.Damage:
                Damage(healthEffectBehavior.ComputeStrength(collision));
                break;
        }
        lastHealthEffect = Time.fixedTime;
    }

    private void SetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0f, maxHealth);
        ui.healthGauge.SetVal(health, maxHealth);
        if (health <= 0) levelManager.Lose();
    }
    
    private void Heal(int heal)
    {
        SetHealth(health + heal);
    }
    
    private void Damage(int dmg)
    {
        if (killOnNextHit)
        {
            SetHealth(0);
        }
        else
        {
            SetHealth(health - dmg);
        }
    }

    public void SetKillOnNextHit(bool val)
    {
        killOnNextHit = val;
    }
}

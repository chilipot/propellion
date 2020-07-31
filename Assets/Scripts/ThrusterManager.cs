using UnityEngine;

public class ThrusterManager : MonoBehaviour
{
    private const KeyCode EngageKey = KeyCode.Space;
    
    public float power = 1000;
    public float maxCapacity = 30; // in seconds
    public AudioSource thrusterEngineSfx, thrusterEmptySfx;
    
    private bool engaged;
    private float capacity; // in seconds
    private LevelManager levelManager;
    private UIManager ui;

    private void Start()
    {
        engaged = false;
        capacity = maxCapacity;
        levelManager = FindObjectOfType<LevelManager>();
        ui = FindObjectOfType<UIManager>();
        ui.fuelGauge.SetVal(maxCapacity, maxCapacity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(EngageKey)) Engage();
        
        if (engaged)
        {
            if (levelManager.LevelIsOver()) Disengage();
            else if (capacity > 0) capacity -= 1 * Time.deltaTime;
            else
            {
                Disengage();
                capacity = 0;
                thrusterEmptySfx.Play();
            }
            ui.fuelGauge.SetVal(capacity, maxCapacity);
        }

        if (Input.GetKeyUp(EngageKey) && engaged) Disengage();
    }
    
    private void Engage()
    {
        if (levelManager.LevelIsOver()) return;
        if (capacity > 0)
        {
            engaged = true;
            thrusterEngineSfx.Play();
        }
        else thrusterEmptySfx.Play();
    }

    public void Disengage()
    {
        engaged = false;
        thrusterEngineSfx.Pause();
    }

    public bool IsEngaged()
    {
        return engaged;
    }
    
}

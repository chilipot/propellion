﻿using UnityEngine;

public class ThrusterManager : MonoBehaviour
{
    private const KeyCode EngageKey = KeyCode.Space;
    
    public float power = 10f;
    public float maxCapacity = 10f; // in seconds
    public AudioSource thrusterEngineSfx, thrusterEmptySfx;
    
    private bool engaged;
    private float capacity; // in seconds
    private UIManager ui;


    private void Start()
    {
        engaged = false;
        capacity = maxCapacity;
        ui = FindObjectOfType<UIManager>();
        ui.fuelGauge.SetVal(maxCapacity, maxCapacity);
    }

    public void Refuel(float refuelAmount)
    {
        SetCapacity(capacity + refuelAmount);
    }

    private void SetCapacity(float newCapacity)
    {
        capacity = Mathf.Clamp(newCapacity, 0, maxCapacity);
        ui.fuelGauge.SetVal(capacity, maxCapacity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(EngageKey)) Engage();
        
        if (engaged)
        {
            if (LevelManager.LevelInactive) Disengage();
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
        if (LevelManager.LevelInactive) return;
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

    public void Burst(float burstStrength)
    {
        if (LevelManager.LevelInactive) return;
        capacity -= 0.5f * burstStrength;
    }
    
}

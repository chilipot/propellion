﻿using UnityEngine;

public class ThrusterManager : MonoBehaviour
{
    private const KeyCode EngageKey = KeyCode.Space;
    
    public float power = 1000;
    public float maxCapacity = 30; // in seconds
    public AudioSource thrusterEngineSfx, thrusterEmptySfx;
    
    private bool engaged;
    private float capacity; // in seconds
    private UIManager ui;

    private void Start()
    {
        engaged = false;
        capacity = maxCapacity;
        ui = FindObjectOfType<UIManager>();
        ui.SetThrustCapacityBar(capacity, maxCapacity);
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
            ui.SetThrustCapacityBar(capacity, maxCapacity);
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
    
}

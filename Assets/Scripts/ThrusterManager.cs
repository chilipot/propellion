using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterManager : MonoBehaviour
{
    private const KeyCode EngageKey = KeyCode.Space;
    
    public float power = 1000;
    public float maxCapacity = 30; // in seconds
    public AudioSource thrusterEngineSfx, thrusterEmptySfx;
    public UIManager UI;
    
    private float _capacity; // in seconds
    private bool _engaged;

    private void EngageThruster()
    {
        if (_capacity > 0)
        {
            _engaged = true;
            thrusterEngineSfx.Play();
        }
        else
        {
            thrusterEmptySfx.Play();
        }
    }
    
    private void Start()
    {
        _engaged = false;
        _capacity = maxCapacity;
        UI.SetThrustCapacityBar(_capacity, maxCapacity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(EngageKey))
        {
            EngageThruster();
        }
        
        if (_engaged)
        {
            if (_capacity > 0)
            {
                _capacity -= 1 * Time.deltaTime;
            }
            else
            {
                _capacity = 0;
                _engaged = false;
                thrusterEngineSfx.Pause();
                thrusterEmptySfx.Play();
            }
            UI.SetThrustCapacityBar(_capacity, maxCapacity);
        }

        if (Input.GetKeyUp(EngageKey) && _engaged)
        {
            thrusterEngineSfx.Pause();
            _engaged = false;
        }
    }

    public bool IsEngaged()
    {
        return _engaged;
    }
    
}

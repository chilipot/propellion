using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterManager : MonoBehaviour
{
    private const KeyCode EngageKey = KeyCode.Space;
    
    public float power = 1000;
    public float maxCapacity = 30; // in seconds
    public AudioSource thrusterEngineSfx, thrusterEmptySfx;
    
    private float _capacity; // in seconds
    private bool _engaged;

    private void Start()
    {
        _engaged = false;
        _capacity = maxCapacity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(EngageKey))
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
        
        if (_engaged)
        {
            if (_capacity > 0)
            {
                _capacity -= 1 * Time.deltaTime;
                // TODO: update capacity UI
            }
            else
            {
                _capacity = 0;
                _engaged = false;
                thrusterEngineSfx.Pause();
                thrusterEmptySfx.Play();
                // TODO: highlight on UI that you're out of fuel
            }
        }

        if (Input.GetKeyUp(EngageKey))
        {
            if (_engaged)
            {
                thrusterEngineSfx.Pause();
                _engaged = false;
            }
        }
    }

    public bool IsEngaged()
    {
        return _engaged;
    }
    
}

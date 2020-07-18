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
    private LevelManager _levelManager;

    private void Start()
    {
        _engaged = false;
        _capacity = maxCapacity;
        _levelManager = FindObjectOfType<LevelManager>();
        UI.SetThrustCapacityBar(_capacity, maxCapacity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(EngageKey))
        {
            Engage();
        }
        
        if (_engaged)
        {
            if (_levelManager.LevelIsOver())
            {
                Disengage();
            }
            else if (_capacity > 0)
            {
                _capacity -= 1 * Time.deltaTime;
            }
            else
            {
                Disengage();
                _capacity = 0;
                thrusterEmptySfx.Play();
            }
            UI.SetThrustCapacityBar(_capacity, maxCapacity);
        }

        if (Input.GetKeyUp(EngageKey) && _engaged) Disengage();
    }
    
    private void Engage()
    {
        if (_levelManager.LevelIsOver()) return;
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

    public void Disengage()
    {
        _engaged = false;
        thrusterEngineSfx.Pause();
    }

    public bool IsEngaged()
    {
        return _engaged;
    }
    
}

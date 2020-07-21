﻿using UnityEngine;

public class BlackHoleConsumption : MonoBehaviour
{

    public BlackHolePull blackHolePull;
    
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: only consume when the colliding object is FULLY within the collider
        var consumedBody = other.gameObject.GetComponent<Rigidbody>();
        if (!consumedBody) return;
        if (other.gameObject.CompareTag("Player"))
        {
            // TODO: if this behavior is necessary past the prototype, make it a public method on a Player script, instead of sporadically duplicating this line
            consumedBody.constraints = RigidbodyConstraints.FreezeAll;
            levelManager.Lose();
        }
        else
        {
            blackHolePull.RemovePulledBody(consumedBody);
            Destroy(other.gameObject);
        }
    }
}

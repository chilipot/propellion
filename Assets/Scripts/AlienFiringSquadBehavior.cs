using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AlienFiringSquadBehavior : MonoBehaviour
{
    public GameObject AlienFiringSquad;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.PlayerRb.constraints = RigidbodyConstraints.FreezePosition;
            Instantiate(AlienFiringSquad,
                LevelManager.Player.position + LevelManager.Player.forward * 30,
                LevelManager.Player.rotation);
        }
    }
}
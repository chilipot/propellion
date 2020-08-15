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
            LevelManager.Player.GetComponent<SpaceSuitManager>().SetKillOnNextHit(true);
            LevelManager.PlayerRb.constraints = RigidbodyConstraints.FreezePosition;
            var firingSquad = Instantiate(AlienFiringSquad, 
                LevelManager.Player.position + LevelManager.Player.forward * 50,
                Quaternion.identity);
        }
    }
}

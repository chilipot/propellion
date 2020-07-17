using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSoundManager : MonoBehaviour
{
    private AudioSource collisionSfx;
    private void Start()
    {
        collisionSfx = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO: Check who this asteroid is colliding with
        if (!collisionSfx.isPlaying)
        {
            collisionSfx.Play();
        }
    }
}

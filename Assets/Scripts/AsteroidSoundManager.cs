using UnityEngine;

public class AsteroidSoundManager : MonoBehaviour
{
    private AudioSource collisionSfx;
    private void Start()
    {
        collisionSfx = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO: Check who this asteroid is colliding with
        if (!collisionSfx.isPlaying) collisionSfx.Play();
    }
}

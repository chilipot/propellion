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
        // TODO: fix audio clip being cut off when black hole destroys the asteroid, and similarly the occasional
        //       console warning saying it can't play from disabled AudioSource
        if (!other.gameObject.CompareTag("Player")) return;
        collisionSfx.PlayOneShot(collisionSfx.clip);
    }

}

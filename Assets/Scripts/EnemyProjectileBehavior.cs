using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
    public AudioClip hitSfx;

    private void Start()
    {
        transform.LookAt(LevelManager.Player);
    }

    private void Update()
    {
        var tf = transform;
        tf.position += Time.deltaTime * speed * tf.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: play a particle effect to represent projectile collision
        Debug.Log($"projectile hit {other.name} at {Time.time}!"); // TODO: delete
        if (other.CompareTag("Player"))
        {
            var hitSfxSource = AudioSourceExtension.PlayClipAndGetSource(hitSfx, transform.position);
            hitSfxSource.volume = 0.7f;
            hitSfxSource.minDistance = 10f;
            hitSfxSource.maxDistance = 500f;
            hitSfxSource.rolloffMode = AudioRolloffMode.Linear;
        }
        Destroy(gameObject);
    }
}
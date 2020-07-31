using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
    public AudioClip hitSfx;
    
    private AudioSource fireSfx;

    private void Start()
    {
        fireSfx = GetComponent<AudioSource>();
        transform.LookAt(GameObject.FindWithTag("Player").transform);
    }

    private void Update()
    {
        var tf = transform;
        tf.position += Time.deltaTime * speed * tf.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioSource.PlayClipAtPoint(hitSfx, transform.position);
        var destroyDelay = fireSfx.isPlaying ? fireSfx.clip.length - fireSfx.time : 0;
        Destroy(gameObject, destroyDelay);
    }
}
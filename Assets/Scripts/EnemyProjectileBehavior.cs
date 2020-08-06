using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    public float speed = 50f;
    public float range = 300f;
    [Tooltip("Degrees of rotation per second")]
    public float homingAmount = 200f;
    public AudioClip hitSfx;

    private float distanceTraveled;

    private void Start()
    {
        distanceTraveled = 0f;
        transform.LookAt(LevelManager.Player);
    }
    
    private void Update()
    {
        if (distanceTraveled >= range) Destroy(gameObject);
        else
        {
            var tf = transform;
            var currentRotation = tf.rotation;
            var rotationToPlayer = Quaternion.LookRotation(LevelManager.Player.position - tf.position);
            var degreesToPlayer = Quaternion.Angle(currentRotation, rotationToPlayer);
            var degreesToAdjust = Time.deltaTime * homingAmount;
            tf.rotation = Quaternion.Slerp(currentRotation, rotationToPlayer, degreesToAdjust / degreesToPlayer);
            
            var translationDistance = Time.deltaTime * speed;
            distanceTraveled += translationDistance;
            tf.Translate(translationDistance * Vector3.forward);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: play a particle effect to represent projectile collision
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
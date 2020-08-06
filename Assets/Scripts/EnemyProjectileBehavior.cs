using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    public float speed = 50f;
    public float range = 300f;
    [Tooltip("Degrees of rotation per second")]
    public float homingAmount = 200f;
    public AudioClip hitSfx;
    public GameObject hitVfx;

    private static readonly int ShouldShrink = Animator.StringToHash("shouldShrink");
    
    private float distanceTraveled;
    private bool outOfRange;

    private void Start()
    {
        distanceTraveled = 0f;
        outOfRange = false;
        
        transform.LookAt(LevelManager.Player);
    }
    
    private void Update()
    {
        if (!outOfRange && distanceTraveled >= range)
        {
            outOfRange = true;
            var shrinkAnimation = GetComponent<Animator>();
            shrinkAnimation.SetTrigger(ShouldShrink);
            var animationDuration = shrinkAnimation.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, animationDuration);
            GetComponent<Collider>().enabled = false;
        }
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
        if (other.CompareTag("BlackHole")) return;
        var tf = transform;
        Instantiate(hitVfx, tf.position, tf.rotation);
        if (other.CompareTag("Player"))
        {
            // TODO: adjust hitVfx transform so it's always on screen, but placed relative to the camera based on where the projectile came from
            var hitSfxSource = AudioSourceExtension.PlayClipAndGetSource(hitSfx, transform.position);
            hitSfxSource.volume = 0.7f;
            hitSfxSource.minDistance = 10f;
            hitSfxSource.maxDistance = 500f;
            hitSfxSource.rolloffMode = AudioRolloffMode.Linear;
        }
        Destroy(gameObject);
    }
}
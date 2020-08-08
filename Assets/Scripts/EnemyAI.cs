using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour, IGrappleResponse
{
    private enum State
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }
    
    public float speed = 15f;
    public float chaseDistance = 100f;
    public float attackDistance = 50f;
    public float chaseSpeedMultiplier = 3f;
    public float rotationSpeed = 5f;
    public Vector2 patrolPointDistanceRange = new Vector2(10f, 50f);
    public AudioClip stabSfx;
    public AudioMixerGroup stabSfxMixer;
    public GameObject deathVfxPrefab;
    public GameObject projectile;
    public GameObject muzzleFlash;
    public Transform gunTip;
    public float fireRate = 1f;
    
    private State currentState;
    private Vector3 currentDestination;
    private bool destinationIsPatrolPoint;
    private Transform player;
    private float distanceToPlayer;
    private bool isGrappled;
    private float lastFireTime;
    private AudioSource fireSfx;
    private AudioSource aggroSfx;
    private AudioSource screamSfx;
    private Transform mainCamera;
    private ThrusterParticleManager thrusterParticleManager;
    private Transform projectileParent;
    private ProceduralGeneration entityManager;
    private GrappleGunBehavior grapple;
    private Vector3? impulseVector;

    private void Start()
    {
        currentState = State.Patrol;
        currentDestination = transform.position;
        destinationIsPatrolPoint = false;
        player = LevelManager.Player;
        distanceToPlayer = Mathf.Infinity;
        isGrappled = false;
        lastFireTime = -fireRate;
        fireSfx = gunTip.GetComponent<AudioSource>();
        fireSfx.maxDistance = attackDistance * 2f;
        var audioSources= GetComponents<AudioSource>();
        aggroSfx = audioSources[0];
        aggroSfx.maxDistance = chaseDistance * 2f;
        screamSfx = audioSources[1];
        mainCamera = LevelManager.MainCamera.transform;
        thrusterParticleManager = GetComponentInChildren<ThrusterParticleManager>();
        projectileParent = GameObject.FindWithTag("ProjectileCollection").transform;
        entityManager = FindObjectOfType<ProceduralGeneration>();
        grapple = player.GetComponentInChildren<GrappleGunBehavior>();
        impulseVector = null;
    }

    private void Update()
    {
        if (!ProceduralGeneration.FinishedGenerating || currentState == State.Dead) return;
        if (LevelManager.LevelIsOver) currentState = State.Patrol;
        if (LevelManager.DebugMode) ListenForDebugClicks();
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        switch (currentState)
        {
            case State.Chase:
                UpdateChaseState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
            default:
                UpdatePatrolState();
                break;
        }
    }

    private void ListenForDebugClicks()
    {
        if (!Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit) || hit.transform != transform) return;
        if (Input.GetMouseButtonDown(0)) Debug.Log($"Current enemy AI state: {currentState}");
        if (Input.GetMouseButtonDown(1))
        {
            entityManager.RemoveEntity(gameObject);
            Destroy(gameObject);
        }
    }

    private void UpdatePatrolState()
    {
        if (!destinationIsPatrolPoint) SetRandomDestination();
        // TODO: update animation
        ApproachDestination();
        if (Vector3.Distance(transform.position, currentDestination) < 1f) SetRandomDestination();
        else if (!LevelManager.LevelIsOver && distanceToPlayer <= chaseDistance)
        {
            aggroSfx.PlayOneShot(aggroSfx.clip);
            currentState = State.Chase;
        }
    }
    
    private void UpdateChaseState()
    {
        // TODO: update animation
        SetPlayerDestination();
        ApproachDestination(chaseSpeedMultiplier);
        if (distanceToPlayer <= attackDistance) currentState = State.Attack;
        else if (distanceToPlayer > chaseDistance) currentState = State.Patrol;
    }
    
    private void UpdateAttackState()
    {
        // TODO: update animation
        if (thrusterParticleManager.ExhaustTrailActive) thrusterParticleManager.StopExhaustTrail();
        SetPlayerDestination();
        LookTowardsDestination();
        FireWeapon();
        if (distanceToPlayer > attackDistance) currentState = distanceToPlayer > chaseDistance ? State.Patrol : State.Chase;
    }

    private void SetRandomDestination()
    {
        // TODO: more intelligently choose a patrol point so that a point is not within an asteroid
        currentDestination = transform.position + Random.onUnitSphere * Random.Range(patrolPointDistanceRange[0], patrolPointDistanceRange[1]);
        destinationIsPatrolPoint = true;
    }

    private void SetPlayerDestination()
    {
        currentDestination = player.transform.position;
        destinationIsPatrolPoint = false;
    }
    
    private void ApproachDestination(float speedMultiplier = 1f)
    {
        LookTowardsDestination();
        if (isGrappled) return;
        if (!thrusterParticleManager.ExhaustTrailActive) thrusterParticleManager.StartExhaustTrail();
        // TODO: use a rigidbody force instead of changing transform.position?
        transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * speedMultiplier * Time.deltaTime); // TODO: uncomment
    }

    private void LookTowardsDestination()
    {
        var targetDirection = (currentDestination - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
    
    private void FireWeapon()
    {
        if (isGrappled || Time.time < lastFireTime + fireRate) return;
        lastFireTime = Time.time;
        // TODO: play animation
        Instantiate(projectile, gunTip.position, Quaternion.identity, projectileParent);
        Instantiate(muzzleFlash, gunTip);
        fireSfx.PlayOneShot(fireSfx.clip);
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentState == State.Dead || !other.CompareTag("SpaceKatana")) return;
        currentState = State.Dead;
        grapple.StopGrapple();
        var position = transform.position;
        var sliceSfxSource = AudioSourceExtension.PlayClipAndGetSource(stabSfx, position);
        sliceSfxSource.outputAudioMixerGroup = stabSfxMixer;
        screamSfx.Play();
        var deathVfx = Instantiate(deathVfxPrefab, position, Quaternion.identity);
        deathVfx.transform.LookAt(player);
        var hitDirection = (position - player.position).normalized;
        impulseVector = hitDirection * 50f;
        // TODO: update animation
        // TODO: corpse dissolve VFX
        // TODO: make not grappleable
    }

    private void FixedUpdate()
    {
        if (!impulseVector.HasValue) return;
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(impulseVector.Value, ForceMode.VelocityChange);
        impulseVector = null;
    }

    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, attackDistance);
    }
    
    public void OnGrappleStart()
    {
        isGrappled = true;
    }

    public void OnGrappleStop()
    {
        isGrappled = false;
        lastFireTime = Time.time;
    }
}
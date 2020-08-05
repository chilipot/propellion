using UnityEngine;
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
    public GameObject projectile;
    public Transform gunTip;
    public float fireRate = 1f;
    public AudioClip fireSfx;
    
    private State currentState;
    private Vector3 currentDestination;
    private bool destinationIsPatrolPoint;
    private Transform player;
    private float distanceToPlayer;
    private bool isGrappled;
    private float lastFireTime;
    private AudioSource fireAudioSource;
    private AudioSource alienAggroSfx;
    private Transform mainCamera;
    private ThrusterParticleManager thrusterParticleManager;

    private void Start()
    {
        currentState = State.Patrol;
        currentDestination = transform.position;
        destinationIsPatrolPoint = false;
        player = LevelManager.Player;
        distanceToPlayer = Mathf.Infinity;
        isGrappled = false;
        lastFireTime = -fireRate;
        fireAudioSource = gunTip.GetComponent<AudioSource>();
        fireAudioSource.maxDistance = attackDistance * 2f;
        alienAggroSfx = GetComponent<AudioSource>();
        alienAggroSfx.maxDistance = chaseDistance * 2f;
        mainCamera = LevelManager.MainCamera.transform;
        thrusterParticleManager = GetComponentInChildren<ThrusterParticleManager>();
    }

    private void Update()
    {
        if (!ProceduralGeneration.FinishedGenerating) return;
        if (LevelManager.LevelIsOver) currentState = State.Patrol;
        if (LevelManager.DebugMode) ListenForDebugClicks();
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrolState();
                break;
            case State.Chase:
                UpdateChaseState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    private void ListenForDebugClicks()
    {
        if (!Input.GetMouseButtonDown(0) ||
            !Physics.Raycast(mainCamera.position, mainCamera.forward, out var hit) ||
            hit.transform != transform) return; 
        Debug.Log($"Current enemy AI state: {currentState}");
    }

    private void UpdatePatrolState()
    {
        if (!destinationIsPatrolPoint) SetRandomDestination();
        // TODO: update animation
        ApproachDestination();
        if (Vector3.Distance(transform.position, currentDestination) < 1f) SetRandomDestination();
        else if (!LevelManager.LevelIsOver && distanceToPlayer <= chaseDistance)
        {
            alienAggroSfx.PlayOneShot(alienAggroSfx.clip);
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
        LookTowardsDestination();  // TODO: ApproachDestination() with a minimum distance to keep from it
        FireWeapon();
        if (distanceToPlayer > attackDistance) currentState = distanceToPlayer > chaseDistance ? State.Patrol : State.Chase;
    }
    
    private void UpdateDeadState()
    {
        // TODO (or remove this state if not necessary)
    }
    
    private void SetRandomDestination()
    {
        // TODO: more intelligently choose a patrol point so that a point is not within an asteroid
        currentDestination = transform.position + Random.onUnitSphere * Random.Range(10, 30); // TODO: make the range min/max inspector variables, if a similar methodology is maintained
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
        transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * speedMultiplier * Time.deltaTime); // TODO: use navmesh instead
    }

    private void LookTowardsDestination()
    {
        var targetDirection = (currentDestination - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime); // TODO: use a better T, within [0, 1]
    }
    
    private void FireWeapon()
    {
        if (isGrappled || Time.time < lastFireTime + fireRate) return;
        lastFireTime = Time.time;
        // TODO: play animation
        Instantiate(projectile, gunTip);
        fireAudioSource.PlayOneShot(fireSfx); // TODO: fix this sometimes not playing properly, and change fireSfx to fireAudioSource.clip if that still works
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
    }
}
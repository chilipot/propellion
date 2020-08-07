using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour, IGrappleResponse
{
    private enum State
    {
        Patrol,
        Chase,
        Attack
    }
    
    public float speed = 15f;
    public float chaseDistance = 150f;
    public float maxAttackDistance = 75f;
    public float minAttackDistance = 25f;
    public float chaseSpeedMultiplier = 3f;
    public float rotationSpeed = 5f;
    public Vector2 patrolPointDistanceRange = new Vector2(10f, 50f);
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
    private AudioSource alienAggroSfx;
    private Transform mainCamera;
    private ThrusterParticleManager thrusterParticleManager;
    private Transform projectileParent;
    private ProceduralGeneration entityManager;
    private Animator animator;

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
        fireSfx.maxDistance = maxAttackDistance * 2f;
        alienAggroSfx = GetComponent<AudioSource>();
        alienAggroSfx.maxDistance = chaseDistance * 2f;
        mainCamera = LevelManager.MainCamera.transform;
        thrusterParticleManager = GetComponentInChildren<ThrusterParticleManager>();
        projectileParent = GameObject.FindWithTag("ProjectileCollection").transform;
        entityManager = FindObjectOfType<ProceduralGeneration>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("Flying");
    }

    private void Update()
    {
        if (!ProceduralGeneration.FinishedGenerating) return;
        if (LevelManager.LevelIsOver)
        {
            currentState = State.Patrol;
            animator.ResetTrigger("Shooting");
            animator.ResetTrigger("Grappled");
            animator.SetTrigger("Flying");
        }
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
        SetPlayerDestination();
        ApproachDestination(chaseSpeedMultiplier);
        if (distanceToPlayer <= maxAttackDistance)
        {
            lastFireTime = Time.time - 0.5f; // TODO: don't hardcode the animation transition state time
            currentState = State.Attack;
            animator.SetTrigger("Shooting");
            // TODO: figure out whether resetting old triggers every time a new trigger is set is necessary (if not, remove all ResetTrigger calls, if it is, try just resetting previous trigger, not all triggers)
            animator.ResetTrigger("Flying");
            animator.ResetTrigger("Grappled");
        }
        else if (distanceToPlayer > chaseDistance) currentState = State.Patrol;
    }
    
    private void UpdateAttackState()
    {
        if (thrusterParticleManager.ExhaustTrailActive) thrusterParticleManager.StopExhaustTrail();
        SetPlayerDestination();
        
        // TODO: refine this so they aim to be in between states and not jitter, and also adjust thruster exhaust trail particles accordingly (based on when they are/aren't moving)
        if (distanceToPlayer > minAttackDistance) ApproachDestination();
        else LookTowardsDestination();
        
        FireWeapon();
        if (distanceToPlayer > maxAttackDistance)
        {
            currentState = distanceToPlayer > chaseDistance ? State.Patrol : State.Chase;
            animator.ResetTrigger("Shooting");
            animator.ResetTrigger("Grappled");
            animator.SetTrigger("Flying");
        }
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
        transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * speedMultiplier * Time.deltaTime);
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
        Instantiate(projectile, gunTip.position, Quaternion.identity, projectileParent);
        Instantiate(muzzleFlash, gunTip);
        fireSfx.PlayOneShot(fireSfx.clip);
    }

    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, maxAttackDistance);
    }
    
    // TODO: enter a Grappled state
    public void OnGrappleStart()
    {
        isGrappled = true;
        animator.SetTrigger("Grappled");
        animator.ResetTrigger("Flying");
        animator.ResetTrigger("Shooting");
    }

    // TODO: exit a Grappled state
    public void OnGrappleStop()
    {
        isGrappled = false;
        if (distanceToPlayer < maxAttackDistance)
        {
            animator.SetTrigger("Shooting");
            animator.ResetTrigger("Flying");
            animator.ResetTrigger("Grappled");
            lastFireTime = Time.time; // TODO: don't hardcode the animation transition state time
        }
        else
        {
            animator.SetTrigger("Flying");
            animator.ResetTrigger("Shooting");
            animator.ResetTrigger("Grappled");
        }
    }
}
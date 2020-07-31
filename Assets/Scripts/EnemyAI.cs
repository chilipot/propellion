using UnityEngine;

public class EnemyAI : MonoBehaviour, IGrappleResponder
{
    private enum State
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }
    
    public float speed = 5f;
    public float chaseDistance = 100f;
    public float attackDistance = 50f;
    public GameObject projectile;
    public Transform gunTip;
    public float fireRate = 1f;
    
    private State currentState;
    private Vector3 currentDestination;
    private Transform player;
    private float distanceToPlayer;
    private bool isGrappled;
    private float lastFireTime;

    private void Start()
    {
        currentState = State.Patrol;
        currentDestination = transform.position;
        player = GameObject.FindWithTag("Player").transform;
        distanceToPlayer = Mathf.Infinity;
        isGrappled = false;
        lastFireTime = -fireRate;
    }

    private void Update()
    {
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

    private void UpdatePatrolState()
    {
        // TODO: update animation
        ApproachDestination();
        if (Vector3.Distance(transform.position, currentDestination) < 1f) SetRandomDestination();
        else if (distanceToPlayer <= chaseDistance) currentState = State.Chase;
        
    }
    
    private void UpdateChaseState()
    {
        // TODO: update animation
        currentDestination = player.transform.position;
        ApproachDestination();
        if (distanceToPlayer <= attackDistance) currentState = State.Attack;
        else if (distanceToPlayer > chaseDistance) currentState = State.Patrol;
    }
    
    private void UpdateAttackState()
    {
        // TODO: update animation
        currentDestination = player.transform.position;
        LookTowardsDestination();  // TODO: ApproachDestination() with a minimum distance to keep from it
        FireWeapon();
        if (distanceToPlayer > attackDistance) currentState = distanceToPlayer > chaseDistance ? State.Patrol : State.Chase;
    }
    
    private void FireWeapon()
    {
        if (isGrappled || Time.time < lastFireTime + fireRate) return;
        lastFireTime = Time.time;
        Instantiate(projectile, gunTip);
        // TODO: play animation
    }
    
    private void UpdateDeadState()
    {
        // TODO (or delete if not necessary)
    }

    private void SetRandomDestination()
    {
        // TODO: more intelligently choose a patrol point as to not stupidly run into asteroids
        currentDestination = transform.position + Random.onUnitSphere * Random.Range(10, 30); // TODO: make the range min/max inspector variables, if a similar methodology is maintained
    }

    private void ApproachDestination()
    {
        LookTowardsDestination();
        if (!isGrappled)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime); // TODO: use navmesh instead
        }
    }

    private void LookTowardsDestination()
    {
        var targetDirection = (currentDestination - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime); // TODO: use a better T, within [0, 1]
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
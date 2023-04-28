using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD,
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]

public class EnemyController : MonoBehaviour
{
    protected EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator animator;
    protected CharacterStats enemyStats;
    private Collider collider;

    public GameObject bulletPrefab;
    public Transform generatePos;


    public float visionRange;   // range that enemy will find characters
    public float patrolRange;   // range that enemy patrol

    public bool isGuard;

    protected GameObject attackTarget;    // target to attack
    private float moveSpeed;    // enemy move speed

    public float dwellTime;    // patrol enemy dwell time at each stop point
    private float remainDwellTime;  // time to stay that point
    
    // judge animation states
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;

    private Vector3 wayPoint;   // new point to move in patrol
    private Vector3 startPos;   // enemy start pos
    private Quaternion startQuaternion; // enemy start quaternion

    private float attackCD;

    void Awake()    // initialization
    {
        animator = this.GetComponent<Animator>();  // get Animator Component      
        agent = this.GetComponent<NavMeshAgent>();   // get NavMeshAgent Component        
        enemyStats = this.GetComponent<CharacterStats>();   // get enemy data
        collider = this.GetComponent<Collider>();   // get enemy collider

        //enemyStats.CurrentHP = enemyStats.MaxHP;    // initialization hp
        moveSpeed = agent.speed;
        startPos = transform.position;
        startQuaternion = transform.rotation;
        remainDwellTime = dwellTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize enemy states
        if (isGuard)
            enemyStates = EnemyStates.GUARD;
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        attackCD -= Time.deltaTime;
        if (enemyStats.CurrentHP == 0)
            isDead = true;
        SwitchStates();
        SwitchAnimtaion();
    }

    void SwitchAnimtaion()  
    {
        // change animator settings
        animator.SetBool("Walk", isWalk);
        animator.SetBool("Chase", isChase);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Critical", enemyStats.isCritical);
        animator.SetBool("Die", isDead);
    }

    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FindCharacter())
            enemyStates = EnemyStates.CHASE;

        // switch enemy states
        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != startPos) // if enemy not in start position
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.SetDestination(startPos); // back to start position
                    if (Vector3.Distance(startPos, transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, startQuaternion, 0.1f);
                    }
                }
                break;

            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = moveSpeed * 0.5f; // patrol state with a lower move speed

                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)   // enemy reaches patrol border
                {
                    isWalk = false; // stop move
                    if (remainDwellTime > 0) 
                        remainDwellTime -= Time.deltaTime;
                    else
                    {
                        remainDwellTime = dwellTime;    // reset dwellTime
                        GetNewWayPoint();   // find a new point to move
                    }
                }
                else{   // enemy does not reach patrol border
                    isWalk = true;
                    agent.SetDestination(wayPoint);
                }
                break;

            case EnemyStates.CHASE:
                // change animation from guard to chase
                isWalk = false;
                isChase = true;
                
                if (!FindCharacter())   // character out the range
                {
                    isFollow = false;
                    if (remainDwellTime > 0) 
                    {
                        agent.SetDestination(transform.position);   // move to target
                        remainDwellTime -= Time.deltaTime;
                    }
                    else if (isGuard)   // back to guard state
                        enemyStates = EnemyStates.GUARD;
                    else    // back to patrol state
                        enemyStates = EnemyStates.PATROL;
                }
                else    // character in range
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.SetDestination(attackTarget.transform.position);
                }
                if (TargetInAttackRange() || TargetInSkillRange())  // if target in attack range or skill range
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (attackCD < 0)
                    {
                        attackCD = enemyStats.attackData.coolDown;  // set attack CD
                        enemyStats.isCritical = Random.value < enemyStats.attackData.criticalChance;    // check if attack is crit
                        Attack();
                    }
                }
                break;

            case EnemyStates.DEAD:
                collider.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void GetNewWayPoint()   // get new point to move in patrol case
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 newPoint = new Vector3(startPos.x + randomX, transform.position.y, startPos.z + randomZ);   // get a random new point
        NavMeshHit hit; // find new point without obstacle
        wayPoint = NavMesh.SamplePosition(newPoint, out hit, patrolRange, 1) ? hit.position : transform.position;   // judge new point walkable or not
    }

    void Attack()
    {
        // if (isDead) return;
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            if (TargetInAttackRange())
                animator.SetTrigger("Attack");
            if (TargetInSkillRange()) 
                animator.SetTrigger("Skill");
        }
    }

    public void Hit()
    { 
        if (isDead) return;
        // calculate damage when hit
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(enemyStats, targetStats);
        }
    }

    public void Shoot()
    {
        if (isDead) return;
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            // generate bullet
            var bullet = Instantiate(bulletPrefab, generatePos.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().attackTarget = attackTarget;
        }
    }

    bool FindCharacter()    // check is there characters in range
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionRange); // get colliders in area
        for (int i = 0; i < colliders.Length; i++) // can select up to 12 objs at once
        {
            if (colliders[i].CompareTag("Character"))   // check is there characters in colliders (range)
            {
                attackTarget = colliders[i].gameObject;   // if find, set it as attack target
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)   // judge target in attack range or not
            return (Vector3.Distance(attackTarget.transform.position, transform.position) <= (enemyStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius));
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)   // judge target in skill range or not
            return (Vector3.Distance(attackTarget.transform.position, transform.position) <= (enemyStats.attackData.skillRange + attackTarget.GetComponent<NavMeshAgent>().radius));
        else
            return false;
    }

    void OnDrawGizmosSelected() // draw vision range
    {
        if (isDead) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}

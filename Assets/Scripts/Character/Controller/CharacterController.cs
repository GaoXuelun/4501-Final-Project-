using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum CharacterType
{
    Hero,
    Footman,
    TrainingDummy,
    MaleCharacter,
    FemaleCharacter,
    Grunt,
    Golem,
    Wizard,
    FreeLich,
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]

public class CharacterController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    protected CharacterStats characterStats;
    private GameObject SelectedCircle;
    private Collider collider;
    private ResourceManager resourceManager;

    private Vector3 targetPosition;
    private Transform followTarget; // followed Target

    public CharacterType type;

    protected GameObject attackTarget;
    private float attackCD;

    bool isDead;

    void Awake()    // initialization
    {
        animator = this.GetComponentInChildren<Animator>(); // get Animator Component
        agent = this.GetComponent<NavMeshAgent>();   // get NavMeshAgent Component
        characterStats = this.GetComponent<CharacterStats>();   // get character data
        SelectedCircle = this.transform.Find("SelectedCircle").gameObject;  // get Child
        collider = this.GetComponent<Collider>();   // get enemy collider
        resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager == null)
        {
            Debug.LogError("Could not find ResourceManager object in scene!");
        }
        //characterStats.CurrentHP = characterStats.MaxHP; // initialization hp
        SetSelected(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // MouseManager.Instance.OnMouseClicked += Move;
    }

    // Update is called once per frame
    void Update()
    {
        attackCD -= Time.deltaTime;
        if (characterStats.CurrentHP == 0)
        {
            isDead = true;
            collider.enabled = false;
            agent.enabled = false;
            Destroy(gameObject, 2f);
        }
        SwitchAnimtaion();
        if (followTarget != null)   //Set follow target
            agent.SetDestination(followTarget.position);
    }
    
    public void SetSelected(bool isSelected) // Enable SelectedCircle when selected
    {
        if (isDead) return;
        if (SelectedCircle != null)
            SelectedCircle.SetActive(isSelected);
    }

    public void SwitchAnimtaion()   // control character to move
    {
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetBool("Die", isDead);
    }

    public void EventMove(Vector3 pos)   // control character to move
    {
        if (isDead) return;
        if (agent != null)
        {
            StopAllCoroutines();
            agent.isStopped = false;
            targetPosition = pos;
            agent.SetDestination(targetPosition);
            followTarget = null; // Reset follow target if it's moving to other position
        }
    }

    public void FollowMove(Transform target) // Make the character follow target
    {
        if (isDead) return;
        if (target != null)
            followTarget = target;        
    }

    // public void FollowMove(GameObject target)   // control character to move
    // {
    //     if (target != null)
    //     {
    //         attackTarget = target;
    //         characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;    // check if attack is crit
    //         StartCoroutine(MoveToFollowTarget());
    //     }
    // }

    public void EventAttack(GameObject target)   // control character to move
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;    // check if attack is crit
            StartCoroutine(MoveToAttackTarget());
        }
    }

    public void EventGold(GameObject target)   // control character to gather resources
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;    // check if attack is crit
            StartCoroutine(MoveToGold());
        }
    }
    public void EventGem(GameObject target)   // control character to gather resources
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;    // check if attack is crit
            StartCoroutine(MoveToGem());
        }
    }
    public void EventGas(GameObject target)   // control character to gather resources
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;    // check if attack is crit
            StartCoroutine(MoveToGas());
        }
    }
    public void EventAttackBase(GameObject target)   // control character to move
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;    // check if attack is crit
            StartCoroutine(MoveToAttackTargetBase());
        }
    }

    // IEnumerator MoveToFollowTarget()
    // {
    //     if (agent != null && attackTarget != null)
    //     {
    //         // keep move before arrive
    //         agent.isStopped = false;

    //         // move to target
    //         transform.LookAt(attackTarget.transform);
    //         // if target out the attack range
    //         while (Vector3.Distance(attackTarget.transform.position, transform.position) > (characterStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius) && agent != null)
    //         {                                         
    //             if (agent != null && attackTarget != null)     
    //                 agent.SetDestination(attackTarget.transform.position);  // move to target until target in attack range
    //             yield return null;
    //         }

    //         //  stop when in attack range
    //         agent.isStopped = true;
    //     }
    // }

    IEnumerator MoveToAttackTarget()
    {
        if (agent != null && attackTarget != null)
        {
            // keep move before arrive
            agent.isStopped = false;
            // if attack action, set stop distance equal attackTarget radius
            agent.stoppingDistance = attackTarget.GetComponent<CapsuleCollider>().radius;
            // move to target
            transform.LookAt(attackTarget.transform);
            // if target out the attack range
            while (Vector3.Distance(attackTarget.transform.position, transform.position) > (characterStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius) && agent != null)
            {                                         
                if (agent != null && attackTarget != null)     
                    agent.SetDestination(attackTarget.transform.position);  // move to target until target in attack range
                yield return null;
            }
            //  stop when in attack range
            agent.isStopped = true;
            // Attack if not in CD
            if (attackCD < 0)
            {
                animator.SetBool("Critical", characterStats.isCritical);
                animator.SetTrigger("Attack");
                attackCD = characterStats.attackData.coolDown;
            }
        }
    }
    IEnumerator MoveToGold()
    {
        if (agent != null && attackTarget != null)
        {
            // keep move before arrive
            agent.isStopped = false;
            // if attack action, set stop distance equal attackTarget radius
            agent.stoppingDistance = attackTarget.GetComponent<CapsuleCollider>().radius;
            // move to target
            transform.LookAt(attackTarget.transform);
            // if target out the attack range
            while (Vector3.Distance(attackTarget.transform.position, transform.position) > (characterStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius) && agent != null)
            {
                if (agent != null && attackTarget != null)
                    agent.SetDestination(attackTarget.transform.position);  // move to target until target in attack range
                yield return null;
            }
            //  stop when in attack range
            agent.isStopped = true;
            // Attack if not in CD
            if (attackCD < 0)
            {
                animator.SetBool("Critical", characterStats.isCritical);
                animator.SetTrigger("Attack");
                attackCD = characterStats.attackData.coolDown;
                int goldCost = 10;
                resourceManager.AddResources(goldCost, 0, 0);
            }
        }
    }
    IEnumerator MoveToGem()
    {
        if (agent != null && attackTarget != null)
        {
            // keep move before arrive
            agent.isStopped = false;
            // if attack action, set stop distance equal attackTarget radius
            agent.stoppingDistance = attackTarget.GetComponent<CapsuleCollider>().radius;
            // move to target
            transform.LookAt(attackTarget.transform);
            // if target out the attack range
            while (Vector3.Distance(attackTarget.transform.position, transform.position) > (characterStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius) && agent != null)
            {
                if (agent != null && attackTarget != null)
                    agent.SetDestination(attackTarget.transform.position);  // move to target until target in attack range
                yield return null;
            }

            //  stop when in attack range
            agent.isStopped = true;
            // Attack if not in CD
            if (attackCD < 0)
            {
                animator.SetBool("Critical", characterStats.isCritical);
                animator.SetTrigger("Attack");
                attackCD = characterStats.attackData.coolDown;
                int gemsCost = 10;
                resourceManager.AddResources(0, gemsCost, 0);
            }
        }
    }
    IEnumerator MoveToGas()
    {
        if (agent != null && attackTarget != null)
        {
            // keep move before arrive
            agent.isStopped = false;
            // if attack action, set stop distance equal attackTarget radius
            agent.stoppingDistance = attackTarget.GetComponent<CapsuleCollider>().radius;
            // move to target
            transform.LookAt(attackTarget.transform);
            // if target out the attack range
            while (Vector3.Distance(attackTarget.transform.position, transform.position) > (characterStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius) && agent != null)
            {
                if (agent != null && attackTarget != null)
                    agent.SetDestination(attackTarget.transform.position);  // move to target until target in attack range
                yield return null;
            }
            //  stop when in attack range
            agent.isStopped = true;
            // Attack if not in CD
            if (attackCD < 0)
            {
                animator.SetBool("Critical", characterStats.isCritical);
                animator.SetTrigger("Attack");
                attackCD = characterStats.attackData.coolDown;
                int gasCost = 10;
                resourceManager.AddResources(0, 0, gasCost);
            }
        }
    }
    IEnumerator MoveToAttackTargetBase()
    {
        if (agent != null && attackTarget != null)
        {
            // keep move before arrive
            agent.isStopped = false;
            // if attack action, set stop distance equal attackTarget radius
            agent.stoppingDistance = attackTarget.GetComponent<CapsuleCollider>().radius;
            // move to target
            transform.LookAt(attackTarget.transform);
            // if target out the attack range
            while (Vector3.Distance(attackTarget.transform.position, transform.position) > (characterStats.attackData.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius) && agent != null)
            {
                if (agent != null && attackTarget != null)
                    agent.SetDestination(attackTarget.transform.position);  // move to target until target in attack range
                yield return null;
            }
            //  stop when in attack range
            agent.isStopped = true;
            // Attack if not in CD
            if (attackCD < 0)
            {
                animator.SetBool("Critical", characterStats.isCritical);
                animator.SetTrigger("Attack");
                attackCD = characterStats.attackData.coolDown;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
    public void Hit()
    { 
        // if (isDead) return;
        // calculate damage when hit
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}

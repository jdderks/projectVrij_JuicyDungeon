using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using UnityEditor;

public enum EnemyState
{
	IDLE,
	WANDERING,
	CHASING,
	ATTACKING,
	DEAD
}

public class EnemyBehaviour : MonoBehaviour, IDamageable
{
    [Expandable] public ScriptableEnemyObject scriptableEnemy;

    #region Stats
    [Foldout("Enemy Stats")] public EnemyState state;
    [Foldout("Enemy Stats")] public EnemyType type;
    [Foldout("Enemy Stats")] public LayerMask targetLayer;
    [Foldout("Enemy Stats")] public GameObject prefabObject;
    [Space]
    [Foldout("Enemy Stats")] public float health;
    [Space]
    [Foldout("Enemy Stats")] public float walkSpeed;
    [Foldout("Enemy Stats")] public float runSpeed;
    [Space]
    [Foldout("Enemy Stats")] public float wanderIntervalMIN;
    [Foldout("Enemy Stats")] public float wanderIntervalMAX;
    [Foldout("Enemy Stats")] public float wanderRadius;
    [Space]
    [Foldout("Enemy Stats")] public float attackSpeed;
    [Foldout("Enemy Stats")] public float attackDamage;
    [Foldout("Enemy Stats")] public float attackDistance;
    [Space]
    [Foldout("Enemy Stats")] public float targetDetectionRadius;
    [Foldout("Enemy Stats")] public float targetDetectionInterval;
    [Foldout("Enemy Stats")] public float targetMaxChaseDistance;
    [Space]
    [Foldout("Enemy Stats")] public GameObject graphics;
    [Foldout("Enemy Stats")] public GameObject deathParticles;
    [Foldout("Enemy Stats")] public GameObject onHitParticles;
    #endregion

    #region Runtime Variables
    [SerializeField] private Vector3 startingPos = Vector3.zero;
    [SerializeField] private bool targetAcquired = false;
    [SerializeField] private GameObject target;
    [Space]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    #endregion

    [ContextMenu("Setup Stats")]
    /// <summary>
    /// A Simple method that sets all the stats to that of the scriptable object.
    /// </summary>
    public virtual void Setup()
    {
        // Setup all the Stats
        type = scriptableEnemy.type;
        targetLayer = scriptableEnemy.targetLayer;
        prefabObject = scriptableEnemy.prefabObject;

        health = scriptableEnemy.health;

        walkSpeed = scriptableEnemy.walkSpeed;
        runSpeed = scriptableEnemy.runSpeed;

        wanderIntervalMIN = scriptableEnemy.wanderIntervalMIN;
        wanderIntervalMAX = scriptableEnemy.wanderIntervalMAX;
        wanderRadius = scriptableEnemy.wanderRadius;

        attackSpeed = scriptableEnemy.attackSpeed;
        attackDamage = scriptableEnemy.attackDamage;
        attackDistance = scriptableEnemy.attackDistance;

        targetDetectionRadius = scriptableEnemy.targetDetectionRadius;
        targetDetectionInterval = scriptableEnemy.targetDetectionInterval;
        targetMaxChaseDistance = scriptableEnemy.targetMaxChaseDistance;

        graphics = scriptableEnemy.graphics;
        deathParticles = scriptableEnemy.deathParticles;
        onHitParticles = scriptableEnemy.onHitParticles;

        // Setup Components
        //GameObject prefabObjectInstance = Instantiate( prefabObject, transform.position, Quaternion.identity, transform );
        switch (type)
        {
            case EnemyType.Common:
                transform.localScale = Vector3.one;
                break;

            case EnemyType.Elite:
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;

            case EnemyType.Legendary:
                transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                break;

            case EnemyType.Boss:
                break;

            default:
                break;
        }

        anim = GetComponentInChildren<Animator>();

        SetBehaviour(EnemyState.IDLE);
        SetBehaviour(EnemyState.WANDERING);
    }

    public virtual void SetPosition(Vector3 pos)
    {
        agent.enabled = false;
        transform.position = pos;
        startingPos = pos;
        agent.enabled = true;
    }

    /// <summary>
    /// Updates the Animator Values.
    /// </summary>
    public virtual void UpdateAnimator()
    {
        anim.SetFloat("Velocity", agent.velocity.magnitude / walkSpeed);
    }

    /// <summary>
    /// Sets the enemy behaviour relative to the current state.
    /// </summary>
    /// <param name="_state"> current state. </param>
    public virtual void SetBehaviour(EnemyState _state)
    {
        state = _state;

        switch (state)
        {
            case EnemyState.IDLE:
                StartCoroutine(DetectTarget());
                break;

            case EnemyState.WANDERING:
                StartCoroutine(Wander());
                break;

            case EnemyState.CHASING:
                StartCoroutine(ChaseTarget());
                break;

            case EnemyState.ATTACKING:
                StartCoroutine(AttackTarget());
                break;

            case EnemyState.DEAD:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// IDamageable Take Damage Implementation.
    /// </summary>
    /// <param name="damage"> How much damage should be aplied. </param>
    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            state = EnemyState.DEAD;
        }
    }

    private void ReturnToStartingPos()
    {
        agent.destination = startingPos;
    }

    private IEnumerator Wander()
    {
        while (state == EnemyState.WANDERING)
        {
            NavMeshPath path = new NavMeshPath();
            Vector3 pathDestination = new Vector3
            {
                x = Random.Range(transform.position.x - wanderRadius, transform.position.x + wanderRadius),
                y = transform.position.y,
                z = Random.Range(transform.position.z - wanderRadius, transform.position.z + wanderRadius)
            };

            agent.CalculatePath(pathDestination, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(path);
            }
            else
            {
                Debug.Log("Invalid Path! Waiting for interval");
            }

            yield return new WaitForSeconds(Random.Range(wanderIntervalMIN, wanderIntervalMAX));
        }
    }

    /// <summary>
    /// The Attack loop. The first attack will have to yield at first to remove the possibility of enemies spam attacking the player upon moving.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackTarget()
    {
        while (state == EnemyState.ATTACKING)
        {
            agent.isStopped = true;

            if (Vector3.Distance(transform.position, target.transform.position) > attackDistance)
            {
                anim.SetBool("Melee Attack", false);
                SetBehaviour(EnemyState.CHASING);
            }
            else
            {
                anim.SetBool("Melee Attack", true);
            }
            
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    /// <summary>
    /// Sets the navmesh agent destination to that of the target.
    /// The destination gets set every 0.5 seconds as to avoid spamming the agent with commands.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChaseTarget()
    {
        while (state == EnemyState.CHASING)
        {
            agent.isStopped = false;

            if (target != null)
            {
                agent.destination = target.transform.position;
            }
            if (Vector3.Distance(transform.position, target.transform.position) >= targetMaxChaseDistance)
            {
                targetAcquired = false;
                SetBehaviour(EnemyState.IDLE);
                ReturnToStartingPos();
            }
            if (Vector3.Distance(transform.position, target.transform.position) <= attackDistance)
            {
                SetBehaviour(EnemyState.ATTACKING);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// A Simple target detection IEnumerator. It checks within a radius, within a layermask for colliders.
    /// It then loops through all colliders to see which one is the nearest one.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectTarget()
    {
        while (state == EnemyState.IDLE || !targetAcquired)
        {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, targetDetectionRadius, targetLayer);
            float dist = Mathf.Infinity;
            Transform nearestTransform = null;

            foreach (Collider collider in collidersInRange)
            {
                float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToTarget < dist)
                {
                    dist = distanceToTarget;
                    nearestTransform = collider.transform;
                    targetAcquired = true;
                    target = nearestTransform.gameObject;
                }
            }

            if (nearestTransform == null) targetAcquired = false;
            else SetBehaviour(EnemyState.CHASING);

            yield return new WaitForSeconds(targetDetectionInterval);
        }
    }

    public void PlayRunningAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy/Locomotion/Enemy_Footsteps_Running", transform.position);
    }

    public void PlayWalkingAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy/Locomotion/Enemy_Footsteps_Walking", transform.position);
    }

    public void PlaySwordSwingAudio()
    {
        if (Vector3.Distance(target.transform.position, transform.position) <= attackDistance)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy/Attacks/Melee/Enemy_Sword_Hit", transform.position);
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy/Attacks/Melee/Enemy_Sword_Miss", transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, targetDetectionRadius);

        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, attackDistance);
    }

    public void HitTarget()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= attackDistance)
        {
            target.GetComponent<IDamageable>()?.TakeDamage((int)attackDamage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;

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
	[Foldout( "Enemy Stats" )] public EnemyState state;
	[Foldout( "Enemy Stats" )] public EnemyType type;
	[Foldout( "Enemy Stats" )] public LayerMask targetLayer;
	[Foldout( "Enemy Stats" )] public GameObject prefabObject;
	[Space]
	[Foldout( "Enemy Stats" )] public float health;
	[Space]
	[Foldout( "Enemy Stats" )] public float walkSpeed;
	[Foldout( "Enemy Stats" )] public float runSpeed;
	[Space]
	[Foldout( "Enemy Stats" )] public float attackSpeed;
	[Foldout( "Enemy Stats" )] public float attackDamage;
	[Space]
	[Foldout( "Enemy Stats" )] public float targetDetectionRadius;
	[Foldout( "Enemy Stats" )] public float targetDetectionInterval;
	[Space]
	[Foldout( "Enemy Stats" )] public GameObject graphics;
	[Foldout( "Enemy Stats" )] public GameObject deathParticles;
	[Foldout( "Enemy Stats" )] public GameObject onHitParticles;
	#endregion

	#region Runtime Variables
	[SerializeField] private bool targetAcquired = false;
	[SerializeField] private GameObject target;
	[Space]
	[SerializeField] private NavMeshAgent agent;
	[SerializeField] private Animator anim;
	#endregion

	[ContextMenu( "Setup Stats" )]
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

		attackSpeed = scriptableEnemy.attackSpeed;
		attackDamage = scriptableEnemy.attackDamage;

		targetDetectionRadius = scriptableEnemy.targetDetectionRadius;
		targetDetectionInterval = scriptableEnemy.targetDetectionInterval;

		graphics = scriptableEnemy.graphics;
		deathParticles = scriptableEnemy.deathParticles;
		onHitParticles = scriptableEnemy.onHitParticles;

		// Setup Prefab Object
		Instantiate( prefabObject, transform );
		anim = GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// IDamageable Take Damage Implementation.
	/// </summary>
	/// <param name="damage"> How much damage should be aplied. </param>
	public virtual void TakeDamage( int damage )
	{
		health -= damage;

		if( health <= 0 )
		{
			state = EnemyState.DEAD;
		}
	}

	/// <summary>
	/// A Virtual Voide of the MoveToTargetLoop Method.
	/// </summary>
	public virtual void MoveToTarget()
	{
		state = EnemyState.CHASING;

		StartCoroutine( MoveToTargetLoop() );
	}

	/// <summary>
	/// Sets the navmesh agent destination to that of the target.
	/// The destination gets set every 0.5 seconds as to avoid spamming the agent with commands.
	/// </summary>
	/// <returns></returns>
	private IEnumerator MoveToTargetLoop()
	{
		while( state == EnemyState.CHASING )
		{
			if( target != null ) agent.destination = target.transform.position;

			yield return new WaitForSeconds( 0.5f );
		}
	}

	/// <summary>
	/// A Virtual Void of the DetectTargetLoop Method.
	/// </summary>
	public virtual void DetectTarget()
	{
		StartCoroutine( DetectTargetLoop() );
	}

	/// <summary>
	/// A Simple target detection IEnumerator. It checks within a radius, within a layermask for colliders.
	/// It then loops through all colliders to see which one is the nearest one.
	/// </summary>
	/// <returns></returns>
	private IEnumerator DetectTargetLoop()
	{
		while( !targetAcquired )
		{
			Collider[] collidersInRange = Physics.OverlapSphere( transform.position, targetDetectionRadius, targetLayer );
			float dist = Mathf.Infinity;
			Transform nearestTransform = null;

			foreach( Collider collider in collidersInRange )
			{
				float distanceToTarget = Vector3.Distance( transform.position, collider.transform.position );
				if( distanceToTarget < dist )
				{
					dist = distanceToTarget;
					nearestTransform = collider.transform;
					targetAcquired = true;
					target = nearestTransform.gameObject;
				}
			}

			if( nearestTransform == null ) targetAcquired = false;

			yield return new WaitForSeconds( targetDetectionInterval );
		}
	}
}

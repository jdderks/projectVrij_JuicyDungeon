using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
	Common, Elite, Legendary, Boss
}

[CreateAssetMenu( fileName = "ScriptableEnemy", menuName = "Scriptable Objects/New Scriptable Enemy" )]
public class ScriptableEnemyObject : ScriptableObject
{
	public EnemyType type = EnemyType.Common;
	public LayerMask targetLayer;
	public GameObject prefabObject;
	[Space]
	public float health = 100;
	[Space]
	public float walkSpeed = 5;
	public float runSpeed = 9;
	[Space]
	public float attackSpeed = 2;
	public float attackDamage = 10;
	public float attackDistance = 2;
	[Space]
	public float targetDetectionRadius = 15;
	public float targetDetectionInterval = 1;
	[Space]
	public GameObject graphics = default;
	public GameObject deathParticles = default;
	public GameObject onHitParticles = default;
}

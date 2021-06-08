using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
	[ProgressBar( "Current Health", "currentHealth", EColor.Red ), SerializeField] private float health = 100;

	[SerializeField] private Transform arrowSpawnPoint;
	[SerializeField] private ScriptableArrowObject arrowObject;

	private PlayerController controller;
	private float currentHealth = 100f;
	private Vector3 hitPosition = Vector3.zero;

	public Transform ArrowSpawnPoint { get => arrowSpawnPoint; set => arrowSpawnPoint =  value ; }
    public float Health { get => health; set => health = value; }

    private void Awake()
	{
		controller = GetComponent<PlayerController>();
	}

	public void TakeDamage( int damage )
	{
		Health -= damage;
		if( Health < 0 )
		{
			Die();
		}
	}

	public void Die()
	{
		SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
	}

	public void Shoot()
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;

		if( Physics.Raycast( ray, out hit, 100 ) )
		{
			hitPosition = new Vector3( hit.point.x, arrowSpawnPoint.position.y, hit.point.z );
			Debug.DrawLine( transform.position, hitPosition );
			SpawnArrow();
		}
	}

	public void SpawnArrow()
	{
		GameObject arrGO = new GameObject( "Arrow" );
		Arrow arr = arrGO.AddComponent<Arrow>();
		arr.Setup( this.gameObject, arrowSpawnPoint.position, hitPosition, arrowObject, 1 );

	}
}

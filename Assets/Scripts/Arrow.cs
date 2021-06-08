using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	private LayerMask detectionLayer;

	private GameObject prefabObject;
	private GameObject sender;
	private Rigidbody rigidbody;
	private BoxCollider collider;

	private int damage;
	private float force;

	private Vector3 position;
	private Vector3 prevPos;
	private Vector3 currPos;

	private int maxHits = 1;

	public void Setup( GameObject sender, Vector3 position, Vector3 targetPos, ScriptableArrowObject arrowObject, int maxHits = default )
	{
		this.sender = sender;
		this.prefabObject = arrowObject.prefabObject;

		this.position = position;
		this.force = arrowObject.force;
		this.detectionLayer = arrowObject.detectionLayer;
		this.damage = arrowObject.damage;
		this.maxHits = maxHits;

		collider = gameObject.AddComponent<BoxCollider>();
		collider.size = new Vector3( 0.1f, 0.1f, 1f );
		rigidbody = gameObject.AddComponent<Rigidbody>();
		collider.isTrigger = true;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		rigidbody.useGravity = false;
		transform.position = position;
		prevPos = transform.position;

		transform.LookAt( targetPos );

		rigidbody.AddForce( transform.forward * force );

		Destroy( gameObject, 10 );
		Instantiate( prefabObject, transform );
		FMODUnity.RuntimeManager.PlayOneShot( "event:/Effects/Bow/Bow_Shoot", transform.position );
	}


	void Update()
	{
		currPos = this.transform.position;
		Ray ray = new Ray( currPos, prevPos );
		if( Physics.Raycast( ray, out RaycastHit hit, detectionLayer ) )
		{
			//Debug.Log("Yeet");
		}
		prevPos = currPos;

		if( maxHits <= 0 )
		{
			Destroy( gameObject );
		}
	}

	private void OnTriggerEnter( Collider other )
	{
		if( other.gameObject == sender ) return;
		other.GetComponent<IDamageable>()?.TakeDamage( damage );
		maxHits--;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay( currPos, prevPos );
	}
}
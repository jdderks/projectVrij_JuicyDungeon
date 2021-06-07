using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamFollow : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float smoothing;
	[SerializeField] private Vector3 offset;

	private Vector3 velocity;
	private Vector3 desiredPos;
	private Vector3 smoothedPos;

	private void Update()
	{
		desiredPos = new Vector3( target.transform.position.x + offset.x, target.transform.position.y + offset.y, target.transform.position.z + offset.z );
		smoothedPos = Vector3.SmoothDamp( transform.position, desiredPos, ref velocity, smoothing * Time.deltaTime );
	}

	private void LateUpdate()
	{
		transform.position = smoothedPos;
	}
}

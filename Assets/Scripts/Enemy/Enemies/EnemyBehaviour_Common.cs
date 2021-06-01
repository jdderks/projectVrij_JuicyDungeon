using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_Common : EnemyBehaviour
{
	private void Start()
	{
		Setup();
	}

	private void Update()
	{
		UpdateAnimator();
	}
}

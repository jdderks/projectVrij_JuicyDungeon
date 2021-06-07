using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_Common_Melee : EnemyBehaviour
{
	private void Start()
	{
		Setup();
	}

	private void Update()
	{
		UpdateAnimator();
	}

	public void PlaySwordSwingAudio()
	{
		if( Vector3.Distance( target.transform.position, transform.position ) <= attackDistance )
		{
			FMODUnity.RuntimeManager.PlayOneShot( "event:/Enemy/Attacks/Melee/Enemy_Sword_Hit", transform.position );
		}
		else
		{
			FMODUnity.RuntimeManager.PlayOneShot( "event:/Enemy/Attacks/Melee/Enemy_Sword_Miss", transform.position );
		}
	}
}

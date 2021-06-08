using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_Boss : EnemyBehaviour
{
    [SerializeField]
    private bool isHalfHealth = false;
    private float startHealth;

    private void Start()
	{
		Setup();
        startHealth = health;
	}

	private void Update()
	{
		UpdateAnimator();
        ChargeUpWhenHalfHealth();

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

    public void ChargeUpWhenHalfHealth()
    {
        if (health < startHealth/2 && !isHalfHealth)
        {
            anim.SetTrigger("ChargeUp");
            isHalfHealth = true;
            agent.speed = 10f;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
	public Ability ability;
	public float cooldownTime;
	public float activeTime;
	public int uiSlotIndex = 0;

	public enum AbilityState
	{
		ready,
		active,
		cooldown
	}

	AbilityState state = AbilityState.ready;

	public KeyCode activateKey;

	private void Update()
	{
		switch( state )
		{
			case AbilityState.ready:
				if( Input.GetKeyDown( activateKey ) )
				{
					AbilityUIManager.Instance.SetUISlotCooldown( uiSlotIndex, ability.cooldownTime );
					ability.Activate();
					state = AbilityState.active;
					activeTime = ability.activeTime;
				}
				break;

			case AbilityState.active:
				if( activeTime > 0 )
				{
					activeTime -= Time.deltaTime;
				}
				else
				{
					state = AbilityState.cooldown;
					cooldownTime = ability.cooldownTime;
				}
				break;

			case AbilityState.cooldown:
				if( cooldownTime > 0 )
				{
					cooldownTime -= Time.deltaTime;
				}
				else
				{
					state = AbilityState.ready;
				}
				break;

			default:
				break;
		}
	}
}

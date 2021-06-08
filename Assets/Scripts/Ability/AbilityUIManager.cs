using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIManager : MonoBehaviour
{
	public static AbilityUIManager Instance;

	[SerializeField] private List<AbilityUISlot> slots = new List<AbilityUISlot>();

	private void Awake()
	{
		if( Instance == null || Instance != this )
		{
			Instance = this;
		}
	}

	public void SetUISlotCooldown( int index, float cooldownTime )
	{
		slots[index].cooldownTime = cooldownTime;

		slots[index].SetCooldownTimer();

		slots[index].onCooldown = true;
	}
}

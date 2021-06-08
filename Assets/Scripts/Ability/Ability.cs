using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
	public new string name;
	public int manaCost;
	public float cooldownTime;
	public float activeTime;

	public virtual void Activate()
	{
		FindObjectOfType<Player>().Mana -= manaCost;
	}
}

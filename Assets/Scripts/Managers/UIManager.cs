using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	[SerializeField] private Image playerHealthImage;
	[SerializeField] private Image playerManaImage;
	[SerializeField] private Player playerInstance;

	private float playerStartHealth;
	private float playerStartMana;

	public void SetPlayerInstance( Player player )
	{
		playerInstance = player;

		playerStartHealth = playerInstance.Health;
		playerStartMana = playerInstance.Mana;
	}

	void Update()
	{
		if( playerInstance )
		{
			playerHealthImage.fillAmount = playerInstance.Health / playerStartHealth;
			playerManaImage.fillAmount = playerInstance.Mana / playerStartMana;
		}
	}
}

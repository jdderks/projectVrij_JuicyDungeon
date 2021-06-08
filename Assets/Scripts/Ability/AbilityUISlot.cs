using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUISlot : MonoBehaviour
{
	public bool onCooldown = false;
	public float cooldownTime;

	[SerializeField] private Image abilityImage;
	[SerializeField] private TextMeshProUGUI cooldownText;

	private Color imageStartingColor;
	private Color imageCooldownColor;

	private float cooldownTimer;

	private void Start()
	{
		cooldownText.enabled = false;

		imageStartingColor = abilityImage.color;
		imageCooldownColor = new Color( imageStartingColor.r, imageStartingColor.g, imageStartingColor.b, 0.5f );
	}

	private void Update()
	{
		if( onCooldown )
		{
			cooldownTimer -= Time.deltaTime;

			cooldownText.enabled = true;
			cooldownText.text = cooldownTimer.ToString( "F0" );

			abilityImage.color = imageCooldownColor;

			if( cooldownTimer <= 0 )
			{
				cooldownText.enabled = false;
				onCooldown = false;
				cooldownTimer = cooldownTime;
				abilityImage.color = imageStartingColor;
			}
		}
	}

	public void SetCooldownTimer()
	{
		cooldownTimer = cooldownTime;
	}
}

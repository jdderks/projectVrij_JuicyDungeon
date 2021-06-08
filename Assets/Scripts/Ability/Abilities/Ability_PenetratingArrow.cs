using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "Penetrating Arrow", menuName = "Scriptable Objects/New Penetrating Arrow Ability" )]
public class Ability_PenetratingArrow : Ability
{
	public ScriptableArrowObject scriptableArrowObject;
	private Vector3 hitPosition;
	private int maxHits = 5;

	private Player player;

	public override void Activate()
	{
		base.Activate();

		player = FindObjectOfType<Player>();

		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;

		if( Physics.Raycast( ray, out hit, 100 ) )
		{
			hitPosition = new Vector3( hit.point.x, player.ArrowSpawnPoint.position.y, hit.point.z );
		}

		GameObject arrGO = new GameObject( "Penetrating Arrow" );
		Arrow arr = arrGO.AddComponent<Arrow>();
		arr.Setup( player.gameObject, player.ArrowSpawnPoint.position, hitPosition, scriptableArrowObject, maxHits );
	}
}

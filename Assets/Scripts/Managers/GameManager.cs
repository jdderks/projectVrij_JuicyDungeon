using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DungeonGenerationPathFirst;

public class GameManager : MonoBehaviour
{
	[BoxGroup( "Dungeon Generator" )] [SerializeField] private DungenPathFirst dungeonGenerator;

	[BoxGroup( "Prefabs" )] [SerializeField] private GameObject playerPrefab;
	[BoxGroup( "Prefabs" )] [SerializeField] private GameObject cameraPrefab;

	[Foldout( "Active Instances" )] [SerializeField] private GameObject playerInstance;
	[Foldout( "Active Instances" )] [SerializeField] private GameObject cameraInstance;

	private void Start()
	{
		dungeonGenerator.GenerateDungeon();

		SpawnPlayerInstance();

	}

	private void SpawnPlayerInstance()
	{
		Vector3 spawnPos = dungeonGenerator.GetRoomByType( RoomType.SPAWN ).Tiles[dungeonGenerator.GetRoomByType( RoomType.SPAWN ).Tiles.Count / 2].transform.position;

		playerInstance = Instantiate( playerPrefab, spawnPos, Quaternion.identity );
		cameraInstance = Instantiate( cameraPrefab, playerInstance.transform.position, Quaternion.identity );

		cameraInstance.GetComponent<CameraPivotBehaviour>().Player = playerInstance;
		playerInstance.GetComponent<PlayerController>().CameraTransform = cameraInstance.transform;
	}
}

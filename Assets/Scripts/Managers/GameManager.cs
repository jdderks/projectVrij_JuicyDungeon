using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DungeonGenerationPathFirst;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[BoxGroup( "Dungeon Generator" )] [SerializeField] private DungenPathFirst dungeonGenerator;

	[BoxGroup( "Prefabs" )] [SerializeField] private GameObject playerPrefab;
	[BoxGroup( "Prefabs" )] [SerializeField] private GameObject cameraPrefab;

	[Foldout( "Active Instances" )] [SerializeField] private GameObject playerInstance;
	[Foldout( "Active Instances" )] [SerializeField] private GameObject cameraInstance;
	[Foldout( "Active Instances" )] [SerializeField] private NavMeshBaker navMeshBakerInstance;

	private void Awake()
	{
		if( Instance == null || Instance != this )
		{
			Destroy( Instance );
			Instance = this;
		}
	}

	private void Start()
	{
		InstantiateNavMeshBaker();

		dungeonGenerator.GenerateDungeon();

		InstantiatePlayerInstance();
	}

	private void InstantiatePlayerInstance()
	{
		Vector3 spawnPos = dungeonGenerator.GetRoomByType( RoomType.SPAWN ).Tiles[dungeonGenerator.GetRoomByType( RoomType.SPAWN ).Tiles.Count / 2].transform.position;

		playerInstance = Instantiate( playerPrefab, spawnPos, Quaternion.identity );
		cameraInstance = Instantiate( cameraPrefab, playerInstance.transform.position, Quaternion.identity );

		cameraInstance.GetComponent<CameraPivotBehaviour>().Player = playerInstance;
		playerInstance.GetComponent<PlayerController>().CameraTransform = cameraInstance.transform;
	}

	[ContextMenu( "InstantiateNavMeshBaker" )]
	private void InstantiateNavMeshBaker()
	{
		GameObject navMeshBakerGO = new GameObject( "NavMesh Baker" );
		navMeshBakerInstance = navMeshBakerGO.AddComponent<NavMeshBaker>();
	}

	public void BakeNavMesh()
	{
		navMeshBakerInstance.Bake();
	}
}

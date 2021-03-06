using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DungeonGenerationPathFirst;

public class GameManager : MonoBehaviour
{
	[BoxGroup( "Dungeon Generator" )] [SerializeField] private DungenPathFirst dungeonGenerator;

	[BoxGroup( "Seperate Managers" )] [SerializeField] private UIManager uiManager;

	[BoxGroup( "Prefabs" )] [SerializeField] private GameObject playerPrefab;
	[BoxGroup( "Prefabs" )] [SerializeField] private GameObject cameraPrefab;

	[Foldout( "Active Instances" )] [SerializeField] private GameObject playerInstance;
	[Foldout( "Active Instances" )] [SerializeField] private GameObject cameraInstance;
	[Foldout( "Active Instances" )] [SerializeField] private NavMeshBaker navMeshBakerInstance;

	public static GameManager Instance { get; private set; }

	private void Awake()
	{
		if( Instance == null || Instance != this )
		{
			Destroy( Instance );
			Instance = this;
		}
	}

	[ContextMenu( "Start Game Setup" )]
	private void Start()
	{
		uiManager = FindObjectOfType<UIManager>();

		InstantiateNavMeshBaker();

		dungeonGenerator.GenerateDungeon();

		InstantiatePlayerInstance();
	}

	private void InstantiatePlayerInstance()
	{
		if( playerInstance == null )
		{
			Vector3 spawnPos = dungeonGenerator.GetRoomByType( RoomType.SPAWN ).Tiles[dungeonGenerator.GetRoomByType( RoomType.SPAWN ).Tiles.Count / 2].transform.position;

			playerInstance = Instantiate( playerPrefab, spawnPos, Quaternion.identity );
			cameraInstance = Instantiate( cameraPrefab, playerInstance.transform.position, Quaternion.identity );

			cameraInstance.GetComponent<SmoothCamFollow>().Target = playerInstance.transform;
			playerInstance.GetComponent<PlayerController>().CameraTransform = cameraInstance.transform;

			cameraInstance.transform.position = playerInstance.transform.position;

			uiManager.SetPlayerInstance( FindObjectOfType<Player>() );
		}
	}

	private void InstantiateNavMeshBaker()
	{
		if( navMeshBakerInstance == null )
		{
			GameObject navMeshBakerGO = new GameObject( "NavMesh Baker" );
			navMeshBakerInstance = navMeshBakerGO.AddComponent<NavMeshBaker>();
		}
	}

	public void BakeNavMesh()
	{
		navMeshBakerInstance.Bake();
	}
}

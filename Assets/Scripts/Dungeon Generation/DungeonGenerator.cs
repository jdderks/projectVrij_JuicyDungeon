//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using NaughtyAttributes;
//using Random = UnityEngine.Random;

///// <summary>
///// The Dungeon Generator class is responsible for generatin the entire level a.k.a. Dungeon.
///// This dungeon involves rooms and pathways to those rooms. Each room can contain: Enemies, Treasures, NPC and more.
///// </summary>
//public class DungeonGenerator : MonoBehaviour
//{
//	#region Private Variables
//	[SerializeField] private string seed = "";

//	[BoxGroup( "Room Settings" )] [SerializeField] private Transform roomParentTransform = default;         // Parent of the rooms in the scene.
//	[BoxGroup( "Room Settings" )] [SerializeField] private Vector2Int minRoomSize = Vector2Int.zero;        // Min Room size.
//	[BoxGroup( "Room Settings" )] [SerializeField] private Vector2Int maxRoomSize = Vector2Int.zero;        // Max Room size.

//	[BoxGroup( "Pathway Settings" )] [SerializeField] private Transform pathwayParentTransform = default;   // Parent of the pathways in the scene.
//	[BoxGroup( "Pathway Settings" )] [SerializeField] private int minPathwayCount = 15;                     // Minimum amount of pathways that will be generated.
//	[BoxGroup( "Pathway Settings" )] [SerializeField] private int maxPathwayCount = 30;                     // Maximum amount of pathways that will be generated.
//	[BoxGroup( "Pathway Settings" )] [SerializeField] private int minPathwayLength = 10;                    // Minimum length of the pathway before making a turn.
//	[BoxGroup( "Pathway Settings" )] [SerializeField] private int maxPathwayLength = 20;                    // Maximum length of the pathway before making a turn.

//	[BoxGroup( "Tile Objects" )] [SerializeField] private int tileScale;                                    // Scale of the Tiles.
//	[BoxGroup( "Tile Objects" )] [SerializeField] private GameObject[] tileGroundObjects = null;            // Tile Ground Objects.
//	[BoxGroup( "Tile Objects" )] [SerializeField] private GameObject[] tileWallObjects = null;              // Tile Wall Objects.

//	[BoxGroup( "Dungeon Objects" )] [SerializeField] private List<Room> Rooms = new List<Room>();           // List with all the rooms in the dungeon.
//	[BoxGroup( "Dungeon Objects" )] [SerializeField] private List<Tile> tiles = new List<Tile>();           // List with all the tiles in the dungeon.

//	private DateTime startTime; // At which time we started generating the dungeon.

//	private int roomIndex = 0;      // Index of the room (Used for giving the rooms their unique ID in their names).
//	private int pathwayIndex = 0;   // Index of the Pathway (Used for giving the Pathways their unique ID in their names).
//	#endregion

//	#region Public Properties
//	public string Seed { get => seed; set => seed = value; }
//	#endregion

//	#region Monobehaviour Callbacks
//	private void Awake()
//	{
//		if( seed == "" ) seed = Random.Range( 0, int.MaxValue ).ToString();

//		Random.InitState( seed.GetHashCode() );
//	}
//	private void Start()
//	{
//		startTime = DateTime.Now;
//		Debug.Log( "Generating Dungeon" );

//		GenerateDungeon();

//		Debug.Log( "Dungeon Generation Took: " + ( DateTime.Now - startTime ).Milliseconds + "ms" );
//	}
//	#endregion

//	#region Dungeon Generation
//	/// <summary>
//	/// The Generate Dungeon Function in theory is just a function that calls other functions.
//	/// Might sound a bit backwards, but this makes a nice readeable script layout.
//	/// </summary>
//	public void GenerateDungeon()
//	{
//		GeneratePathwaysWithRooms();
//		PlaceWalls(); // Highly Inefficient... But it works!
//	}

//	/// <summary>
//	/// This function generates a path in a random direction. After the path is generated, a room will be generated at the end of the path.
//	/// This goes on until we reach the amount of pathways chosen.
//	/// </summary>
//	private void GeneratePathwaysWithRooms()
//	{
//		// pick a random amount of pathways that need to be generated.
//		int pathwayAmount = Random.Range( minPathwayCount, maxPathwayCount );

//		// Store the previous pathway direction so we dont get overlapping pathways.

//		int previousPathwayDir = 1;
//		Vector2Int coordinates = new Vector2Int();
//		Vector2Int coordinatesDir = new Vector2Int();

//		for( int i = 0; i < pathwayAmount; i++ )
//		{
//			// Decide which way the dungeon should start going in.
//			// 1: Left, 2: Up, 3: Right, 4: Down.
//			// Keep generating a direction as long as it's the same as the previous one.
//			int pathwayStartingDirection = Random.Range( 1, 5 );

//			while( pathwayStartingDirection == 1 && previousPathwayDir == 3 || pathwayStartingDirection == 3 && previousPathwayDir == 1 ||
//				  pathwayStartingDirection == 2 && previousPathwayDir == 4 || pathwayStartingDirection == 4 && previousPathwayDir == 2 )
//			{
//				pathwayStartingDirection = Random.Range( 1, 5 );
//			}

//			// Set coordinates direction.
//			switch( pathwayStartingDirection )
//			{
//				case 1:
//					coordinatesDir = new Vector2Int( -1, 0 );
//					break;

//				case 2:
//					coordinatesDir = new Vector2Int( 0, 1 );
//					break;

//				case 3:
//					coordinatesDir = new Vector2Int( 1, 0 );
//					break;

//				case 4:
//					coordinatesDir = new Vector2Int( 0, -1 );
//					break;

//				default:
//					break;
//			}

//			// Store current direction into the previous direction variable.
//			previousPathwayDir = pathwayStartingDirection;

//			// Decide how long the pathway should be before generating a new one.
//			int pathwayLength = Random.Range( minPathwayLength, maxPathwayLength );

//			GameObject pathwayParent = new GameObject
//			{
//				name = "Pathway [" + pathwayIndex + "]",
//			};
//			pathwayParent.transform.parent = pathwayParentTransform;

//			// Generate the path for the generated length
//			// Make the path 5 wide. We dont have to worry about duplicate tiles because those won't get generated anyway.
//			for( int j = 0; j < pathwayLength; j++ )
//			{
//				GenerateGroundTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ), coordinates.y + ( coordinatesDir.y * j ) ), pathwayParent.transform );
//			}

//			// Create a room at the end of each pathway.
//			GenerateRoom( coordinates );
//			coordinates.x += coordinatesDir.x * pathwayLength;
//			coordinates.y += coordinatesDir.y * pathwayLength;

//			pathwayIndex++;
//		}
//		// Generate one final room for the final pathway to fix the dead end.
//		GenerateRoom( coordinates );
//	}

//	/// <summary>
//	/// Generates a room at the givin coordinates with the givin min/max roomsize.
//	/// </summary>
//	/// <param name="coordinates"> Room Starting Coordinates. </param>
//	private void GenerateRoom( Vector2Int coordinates )
//	{
//		roomIndex++;
//		GameObject newRoomGO = new GameObject { name = "Room [" + roomIndex + "]" };

//		newRoomGO.transform.parent = roomParentTransform;
//		newRoomGO.transform.position = new Vector3( coordinates.x, coordinates.y, 0 );
//		newRoomGO.AddComponent<Room>();

//		Room room = newRoomGO.GetComponent<Room>();

//		int roomSizeX = Random.Range( minRoomSize.x, maxRoomSize.x );
//		int roomSizeY = Random.Range( minRoomSize.y, maxRoomSize.y );

//		// Force the width and height to be an Odd number
//		if( roomSizeX % 2 == 0 )
//			roomSizeX -= 1;
//		if( roomSizeY % 2 == 0 )
//			roomSizeY -= 1;

//		room.RoomSize = new Vector2Int( roomSizeX, roomSizeY );
//		Rooms.Add( room );

//		for( int x = 0; x < roomSizeX; x++ )
//		{
//			for( int y = 0; y < roomSizeY; y++ )
//			{
//				GenerateGroundTile( "Tile [" + ( coordinates.x + x ) + "]" + " " + "[" + ( coordinates.y + y ) + "]", new Vector2Int( coordinates.x - ( roomSizeX / 2 ) + x, coordinates.y - ( roomSizeY / 2 ) + y ), newRoomGO.transform );
//			}
//		}
//	}

//	/// <summary>
//	/// Generates a Ground Tile with the given name, coordinates and parrentroom (if given)
//	/// </summary>
//	/// <param name="tileName"> The name of the tile. </param>
//	/// <param name="coordinates"> The coordinates of the tile. </param>
//	/// <param name="parentTransform"> The parentTransform of the tile. (This is not necessary!). </param>
//	private void GenerateGroundTile( string tileName, Vector2Int coordinates, Transform parentTransform )
//	{
//		// This removes a possible duplicate tile with the same coordinates.
//		// We could check for duplicate tile and return the function, but this makes the hierarchy cleaner.
//		for( int t = 0; t < tiles.Count; t++ )
//		{
//			if( tiles[t].Coordinates == coordinates )
//			{
//				GameObject tileOBJ = tiles[t].gameObject;
//				tiles.RemoveAt( t );
//				Destroy( tileOBJ );
//			}
//		}
//		int randIndex = Random.Range( 0, tileGroundObjects.Length );
//		GameObject newTileGO = Instantiate( tileGroundObjects[randIndex] );

//		Tile tile = newTileGO.AddComponent<Tile>();

//		tile.Coordinates = new Vector2Int( coordinates.x, coordinates.y );

//		newTileGO.transform.position = new Vector3( tile.Coordinates.x * tileScale, 0, tile.Coordinates.y * tileScale );

//		if( parentTransform )
//		{
//			parentTransform.GetComponent<Room>()?.Tiles.Add( tile );
//			newTileGO.transform.parent = parentTransform.transform;
//		}
//		tiles.Add( tile );
//	}

//	/// <summary>
//	/// The placewalls functions will loop through all the tiles in the dungeoon, look at their coordinates and the amount of neighbouring tiles.
//	/// Depending on which neighbouring tiles is missing, it places a wall.
//	/// </summary>
//	private void PlaceWalls()
//	{
//		for( int t = 0; t < tiles.Count; t++ )
//		{
//			List<Tile> neighbourTiles = new List<Tile>();
//			Tile tile = tiles[t];

//			int randIndex = Random.Range( 0, tileWallObjects.Length );

//			// Left, Right, Top and Bottom local Tiles.
//			Tile leftTile = null;
//			Tile rightTile = null;
//			Tile topTile = null;
//			Tile bottomTile = null;

//			// Top Left, Top Right, Bottom Left and Bottom Right Tiles.
//			Tile topLeftTile = null;
//			Tile topRightTile = null;
//			Tile bottomLeftTile = null;
//			Tile bottomRightTile = null;

//			#region Get neighbouring tiles
//			// Get all the neighbour tiles.
//			for( int i = 0; i < tiles.Count; i++ )
//			{
//				// Get Left tile
//				if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x - 1, tile.Coordinates.y ) )
//				{
//					leftTile = tiles[i];
//					neighbourTiles.Add( leftTile );
//				}

//				// Get Right tile
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x + 1, tile.Coordinates.y ) )
//				{
//					rightTile = tiles[i];
//					neighbourTiles.Add( rightTile );
//				}

//				// Get Up tile
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x, tile.Coordinates.y + 1 ) )
//				{
//					topTile = tiles[i];
//					neighbourTiles.Add( topTile );
//				}

//				// Get Down tile
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x, tile.Coordinates.y - 1 ) )
//				{
//					bottomTile = tiles[i];
//					neighbourTiles.Add( bottomTile );
//				}

//				// Get Top Left Tile
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x - 1, tile.Coordinates.y + 1 ) )
//				{
//					topLeftTile = tiles[i];
//					neighbourTiles.Add( topLeftTile );
//				}

//				// Get Top Right Tile
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x + 1, tile.Coordinates.y + 1 ) )
//				{
//					topRightTile = tiles[i];
//					neighbourTiles.Add( topRightTile );
//				}

//				// Get Bottom Left
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x - 1, tile.Coordinates.y - 1 ) )
//				{
//					bottomLeftTile = tiles[i];
//					neighbourTiles.Add( bottomLeftTile );
//				}

//				// Get Bottom Right
//				else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x + 1, tile.Coordinates.y - 1 ) )
//				{
//					bottomRightTile = tiles[i];
//					neighbourTiles.Add( bottomRightTile );
//				}
//			}

//			if( neighbourTiles.Count < 8 )
//			{
//				tile.Type = Tile.TileType.Wall;
//			}
//			#endregion

//			#region Left, Right, Top and Bottom checks
//			// Check if this tile is all the way in the left of a room. a.k.a. no Left neighbour.
//			if( leftTile == null && rightTile != null && topTile != null && bottomTile != null )
//			{
//			}

//			// Check if this tile is all the way in the Right of a room. a.k.a. no Right neighbour.
//			else if( leftTile != null && rightTile == null && topTile != null && bottomTile != null )
//			{
//			}

//			// Check if this tile is all the way in the Top of a room. a.k.a. no top neighbour.
//			else if( leftTile != null && rightTile != null && topTile == null && bottomTile != null )
//			{
//			}

//			// Check if this tile is all the way in the Bottom of a room. a.k.a. no bottom neighbour.
//			else if( leftTile != null && rightTile != null && topTile != null && bottomTile == null )
//			{
//			}
//			#endregion

//			#region Outer Corner Checks
//			// Top Left Outer Corner.
//			else if( leftTile == null && rightTile != null && topTile == null && bottomTile != null )
//			{
//			}

//			// Top Right Outer Corner.
//			else if( leftTile != null && rightTile == null && topTile == null && bottomTile != null )
//			{
//			}

//			// Bottom Left Outer Corner.
//			else if( leftTile == null && rightTile != null && topTile != null && bottomTile == null )
//			{
//			}

//			// Bottom Right Outer Corner.
//			else if( leftTile != null && rightTile == null && topTile != null && bottomTile == null )
//			{
//			}
//			#endregion

//			#region Inner Corner Checks
//			// Top Left Inner Corner
//			// We use a right sprite because the tile might be topleft. But the Mesh is top right because it's an inside corner.
//			else if( topLeftTile == null )
//			{
//			}
//			else if( topRightTile == null )
//			{
//			}
//			else if( bottomLeftTile == null )
//			{
//			}
//			else if( bottomRightTile == null )
//			{
//			}
//			#endregion
//		}
//	}
//	#endregion
//}

//[System.Serializable]
//public class Tile : MonoBehaviour
//{
//	#region Private Variables
//	public enum TileType
//	{
//		Ground,
//		Wall,
//		Corner
//	}

//	[SerializeField] private TileType type = TileType.Ground;
//	[SerializeField] private Vector2Int coordinates = Vector2Int.zero;
//	#endregion

//	#region Public Properties
//	public TileType Type { get => type; set => type = value; }
//	public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
//	#endregion
//}

//[System.Serializable]
//public class Room : MonoBehaviour
//{
//	#region Private Variables
//	[SerializeField] private Vector2Int roomSize = new Vector2Int();
//	[SerializeField] private List<Tile> tiles = new List<Tile>();
//	#endregion

//	#region Public Properties
//	public List<Tile> Tiles { get => tiles; set => tiles = value; }
//	public Vector2Int RoomSize { get => roomSize; set => roomSize = value; }
//	#endregion
//}

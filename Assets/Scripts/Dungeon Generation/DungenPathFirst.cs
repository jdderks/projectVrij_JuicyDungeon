using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

namespace DungeonGenerationPathFirst
{
	/// <summary>
	/// The Dungeon Generator class is responsible for generatin the entire level a.k.a. Dungeon.
	/// This dungeon involves rooms and pathways to those rooms. Each room can contain: Enemies, Treasures, NPC and more.
	/// </summary>
	public class DungenPathFirst : MonoBehaviour
	{
		[BoxGroup( "Generation Settings" )] [SerializeField] private bool randomizeSeed = false;               // Determines if the seed should be randomized each time.
		[BoxGroup( "Generation Settings" )] [SerializeField] private string seed = "";
		[BoxGroup( "Generation Settings" )] [SerializeField] private Transform roomParentTransform = default;   // Parent of the rooms in the scene.
		[BoxGroup( "Generation Settings" )] [SerializeField] private Vector2Int minRoomSize = Vector2Int.zero;  // Min Room size.
		[BoxGroup( "Generation Settings" )] [SerializeField] private Vector2Int maxRoomSize = Vector2Int.zero;  // Max Room size.
		[BoxGroup( "Generation Settings" )] [SerializeField] private Transform pathwayParentTransform = default;    // Parent of the pathways in the scene.
		[BoxGroup( "Generation Settings" )] [SerializeField] private int minPathwayCount = 15;                  // Minimum amount of pathways that will be generated.
		[BoxGroup( "Generation Settings" )] [SerializeField] private int maxPathwayCount = 30;                  // Maximum amount of pathways that will be generated.
		[BoxGroup( "Generation Settings" )] [SerializeField] private int minPathwayLength = 10;                 // Minimum length of the pathway before making a turn.
		[BoxGroup( "Generation Settings" )] [SerializeField] private int maxPathwayLength = 20;                 // Maximum length of the pathway before making a turn.

		[BoxGroup( "Tile Objects" )] [SerializeField] private int tileSize = 1;                                 // Tile Size.
		[BoxGroup( "Tile Objects" )] [SerializeField] private List<GameObject> tileGroundObjects = new List<GameObject>();              // Tile Ground Objects.
		[BoxGroup( "Tile Objects" )] [SerializeField] private List<GameObject> tileWallObjects = new List<GameObject>();                // Tile Wall Left Objects.
		[BoxGroup( "Tile Objects" )] [SerializeField] private List<GameObject> tileOuterCornerObjects = new List<GameObject>();         // Tile Outer Corner Left Sprite
		[BoxGroup( "Tile Objects" )] [SerializeField] private List<GameObject> tileInnerCornerObjects = new List<GameObject>();         // Tile Inner Corner Left Sprite

		[BoxGroup( "Enemy Settings" )] [SerializeField] private Transform enemyParentTransform = default;               // The parent transform of the enemies.
		[BoxGroup( "Enemy Settings" )] [SerializeField] private List<EnemyList> enemyLists = new List<EnemyList>();     // List with Enemy Lists. Within these lists are the enemies that can be spawned per theme.
		[BoxGroup( "Enemy Settings" )] [SerializeField] private int spawnChance = 1;                                    // How much percentage chance there is to spawn an enemy.

		[BoxGroup( "Generated Assets" )] [SerializeField] private List<Room> Rooms = new List<Room>();       // List with all the rooms in the dungeon.
		[BoxGroup( "Generated Assets" )] [SerializeField] private List<Tile> tiles = new List<Tile>();       // List with all the tiles in the dungeon.

		[BoxGroup( "Debugging" )] [SerializeField] private bool renderDungeonAsGizmos = false;      // Render the entire dungeon using gizmos.

		private DateTime startTime; // At which time we started generating the dungeon.

		private int roomIndex = 0;      // Index of the room (Used for giving the rooms their unique ID in their names).
		private int pathwayIndex = 0;   // Index of the Pathway (Used for giving the Pathways their unique ID in their names).

		public string Seed { get => seed; set => seed = value; }

		private void Start()
		{
			GenerateDungeon();
		}

		/// <summary>
		/// The Generate Dungeon Function in theory is just a function that calls other functions.
		/// Might sound a bit backwards, but this makes a nice readeable script layout.
		/// </summary>
		public void GenerateDungeon()
		{
			if( randomizeSeed ) seed = Random.Range( 0, int.MaxValue ).ToString();
			Random.InitState( seed.GetHashCode() );

			startTime = DateTime.Now;

			ClearDungeon();

			Debug.Log( "Generating Dungeon" );
			GeneratePathways();
			GeneratePlayerSpawnRoom();
			GenerateBossRoom();

			// Highly Inefficient... But it works!
			SetTilesType();

			InstantiateTilesGraphic();

			//SpawnEnemiesInsideTheRooms();

			Debug.Log( "Dungeon Generation Took: " + ( DateTime.Now - startTime ).Milliseconds + "ms" );
		}

		/// <summary>
		/// Clears the entire dungeon. Mainly used for debugging purposes.
		/// </summary>
		public void ClearDungeon()
		{
			// Get any left behind room objects and put them in a temp list.
			List<GameObject> roomChildren = new List<GameObject>();
			for( int rc = 0; rc < roomParentTransform.childCount; rc++ )
			{
				roomChildren.Add( roomParentTransform.GetChild( rc ).gameObject );
			}

			// Get any left behind Pathway objects and put them in a temp list.
			List<GameObject> pathwayChildren = new List<GameObject>();
			for( int pc = 0; pc < pathwayParentTransform.childCount; pc++ )
			{
				pathwayChildren.Add( pathwayParentTransform.GetChild( pc ).gameObject );
			}

			// Destroy all room and Pathways children. (execute order 66)
			foreach( GameObject roomChildGO in roomChildren )
			{
				DestroyImmediate( roomChildGO );
			}
			foreach( GameObject pathwayChildGO in pathwayChildren )
			{
				DestroyImmediate( pathwayChildGO );
			}

			roomIndex = 0;
			pathwayIndex = 0;

			Rooms.Clear();
			tiles.Clear();

			roomChildren.Clear();
			pathwayChildren.Clear();
		}



		/// <summary>
		/// This function generates a path in a random direction. After the path is generated, a room will be generated at the end of the path.
		/// This goes on until we reach the amount of pathways chosen.
		/// </summary>
		private void GeneratePathways()
		{
			// pick a random amount of pathways that need to be generated.
			int pathwayAmount = Random.Range( minPathwayCount, maxPathwayCount );
			// Store the previous pathway direction so we dont get overlapping pathways.
			int previousPathwayDir = 1;
			Vector2Int coordinates = new Vector2Int();
			Vector2Int coordinatesDir = new Vector2Int();
			for( int i = 0; i < pathwayAmount; i++ )
			{
				// Decide which way the dungeon should start going in.
				// 1: Left, 2: Up, 3: Right, 4: Down.
				// Keep generating a direction as long as it's the same as the previous one.
				int pathwayStartingDirection = Random.Range( 1, 5 );
				while( pathwayStartingDirection == 1 && previousPathwayDir == 3 || pathwayStartingDirection == 3 && previousPathwayDir == 1 ||
					  pathwayStartingDirection == 2 && previousPathwayDir == 4 || pathwayStartingDirection == 4 && previousPathwayDir == 2 )
				{
					pathwayStartingDirection = Random.Range( 1, 5 );
				}

				//Debug.Log(previousPathwayDir + " " + pathwayStartingDirection);

				// Set coordinates direction.
				switch( pathwayStartingDirection )
				{
					case 1:
						coordinatesDir = new Vector2Int( -1, 0 );
						break;

					case 2:
						coordinatesDir = new Vector2Int( 0, 1 );
						break;

					case 3:
						coordinatesDir = new Vector2Int( 1, 0 );
						break;

					case 4:
						coordinatesDir = new Vector2Int( 0, -1 );
						break;

					default:
						break;
				}

				// Store current direction into the previous direction variable.
				previousPathwayDir = pathwayStartingDirection;

				// Decide how long the pathway should be before generating a new one.
				int pathwayLength = Random.Range( minPathwayLength, maxPathwayLength );

				GameObject pathwayGO = new GameObject
				{
					name = "Pathway [" + pathwayIndex + "]",
				};
				pathwayGO.transform.parent = pathwayParentTransform;


				// Generate the path for the generated length
				// Make the path 3 wide. We dont have to worry about duplicate tiles because those won't get generated anyway.
				for( int j = 0; j < pathwayLength; j++ )
				{
					CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ), coordinates.y + ( coordinatesDir.y * j ) ), pathwayGO.transform );
					CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ) - 1, coordinates.y + ( coordinatesDir.y * j ) - 1 ), pathwayGO.transform );
					CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ) + 1, coordinates.y + ( coordinatesDir.y * j ) + 1 ), pathwayGO.transform );
				}

				// Create a room at the end of each pathway.
				GenerateRoom( coordinates, minRoomSize, maxRoomSize );
				coordinates.x += coordinatesDir.x * pathwayLength;
				coordinates.y += coordinatesDir.y * pathwayLength;

				pathwayIndex++;
			}
			// Generate one final room for the final pathway to fix the dead end.
			GenerateRoom( coordinates, minRoomSize, maxRoomSize );
		}

		/// <summary>
		/// Generates a room at the givin coordinates with the givin min/max roomsize.
		/// </summary>
		/// <param name="coordinates"> Room Starting Coordinates. </param>
		private void GenerateRoom( Vector2Int coordinates, Vector2Int _roomsizeMIN, Vector2Int _roomsizeMAX, string roomName = default, RoomType roomType = RoomType.NONE )
		{
			roomIndex++;

			int roomSizeX = Random.Range( _roomsizeMIN.x, _roomsizeMAX.x );
			int roomSizeY = Random.Range( _roomsizeMIN.y, _roomsizeMAX.y );

			// Force the width and height to be an Odd number
			if( roomSizeX % 2 == 0 )
				roomSizeX -= 1;
			if( roomSizeY % 2 == 0 )
				roomSizeY -= 1;

			GameObject roomGO = new GameObject();
			Room room = roomGO.AddComponent<Room>();

			room.Name = ( roomName == default ) ? "Room [" + roomIndex + "]" : roomName;
			room.Coordinates = coordinates;
			room.RoomSize = new Vector2Int( roomSizeX, roomSizeY );
			room.Type = roomType;

			if( roomType == RoomType.NONE )
			{
				int randInt = Random.Range( 0, 50 );

				if( randInt > 0 && randInt < 15 ) room.Type = RoomType.ENEMYSPAWN;
				else if( randInt > 15 && randInt < 35 ) room.Type = RoomType.HUB;
				else if( randInt > 35 && randInt < 45 ) room.Type = RoomType.SHOP;
				else if( randInt > 45 && randInt < 50 ) room.Type = RoomType.TREASURE;
			}

			roomGO.name = room.Name;
			roomGO.transform.position = new Vector3Int( coordinates.x, 0, coordinates.y );
			roomGO.transform.parent = roomParentTransform;

			Rooms.Add( room );

			for( int x = 0; x < roomSizeX; x++ )
			{
				for( int y = 0; y < roomSizeY; y++ )
				{
					CreateTile( "Tile [" + ( coordinates.x + x ) + "]" + " " + "[" + ( coordinates.y + y ) + "]", new Vector2Int( coordinates.x - ( roomSizeX / 2 ) + x, coordinates.y - ( roomSizeY / 2 ) + y ), roomGO.transform );
				}
			}
		}

		/// <summary>
		/// After all the main rooms have been generated, we generate the player spawn room.
		/// The player spawn room will be generated next to the room that is the furthest away from the center of the map.
		/// </summary>
		private void GeneratePlayerSpawnRoom()
		{
			float distance = 0;
			Vector3 dir = Vector3.zero;
			Room froom = Rooms[0];

			foreach( Room room in Rooms )
			{
				if( Vector3.Distance( room.transform.position, Vector3.zero ) > distance )
				{
					distance = Vector3.Distance( room.transform.position, Vector3.zero );
					dir = Vector3.zero - room.transform.position;
					froom = room;
				}
			}

			Debug.Log( "Dir: " + dir );

			Vector2Int coordinates = froom.Coordinates;
			Vector2Int coordinatesDir = Vector2Int.zero;

			if( dir.x >= 0 && dir.z >= 0 )
			{
				coordinatesDir = new Vector2Int( -1, 0 );
			}
			else if( dir.x <= 0 && dir.z >= 0 )
			{
				coordinatesDir = new Vector2Int( 0, -1 );
			}
			else if( dir.x >= 0 && dir.z <= 0 )
			{
				coordinatesDir = new Vector2Int( 0, 1 );
			}
			else if( dir.x <= 0 && dir.z <= 0 )
			{
				coordinatesDir = new Vector2Int( 1, 0 );
			}

			// Decide how long the pathway should be before generating a new one.
			int pathwayLength = Random.Range( minPathwayLength, maxPathwayLength );

			GameObject pathwayGO = new GameObject
			{
				name = "Pathway [" + pathwayIndex + "]",
			};
			pathwayGO.transform.parent = pathwayParentTransform;

			// Generate the path for the generated length
			// Make the path 3 wide. We dont have to worry about duplicate tiles because those won't get generated anyway.
			for( int j = 0; j < pathwayLength; j++ )
			{
				CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ), coordinates.y + ( coordinatesDir.y * j ) ), pathwayGO.transform );
				CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ) - 1, coordinates.y + ( coordinatesDir.y * j ) - 1 ), pathwayGO.transform );
				CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ) + 1, coordinates.y + ( coordinatesDir.y * j ) + 1 ), pathwayGO.transform );
			}

			// Create a room at the end of each pathway.
			coordinates.x += coordinatesDir.x * pathwayLength;
			coordinates.y += coordinatesDir.y * pathwayLength;
			GenerateRoom( coordinates, minRoomSize, maxRoomSize, "Room[PLAYER SPAWN]", RoomType.SPAWN );

			pathwayIndex++;
		}

		/// <summary>
		/// After all the main rooms have been generated, we generate the Boss room.
		/// The Boss room will be generated furthest away from the player spawn.
		/// </summary>
		private void GenerateBossRoom()
		{
			float distance = 0;
			Vector3 dir = Vector3.zero;
			Room froom = Rooms[0];

			foreach( Room room in Rooms )
			{
				if( Vector3.Distance( room.transform.position, Rooms[Rooms.Count - 1].transform.position ) > distance )
				{
					distance = Vector3.Distance( room.transform.position, Rooms[Rooms.Count - 1].transform.position );
					dir = room.transform.position - Rooms[Rooms.Count - 1].transform.position;
					froom = room;
				}
			}

			//int roomIndex = Rooms.IndexOf( froom ) - 1;
			//dir = Rooms[roomIndex].transform.position - froom.transform.position;

			Debug.Log( "Dir: " + dir );

			Vector2Int coordinates = froom.Coordinates;
			Vector2Int coordinatesDir = Vector2Int.zero;

			if( dir.x >= 0 && dir.z >= 0 )
			{
				coordinatesDir = new Vector2Int( 1, 0 );
			}
			else if( dir.x <= 0 && dir.z >= 0 )
			{
				coordinatesDir = new Vector2Int( 0, 1 );
			}
			else if( dir.x >= 0 && dir.z <= 0 )
			{
				coordinatesDir = new Vector2Int( 0, -1 );
			}
			else if( dir.x <= 0 && dir.z <= 0 )
			{
				coordinatesDir = new Vector2Int( -1, 0 );
			}

			// Decide how long the pathway should be before generating a new one.
			int pathwayLength = Random.Range( 35, 35 );

			GameObject pathwayGO = new GameObject
			{
				name = "Pathway [" + pathwayIndex + "]",
			};
			pathwayGO.transform.parent = pathwayParentTransform;

			// Generate the path for the generated length
			// Make the path 3 wide. We dont have to worry about duplicate tiles because those won't get generated anyway.
			for( int j = 0; j < pathwayLength; j++ )
			{
				CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ), coordinates.y + ( coordinatesDir.y * j ) ), pathwayGO.transform );
				CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ) - 1, coordinates.y + ( coordinatesDir.y * j ) - 1 ), pathwayGO.transform );
				CreateTile( "Pathway [" + pathwayIndex + "]", new Vector2Int( coordinates.x + ( coordinatesDir.x * j ) + 1, coordinates.y + ( coordinatesDir.y * j ) + 1 ), pathwayGO.transform );
			}

			// Create a room at the end of each pathway.
			coordinates.x += coordinatesDir.x * pathwayLength;
			coordinates.y += coordinatesDir.y * pathwayLength;
			GenerateRoom( coordinates, new Vector2Int( 25, 25 ), new Vector2Int( 25, 25 ), "Room[BOSS ROOM]", RoomType.BOSS );

			pathwayIndex++;
		}

		/// <summary>
		/// Generates a Ground Tile with the given name, coordinates and parrentroom (if given)
		/// </summary>
		/// <param name="tileName"> The name of the tile. </param>
		/// <param name="coordinates"> The coordinates of the tile. </param>
		private void CreateTile( string tileName, Vector2Int coordinates, Transform parentTransform = null )
		{
			// This removes a possible duplicate tile with the same coordinates.
			// We could check for duplicate tile and return the function, but this makes the hierarchy cleaner.
			for( int t = 0; t < tiles.Count; t++ )
			{
				if( tiles[t].Coordinates == new Vector2Int( coordinates.x, coordinates.y ) * tileSize )
				{
					GameObject tileOBJ = tiles[t].gameObject;
					tiles.RemoveAt( t );
					DestroyImmediate( tileOBJ );
				}
			}

			GameObject tileGO = new GameObject();
			Tile tile = tileGO.AddComponent<Tile>();

			tile.Name = tileName;
			tile.Type = TileType.GROUND;
			tile.Size = tileSize;
			tile.Coordinates = new Vector2Int( coordinates.x, coordinates.y ) * tileSize;
			tile.Graphic = tileGroundObjects[Random.Range( 0, tileGroundObjects.Count )];
			tile.GraphicRotation = Quaternion.Euler( -90, 0, 0 );

			tileGO.name = tile.Name;
			tileGO.transform.position = new Vector3Int( tile.Coordinates.x, 0, tile.Coordinates.y );

			if( parentTransform != null )
			{
				tile.ParentTransform = parentTransform;
				tileGO.transform.parent = tile.ParentTransform;
				parentTransform.GetComponent<Room>()?.Tiles.Add( tile );
			}

			tiles.Add( tile );
		}

		/// <summary>
		/// The placewalls functions will loop through all the tiles in the dungeoon, look at their coordinates and the amount of neighbouring tiles.
		/// Depending on which neighbouring tiles is missing, it places a wall.
		/// </summary>
		private void SetTilesType()
		{
			for( int t = 0; t < tiles.Count; t++ )
			{
				List<Tile> neighbourTiles = new List<Tile>();

				Tile tile = tiles[t];

				//tile.name = tiles[t].name;
				tile.Coordinates = tiles[t].Coordinates;
				tile.Graphic = tiles[t].Graphic;
				tile.ParentTransform = tiles[t].ParentTransform;

				// Left, Right, Top and Bottom local Tiles.
				Tile leftTile = null;
				Tile rightTile = null;
				Tile topTile = null;
				Tile bottomTile = null;

				// Top Left, Top Right, Bottom Left and Bottom Right Tiles.
				Tile topLeftTile = null;
				Tile topRightTile = null;
				Tile bottomLeftTile = null;
				Tile bottomRightTile = null;

				// Get all the neighbour tiles.
				for( int i = 0; i < tiles.Count; i++ )
				{
					// Get Left tile
					if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x - tileSize, tile.Coordinates.y ) )
					{
						leftTile = tiles[i];
						neighbourTiles.Add( leftTile );
					}

					// Get Right tile
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x + tileSize, tile.Coordinates.y ) )
					{
						rightTile = tiles[i];
						neighbourTiles.Add( rightTile );
					}

					// Get Up tile
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x, tile.Coordinates.y + tileSize ) )
					{
						topTile = tiles[i];
						neighbourTiles.Add( topTile );
					}

					// Get Down tile
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x, tile.Coordinates.y - tileSize ) )
					{
						bottomTile = tiles[i];
						neighbourTiles.Add( bottomTile );
					}

					// Get Top Left Tile
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x - tileSize, tile.Coordinates.y + tileSize ) )
					{
						topLeftTile = tiles[i];
						neighbourTiles.Add( topLeftTile );
					}

					// Get Top Right Tile
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x + tileSize, tile.Coordinates.y + tileSize ) )
					{
						topRightTile = tiles[i];
						neighbourTiles.Add( topRightTile );
					}

					// Get Bottom Left
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x - tileSize, tile.Coordinates.y - tileSize ) )
					{
						bottomLeftTile = tiles[i];
						neighbourTiles.Add( bottomLeftTile );
					}

					// Get Bottom Right
					else if( tiles[i].Coordinates == new Vector2Int( tile.Coordinates.x + tileSize, tile.Coordinates.y - tileSize ) )
					{
						bottomRightTile = tiles[i];
						neighbourTiles.Add( bottomRightTile );
					}
				}

				if( neighbourTiles.Count == 8 )
				{
					tile.Type = TileType.GROUND;
					tile.Graphic = tileGroundObjects[Random.Range( 0, tileGroundObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 0, 0 );
				}

				//#####################\\
				// WALLS        CHECKS \\
				//#####################\\

				// Check if this tile is all the way in the left of a room. a.k.a. no Left neighbour.
				if( leftTile == null && rightTile != null && topTile != null && bottomTile != null )
				{
					tile.Type = TileType.WALL;
					tile.Graphic = tileWallObjects[Random.Range( 0, tileWallObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 90, 0 );
				}

				// Check if this tile is all the way in the Right of a room. a.k.a. no Right neighbour.
				if( leftTile != null && rightTile == null && topTile != null && bottomTile != null )
				{
					tile.Type = TileType.WALL;
					tile.Graphic = tileWallObjects[Random.Range( 0, tileWallObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, -90, 0 );
				}

				// Check if this tile is all the way in the Top of a room. a.k.a. no top neighbour.
				if( leftTile != null && rightTile != null && topTile == null && bottomTile != null )
				{
					tile.Type = TileType.WALL;
					tile.Graphic = tileWallObjects[Random.Range( 0, tileWallObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 180, 0 );
				}

				// Check if this tile is all the way in the Bottom of a room. a.k.a. no bottom neighbour.
				if( leftTile != null && rightTile != null && topTile != null && bottomTile == null )
				{
					tile.Type = TileType.WALL;
					tile.Graphic = tileWallObjects[Random.Range( 0, tileWallObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 0, 0 );
				}

				//#####################\\
				// OUTER CORNER CHECKS \\
				//#####################\\

				// Top Left Outer Corner.
				if( leftTile == null && topLeftTile == null && topTile == null )
				{
					tile.Type = TileType.OUTER_CORNER;
					tile.Graphic = tileOuterCornerObjects[Random.Range( 0, tileOuterCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 90, 0 );
				}

				// Top Right Outer Corner.
				if( rightTile == null && topRightTile == null && topTile == null )
				{
					tile.Type = TileType.OUTER_CORNER;
					tile.Graphic = tileOuterCornerObjects[Random.Range( 0, tileOuterCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 180, 0 );
				}

				// Bottom Left Outer Corner.
				if( leftTile == null && bottomLeftTile == null && bottomTile == null )
				{
					tile.Type = TileType.OUTER_CORNER;
					tile.Graphic = tileOuterCornerObjects[Random.Range( 0, tileOuterCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 0, 0 );
				}

				// Bottom Right Outer Corner.
				if( rightTile == null && bottomRightTile == null && bottomTile == null )
				{
					tile.Type = TileType.OUTER_CORNER;
					tile.Graphic = tileOuterCornerObjects[Random.Range( 0, tileOuterCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, -90, 0 );
				}

				//#####################\\
				// INNER CORNER CHECKS \\
				//#####################\\

				// Top Left Inner Corner
				if( leftTile != null && topLeftTile == null && rightTile != null && topRightTile != null && bottomLeftTile != null && bottomRightTile != null && topTile != null && bottomTile != null )
				{
					tile.Type = TileType.INNER_CORNER;
					tile.Graphic = tileInnerCornerObjects[Random.Range( 0, tileInnerCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, -90, 0 );
				}
				// Top Right Inner Corner
				if( leftTile != null && topLeftTile != null && rightTile != null && topRightTile == null && bottomLeftTile != null && bottomRightTile != null && topTile != null && bottomTile != null )
				{
					tile.Type = TileType.INNER_CORNER;
					tile.Graphic = tileInnerCornerObjects[Random.Range( 0, tileInnerCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 0, 0 );
				}
				// Bottom Left Inner Corner
				if( leftTile != null && topLeftTile != null && rightTile != null && topRightTile != null && bottomLeftTile == null && bottomRightTile != null && topTile != null && bottomTile != null )
				{
					tile.Type = TileType.INNER_CORNER;
					tile.Graphic = tileInnerCornerObjects[Random.Range( 0, tileInnerCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, -180, 0 );
				}
				// Bottom Right Inner Corner
				if( leftTile != null && topLeftTile != null && rightTile != null && topRightTile != null && bottomLeftTile != null && bottomRightTile == null && topTile != null && bottomTile != null )
				{
					tile.Type = TileType.INNER_CORNER;
					tile.Graphic = tileInnerCornerObjects[Random.Range( 0, tileInnerCornerObjects.Count )];
					tile.GraphicRotation = Quaternion.Euler( -90, 90, 0 );
				}

				//#####################\\
				// ROOM ENTRANCE CHECK \\
				//#####################\\
			}
		}

		/// <summary>
		/// Instantiate all the tiles graphics.
		/// </summary>
		private void InstantiateTilesGraphic()
		{
			foreach( Tile tile in tiles )
			{
				GameObject tileGO = Instantiate( tile.Graphic, Vector3.zero, tile.GraphicRotation, tile.transform );
				tileGO.transform.position = tile.transform.position;
			}
		}


		/// <summary>
		/// Spawns enemies inside the rooms.
		/// </summary>
		private void SpawnEnemiesInsideTheRooms()
		{
			// Ignore the first room because that's where the player spawns.
			for( int r = 1; r < Rooms.Count; r++ )
			{
				if( Rooms[r].Tiles.Count == 0 )
					return;

				for( int t = 0; t < Rooms[r].Tiles.Count; t++ )
				{
					Tile tile = Rooms[r].Tiles[t];

					if( tile.Type == TileType.GROUND )
						SpawnEnemy( tile.Coordinates, Rooms[r].transform );
				}
			}
		}
		/// <summary>
		/// Spawns an enemy at the given coordinates
		/// </summary>
		/// <param name="coordinates"> Which coordinate to spawn the enemy on. </param>
		private void SpawnEnemy( Vector2 coordinates, Transform parent = null )
		{
			int spawnPercentage = Random.Range( 0, 100 );
			if( spawnPercentage <= spawnChance )
			{
				int randEnemyIndex = Random.Range( 0, enemyLists[0].Enemies.Count );
				GameObject newEnemyGO = Instantiate( enemyLists[0].Enemies[randEnemyIndex], coordinates, Quaternion.identity, parent );
			}
		}



		private void OnDrawGizmos()
		{
			if( renderDungeonAsGizmos )
			{
				foreach( Tile tile in tiles )
				{
					Gizmos.color = Color.white;
					if( tile.Type == TileType.INNER_CORNER ) Gizmos.color = Color.blue;
					else if( tile.Type == TileType.OUTER_CORNER ) Gizmos.color = Color.cyan;
					else if( tile.Type == TileType.WALL ) Gizmos.color = Color.red;

					Gizmos.DrawCube( new Vector3Int( tile.Coordinates.x, tileSize, tile.Coordinates.y ), new Vector3Int( tile.Size, tile.Size, tile.Size ) );
				}
			}
		}
	}

	public enum TileType
	{
		GROUND,         // A normal ground tile. (surrounded by other tiles)
		WALL,           // Wall Tile. (Missing Neighbour on one side)
		OUTER_CORNER,   // Outer Corner Tile. (Missing Neighbour on atleast 2 sides)
		INNER_CORNER,   // Outer Corner Tile.
		DOOR            // Door Tile. (Missing Corner neighbours)
	}

	public enum RoomType
	{
		NONE,
		SPAWN,          // Player Spawn Room. (Always only 1 per level)
		EXIT,           // Player Exit Room. (Always only 1 per level)
		ENEMYSPAWN,     // Enemy Spawn Room. Most commong room type
		SHOP,           // Shop where player can buy/sell items. 30% of spawning per level.
		BOSS,           // Boss room, always present and always in the way of the exit.
		TREASURE,       // Treasure rooms are rare, but grant some nice loot.
		HUB,            // A hub room is nothing more than a room that connects to other rooms. See it as a place to breath and regen.
		OBJECTIVE       // Objective rooms are rooms that hold the item(s) required to finish the objective.
	}

	[System.Serializable]
	public class Tile : MonoBehaviour
	{
		[SerializeField] private new string name = default;
		[SerializeField] private int size = 1;
		[SerializeField] private TileType type = TileType.GROUND;
		[SerializeField] private Vector2Int coordinates = Vector2Int.zero;
		[SerializeField] private GameObject graphic = null;
		[SerializeField] private Quaternion graphicRotation = Quaternion.identity;
		[SerializeField] private Transform parentTransform = default;

		public string Name { get => name; set => name = value; }
		public int Size { get => size; set => size = value; }
		public TileType Type { get => type; set => type = value; }
		public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
		public GameObject Graphic { get => graphic; set => graphic = value; }
		public Quaternion GraphicRotation { get => graphicRotation; set => graphicRotation = value; }
		public Transform ParentTransform { get => parentTransform; set => parentTransform = value; }

		public Tile( string name = null, TileType _type = TileType.GROUND, Vector2Int _coordinates = default, GameObject _graphic = null, Quaternion _graphicRotation = default, Transform _parentTransform = default )
		{
			this.name = name;
			type = _type;
			coordinates = _coordinates;
			graphic = _graphic;
			graphicRotation = _graphicRotation;
			parentTransform = _parentTransform;
		}
	}

	[System.Serializable]
	public class Room : MonoBehaviour
	{
		[SerializeField] private new string name;
		[SerializeField] private Vector2Int roomSize = new Vector2Int();
		[SerializeField] private Vector2Int coordinates;
		[SerializeField] private List<Tile> tiles = new List<Tile>();
		[SerializeField] private RoomType type = RoomType.HUB;

		public List<Tile> Tiles { get => tiles; set => tiles = value; }
		public Vector2Int RoomSize { get => roomSize; set => roomSize = value; }
		public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
		public string Name { get => name; set => name = value; }
		public RoomType Type { get => type; set => type = value; }
	}

	[System.Serializable]
	public struct EnemyList
	{
		[SerializeField] private new string name;
		[SerializeField] private List<GameObject> enemies;

		public List<GameObject> Enemies { get => enemies; set => enemies = value; }
	}
}

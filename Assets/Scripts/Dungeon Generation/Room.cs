using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerationPathFirst
{
	public enum RoomType
	{
		NONE,
		SPAWN,          // Player Spawn Room. (Always only 1 per level)
		EXIT,           // Player Exit Room. (Always only 1 per level)
		ENEMYSPAWN,     // Enemy Spawn Room. Most common room type
		SHOP,           // Shop where player can buy/sell items. 30% of spawning per level.
		BOSS,           // Boss room, always present and always in the way of the exit.
		TREASURE,       // Treasure rooms are rare, but grant some nice loot.
		HUB,            // A hub room is nothing more than a room that connects to other rooms. See it as a place to breath and regen.
		OBJECTIVE       // Objective rooms are rooms that hold the item(s) required to finish the objective.
	}

	public class Room : MonoBehaviour
	{
		[SerializeField] private new string name;
		[SerializeField] private Vector2Int roomSize = new Vector2Int();
		[SerializeField] private Vector2Int coordinates;
		[SerializeField] private List<Tile> tiles = new List<Tile>();
		[SerializeField] private RoomType type = RoomType.HUB;

		public string Name { get => name; set => name = value; }
		public Vector2Int RoomSize { get => roomSize; set => roomSize = value; }
		public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
		public List<Tile> Tiles { get => tiles; set => tiles = value; }
		public RoomType Type { get => type; set => type = value; }
	}
}

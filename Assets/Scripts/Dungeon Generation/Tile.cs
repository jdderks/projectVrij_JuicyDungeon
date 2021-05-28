using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerationPathFirst
{
	public enum TileType
	{
		GROUND,         // A normal ground tile. (surrounded by other tiles)
		WALL,           // Wall Tile. (Missing Neighbour on one side)
		OUTER_CORNER,   // Outer Corner Tile. (Missing Neighbour on atleast 2 sides)
		INNER_CORNER,   // Inner Corner Tile.
		DOOR            // Door Tile. (Missing Corner neighbours)
	}

	public class Tile : MonoBehaviour
	{
		[SerializeField] private new string name = default;
		[SerializeField] private int size = 1;
		[SerializeField] private TileType type = TileType.GROUND;
		[SerializeField] private Vector2Int coordinates = Vector2Int.zero;
		[SerializeField] private GameObject graphic = null;
		[SerializeField] private Quaternion graphicRotation = Quaternion.identity;
		[SerializeField] private Transform parentTransform = default;
		[SerializeField] private bool populated = false;

		public string Name { get => name; set => name = value; }
		public int Size { get => size; set => size = value; }
		public TileType Type { get => type; set => type = value; }
		public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
		public GameObject Graphic { get => graphic; set => graphic = value; }
		public Quaternion GraphicRotation { get => graphicRotation; set => graphicRotation = value; }
		public Transform ParentTransform { get => parentTransform; set => parentTransform = value; }
		public bool Populated { get => populated; set => populated = value; }

		public Tile( string name, int size, TileType type, Vector2Int coordinates, GameObject graphic, Quaternion graphicRotation, Transform parentTransform, bool populated )
		{
			this.name = name;
			this.size = size;
			this.type = type;
			this.coordinates = coordinates;
			this.graphic = graphic;
			this.graphicRotation = graphicRotation;
			this.parentTransform = parentTransform;
			this.populated = populated;
		}
	}
}

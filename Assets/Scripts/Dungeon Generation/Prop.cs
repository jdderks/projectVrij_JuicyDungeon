using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerationPathFirst
{
	public class Prop : MonoBehaviour
	{
		[SerializeField] private new string name;
		[SerializeField] private GameObject prefabObject;
		[SerializeField] private float spawnChance;
		[Space]
		[SerializeField] private Vector3 rotationOffset;
		[SerializeField] private Vector3 positionOffset;

		public string Name { get => name; set => name = value; }
		public GameObject PrefabObject { get => prefabObject; set => prefabObject = value; }
		public float SpawnChance { get => spawnChance; set => spawnChance = value; }
		public Vector3 RotationOffset { get => rotationOffset; set => rotationOffset = value; }
		public Vector3 PositionOffset { get => positionOffset; set => positionOffset = value; }
	}
}

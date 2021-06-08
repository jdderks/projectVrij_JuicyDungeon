using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "Scriptable Arrow Object", menuName = "Scriptable Objects/New Scriptable Arrow" )]
public class ScriptableArrowObject : ScriptableObject
{
	public LayerMask detectionLayer;
	public GameObject prefabObject;
	public float force;
	public int damage = 100;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
	[SerializeField] private List<NavMeshSurface> navMeshSurfaces = new List<NavMeshSurface>();

	public void Bake()
	{
		Debug.Log( "Baking NavMeshSurfaces..." );
		navMeshSurfaces.AddRange( Object.FindObjectsOfType<NavMeshSurface>() );

		foreach( NavMeshSurface surface in navMeshSurfaces )
		{
			surface.BuildNavMesh();
		}
		Debug.Log( "Done Baking!" );
	}
}

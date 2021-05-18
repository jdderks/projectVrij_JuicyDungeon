using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using DungeonGenerationPathFirst;

public class DungenPathFirstCustomInspector : MonoBehaviour
{
	private static DungenPathFirst dungenPathFirst;

	[MenuItem( "Dungen/Path First/Generate Dungeon" )]
	private static void GenerateDungeon()
	{
		ClearConsole();

		dungenPathFirst = FindObjectOfType<DungenPathFirst>();
		dungenPathFirst.GenerateDungeon();
	}

	[MenuItem( "Dungen/Path First/Clear Dungeon" )]
	private static void ClearDungeon()
	{
		ClearConsole();

		dungenPathFirst = FindObjectOfType<DungenPathFirst>();
		dungenPathFirst.ClearDungeon();
	}

	public static void ClearConsole()
	{
		var assembly = Assembly.GetAssembly( typeof( SceneView ) );
		var type = assembly.GetType( "UnityEditor.LogEntries" );
		var method = type.GetMethod( "Clear" );
		method.Invoke( new object(), null );
	}
}

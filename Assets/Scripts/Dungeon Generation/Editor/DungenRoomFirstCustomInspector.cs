using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using DungeonGenerationRoomFirst;

public class DungenRoomFirstCustomInspector : MonoBehaviour
{
	private static DungenRoomFirst dungenRoomFirst;

	[MenuItem( "Dungen/Room First/Generate Dungeon" )]
	private static void GenerateDungeon()
	{
		ClearConsole();

		dungenRoomFirst = FindObjectOfType<DungenRoomFirst>();
		dungenRoomFirst.Generate();
	}

	[MenuItem( "Dungen/Room First/Clear Dungeon" )]
	private static void ClearDungeon()
	{
		ClearConsole();

		dungenRoomFirst = FindObjectOfType<DungenRoomFirst>();
		dungenRoomFirst.ClearDungeon();
	}

	public static void ClearConsole()
	{
		var assembly = Assembly.GetAssembly( typeof( SceneView ) );
		var type = assembly.GetType( "UnityEditor.LogEntries" );
		var method = type.GetMethod( "Clear" );
		method.Invoke( new object(), null );
	}
}

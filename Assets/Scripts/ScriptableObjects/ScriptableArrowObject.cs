using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Arrow Object", menuName = "Scriptable Objects/New Scriptable Arrow")]
public class ScriptableArrowObject : ScriptableObject
{
    public GameObject prefabObject;
    [Space]

    public float force;
    [Space]

    public int damage = 100;
    [Space]

    public LayerMask detectionLayer;
}

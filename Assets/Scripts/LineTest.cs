using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour
{

    public int length = 10;

    [Range(0,100)]
    public int segments = 2;
    private void Start()
    {



    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(-length, 0, 0), new Vector3(length, 0, 0));
        float segmentSize = (float)length / (float)segments;

        for (int i = 1; i < segments - 1; i++)
        {
            float totalLength = (float)length - (float)-length;
            float c = totalLength-length - (segmentSize*i);
            Debug.Log(c);
            Gizmos.DrawSphere(new Vector3(c, 0, 0), 0.5f);
        }
    }
}

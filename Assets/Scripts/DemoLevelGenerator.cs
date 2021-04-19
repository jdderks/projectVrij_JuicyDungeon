using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoLevelGenerator : MonoBehaviour
{
    public int width = 10;
    public int depth = 10;


    [SerializeField]
    private List<GameObject> tiles = new List<GameObject>();

    public GameObject cubePrefab;

    private void Start()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                if (i == 0 || i == width-1 || j == 0 || j == depth-1)
                {
                    float r = Random.Range(0f, 1f);
                    GameObject gObject = Instantiate(cubePrefab, new Vector3(i, r-10, j),Quaternion.identity);
                    gObject.transform.localScale = new Vector3(1,21+r,1);
                    gObject.transform.SetParent(transform);
                    tiles.Add(gObject);
                } 
                else
                {
                    GameObject gObject = Instantiate(cubePrefab, new Vector3(i, 0, j),Quaternion.identity);
                    gObject.transform.SetParent(transform);
                    tiles.Add(gObject);
                }
            }
        }
    }



}

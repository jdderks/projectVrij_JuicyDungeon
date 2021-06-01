using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float cameraFollowSmoothing = 0.01f;


    [Header("Camera speeds")]
    [SerializeField] private float keyboardMovementSpeed = 5.0f;
    [SerializeField] private float edgeScrollingMovementspeed = 3.0f;
    [SerializeField] private float travelTime = 0.1f;

    [Header("Inputgroups & Layers")]
    [SerializeField] private string horizontalInputName = "Horizontal";
    [SerializeField] private string verticalInputName = "Vertical";

    [Header("Edge scrolling settings")]
    [SerializeField] private float screenEdgeBorder = 25.0f;


    [Header("Use keyboard or edge scrolling")]
    [SerializeField] private bool useKeyboardInput = true;
    [SerializeField] private bool useEdgeScrolling = true;

    private float firstClickTime = 0f;
    private float timeBetweenClicks = 0.2f;
    private int clickCounter = 0;

    private Transform m_transform; //Camera transform

	public GameObject Player { get => player; set => player =  value ; }

	private void Start()
    {
        m_transform = transform;
    }

    private void LateUpdate()
    {
        MoveToPosition(player.transform.position, cameraFollowSmoothing);
    }


    private void MoveToPosition(Vector3 to, float time)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        while (elapsedTime < travelTime)
        {
            transform.position = Vector3.Lerp(startPosition, new Vector3(to.x, transform.position.y, to.z), (elapsedTime / time));
            elapsedTime += Time.deltaTime / 2;
            //yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
    }

}
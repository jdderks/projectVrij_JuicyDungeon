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

    private void Start()
    {
        m_transform = transform;
    }

    private void LateUpdate()
    {
        MoveCamera();
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

    private void MoveCamera()
    {
        if (useKeyboardInput)
        {
            float horizInput = Input.GetAxis(horizontalInputName);
            float vertInput = Input.GetAxis(verticalInputName);

            Vector3 desiredMove = new Vector3(horizInput, 0, vertInput);

            desiredMove *= keyboardMovementSpeed;
            desiredMove *= Time.deltaTime;
            desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
            desiredMove = m_transform.InverseTransformDirection(desiredMove);

            m_transform.Translate(desiredMove, Space.Self);
        }

        if (useEdgeScrolling)
        {
            Vector3 desiredMove = new Vector3();

            Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
            Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
            Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
            Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

            desiredMove.x = leftRect.Contains(Input.mousePosition) ? -1 : rightRect.Contains(Input.mousePosition) ? 1 : 0;
            desiredMove.z = upRect.Contains(Input.mousePosition) ? 1 : downRect.Contains(Input.mousePosition) ? -1 : 0;

            desiredMove *= edgeScrollingMovementspeed;
            desiredMove *= Time.deltaTime;
            desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
            desiredMove = m_transform.InverseTransformDirection(desiredMove);

            m_transform.Translate(desiredMove, Space.Self);
        }
    }

    //DoubleClick behaviour
    private bool DoubleClick()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            clickCounter++;
            if (clickCounter == 1) firstClickTime = Time.time;
        }
        if (Time.time - firstClickTime < timeBetweenClicks)
        {
            if (clickCounter > 1)
            {
                return true;
            }
        }
        else
        {
            clickCounter = 0;
            firstClickTime = 0;
        }
        return false;
    }
}
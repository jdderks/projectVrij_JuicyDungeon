using UnityEngine;
using System.Collections;

public class CameraOrbitBehaviour : MonoBehaviour
{
    private Transform _XForm_Camera;
    private Transform _XForm_Parent;

    private Quaternion _LocalRotation;
    private float _CameraDistance = 10.0f;

    [Space, SerializeField] private Quaternion startingRotation = new Quaternion(45, 45, 0, 0);
    [SerializeField] private float startingLength = 2.0f;
    [Space, SerializeField] private float MouseSensitivity = 4.0f;
    [SerializeField] private float ScrollSensitvity = 2.0f;
    [SerializeField] private float OrbitDampening = 10.0f;
    [SerializeField] private float ScrollDampening = 6.0f;
    [SerializeField] private float MaxOrbitAngleFromGround = 10.0f;
    [SerializeField] private float minZoomDistance = 3.0f;
    [SerializeField] private float maxZoomDistance = 100.0f;

    [SerializeField] private bool CameraDisabled = false;
    [SerializeField] private KeyCode lockKey = KeyCode.None;

    void Start()
    {
        if (lockKey == KeyCode.None)
        {
            Debug.LogWarning("You did not assign the key to pause camera orbiting");
        }
        _XForm_Camera = transform;
        _XForm_Parent = transform.parent;


    }


    void LateUpdate()
    {
        if (Input.GetKeyDown(lockKey))
        {
            CameraDisabled = !CameraDisabled;
        }

        if (Input.GetButton("Fire2"))
        {
            //Rotation of the Camera based on Mouse Coordinates
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                _LocalRotation.y += Input.GetAxis("Mouse Y") * -MouseSensitivity;
            }


            //Clamp the y rotation to horizon and not flipping over at the top
            if (_LocalRotation.y < MaxOrbitAngleFromGround)
            {
                _LocalRotation.y = MaxOrbitAngleFromGround;
            }
            else if (_LocalRotation.y > 90f) //90 because that is the fixed highest of the camera angle
            {
                _LocalRotation.y = 90f;
            }
        }

        //Zooming Input from our Mouse Scroll Wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

            ScrollAmount *= (_CameraDistance * 0.3f);

            _CameraDistance += ScrollAmount * -1f;

            _CameraDistance = Mathf.Clamp(_CameraDistance, minZoomDistance, maxZoomDistance);
        }

        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        _XForm_Parent.rotation = Quaternion.Lerp(_XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

        if (_XForm_Camera.localPosition.z != _CameraDistance * -1f)
        {
            _XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(_XForm_Camera.localPosition.z, _CameraDistance * -1f, Time.deltaTime * ScrollDampening));
        }
    }
}
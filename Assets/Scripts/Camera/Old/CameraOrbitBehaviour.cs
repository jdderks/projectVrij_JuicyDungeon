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

    [SerializeField] private float zoomDistance = 3.0f;

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
        _XForm_Parent.transform.rotation = startingRotation;

        
        float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

        ScrollAmount *= (this._CameraDistance * 0.3f);

        this._CameraDistance += ScrollAmount * -1f;

        this._CameraDistance = zoomDistance;
        

        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        //this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

        if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
        {
            this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Code modified from https://www.youtube.com/watch?v=ZwD1UHNCzOc&list=PLFt_AvWsXl0djuNM22htmz3BUtHHtOh7v&index=7
/// by Sebastian Lague
/// </summary>

enum PlayerStates
{
    IDLE = 0,
    WALKING = 1,
    RUNNING = 2,
    ROLLING = 3
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;

    [SerializeField] private PlayerStates playerState = PlayerStates.IDLE;

    [SerializeField] private float turnSmoothTime = 0.2f;
    [SerializeField] private float turnSmoothVelocity = default;

    [SerializeField] private float rollDistance = 1f;
    [SerializeField] private float rollSpeed = 100f;

    private float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity = default;
    private float currentSpeed;
    private float currentRollSpeed;

    private Vector2 input;
    private Vector2 inputDir;
    private bool running = false;

    private Animator animator;

    [SerializeField]
    private Transform cameraTransform;

	public Transform CameraTransform { get => cameraTransform; set => cameraTransform =  value ; }

	private void Start()
    {
        animator = GetComponent<Animator>();

        currentRollSpeed = rollSpeed;
    }

    private void Update()
    {
        //Input
        //Set states
        //If in rolling state do rolling functionality


        HandlePlayerInput();
        HandleStates();
    }

    private void HandleStates()
    {
        switch (playerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.WALKING:
            case PlayerStates.RUNNING:
                MovePlayer();
                break;

            case PlayerStates.ROLLING:
                Roll();
                break;

            default:
                break;
        }
    }

    private void HandlePlayerInput()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;
        running = Input.GetKey(KeyCode.LeftShift); //TODO: Input needs to be in InputManager

        if(inputDir == Vector2.zero && playerState != PlayerStates.ROLLING)
        {
            playerState = PlayerStates.IDLE;
		}

        if(inputDir != Vector2.zero && playerState != PlayerStates.ROLLING )
        {
            playerState = PlayerStates.WALKING;
		}

        if(running && playerState != PlayerStates.ROLLING)
        {
            playerState = PlayerStates.RUNNING;
		}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerState = PlayerStates.ROLLING;
        }
    }

    private void MovePlayer()
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        float animationSpeedPercent = ((running) ? 1 : 0.5f) * inputDir.magnitude;
        animator.SetFloat("movementSpeedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    private void Roll()
    {
        animator.SetTrigger("roll");
        playerState = PlayerStates.IDLE;

//        Vector3 rollDir = transform.position + transform.forward * rollDistance;
//        Debug.Log(rollDir);
//        transform.Translate( rollDir * currentRollSpeed * Time.deltaTime, Space.World );

//        currentRollSpeed -= currentRollSpeed * 10f * Time.deltaTime;
//        if( currentRollSpeed < 5f)
//        {
//            playerState = PlayerStates.IDLE;
//            currentRollSpeed = rollSpeed;
////		}
    }

    public void PlayRunningAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot( "event:/Player_Footsteps_Running", transform.position );
	}

    public void PlayWalkingAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot( "event:/Player_Footsteps_Walking", transform.position );
    }
}


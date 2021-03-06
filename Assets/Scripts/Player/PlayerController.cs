using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Code modified from https://www.youtube.com/watch?v=ZwD1UHNCzOc&list=PLFt_AvWsXl0djuNM22htmz3BUtHHtOh7v&index=7
/// by Sebastian Lague
/// </summary>

public enum PlayerStates
{
	IDLE = 0,
	WALKING = 1,
	RUNNING = 2,
	ROLLING = 3,
	SHOOTING = 4
}

public class PlayerController : MonoBehaviour
{
	[SerializeField] private PlayerStates playerState = PlayerStates.IDLE;
	[Space]
	[SerializeField] private float walkSpeed = 2f;
	[SerializeField] private float runSpeed = 6f;

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
	private Vector2 mousePos;
	private bool running = false;
	private bool chargingAttack = false;
	private Player player;

	private Animator animator;
	private CapsuleCollider collider;

	[SerializeField]
	private Transform cameraTransform;

	public Transform CameraTransform { get => cameraTransform; set => cameraTransform = value; }

	private void Start()
	{
		player = GetComponent<Player>();
		animator = GetComponent<Animator>();

		currentRollSpeed = rollSpeed;
	}

	private void Update()
	{
		//Input
		//Set states
		//If in rolling state do rolling functionality



		HandlePlayerInput();
		HandleRotation();
		HandleStates();
	}

	private void HandleStates()
	{
		switch( playerState )
		{
			case PlayerStates.IDLE:
				animator.SetBool( "Aiming Bow", false );
				animator.SetFloat( "Movement State", 1, speedSmoothTime, Time.deltaTime );
				break;

			case PlayerStates.WALKING:
				animator.SetBool( "Aiming Bow", false );
				animator.SetFloat( "Movement State", 2, speedSmoothTime, Time.deltaTime );
				break;

			case PlayerStates.RUNNING:
				animator.SetBool( "Aiming Bow", false );
				animator.SetFloat( "Movement State", 3, speedSmoothTime, Time.deltaTime );
				break;

			case PlayerStates.ROLLING:
				animator.SetBool( "Aiming Bow", false );
				//animator.SetFloat( "Movement State", 4, speedSmoothTime, Time.deltaTime );
				break;

			case PlayerStates.SHOOTING:
				animator.SetBool( "Aiming Bow", true );
				Shoot();
				break;

			default:
				break;
		}
	}

	private void HandlePlayerInput()
	{
		input = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
		inputDir = input.normalized;
		running = Input.GetKey( KeyCode.LeftShift ); //TODO: Input needs to be in InputManager

		animator.SetFloat( "X Velocity", input.y, speedSmoothTime, Time.deltaTime );
		animator.SetFloat( "Z Velocity", input.x, speedSmoothTime, Time.deltaTime );

		if( inputDir == Vector2.zero && playerState != PlayerStates.ROLLING )
		{
			playerState = PlayerStates.IDLE;
		}

		if( inputDir != Vector2.zero && playerState != PlayerStates.ROLLING )
		{
			playerState = PlayerStates.WALKING;
		}

		if( running && playerState != PlayerStates.ROLLING )
		{
			playerState = PlayerStates.RUNNING;
		}

		if( Input.GetKeyDown( KeyCode.Space ) && playerState != PlayerStates.ROLLING )
		{
			animator.SetTrigger( "Roll" );
			playerState = PlayerStates.ROLLING;
		}

		if( Input.GetAxisRaw( "Fire2" ) == 1 )
		{
			playerState = PlayerStates.SHOOTING;
		}

	}

	private void HandleRotation()
	{
		if( playerState == PlayerStates.SHOOTING )
		{
			Ray cameraRay = Camera.main.ScreenPointToRay( Input.mousePosition );
			Plane groundPlane = new Plane( Vector3.up, Vector3.zero );
			float rayLength;

			if( groundPlane.Raycast( cameraRay, out rayLength ) )
			{
				Vector3 pointToLook = cameraRay.GetPoint( rayLength );

				transform.LookAt( new Vector3( pointToLook.x, transform.position.y, pointToLook.z ) );
			}
		}
		else
		{
			if( playerState != PlayerStates.ROLLING )
			{
				if( inputDir != Vector2.zero )
				{
					float targetRot = Mathf.Atan2( inputDir.x, inputDir.y ) * Mathf.Rad2Deg + 45;
					transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle( transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, turnSmoothTime );
				}
			}
		}
	}

	private void Shoot()
	{
		if( Input.GetAxisRaw( "Fire1" ) == 1 )
		{
			animator.SetBool( "Shoot Bow", true );
		}
		else
		{
			animator.SetBool( "Shoot Bow", false );
		}
	}

	public void PlayRunningAudio()
	{
		FMODUnity.RuntimeManager.PlayOneShot( "event:/Player/Locomotion/Player_Footsteps_Running", transform.position );
	}

	public void PlayWalkingAudio()
	{
		FMODUnity.RuntimeManager.PlayOneShot( "event:/Player/Locomotion/Player_Footsteps_Walking", transform.position );
	}

	public void SetPlayerState( PlayerStates state )
	{
		playerState = state;
	}

	public void SetPlayerMovementState( int state )
	{
		animator.SetFloat( "Movement State", state );
	}

	public void SetPlayerMovementStateSmoothed( int state )
	{
		animator.SetFloat( "Movement State", state, speedSmoothTime, Time.deltaTime );
	}
}


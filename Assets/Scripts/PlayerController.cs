using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Code modified from https://www.youtube.com/watch?v=ZwD1UHNCzOc&list=PLFt_AvWsXl0djuNM22htmz3BUtHHtOh7v&index=7
/// by Sebastian Lague
/// </summary>

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;

    [SerializeField] private float turnSmoothTime = 0.2f;
    [SerializeField] private float turnSmoothVelocity = default;

    private float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity = default;
    private float currentSpeed;

    private Animator animator;

    [SerializeField]
    private Transform cameraTransform;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        
        bool running = Input.GetKey(KeyCode.LeftShift); //TODO: Input needs to be in InputManager
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity,speedSmoothTime);

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        float animationSpeedPercent = ((running) ? 1 : 0.5f) * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }
}

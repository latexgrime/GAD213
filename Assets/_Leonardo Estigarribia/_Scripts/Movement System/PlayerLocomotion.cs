using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager playerManager;
    private InputManager inputManager;
    private AnimatorManager animatorManager;
    
    private Vector3 moveDirection;
    private Transform cameraObject;
    private Rigidbody playerRigidbody;

    [Header("- Falling")]
    [SerializeField] private float inAirTimer;
    [SerializeField] private float leapingVelocity;
    [SerializeField] private float fallingSpeed;
    [SerializeField] private float rayCastHeightOffset = 0.5f;
    [SerializeField] private float landingSphereCastMaxDistance = 1;
    [SerializeField] private float landingSphereCastRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("- Movement Flags")]
    public bool isSprinting;
    public bool isWalking; // For keyboard.
    public bool isGrounded;

    [Header("- Jumping")] 
    public bool isJumping;
    public bool startJump;
    
    [SerializeField] private float jumpPower = 50f;
    [SerializeField] private float jumpDuration = 0.3f;  // How long the upward force is applied.
    
    [Header("- Movement Speeds")]
    [SerializeField] private float walkingSpeed = 1.5f;
    [SerializeField] private float runningSpeed = 4f;
    [SerializeField] private float sprintingSpeed = 7f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("- Dodge")]
    [SerializeField] private Vector3 rollDirection ;
    [SerializeField] private float dodgeDuration = 0.3f;
    [SerializeField] private float dodgePower = 10f;
    public bool isDodging;
    private void Awake()
    {
        FindComponents();
    }

    private void FindComponents()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        if (startJump)
        {
            HandleJumping();
        }

        if (isDodging)
        {
            HandleDodge();
        }

        HandleFallingAndLanding();
        
        if (playerManager.isInteracting) 
            return;

        HandleMovement();
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        if (isJumping) 
            return;
        
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f && !isWalking)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else if (inputManager.moveAmount > 0.01f && isWalking)
            {
                moveDirection = moveDirection * walkingSpeed; // For keyboard.
            }
            else 
                moveDirection = moveDirection * walkingSpeed; // For controllers [TO BE TESTED]
        }
        

        
        moveDirection = moveDirection * sprintingSpeed;

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (isJumping) 
            return;
        
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;

        // Player falling forces and animation.
        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Fall", true, false);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(Vector3.down * fallingSpeed * inAirTimer); 
        }

        // Player landing forces and animation.
        if (Physics.SphereCast(rayCastOrigin, landingSphereCastRadius, Vector3.down, out hit, landingSphereCastMaxDistance, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true, false);
            }
            

            inAirTimer = 0;
            isGrounded = true;
            playerManager.isInteracting = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleJumping()
    {
        if (isGrounded && !isJumping)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false, false);

            isJumping = true;
            startJump = false;
            StartCoroutine(ApplyJumpForce());
        }
    }
    
    private IEnumerator ApplyJumpForce()
    {
        float timeElapsed = 0f;

        while (timeElapsed < jumpDuration)
        {
            // Quadratic easing in-out function.
            float easeFactor = Mathf.SmoothStep(0f, 2f, timeElapsed / jumpDuration);

            // Apply the force using the easing factor.
            playerRigidbody.AddForce(transform.up * jumpPower * (1 - easeFactor) * Time.deltaTime, ForceMode.Acceleration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isJumping = false;
        animatorManager.animator.SetBool("isJumping", false);
    }

    private void HandleDodge()
    {
        if (!playerManager.isInteracting && isGrounded)
        {
            // If there is movement from the player, the player is going to roll.
            if (inputManager.moveAmount > 0)
            {
                rollDirection = cameraObject.transform.forward * inputManager.verticalInput;
                rollDirection += cameraObject.transform.right * inputManager.horizontalInput;

                rollDirection.y = 0;
                Quaternion.LookRotation(rollDirection);
                rollDirection.Normalize();
            
                animatorManager.PlayTargetAnimation("Roll_Forward_01",true, false);
                StartCoroutine(ApplyDodgeForce());
                
            }
            /*else // Roll.
            {
                isDodging = false;
                // Perform a back step animation
            }*/
        }
    }
    
    private IEnumerator ApplyDodgeForce()
    {
        float timeElapsed = 0f;

        while (timeElapsed < dodgeDuration)
        {
            // Quadratic easing in-out function.
            //float easeFactor = Mathf.SmoothStep(0f, 1f, timeElapsed / dodgeDuration);

            // Apply the force using the easing factor.
            playerRigidbody.AddForce(transform.forward * (dodgePower * Time.deltaTime), ForceMode.Impulse);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isDodging= false;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager playerManager;
    private InputManager inputManager;
    private AnimatorManager animatorManager;
    private AudioSource audioSource;

    private Vector3 moveDirection;
    private Transform cameraObject;
    private Rigidbody playerRigidbody;

    [Header("- Falling")] [SerializeField] private float inAirTimer;
    [SerializeField] private float leapingVelocity;
    [SerializeField] private float fallingSpeed;
    [SerializeField] private float rayCastHeightOffset = 0.5f;
    [SerializeField] private float landingSphereCastMaxDistance = 1;
    [SerializeField] private float landingSphereCastRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("- Movement Flags")] public bool isSprinting;
    public bool isWalking; // For keyboard.
    public bool isGrounded;

    [Header("- Jumping")] public bool isJumping;
    public bool startJump;

    [SerializeField] private float jumpPower = 50f;
    [SerializeField] private float jumpDuration = 0.3f; // How long the upward force is applied.

    [Header("- Movement Speeds")] [SerializeField]
    private float walkingSpeed = 1.5f;

    [SerializeField] private float runningSpeed = 4f;
    [SerializeField] private float sprintingSpeed = 7f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("- Dodge")] [SerializeField] private Vector3 rollDirection;
    [SerializeField] private float dodgeDuration = 0.3f;
    [SerializeField] private float dodgePower = 10f;
    public bool isDodging;

    [Header("- Attack")] 
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackVerticalOffset;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDistance;
    [SerializeField] private AnimationClip attackAnimLength;
    
    [Header("- VFX")]
    [SerializeField] private ParticleSystem attackVFX;
    [FormerlySerializedAs("jumpVFX")] [SerializeField] private ParticleSystem landVFX;

    [Header("- SFX")] 
    private float currentStepInterval = 0;
    [SerializeField] private AudioClip[] jumpSFX;
    [SerializeField] private AudioClip landSFX;
    [SerializeField] private AudioClip[] walkSFX;
    [SerializeField] private AudioClip[] attackSFX;

    private float stepTimer = 0f;
    [FormerlySerializedAs("walkSFXPeriod")] [SerializeField] private float walkSFXinterval = 0.4f;
    [FormerlySerializedAs("jogSFXPeriod")] [SerializeField] private float jogSFXInverval = 0.3f;
    [FormerlySerializedAs("sprintSFXPeriod")] [SerializeField] private float sprintSFXInterval = 0.2f;

    public bool attackTrigger;
    public bool isAttacking;
    
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
        audioSource = GetComponent<AudioSource>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        if (startJump) HandleJumping();

        if (isDodging) HandleDodge();

        if (attackTrigger) HandleAttacking();

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

        // Play walking SFX.
        PlayWalkSFX();
        
        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f && !isWalking)
                moveDirection = moveDirection * runningSpeed;
            else if (inputManager.moveAmount > 0.01f && isWalking)
                moveDirection = moveDirection * walkingSpeed; // For keyboard.
            else
                moveDirection = moveDirection * walkingSpeed; // For controllers [TO BE TESTED]
        }


        moveDirection = moveDirection * sprintingSpeed;

        var movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void PlayWalkSFX()
    {
        float originalPitch;
        float originalVolume;

        originalPitch = audioSource.pitch;
        originalVolume = audioSource.volume;
        
        if (inputManager.moveAmount >= 0.01f)
        {
            if (!isJumping && isGrounded)
            {
                stepTimer += Time.deltaTime;
                if (isWalking)
                {
                    Debug.Log($"Walking.");
                    currentStepInterval = walkSFXinterval;
                }

                if (!isWalking && !isSprinting)
                {
                    Debug.Log($"Jogging.");
                    currentStepInterval = jogSFXInverval;
                }

                if (isSprinting)
                {
                    Debug.Log($"Sprinting.");
                    currentStepInterval = sprintSFXInterval;
                }

                if (stepTimer >= currentStepInterval)
                {
                    Debug.Log($"Playing walk SFX.");

                    audioSource.volume = Random.Range(audioSource.volume - 0.1f, audioSource.volume + 0.1f);
                    audioSource.pitch = Random.Range(audioSource.pitch - 0.2f, audioSource.pitch + 0.2f);
                    
                    audioSource.PlayOneShot(walkSFX[Random.Range(0, walkSFX.Length)]);
                    stepTimer = 0;
                }
            }
        }

        audioSource.pitch = originalPitch;
        audioSource.volume = originalVolume;
    }

    private void HandleRotation()
    {
        if (isJumping)
            return;

        var targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        var targetRotation = Quaternion.LookRotation(targetDirection);
        var playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        var rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;

        // Player falling forces and animation.
        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting) animatorManager.PlayTargetAnimation("Fall", true, false);

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(Vector3.down * fallingSpeed * inAirTimer);
        }

        // Player landing forces and animation.
        if (Physics.SphereCast(rayCastOrigin, landingSphereCastRadius, Vector3.down, out hit,
                landingSphereCastMaxDistance, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true, false);
                landVFX.transform.position = transform.position;
                landVFX.Play();
                audioSource.PlayOneShot(landSFX);
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
            // Jump animation.
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false, false);
            
            // Random Jump SFX.
            audioSource.PlayOneShot(jumpSFX[Random.Range(0,jumpSFX.Length)]);
            
            isJumping = true;
            startJump = false;
            StartCoroutine(ApplyJumpForce());
        }
    }

    private IEnumerator ApplyJumpForce()
    {
        var timeElapsed = 0f;

        while (timeElapsed < jumpDuration)
        {
            // Quadratic easing in-out function.
            var easeFactor = Mathf.SmoothStep(0f, 2f, timeElapsed / jumpDuration);

            // Apply the force using the easing factor.
            playerRigidbody.AddForce(transform.up * jumpPower * (1 - easeFactor) * Time.deltaTime,
                ForceMode.Acceleration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isJumping = false;
        animatorManager.animator.SetBool("isJumping", false);
    }

    private void HandleDodge()
    {
        if (!playerManager.isInteracting && isGrounded)
            // If there is movement from the player, the player is going to roll.
            if (inputManager.moveAmount > 0)
            {
                rollDirection = cameraObject.transform.forward * inputManager.verticalInput;
                rollDirection += cameraObject.transform.right * inputManager.horizontalInput;

                rollDirection.y = 0;
                Quaternion.LookRotation(rollDirection);
                rollDirection.Normalize();

                animatorManager.PlayTargetAnimation("Roll Forward", true, false);
                audioSource.PlayOneShot(jumpSFX[Random.Range(0,jumpSFX.Length)]);
                StartCoroutine(ApplyDodgeForce());
            }   
        /*else // Roll.
            {
                isDodging = false;
                // Perform a back step animation
            }*/
    }

    private IEnumerator ApplyDodgeForce()
    {
        var timeElapsed = 0f;

        while (timeElapsed < dodgeDuration)
        {
            // Quadratic easing in-out function.
            //float easeFactor = Mathf.SmoothStep(0f, 1f, timeElapsed / dodgeDuration);

            // Apply the force using the easing factor.
            playerRigidbody.AddForce(transform.forward * (dodgePower * Time.deltaTime), ForceMode.Impulse);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }

    private void HandleAttacking()
    {
        attackTrigger = false;
        if (!playerManager.isInteracting && isGrounded && !isAttacking)
        {
            // Lock the player control when attacking.
            isAttacking = true;
            
            // Attack animation, VFX and SFX.
            animatorManager.PlayTargetAnimation("Attack", true, false);
            attackVFX.Play();
            audioSource.PlayOneShot(attackSFX[Random.Range(0, attackSFX.Length)]); // Random SFX from the list.
            
            // Attack thing in front of the player.
            DamageHitObject();

            // Reset the isAttacking bool when the animation ends.
            // This is divided by two because I multiplied the speed of this animation by two in the inspector.
            StartCoroutine(ResetAttackingBool(attackAnimLength.length / 2));
        }
    }

    private void DamageHitObject()
    {
        Vector3 sphereCastOffset = transform.position + Vector3.up * attackVerticalOffset;
        RaycastHit[] hits =
            Physics.SphereCastAll(sphereCastOffset, attackRadius, transform.forward, attackDistance);
        Debug.DrawLine(sphereCastOffset,sphereCastOffset + transform.forward * attackDistance, Color.cyan, 0.5f);
        foreach (var hit in hits)
        {
                
            // If the hit is the player itself, then ignore it.
            if (hit.transform.gameObject != this.gameObject)
            {
                // If the hit has the EntityStats class, then damage it.
                if (hit.transform.gameObject.GetComponent<EntityStats>())
                {
                    EntityStats hitStats = hit.transform.gameObject.GetComponent<EntityStats>();
                    hitStats.TakeDamage(attackDamage);
                    Debug.Log($"Damaged {hit.transform.name} for {attackDamage} damage.");
                }
            }
        }
    }

    private IEnumerator ResetAttackingBool(float length)
    {
        yield return new WaitForSeconds(length);
        isAttacking = false;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls; // Reference to the Player Controls C# script generated by the new Input System class by Unity.
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private CameraManager cameraManager;
    // NEW! Project 2
    [SerializeField] private GameObject inventoriesCanvas;
    
    
    public Vector2 movementInput;
    public Vector2 cameraInput;

    [Header("- Camera Input")]
    public float cameraInputX;
    public float cameraInputY;

    [Header("- Moving Input")]
    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    [Header("- Player Actions Input")]
    public bool sprintInput; // West button on the controller, SHIFT in keyboard.
    public bool walkInput; // CTRL button on keyboard.
    public bool jumpInput; // South button on the controller, SPACE in keyboard.
    public bool dodgeInput; // East button on the controller, F in keyboard.
    public bool switchCharacterInput; // North button on the controller, Q in keyboard.
    public bool attackInput; // Click button. Make it R1 in the future.

    // NEW!
    [Header("- Inventory Input")] 
    public bool openInventoryInput;
    
    
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
        inventoriesCanvas = GameObject.FindWithTag("InventoriesCanvas");
        inventoriesCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Movement callback context.
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            
            // Camera callback context.
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            
            // Sprint callback context.
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            
            // Walk callback context.
            playerControls.PlayerActions.Walk.performed += i => walkInput = true;
            playerControls.PlayerActions.Walk.canceled += i => walkInput = false;
            
            // Jump callback context.
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;
            
            // Dodge callback context.
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Dodge.canceled+= i => dodgeInput = false;

            // Character switch callback context.
            playerControls.PlayerActions.CharacterSwitchToggle.performed += i => switchCharacterInput = true;
            playerControls.PlayerActions.CharacterSwitchToggle.canceled += i => switchCharacterInput = false;
            
            // Inventory opening callback context.
            playerControls.PlayerActions.OpenInventoryToggle.performed += i => HandleInventoryInput();
            
            // Attacking callback context.
            playerControls.PlayerActions.Attack.performed += i => attackInput = true;
            playerControls.PlayerActions.Attack.canceled += i => attackInput = false;
        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleWalkingInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleDodgeInput();
        HandleAttackingInput();
        HandleCharacterSwitchInput();
    }

    private void HandleMovementInput()
    {
        if (playerLocomotion.isAttacking)
        {
            verticalInput = 0;
            horizontalInput = 0;
            moveAmount = 0;
            animatorManager.UpdateAnimatorValues(0, 0, false, false);
        }
        else
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
        
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        
            // Handle animation
            animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting, playerLocomotion.isWalking);
        }
        
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
    }

    private void HandleSprintingInput()
    {
        if (sprintInput && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleWalkingInput()
    {
        if (walkInput && sprintInput == false && moveAmount > 0.01f)
        {
            playerLocomotion.isWalking = true;
        }
        else
        {
            playerLocomotion.isWalking = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            playerLocomotion.startJump = true;
            jumpInput = false;
        }
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput && moveAmount > 0.01f)
        {
            playerLocomotion.isDodging = true;
            dodgeInput = false;
        }
    }

    private void HandleCharacterSwitchInput()
    {
        if (switchCharacterInput)
        {
            cameraManager.HandleCharacterSwitch();
            switchCharacterInput = false;
        }
    }

    private void HandleAttackingInput()
    {
        if (attackInput)
        {
            playerLocomotion.attackTrigger = true;
            attackInput = false;
        }
    }
    
    private void HandleInventoryInput()
    {
        openInventoryInput = !openInventoryInput;
        inventoriesCanvas.SetActive(openInventoryInput);
    }
    
}

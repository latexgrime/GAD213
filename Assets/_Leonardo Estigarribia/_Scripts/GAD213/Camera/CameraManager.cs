using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;

    private int characterSwitchCounter;
    [Header("- References")]
    [SerializeField] private Transform cqcTransform;
    [SerializeField] private Transform rangedCharacterTransform;
    
    [SerializeField] private Transform cameraPivot; // Reference to the Camera Pivot GameObject.
    private Transform cameraTransform; // The transform of the Camera Object.
    [SerializeField] private Transform targetTransform; // The object the camera will follow. In this case, the player.

    [SerializeField] private LayerMask collisionLayers;
    
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    [Header("- Camera collision settings")]
    [SerializeField] private float cameraCollisionOffset = 0.2f;
    [SerializeField] private float minimumCollisionOffset = 0.2f;
    [SerializeField] private float cameraCollisionLerpTime = 0.2f;
    [SerializeField] private float cameraCollisionRadius = 2;
    [SerializeField] private float cameraFollowSpeed = 0.2f;
    [SerializeField] private float cameraLookSpeed = 2f;
    [SerializeField] private float cameraPivotSpeed = 2f;
    
    private float lookAngle; // To make camera look up and down.
    private float pivotAngle; // To make camera look left and right.
    [SerializeField] private float minimumPivotAngle = -35;
    [SerializeField] private float maximumPivotAngle = 35;
    
    private void Awake()
    {
        characterSwitchCounter = 1;
        inputManager = FindObjectOfType<InputManager>();        
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
    
    private void FollowTarget()
    {
        Vector3 targetPosition =
            Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        
        lookAngle = lookAngle + ( inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - ( inputManager.cameraInputY * cameraPivotSpeed);
        
        // Pivot limiter;
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, cameraCollisionLerpTime);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    public void HandleCharacterSwitch()
    {
        characterSwitchCounter++;
        if (characterSwitchCounter % 2 == 1)
        {
            ChangeCameraTarget(cqcTransform);
        }
        else
        {
            ChangeCameraTarget(rangedCharacterTransform);
        }
    }
    
    private void ChangeCameraTarget(Transform target)
    {
        targetTransform = target;
    }
}

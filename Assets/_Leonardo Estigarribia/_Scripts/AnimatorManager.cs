using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float normalizedCrossfadeDuration = 0.2f;
    
    private int horizontal;
    private int vertical;
    
    
    [Header("- Foot Placement IK Values")] 
    [SerializeField] [Range(0, 1f)] private float distanceToGround;
    [SerializeField] private float feetRayOffset;
    [SerializeField] private LayerMask ignorePlayerLayer;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    // Call an animation to the player's animator by referencing this Manager.
    public void PlayTargetAnimation(string targetAnimation, bool isInteracting, bool applyRootMotion)
    {
        animator.applyRootMotion = applyRootMotion;
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, normalizedCrossfadeDuration);
    }
    
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting, bool isWalking)
    {
        // Animation snapping, dark souls-like
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement > 0 && horizontalMovement < -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement > 0 && verticalMovement < -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        

        #endregion

        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2; // Snaps into the running animation in the animator.
        }

        if (isWalking)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 0.4f; // Snaps into the walking animation.
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    
    #region Foot Placement Inverse Kinematics Code
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator)
        {
            Debug.LogError("No animator is attached to the object");
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            
            // Left foot
            RaycastHit hit;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + feetRayOffset, ignorePlayerLayer))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward, hit.normal));
            }
            
            // Right foot
            ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + feetRayOffset, ignorePlayerLayer))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward, hit.normal));
                
            }
        }
    }

    #endregion
}

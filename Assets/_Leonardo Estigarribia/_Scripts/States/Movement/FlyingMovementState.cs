using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Leonardo_Estigarribia._Scripts.States.Movement
{
    public class FlyingMovementState : BaseMovementState
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float playerPivotOffsetY = 1f;
        [SerializeField] private bool constrainPositionY;
        
        protected override void MoveTowardsTarget()
        {
            var targetPosition = stateManager.playerTransform.position + Vector3.up * playerPivotOffsetY;
            Vector3 direction = (targetPosition - transform.root.position).normalized;
            
            if (constrainPositionY)
            {
                transform.root.position += new Vector3(direction.x, 0, direction.z) * (moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.root.position += direction * (moveSpeed * Time.deltaTime);
            }
        }

        protected override void RotateTowardsTarget()
        {
            Vector3 targetPosition = stateManager.playerTransform.position + Vector3.up * playerPivotOffsetY;
            transform.LookAt(targetPosition);
        }

        protected override void UpdateWalkingAnimation(bool isMoving)
        {
            stateManager.animator.SetBool("isMoving", isMoving);
        }
    }
}
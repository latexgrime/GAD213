using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Movement
{
    public class FlyingMovementState : BaseMovementState
    {
        [SerializeField] private float moveSpeed = 5f;
        
        protected override void MoveTowardsTarget()
        {
            Vector3 direction = (stateManager.playerTransform.position - transform.root.position).normalized;
            transform.root.position += direction * (moveSpeed * Time.deltaTime);
        }

        protected override void RotateTowardsTarget()
        {
            transform.LookAt(stateManager.playerTransform);
        }

        protected override void UpdateWalkingAnimation(bool isMoving)
        {
            stateManager.animator.SetBool("isMoving", isMoving);
        }
    }
}
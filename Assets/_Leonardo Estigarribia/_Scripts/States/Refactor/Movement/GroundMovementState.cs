using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Movement
{
    public class GroundMovementState : BaseMovementState
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float animatorWalkingValue = 0.7f;
        
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
            float walkingTargetAnimatorValue = isMoving ? animatorWalkingValue : 0f;
            stateManager.animator.SetFloat("Horizontal", walkingTargetAnimatorValue);
        }
    }
}
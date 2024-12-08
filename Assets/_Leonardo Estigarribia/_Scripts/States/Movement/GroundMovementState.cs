using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Movement
{
    public class GroundMovementState : BaseMovementState
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float animatorWalkingValue = 0.7f;

        protected override void MoveTowardsTarget()
        {
            var direction = (stateManager.playerTransform.position - transform.root.position).normalized;
            transform.root.position += direction * (moveSpeed * Time.deltaTime);
        }

        protected override void RotateTowardsTarget()
        {
            transform.LookAt(new Vector3(stateManager.playerTransform.position.x, 0,
                stateManager.playerTransform.position.z));
        }

        protected override void UpdateWalkingAnimation(bool isMoving)
        {
            var walkingTargetAnimatorValue = isMoving ? animatorWalkingValue : 0f;
            stateManager.animator.SetFloat("Horizontal", walkingTargetAnimatorValue);
        }
    }
}
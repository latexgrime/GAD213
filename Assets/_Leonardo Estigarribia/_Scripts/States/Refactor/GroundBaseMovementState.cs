using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor
{
    public class GroundBaseMovementState : BaseMovementState
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float animatorWalkingValue = 0.7f;
        
        protected override void MoveTowardsTarget()
        {
            Vector3 direction = (player.position - transform.root.position).normalized;
            transform.root.position += direction * (moveSpeed * Time.deltaTime);
        }

        protected override void UpdateWalkingAnimation()
        {
            stateManager._animator.SetFloat("Horizontal", animatorWalkingValue);
        }
    }
}
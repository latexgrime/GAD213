using UnityEngine;
using State = _Leonardo_Estigarribia._Scripts.Enemy.State;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Movement
{
    public abstract class BaseMovementState : State
    {
        protected StateManager stateManager;

        [SerializeField] protected float detectionRadius = 10f;
        [SerializeField] protected float attackRange = 2f;

        protected bool isInAttackRange;

        protected virtual void Start()
        {
            InitializeScript();
        }

        private void InitializeScript()
        {
            stateManager = GetComponent<StateManager>();
        }


        protected abstract void MoveTowardsTarget();
        protected abstract void RotateTowardsTarget();
        protected abstract void UpdateWalkingAnimation(bool isMoving);

        public override State RunCurrentState()
        {
            if (stateManager.playerTransform == null) return this;

            if (!IsInChaseRange())
            {
                UpdateWalkingAnimation(false);
                return stateManager.idleState;
            }

            CheckAttackRange();
            
            if (!isInAttackRange)
            {
                MoveTowardsTarget();
                UpdateWalkingAnimation(true);
            }
            else
            {
                UpdateWalkingAnimation(false);
            }

            RotateTowardsTarget();
            
            if (isInAttackRange)
            {
                return stateManager.attackState;
            }

            return this;
        }

        private bool IsInChaseRange()
        {
            return Vector3.Distance(transform.position, stateManager.playerTransform.position) <= detectionRadius;
        }
        
        private void CheckAttackRange()
        {
            isInAttackRange = Vector3.Distance(transform.position, stateManager.playerTransform.position) <= attackRange;
        }
    }
}
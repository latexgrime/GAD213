using UnityEngine;
using State = _Leonardo_Estigarribia._Scripts.Enemy.State;

namespace _Leonardo_Estigarribia._Scripts.States.Idle
{
    public abstract class BaseIdleState : State
    {
        protected StateManager stateManager;

        [SerializeField] protected float detectionRadius = 10f;
        protected bool canSeePlayer;

        protected virtual void Start()
        {
            stateManager = GetComponent<StateManager>();
        }

        public override State RunCurrentState()
        {
            PerformIdleMovement();
            CheckForTarget();
            
            if (canSeePlayer)
            {
                return stateManager.movementState;
            }
            
            return this;
        }

        protected virtual void CheckForTarget()
        {
            canSeePlayer = Physics.CheckSphere(transform.position, detectionRadius, LayerMask.GetMask("Player"));
        }

        protected abstract void PerformIdleMovement();

    }
}
using System;
using _Leonardo_Estigarribia._Scripts.Enemy;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using State = _Leonardo_Estigarribia._Scripts.Enemy.State;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Idle
{
    public abstract class BaseIdleState : State
    {
        protected StateManager stateManager;
        protected BaseMovementState movementState;
        protected Transform player;

        [SerializeField] protected float detectionRadius = 10f;
        protected bool canSeePlayer;

        protected virtual void Start()
        {
            stateManager = GetComponent<StateManager>();
            movementState = GetComponent<BaseMovementState>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public override State RunCurrentState()
        {
            PerformIdleMovement();
            CheckForTarget();
            
            if (canSeePlayer)
            {
                return movementState;
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
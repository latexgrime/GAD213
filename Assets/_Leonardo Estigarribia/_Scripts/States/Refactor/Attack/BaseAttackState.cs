using _Leonardo_Estigarribia._Scripts.Enemy;
using UnityEngine;
using State = _Leonardo_Estigarribia._Scripts.Enemy.State;
using Vector3 = UnityEngine.Vector3;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Attack
{
    public abstract class BaseAttackState : State
    {
        protected StateManager stateManager;
        protected BaseMovementState movementState;
        protected Transform player;

        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float attackCooldown = 1f;
        protected float attackTimePeriod;

        protected virtual void Start()
        {
            stateManager = GetComponent<StateManager>();
            movementState = GetComponent<BaseMovementState>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public override State RunCurrentState()
        {
            if (player == null) return this;

            if (!IsInAttackRange())
            {
                ResetAttackAnimation();
                return movementState;
            }

            if (Time.time >= attackTimePeriod)
            {
                PerformAttack();
                attackTimePeriod = Time.time + attackCooldown;
            }

            return this;
        }

        protected abstract void PerformAttack();
        protected abstract void ResetAttackAnimation();

        protected virtual bool IsInAttackRange()
        {
            return Vector3.Distance(transform.position, player.position) <= attackRange;
        }
    }
}
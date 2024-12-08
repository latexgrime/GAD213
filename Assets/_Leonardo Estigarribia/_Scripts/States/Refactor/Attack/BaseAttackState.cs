using _Leonardo_Estigarribia._Scripts.Enemy;
using _Leonardo_Estigarribia._Scripts.States.Refactor.Movement;
using UnityEngine;
using UnityEngine.Serialization;
using State = _Leonardo_Estigarribia._Scripts.Enemy.State;
using Vector3 = UnityEngine.Vector3;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Attack
{
    public abstract class BaseAttackState : State
    {
        protected StateManager stateManager;

        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float timeBetweenAttacks = 1f;
        protected float attackTimePeriod;

        protected virtual void Start()
        {
            stateManager = GetComponent<StateManager>();
        }

        public override State RunCurrentState()
        {
            if (stateManager.playerTransform == null) return this;

            if (!IsInAttackRange())
            {
                ResetAttackAnimation();
                return stateManager.movementState;
            }

            if (Time.time >= attackTimePeriod)
            {
                PerformAttack();
                attackTimePeriod = Time.time + timeBetweenAttacks;
            }

            return this;
        }

        protected abstract void PerformAttack();
        protected abstract void ResetAttackAnimation();

        protected virtual bool IsInAttackRange()
        {
            return Vector3.Distance(transform.position, stateManager.playerTransform.position) <= attackRange;
        }
    }
}
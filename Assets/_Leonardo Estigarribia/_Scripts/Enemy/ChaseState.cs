using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class ChaseState : State
    {
        private StateManager stateManager;
        private IdleState idleState;
        private AttackState attackState;
        
        private bool isInAttackRange;

        [SerializeField] private float chaseRadius = 10f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float animatorWalkingValue = 0.7f;
        [SerializeField] private float attackRange = 2f;
        private Transform player;
    
        private void Start()
        {
            stateManager = GetComponent<StateManager>();
            idleState = GetComponent<IdleState>();
            attackState = GetComponent<AttackState>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        public override State RunCurrentState()
        {
            if (player == null) return this;
            
            if (!CheckForChaseRadius())
            {
                return idleState;
            }
            
            CheckForAttackRange();
            
            if (!isInAttackRange)
            {
                MoveTowardsPlayer();
            }
            
            LookAtPlayer();
            
            if (isInAttackRange)
                return attackState;
            
            return this;
        }

        private bool CheckForChaseRadius()
        {
            bool isInChaseRadius = Vector3.Distance(transform.root.position, player.position) <= chaseRadius;
            return isInChaseRadius;
        }

        private void CheckForAttackRange()
        {
            isInAttackRange = Vector3.Distance(transform.root.position, player.position) <= attackRange;
        }

        private void LookAtPlayer()
        {
            transform.root.LookAt(player);
        }

        private void MoveTowardsPlayer()
        {
            stateManager._animator.SetFloat("Horizontal", animatorWalkingValue);
            Vector3 direction = (player.position - transform.root.position).normalized;
            transform.root.position += direction * (moveSpeed * Time.deltaTime);
        }
    }
}
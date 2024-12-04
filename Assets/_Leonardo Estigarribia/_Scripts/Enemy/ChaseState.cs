using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class ChaseState : State
    {
        private AttackState attackState;
        private bool isInAttackRange;
    
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float attackRange = 2f;
        private Transform player;
    
        private void Start()
        {
            attackState = GetComponent<AttackState>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        public override State RunCurrentState()
        {
            if (player == null) return this;
            
            MoveTowardsPlayer();
            LookAtPlayer();
            CheckForAttackRange();
        
            if (isInAttackRange)
                return attackState;
            return this;
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
            Vector3 direction = (player.position - transform.root.position).normalized;
            transform.root.position += direction * (moveSpeed * Time.deltaTime);
        }
    }
}
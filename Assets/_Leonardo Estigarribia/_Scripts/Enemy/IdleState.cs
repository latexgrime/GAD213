using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class IdleState : State
    {
        public ChaseState chaseState;
        public bool canSeePlayer;

        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float rotationSpeed = 30f;

        public override State RunCurrentState()
        {
            RotateAround();
            PlayerDetectionRadiusCheck();
            
            if (canSeePlayer)
                return chaseState;
            return this;
        }
            
        private void RotateAround()
        {
            gameObject.transform.root.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
        
        private void PlayerDetectionRadiusCheck()
        {
            canSeePlayer = Physics.CheckSphere(transform.root.position, detectionRadius, LayerMask.GetMask($"Player"));
        }
    }
}
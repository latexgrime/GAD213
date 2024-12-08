using _Leonardo_Estigarribia._Scripts.States.Idle;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Idle
{
    public class GroundIdleState : BaseIdleState
    {
        [SerializeField] private float rotationSpeed = 30f;
    
        protected override void PerformIdleMovement()
        {
            stateManager.animator.SetFloat("Horizontal", 0f);
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }
}

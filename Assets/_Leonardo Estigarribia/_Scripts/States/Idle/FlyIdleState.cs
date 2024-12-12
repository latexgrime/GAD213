using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Idle
{
    public class FlyIdleState : BaseIdleState
    {   
        protected override void PerformIdleMovement()
        {
            stateManager.animator.SetBool("isMoving", false);
        }
    }
}

using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public abstract class State : MonoBehaviour
    {
        public abstract State RunCurrentState();
    }
}

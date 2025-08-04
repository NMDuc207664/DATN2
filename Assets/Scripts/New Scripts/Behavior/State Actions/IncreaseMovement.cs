using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Action/Increase Movement")]
    public class IncreaseMovement : StateActions
    {
        public float increaseIncrement;

        public override void Execute(StateManager states)
        {
            if(states.speed < 100)
            {
                states.speed += increaseIncrement;
            }
        }
    }
}
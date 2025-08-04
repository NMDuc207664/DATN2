using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Action/Decrease Movement")]
    public class DecreaseMovement : StateActions
    {
        public float decreaseIncrement;

        public override void Execute(StateManager states)
        {
            if (states.speed > 0)
            {
                states.speed -= decreaseIncrement;
            }
        }
    }
}
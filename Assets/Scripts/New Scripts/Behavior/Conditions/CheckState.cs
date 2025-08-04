using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Condition/CheckState")]
    public class CheckState : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return state.health > 0;
        }

    }
}
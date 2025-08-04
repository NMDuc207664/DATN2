using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public enum compareType
    {
        Greater,
        Lesser,
    }

    [CreateAssetMenu(menuName = "Condition/CheckSpeed")]
    public class CheckSpeed : Condition
    {
        public float speedGate;
        public compareType type;

        public override bool CheckCondition(StateManager state)
        {
            switch (type)
            {
                case compareType.Greater:
                    {
                        return state.speed >= speedGate;
                    }
                case compareType.Lesser:
                    {
                        return state.speed <= speedGate;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

    }
}

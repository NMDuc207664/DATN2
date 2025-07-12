using DATN2.Assets.Scripts.Manager;
using UnityEngine;

namespace DATN2.Assets.Scripts.BehaviorEditor.State
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool CheckCondition(StateManager stageManager);
    }
}
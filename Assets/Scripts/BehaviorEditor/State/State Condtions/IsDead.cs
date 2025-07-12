using DATN2.Assets.Scripts.Manager;
using UnityEngine;
namespace DATN2.Assets.Scripts.BehaviorEditor.State
{
    [CreateAssetMenu(menuName = "Conditions/IsDead")]
    public class IsDead : Condition
    {
        public override bool CheckCondition(StateManager stageManager)
        {
            return stageManager.health <= 0;
        }
    }
}
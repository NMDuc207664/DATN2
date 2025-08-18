using UnityEngine;
namespace DATN2.Editor.Data.SaveModal.SO
{
    public abstract class DTSConditionSO : ScriptableObject
    {
        [field: SerializeField] public string ConditionName { get; set; }
        public abstract bool Check();
        public virtual void Initialize(string conditionName, string conditionNodeID)
        {
            ConditionName = conditionName;
        }
    }
}
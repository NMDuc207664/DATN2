using UnityEngine;
namespace DATN2.GraphviewEditor.Data.SaveModal.SO
{
    public abstract class DTSConditionAbstract : ScriptableObject
    {
        [field: SerializeField] public string ConditionName { get; set; }
        public abstract bool Check();
        public virtual void Initialize(string conditionName, string conditionNodeID)
        {
            ConditionName = conditionName;
        }
    }
}
using System.Collections.Generic;
using DATN2.GraphviewEditor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.GraphviewEditor.Data.SaveModal.SO
{
    public class DTSConditionSO : ScriptableObject
    {
        [field: SerializeField] public string ConditionName { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public List<DTSConditionAbstract> Conditions { get; set; }
        public void Initialize(string fileName, DTSDialogueType dialogueType)
        {
            ConditionName = fileName;
            DialogueType = dialogueType;
            Conditions = new List<DTSConditionAbstract>();
        }
    }
}

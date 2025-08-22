using System;
using System.Collections.Generic;
using DATN2.Editor.Data.SaveModal.SO;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSConditionSaveData
    {
        [field: SerializeField] public string NodeID { get; set; } // ID riêng của condition node
        [field: SerializeField] public string ParentNodeID { get; set; } // 1 node - n condition
        // [field: SerializeField] public DTSNodeSaveData ParentNode { get; set; }
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] public List<DTSConditionAbstract> Conditions { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public string GroupID { get; set; }// 1 group - n condition
        // [field: SerializeField] public DTSGroupSaveData Group { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; } // vị trí của condition node
    }
}
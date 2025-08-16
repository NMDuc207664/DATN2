using System;
using System.Collections.Generic;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSConditionSaveData
    {
        [field: SerializeField] public string NodeID { get; set; } // ID riêng của condition node
        [field: SerializeField] public string ParentNodeID { get; set; } // ID của node chứa nó
        [field: SerializeField] public List<ScriptableObject> Conditions { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; } // vị trí của condition node
    }
}
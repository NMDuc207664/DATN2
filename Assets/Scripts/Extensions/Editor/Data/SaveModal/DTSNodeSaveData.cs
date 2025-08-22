using System;
using System.Collections.Generic;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]//Node Modal
    public class DTSNodeSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<DTSChoiceSaveData> Choices { get; set; }//1 node có nhiều choice mối quan hệ 1-n
        [field: SerializeField] public List<DTSConditionSaveData> Conditions { get; set; }//1 node có nhiều condition mối quan hệ 1-n
        [field: SerializeField] public string GroupID { get; set; }//1 Node có 1 group mối quan hệ 1 group - n node
        [field: SerializeField] public DTSGroupSaveData Group { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool HasConditions { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public bool IsStartingNode { get; set; }
    }
}
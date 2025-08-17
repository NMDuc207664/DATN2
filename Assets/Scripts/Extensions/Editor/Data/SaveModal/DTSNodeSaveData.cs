using System;
using System.Collections.Generic;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]//dùng cho node bình thường
    public class DTSNodeSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }
        [field: SerializeField] public string Name { get; set; }//field serializable giúp field hiển thị trên inspector
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<DTSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public List<DTSConditionSaveData> Conditions { get; set; }
        [field: SerializeField] public string GroupId { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool HasConditions { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
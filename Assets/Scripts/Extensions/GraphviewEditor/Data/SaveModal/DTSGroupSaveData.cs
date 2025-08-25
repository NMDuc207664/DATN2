using System;
using System.Collections.Generic;
using UnityEngine;
namespace DATN2.GraphviewEditor.Data.SaveModal
{
    [Serializable]
    public class DTSGroupSaveData
    {
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        // [field: SerializeField] public List<DTSNodeSaveData> NodeSaveDatas { get; set; }//quan hệ 1 group - n node
        // [field: SerializeField] public List<DTSConditionSaveData> ConditionSaveDatas { get; set; }//quan hệ 1 group - n condition

    }
}
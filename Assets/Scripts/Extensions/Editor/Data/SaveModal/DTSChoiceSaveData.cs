using System;
using System.Collections.Generic;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSChoiceSaveData
    {
        [field: SerializeField] public string ChoiceID { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeID { get; set; }//1 choice thuộc về 1 NodeID
        [field: SerializeField] public List<string> ConnectedNodeIDs { get; set; }
    }
}


using System;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSChoiceSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeID { get; set; }
    }
}


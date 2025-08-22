using System;
using System.Collections.Generic;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class ChoiceNodeConnectionSaveData
    {
        [field: SerializeField] public string ChoiceID;
        [field: SerializeField] public string ChoiceText;
        [field: SerializeField] public string TargetNodeID;
        [field: SerializeField] public string TargetNodeName;
        [field: SerializeField] public List<string> ConditionID;
    }
}
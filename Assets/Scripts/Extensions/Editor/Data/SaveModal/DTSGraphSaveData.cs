using System;
using System.Collections.Generic;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSGraphSaveData : ScriptableObject
    {
        [field: SerializeField] public string GraphID { get; set; }
        [field: SerializeField] public string GraphName { get; set; }
        [field: SerializeField] public List<DTSGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DTSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<DTSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public List<DTSConditionSaveData> Conditions { get; set; }
        [field: SerializeField] public List<string> GroupsName { get; set; }
        [field: SerializeField] public List<string> NodesName { get; set; }
        [field: SerializeField] public List<string> ConditionNodesName { get; set; }

        public void Initialize(string fileName)
        {
            GraphName = fileName;
            Groups = new List<DTSGroupSaveData>();
            Nodes = new List<DTSNodeSaveData>();
            Conditions = new List<DTSConditionSaveData>();
        }
    }
}
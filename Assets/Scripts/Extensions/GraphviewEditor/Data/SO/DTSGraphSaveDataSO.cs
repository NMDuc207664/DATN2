using System;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using UnityEngine;
namespace DATN2.GraphviewEditor.Data.SaveModal
{
    [Serializable]
    public class DTSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string GraphName { get; set; }
        [field: SerializeField] public List<DTSGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DTSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<DTSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public List<DTSConditionSaveData> Conditions { get; set; }

        public void Initialize(string fileName)
        {
            GraphName = fileName;
            Groups = new List<DTSGroupSaveData>();
            Nodes = new List<DTSNodeSaveData>();
            Conditions = new List<DTSConditionSaveData>();
        }
        public List<string> GetGroupedDialogueNames(DTSDialogueGroupSO dialogueGroup, bool startingDialoguesOnly)
        {
            List<string> groupedDialogueNames = new List<string>();
            Debug.Log("GetGroupedDialogueNames is running");

            foreach (var node in Nodes)
            {
                if (node.Group != null && node.Group.GroupName == dialogueGroup.GroupName)
                {
                    if (startingDialoguesOnly && !node.IsStartingNode)
                    {
                        continue;
                    }

                    groupedDialogueNames.Add(node.Name);
                }
            }

            return groupedDialogueNames;
        }

        public List<string> GetUngroupedDialogueNames(bool startingDialoguesOnly)
        {
            List<string> ungroupedDialogueNames = new List<string>();
            Debug.Log("GetUngroupedDialogueNames is running");

            foreach (var node in Nodes)
            {
                // Nếu GroupID rỗng hoặc null => coi như ungroup
                if (string.IsNullOrEmpty(node.GroupID))
                {
                    if (startingDialoguesOnly && !node.IsStartingNode)
                    {
                        continue;
                    }

                    ungroupedDialogueNames.Add(node.Name);
                }
            }

            return ungroupedDialogueNames;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using UnityEditor;
using UnityEngine;

namespace DATN2.GraphviewEditor.Applications
{
    public class DTSReadSO
    {
        // Separate lists to store node names, group names, and condition names
        public List<string> NodeNames { get; private set; } = new List<string>();
        public List<string> GroupNames { get; private set; } = new List<string>();
        public List<string> ConditionNames { get; private set; } = new List<string>();

        public void GetInformationFromGraph(DTSGraphSaveDataSO graphSO)
        {
            // Clear lists before populating to avoid duplicates
            NodeNames.Clear();
            GroupNames.Clear();
            ConditionNames.Clear();

            // Get Node Names
            foreach (var nodeData in graphSO.Nodes)
            {
                if (!string.IsNullOrEmpty(nodeData.Name))
                {
                    NodeNames.Add(nodeData.Name);
                    Debug.Log($"[DTSReadSO] Found Node: {nodeData.Name}");
                }
            }

            // Get Group Names
            foreach (var groupData in graphSO.Groups)
            {
                if (!string.IsNullOrEmpty(groupData.GroupName))
                {
                    GroupNames.Add(groupData.GroupName);
                    Debug.Log($"[DTSReadSO] Found Group: {groupData.GroupName}");
                }
            }

            // Get Condition Names
            foreach (var conditionData in graphSO.Conditions)
            {
                if (!string.IsNullOrEmpty(conditionData.DialogueName))
                {
                    ConditionNames.Add(conditionData.DialogueName);
                    Debug.Log($"[DTSReadSO] Found Condition: {conditionData.DialogueName} (Parent Node: {conditionData.ParentNodeID})");
                }
            }
        }
        public static void GetInformationFromNode(DTSDialogueSO dialogueSO)
        {
            if (dialogueSO == null)
            {
                Debug.LogWarning("[DialogueDebugger] DialogueSO is null!");
                return;
            }

            // In thông tin cơ bản
            Debug.Log($"[DialogueDebugger] Dialogue: {dialogueSO.DialogueName} | Text: {dialogueSO.Text}");//game text

            // 1. Kiểm tra điều kiện
            if (dialogueSO.Conditions.Count > 0)
            {
                // Debug.Log($"[DialogueDebugger] Dialogue {dialogueSO.DialogueName} has {dialogueSO.Conditions.Count} condition(s):");
                foreach (var condSO in dialogueSO.Conditions)
                {
                    Debug.Log($"   ConditionSO: {condSO.ConditionName}, Type: {condSO.DialogueType}");

                    if (condSO.Conditions != null && condSO.Conditions.Count > 0)
                    {
                        foreach (var cond in condSO.Conditions)
                        {
                            Debug.Log($"      -> {cond.GetType().Name}: {cond}");
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"[DialogueDebugger] Dialogue {dialogueSO.DialogueName} has no conditions.");
            }

            // 2. Kiểm tra choice
            if (dialogueSO.Choices != null)
            {
                Debug.Log($"[DialogueDebugger] Dialogue {dialogueSO.DialogueName} has {dialogueSO.Choices.Count} choice(s):");
                foreach (var choice in dialogueSO.Choices)
                {
                    if (choice.ConnectedNodeIDs != null && choice.ConnectedNodeIDs.Count > 0)
                    {
                        foreach (var nextNodeID in choice.ConnectedNodeIDs)
                        {
                            Debug.Log($"   Choice: {choice.Text} -> Next NodeID: {nextNodeID}");
                        }
                    }
                    else
                    {
                        Debug.Log($"   Choice: {choice.Text} -> No connected nodes (end of dialogue)");
                    }
                }
            }
            else
            {
                Debug.Log($"[DialogueDebugger] Dialogue {dialogueSO.DialogueName} has no choices.");
            }
        }
    }
}
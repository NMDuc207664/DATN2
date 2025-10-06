using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.DialogueSystem.Enum;
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
        public void GetInformationFromNode(DTSDialogueSO dialogueSO)
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
        public DTSNodeSaveData GetNodeByID(DTSGraphSaveDataSO graphSO, string nodeID)
        {
            if (graphSO == null)
            {
                Debug.LogWarning("[DTSReadSO] GraphSaveDataSO is null!");
                return null;
            }

            if (string.IsNullOrEmpty(nodeID))
            {
                Debug.LogWarning("[DTSReadSO] NodeID is null or empty!");
                return null;
            }

            // Tìm node trong danh sách Nodes của graphSO
            var node = graphSO.Nodes.FirstOrDefault(n => n.NodeID == nodeID);

            if (node == null)
            {
                Debug.LogWarning($"[DTSReadSO] No node found with NodeID: {nodeID}");
                return null;
            }

            Debug.Log($"[DTSReadSO] Found Node: ID={node.NodeID}, Name={node.Name}, Text={node.Text}, DialogueType={node.DialogueType}");
            return node;
        }

        public ReadSODto GetInformFromNode(DTSDialogueSO dialogueSO)
        {
            if (dialogueSO == null)
            {
                Debug.LogWarning("[DialogueDebugger] DialogueSO is null!");
                return null;
            }

            // Khởi tạo đối tượng ReadSODto
            var result = new ReadSODto
            {
                Dialogue = dialogueSO.Text,
                NextNodeId = new List<string>()
            };

            // In thông tin cơ bản
            //            Debug.Log($"[DialogueDebugger] Dialogue: {dialogueSO.DialogueName} | Text: {dialogueSO.Text}");

            // 1. Kiểm tra điều kiện
            if (dialogueSO.Conditions != null && dialogueSO.Conditions.Count > 0)
            {
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
            // else
            // {
            //     Debug.Log($"[DialogueDebugger] Dialogue {dialogueSO.DialogueName} has no conditions.");
            // }

            // 2. Kiểm tra choice và điền NextNodeId
            if (dialogueSO.Choices != null && dialogueSO.Choices.Count > 0)
            {
                // Debug.Log($"[DialogueDebugger] Dialogue {dialogueSO.DialogueName} has {dialogueSO.Choices.Count} choice(s):");

                if (dialogueSO.DialogueType == DTSDialogueType.SingleChoice)
                {
                    // Lấy NodeID đầu tiên từ choice đầu tiên (nếu có)
                    var firstChoice = dialogueSO.Choices.FirstOrDefault();
                    if (firstChoice != null && firstChoice.ConnectedNodeIDs != null && firstChoice.ConnectedNodeIDs.Count > 0)
                    {
                        result.NextNodeId.Add(firstChoice.ConnectedNodeIDs[0]);
                        //Debug.Log($"   Choice: {firstChoice.Text} -> Next NodeID: {firstChoice.ConnectedNodeIDs[0]} (SingleNode)");
                    }
                    // else
                    // {
                    //     Debug.Log($"   Choice: {firstChoice?.Text} -> No connected nodes (end of dialogue)");
                    // }
                }
                else if (dialogueSO.DialogueType == DTSDialogueType.MultipleChoice)
                {
                    // Lấy tất cả ConnectedNodeIDs từ tất cả choices
                    foreach (var choice in dialogueSO.Choices)
                    {
                        if (choice.ConnectedNodeIDs != null && choice.ConnectedNodeIDs.Count > 0)
                        {
                            result.NextNodeId.AddRange(choice.ConnectedNodeIDs);
                        }
                        // else
                        // {
                        //     Debug.Log($"   Choice: {choice.Text} -> No connected nodes (end of dialogue)");
                        // }
                    }
                }
            }

            return result;
        }
    }
}
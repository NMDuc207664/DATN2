using System.Collections.Generic;
using System.Linq;
using DATN2.Editor.Data.SaveModal;
using DATN2.Editor.Data.SaveModal.SO;
using DATN2.Editor.DialogueEditor;
using DATN2.Editor.DialogueEditor.Extension;
using DATN2.Editor.DialogueSystem;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.Editor.Applications
{
    public static class DTSLoadGraphSO
    {
        private static string graphFileName;
        private static DTSGraphView graphView;
        public static void Initialize(DTSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
        }
        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }
        public static void Load()
        {
            DTSGraphSaveDataSO graphData = LoadAsset<DTSGraphSaveDataSO>("Assets/Resources/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            graphView.ClearGraph();
            // var groupMap = LoadGroups(graphData.Groups);
            // var nodeMap = LoadNodes(graphData.Nodes, groupMap);
            // LoadConditions(graphData.Conditions, nodeMap, groupMap);
            // RebuildConnections(graphData.Choices);
            DTSEditor.UpdateFileName(graphData.GraphName);
            LoadNodes(graphData);
            LoadConditionNodes(graphData);
            GetGraphDetail(graphData);

        }

        // private static Dictionary<string, DTSGroup> LoadGroups(List<DTSGroupSaveData> groups)
        // {
        //     var groupMap = new Dictionary<string, DTSGroup>();

        //     foreach (var groupData in groups)
        //     {
        //         var group = GraphElementFactory.CreateGroup(graphView, groupData.GroupName, groupData.Position);
        //         group.ID = groupData.GroupID;
        //         groupMap.Add(groupData.GroupID, group);
        //     }

        //     return groupMap;
        // }

        private static Dictionary<string, DTSNode> LoadNodes(List<DTSNodeSaveData> nodes, Dictionary<string, DTSGroup> groupMap)
        {
            var nodeMap = new Dictionary<string, DTSNode>();

            foreach (var nodeData in nodes)
            {
                var baseNode = GraphElementFactory.CreateNode(graphView, nodeData.Name, nodeData.DialogueType, nodeData.Position);
                if (baseNode is DTSNode node)
                {
                    node.NodeID = nodeData.NodeID;
                    node.Text = nodeData.Text;
                    node.HaveConditions = nodeData.HasConditions;
                    node.Choices = new List<DTSChoiceSaveData>(nodeData.Choices);
                    node.userData = nodeData.IsStartingNode;

                    if (!string.IsNullOrEmpty(nodeData.GroupID) && groupMap.TryGetValue(nodeData.GroupID, out var group))
                    {
                        node.Group = group;
                        group.AddElement(node);
                    }

                    graphView.AddElementCustom(node);
                    nodeMap.Add(nodeData.NodeID, node);
                }
                else
                {
                    Debug.LogWarning($"[DTSLoadGraphSO] Node {nodeData.NodeID} is not a DTSNode (Type: {nodeData.DialogueType})");
                }
            }

            return nodeMap;
        }

        // private static void LoadConditions(List<DTSConditionSaveData> conditions, Dictionary<string, DTSNode> nodeMap, Dictionary<string, DTSGroup> groupMap)
        // {
        //     foreach (var conditionData in conditions)
        //     {
        //         if (!nodeMap.ContainsKey(conditionData.ParentNodeID))
        //         {
        //             Debug.LogWarning($"[DTSLoadGraphSO] Parent node {conditionData.ParentNodeID} not found for condition {conditionData.NodeID}");
        //             continue;
        //         }

        //         var parentNode = nodeMap[conditionData.ParentNodeID];
        //         var group = !string.IsNullOrEmpty(conditionData.GroupID) && groupMap.ContainsKey(conditionData.GroupID)
        //             ? groupMap[conditionData.GroupID]
        //             : null;

        //         // Ensure the parent node has a Condition Port
        //         if (!parentNode.HaveConditions)
        //         {
        //             parentNode.HaveConditions = true;
        //             parentNode.ToggleConditionPort(true);
        //             Debug.Log($"[DTSLoadGraphSO] Enabled HaveConditions for node {parentNode.NodeID} to support condition {conditionData.NodeID}");
        //         }

        //         var conditionNode = new DTSConditionNode
        //         {
        //             NodeID = conditionData.NodeID,
        //             DialogueName = conditionData.DialogueName,
        //             DialogueType = conditionData.DialogueType,
        //             Group = group
        //         };

        //         conditionNode.SetPosition(new Rect(conditionData.Position, Vector2.zero));
        //         conditionNode.Initialize(conditionData.DialogueName, graphView, conditionData.Position);
        //         conditionNode.SetParentNode(parentNode);

        //         if (conditionData.Conditions != null)
        //         {
        //             conditionNode.ConditionData = new List<DTSConditionAbstract>(conditionData.Conditions);
        //         }

        //         graphView.AddElementCustom(conditionNode);
        //         if (group != null)
        //         {
        //             group.AddElement(conditionNode);
        //         }

        //         var conditionPort = parentNode.inputContainer.Q<Port>(name: "Condition Port");
        //         if (conditionPort == null)
        //         {
        //             Debug.LogError($"[DTSLoadGraphSO] Condition Port not found on node {parentNode.NodeID} for condition {conditionData.NodeID}");
        //             continue;
        //         }

        //         var edge = new Edge
        //         {
        //             output = conditionNode.outputContainer[0].Q<Port>(),
        //             input = conditionPort
        //         };
        //         edge.input.Connect(edge);
        //         edge.output.Connect(edge);
        //         graphView.AddElement(edge);
        //         graphView.connectionTracker.HandleEdgeCreated(edge);
        //     }
        // }

        // private static void RebuildConnections(List<DTSChoiceSaveData> choices)
        // {
        //     var nodes = graphView.graphElements.OfType<DTSNode>().ToDictionary(n => n.NodeID, n => n);

        //     foreach (var choice in choices)
        //     {
        //         if (!nodes.ContainsKey(choice.NodeID))
        //         {
        //             Debug.LogWarning($"[DTSLoadGraphSO] Node {choice.NodeID} not found for choice {choice.ChoiceID}");
        //             continue;
        //         }

        //         var sourceNode = nodes[choice.NodeID];
        //         var choicePort = sourceNode.outputContainer.Q<Port>(name: choice.ChoiceID);

        //         if (choicePort == null)
        //         {
        //             Debug.LogWarning($"[DTSLoadGraphSO] Choice port {choice.ChoiceID} not found on node {choice.NodeID}");
        //             continue;
        //         }

        //         foreach (var connectedNodeID in choice.ConnectedNodeIDs)
        //         {
        //             if (!nodes.ContainsKey(connectedNodeID))
        //             {
        //                 Debug.LogWarning($"[DTSLoadGraphSO] Connected node {connectedNodeID} not found for choice {choice.ChoiceID}");
        //                 continue;
        //             }

        //             var targetNode = nodes[connectedNodeID];
        //             var targetPort = targetNode.inputContainer.Q<Port>(name: "Dialogue Connection");

        //             var edge = new Edge
        //             {
        //                 output = choicePort,
        //                 input = targetPort
        //             };
        //             edge.output.userData = new DTSChoiceSaveData
        //             {
        //                 ChoiceID = choice.ChoiceID,
        //                 Text = choice.Text,
        //                 NodeID = choice.NodeID,
        //                 ConnectedNodeIDs = new List<string>(choice.ConnectedNodeIDs)
        //             };
        //             edge.input.Connect(edge);
        //             edge.output.Connect(edge);

        //             graphView.AddElement(edge);
        //             graphView.connectionTracker.AddConnection(choice.ChoiceID, connectedNodeID);
        //         }
        //     }

        //     // Validate starting nodes
        //     foreach (var node in nodes.Values)
        //     {
        //         if (node.userData is bool isStartingNode && isStartingNode)
        //         {
        //             var inputPort = node.inputContainer.Q<Port>(name: "Dialogue Connection");
        //             if (!inputPort.connected)
        //             {
        //                 Debug.Log($"[DTSLoadGraphSO] Node {node.NodeID} marked as starting node (no input connections)");
        //             }
        //             else
        //             {
        //                 Debug.LogWarning($"[DTSLoadGraphSO] Node {node.NodeID} marked as starting node but has input connections");
        //             }
        //         }
        //     }
        // }
        public static void GetGraphDetail(DTSGraphSaveDataSO graphData)
        {
            Debug.Log($"[DTSLoadGraphSO] Graph Details for '{graphData.GraphName}':");

            // Groups
            Debug.Log($"[Groups] Total: {graphData.Groups.Count}");
            foreach (var group in graphData.Groups)
            {
                Debug.Log($"  Group ID: {group.GroupID}, Name: {group.GroupName}, Position: {group.Position}");
            }

            // Nodes
            Debug.Log($"[Nodes] Total: {graphData.Nodes.Count}");
            foreach (var node in graphData.Nodes)
            {
                Debug.Log($"  Node ID: {node.NodeID}, Name: {node.Name}, Type: {node.DialogueType}, " +
                          $"Text: {node.Text}, Position: {node.Position}, IsStartingNode: {node.IsStartingNode}, " +
                          $"HasConditions: {node.HasConditions}, GroupID: {(node.GroupID != null ? node.GroupID : "None")}");

                // Choices for this node
                Debug.Log($"    Choices: Total: {node.Choices.Count}");
                foreach (var choice in node.Choices)
                {
                    string connectedNodes = choice.ConnectedNodeIDs != null && choice.ConnectedNodeIDs.Count > 0
                        ? string.Join(", ", choice.ConnectedNodeIDs)
                        : "None";
                    Debug.Log($"      Choice ID: {choice.ChoiceID}, Text: {choice.Text}, NodeID: {choice.NodeID}, " +
                              $"Connected Nodes: [{connectedNodes}]");
                }
            }

            // Conditions
            Debug.Log($"[Conditions] Total: {graphData.Conditions.Count}");
            foreach (var condition in graphData.Conditions)
            {
                Debug.Log($"  Condition ID: {condition.NodeID}, Name: {condition.DialogueName}, Type: {condition.DialogueType}, " +
                          $"ParentNodeID: {condition.ParentNodeID}, GroupID: {(condition.GroupID != null ? condition.GroupID : "None")}, " +
                          $"Position: {condition.Position}");
                Debug.Log($"    Conditions Count: {(condition.Conditions != null ? condition.Conditions.Count : 0)}");
                if (condition.Conditions != null)
                {
                    foreach (var cond in condition.Conditions)
                    {
                        Debug.Log($"      Condition Data: {cond}");
                    }
                }
            }

            // Connections (from connection tracker)
            var connections = graphView.connectionTracker.GetAllConnections();
            Debug.Log($"[Connections] Total Choices with Connections: {connections.Count}");
            foreach (var connection in connections)
            {
                string connectedNodes = connection.Value != null && connection.Value.Count > 0
                    ? string.Join(", ", connection.Value)
                    : "None";
                Debug.Log($"  Choice ID: {connection.Key}, Connected Nodes: [{connectedNodes}]");
            }
        }
        private static void LoadNodes(DTSGraphSaveDataSO graphData)
        {
            foreach (var node in graphData.Nodes)
            {
                var baseNode = GraphElementFactory.CreateNode(graphView, node.Name, node.DialogueType, node.Position, skipDraw: true);
                if (baseNode is DTSNode dtsNode)
                {
                    dtsNode.NodeID = node.NodeID;
                    dtsNode.Text = node.Text;
                    dtsNode.HaveConditions = node.HasConditions;
                    dtsNode.Choices = node.Choices;
                    if (dtsNode.HaveConditions)
                    {
                        dtsNode.ToggleConditionPort(true);
                    }
                    dtsNode.Draw();
                    graphView.AddElementCustom(dtsNode);
                }
                else
                {
                    Debug.LogWarning($"[DTSLoadGraphSO] Node {node.NodeID} is not a DTSNode (Type: {node.DialogueType})");
                }
            }
        }
        private static void LoadConditionNodes(DTSGraphSaveDataSO graphData)
        {
            foreach (var conditionData in graphData.Conditions)
            {
                var baseNode = GraphElementFactory.CreateNode(graphView, conditionData.DialogueName, conditionData.DialogueType, conditionData.Position, skipDraw: true);

                if (baseNode is DTSConditionNode conditionNode)
                {
                    // Set basic properties
                    conditionNode.NodeID = conditionData.NodeID;
                    conditionNode.DialogueName = conditionData.DialogueName;

                    // Set condition data TRƯỚC khi Draw
                    conditionNode.ConditionData = conditionData.Conditions ?? new List<DTSConditionAbstract>();

                    // Tìm parent node dựa trên ParentNodeID
                    if (!string.IsNullOrEmpty(conditionData.ParentNodeID))
                    {
                        // Tìm parent node trong graphView
                        var parentNode = FindNodeByID(conditionData.ParentNodeID);
                        if (parentNode != null)
                        {
                            conditionNode.SetParentNode(parentNode);
                        }
                        else
                        {
                            Debug.LogWarning($"[DTSLoadGraphSO] Parent node with ID {conditionData.ParentNodeID} not found for condition node {conditionData.NodeID}");
                        }
                    }

                    // Draw CUỐI CÙNG để UI reflect đúng state
                    conditionNode.Draw();

                    graphView.AddElementCustom(conditionNode);
                }
                else
                {
                    Debug.LogWarning($"[DTSLoadGraphSO] Node {conditionData.NodeID} is not a DTSConditionNode (Type: {conditionData.DialogueType})");
                }
            }
        }

        // Helper method để tìm node theo ID
        private static DTSNode FindNodeByID(string nodeID)
        {
            var nodes = graphView.graphElements.OfType<DTSNode>();
            return nodes.FirstOrDefault(node => node.NodeID == nodeID);
        }
    }
}
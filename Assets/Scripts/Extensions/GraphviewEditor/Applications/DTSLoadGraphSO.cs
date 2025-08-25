using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.DialogueEditor;
using DATN2.GraphviewEditor.DialogueEditor.Extension;
using DATN2.GraphviewEditor.DialogueSystem;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.GraphviewEditor.Applications
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
            LoadGroups(graphData);
            AssignNodesToGroups(graphData);
            RebuildConditionConnections(graphData);
            RebuildNodeConnections(graphData);
            graphView.validator.ValidateAll(graphView);
            //DebugLogStartNodes();
            //GetGraphDetail(graphData);

        }
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
                    //dtsNode.Group.ID = node.GroupID;
                    if (dtsNode.HaveConditions)
                    {
                        dtsNode.ToggleConditionPort(true);
                    }
                    dtsNode.Draw();
                    graphView.AddElementCustom(dtsNode);
                }

                else
                {
                    //Debug.LogWarning($"[DTSLoadGraphSO] Node {node.NodeID} is not a DTSNode (Type: {node.DialogueType})");
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

                    conditionNode.NodeID = conditionData.NodeID;
                    conditionNode.DialogueName = conditionData.DialogueName;
                    // conditionNode.ConditionData = conditionData.Conditions ?? new List<DTSConditionAbstract>();
                    conditionNode.ConditionData = conditionData.Conditions;
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
                            //Debug.LogWarning($"[DTSLoadGraphSO] Parent node with ID {conditionData.ParentNodeID} not found for condition node {conditionData.NodeID}");
                        }
                    }

                    // Draw CUỐI CÙNG để UI reflect đúng state
                    conditionNode.Draw();
                    graphView.AddElementCustom(conditionNode);
                }
                else
                {
                    // Debug.LogWarning($"[DTSLoadGraphSO] Node {conditionData.NodeID} is not a DTSConditionNode (Type: {conditionData.DialogueType})");
                }
            }
        }

        private static void LoadGroups(DTSGraphSaveDataSO graphData)
        {
            foreach (var groupData in graphData.Groups)
            {
                var baseGroup = GraphElementFactory.CreateGroup(graphView, groupData.GroupName, groupData.Position);
                if (baseGroup is DTSGroup dtsGroup)
                {
                    dtsGroup.ID = groupData.GroupID;
                    dtsGroup.Title = groupData.GroupName;
                    graphView.AddElementCustom(dtsGroup);
                }
                else
                {
                    // Debug.LogWarning($"[DTSLoadGraphSO] Group {groupData.GroupID} is not a DTSGroup (Type: {groupData.GroupName})");
                }
            }
        }

        private static void RebuildConditionConnections(DTSGraphSaveDataSO graphData)
        {
            foreach (var conditionData in graphData.Conditions)
            {
                // Find condition node by NodeID
                var conditionNode = graphView.graphElements.OfType<DTSConditionNode>().FirstOrDefault(n => n.NodeID == conditionData.NodeID);
                if (conditionNode == null)
                {
                    Debug.LogWarning($"[DTSLoadGraphSO] Condition node {conditionData.NodeID} not found");
                    continue;
                }

                if (string.IsNullOrEmpty(conditionData.ParentNodeID))
                {
                    Debug.Log($"[DTSLoadGraphSO] Condition node {conditionData.NodeID} has no parent node");
                    continue;
                }

                // Find parent node by ParentNodeID
                var parentNode = graphView.graphElements.OfType<DTSNode>().FirstOrDefault(n => n.NodeID == conditionData.ParentNodeID);
                if (parentNode == null)
                {
                    Debug.LogWarning($"[DTSLoadGraphSO] Parent node {conditionData.ParentNodeID} not found for condition {conditionData.NodeID}");
                    continue;
                }

                var outputPort = conditionNode.outputContainer.Q<Port>(name: "Output");
                if (outputPort == null)
                {
                    Debug.LogWarning($"[DTSLoadGraphSO] Output port not found on condition node {conditionData.NodeID}");
                    //Debug.Log($"Output port name: {outputPort.name}");
                    continue;
                }

                var conditionPort = parentNode.inputContainer.Q<Port>(name: "Condition Port");
                if (conditionPort == null)
                {
                    Debug.LogError($"[DTSLoadGraphSO] Condition Port not found on parent node {conditionData.ParentNodeID} for condition {conditionData.NodeID}");
                    //Debug.Log($"Condition port name: {conditionPort.name}");
                    continue;
                }

                var edge = new Edge
                {
                    output = outputPort,
                    input = conditionPort
                };
                edge.input.Connect(edge);
                edge.output.Connect(edge);
                graphView.AddElement(edge);
                graphView.connectionTracker.HandleEdgeCreated(edge);
            }
        }
        private static void RebuildNodeConnections(DTSGraphSaveDataSO graphData)
        {
            foreach (var nodeData in graphData.Nodes)
            {
                foreach (var choice in nodeData.Choices)
                {
                    // Find source node by NodeID
                    var sourceNode = graphView.graphElements.OfType<DTSNode>().FirstOrDefault(n => n.NodeID == choice.NodeID);
                    if (sourceNode == null)
                    {
                        Debug.LogWarning($"[DTSLoadGraphSO] Source node {choice.NodeID} not found for choice {choice.ChoiceID}");
                        continue;
                    }

                    // Find output port by ChoiceID (stored in port.userData)
                    var outputPort = sourceNode.outputContainer.Children().OfType<Port>()
                        .FirstOrDefault(p => p.userData != null && p.userData.ToString() == choice.ChoiceID);
                    if (outputPort == null)
                    {
                        // Fallback: Try matching by index if ChoiceID not found in userData
                        int choiceIndex = nodeData.Choices.IndexOf(choice);
                        outputPort = sourceNode.outputContainer.Children().OfType<Port>()
                            .ElementAtOrDefault(choiceIndex);
                        if (outputPort == null)
                        {
                            Debug.LogWarning($"[DTSLoadGraphSO] Output port for choice {choice.ChoiceID} not found on node {choice.NodeID}");
                            continue;
                        }
                    }

                    // Check ConnectedNodeIDs
                    if (choice.ConnectedNodeIDs == null || choice.ConnectedNodeIDs.Count == 0)
                    {
                        Debug.Log($"[DTSLoadGraphSO] Choice {choice.ChoiceID} on node {choice.NodeID} has no connected nodes");
                        continue;
                    }

                    // Connect to each target node (typically one for single-capacity ports)
                    foreach (var targetNodeID in choice.ConnectedNodeIDs)
                    {
                        var targetNode = graphView.graphElements.OfType<DTSNode>().FirstOrDefault(n => n.NodeID == targetNodeID);
                        if (targetNode == null)
                        {
                            Debug.LogWarning($"[DTSLoadGraphSO] Target node {targetNodeID} not found for choice {choice.ChoiceID}");
                            continue;
                        }

                        var inputPort = targetNode.inputContainer.Q<Port>(name: "Dialogue Connection");
                        if (inputPort == null)
                        {
                            Debug.LogError($"[DTSLoadGraphSO] Dialogue Connection port not found on target node {targetNodeID} for choice {choice.ChoiceID}");
                            continue;
                        }

                        var edge = new Edge
                        {
                            output = outputPort,
                            input = inputPort
                        };
                        edge.input.Connect(edge);
                        edge.output.Connect(edge);
                        graphView.AddElement(edge);
                        graphView.connectionTracker.HandleEdgeCreated(edge);
                        Debug.Log($"[DTSLoadGraphSO] Connected choice {choice.ChoiceID} from node {choice.NodeID} to node {targetNodeID}");
                    }
                }
            }
        }
        private static void AssignNodesToGroups(DTSGraphSaveDataSO graphData)
        {
            // Cache groups for quick lookup
            var groups = graphView.graphElements.OfType<DTSGroup>().ToDictionary(g => g.ID, g => g);

            // Assign DTSNodes to groups
            foreach (var node in graphView.graphElements.OfType<DTSNode>())
            {
                string groupID = graphData.Nodes.FirstOrDefault(n => n.NodeID == node.NodeID)?.GroupID;
                if (!string.IsNullOrEmpty(groupID))
                {
                    if (groups.TryGetValue(groupID, out var group))
                    {
                        node.Group = group;
                        group.AddElement(node);
                        // Debug.Log($"[DTSLoadGraphSO] Assigned node {node.NodeID} to group {groupID}");
                    }
                    else
                    {
                        Debug.LogWarning($"[DTSLoadGraphSO] Group {groupID} not found for node {node.NodeID}");
                    }
                }
            }

            // Assign DTSConditionNodes to groups
            foreach (var conditionNode in graphView.graphElements.OfType<DTSConditionNode>())
            {
                string groupID = graphData.Conditions.FirstOrDefault(c => c.NodeID == conditionNode.NodeID)?.GroupID;
                if (!string.IsNullOrEmpty(groupID))
                {
                    if (groups.TryGetValue(groupID, out var group))
                    {
                        conditionNode.Group = group;
                        group.AddElement(conditionNode);
                        // Debug.Log($"[DTSLoadGraphSO] Assigned condition node {conditionNode.NodeID} to group {groupID}");
                    }
                    else
                    {
                        Debug.LogWarning($"[DTSLoadGraphSO] Group {groupID} not found for condition node {conditionNode.NodeID}");
                    }
                }
            }
        }
        // Helper method để tìm node theo ID
        private static DTSNode FindNodeByID(string nodeID)
        {
            var nodes = graphView.graphElements.OfType<DTSNode>();
            return nodes.FirstOrDefault(node => node.NodeID == nodeID);
        }
        private static void DebugLogStartNodes()
        {
            var startNodes = graphView.graphElements
                .OfType<DTSNode>()
                .Where(n => n.IsStartingNode())
                .ToList();

            Debug.Log($"[DTSLoadGraphSO] Found {startNodes.Count} starting node(s):");

            foreach (var node in startNodes)
            {
                Debug.Log($"   ▶ NodeID: {node.NodeID}, Name: {node.DialogueName}, Text: {node.Text}");
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.DialogueEditor;
using DATN2.GraphviewEditor.DialogueEditor.Extension;
using DATN2.GraphviewEditor.DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DATN2.GraphviewEditor.Applications
{
    public class DTSReadRuntime
    {
        private DTSGraphView _graphView;
        private GraphConnectionTracker _connectionTracker;
        public void Initialize(DTSGraphView graphView)
        {
            _graphView = graphView;
            _connectionTracker = graphView.connectionTracker;
        }
        // 1. Group
        public List<DTSGroupSaveData> GetAllGroupInformation()
        {
            var groups = _graphView.graphElements.OfType<DTSGroup>().ToList();
            var results = new List<DTSGroupSaveData>();
            foreach (var group in groups)
            {
                results.Add(new DTSGroupSaveData
                {
                    GroupID = group.ID,
                    GroupName = group.Title,
                    Position = group.GetPosition().position,
                    HasADialogueTalked = group.HasADialogueTalked
                });
            }
            return results;
        }

        // 2. Node
        public List<DTSNodeSaveData> GetAllDialogueNodeInformation()
        {
            var nodes = _graphView.graphElements.OfType<DTSNode>().ToList();
            var results = new List<DTSNodeSaveData>();
            var allChoices = GetAllChoiceInformation(); // Lấy toàn bộ thông tin Choices

            foreach (var node in nodes)
            {
                // Lọc ra các Choices thuộc về node hiện tại
                var nodeChoices = allChoices.Where(c => c.NodeID == node.NodeID).ToList();

                results.Add(new DTSNodeSaveData
                {
                    NodeID = node.NodeID,
                    Name = node.DialogueName,
                    Text = node.Text,
                    GroupID = node.Group != null ? node.Group.ID : null,
                    Group = node.Group != null ? new DTSGroupSaveData
                    {
                        GroupID = node.Group.ID,
                        GroupName = node.Group.Title,
                        Position = node.Group.GetPosition().position,
                    } : null,
                    DialogueType = node.DialogueType,
                    HasConditions = node.HaveConditions,
                    Position = node.GetPosition().position,
                    Choices = nodeChoices, // Sử dụng nodeChoices thay vì node.Choices?.ToList()
                    Conditions = GetConditionByNodeID(node.NodeID),
                    IsStartingNode = node.IsStartingNode(),
                    HasTalked = node.HasTalked,
                    ActiveKey = node.ActiveKey ?? new List<string>()
                });
            }

            return results;
        }
        public List<DTSConditionSaveData> GetAllConditionNodeInformation()
        {
            var nodes = _graphView.graphElements.OfType<DTSConditionNode>().ToList();
            var results = new List<DTSConditionSaveData>();

            foreach (var node in nodes)
            {
                results.Add(new DTSConditionSaveData
                {
                    NodeID = node.NodeID,
                    ParentNodeID = node.ParentNode.NodeID,
                    DialogueName = node.DialogueName,
                    GroupID = node.Group != null ? node.Group.ID : null,
                    DialogueType = node.DialogueType,
                    Position = node.GetPosition().position,
                    // ConditionSaveData sẽ xử lý riêng bên dưới
                    // Conditions = new List<DTSConditionAbstract>()
                    Conditions = node.ConditionData
                });
            }

            return results;
        }

        // 3. Connections
        public List<(string FromNode, string ToNode)> GetAllConnectionInformation()
        {
            var edges = _graphView.graphElements.OfType<Edge>().ToList();
            var results = new List<(string, string)>();

            foreach (var edge in edges)
            {
                if (edge.output?.node is DTSBaseNode outNode &&
                    edge.input?.node is DTSBaseNode inNode)
                {
                    results.Add((outNode.NodeID, inNode.NodeID));
                }
            }

            return results;
        }
        public List<DTSChoiceSaveData> GetAllChoiceInformation()
        {
            var nodes = _graphView.graphElements.OfType<DTSNode>().ToList();
            var results = new List<DTSChoiceSaveData>();

            // Lấy toàn bộ connections hiện tại từ tracker
            var allConnections = _connectionTracker.GetAllConnections();
            // Dictionary<string, List<string>>: choiceID -> list nodeIDs

            foreach (var node in nodes)
            {
                foreach (var choice in node.Choices)
                {
                    // Clone để tránh modify trực tiếp
                    var choiceCopy = new DTSChoiceSaveData
                    {
                        ChoiceID = choice.ChoiceID,
                        Text = choice.Text,
                        NodeID = node.NodeID,
                        ConnectedNodeIDs = allConnections.ContainsKey(choice.ChoiceID)
                            ? new List<string>(allConnections[choice.ChoiceID])
                            : new List<string>()
                    };

                    results.Add(choiceCopy);
                }
            }

            return results;
        }


        public List<DTSConditionSaveData> GetConditionByNodeID(string nodeID)
        {
            var conditions = _graphView.graphElements
                                       .OfType<DTSConditionNode>()
                                       .Where(c => c.ParentNode != null && c.ParentNode.NodeID == nodeID)
                                       .Select(c => new DTSConditionSaveData
                                       {
                                           NodeID = c.NodeID,
                                           ParentNodeID = c.ParentNode.NodeID,
                                           DialogueName = c.DialogueName,
                                           GroupID = c.Group != null ? c.Group.ID : null,
                                           DialogueType = c.DialogueType,
                                           Position = c.GetPosition().position,
                                           Conditions = c.ConditionData != null
                                               ? new List<DTSConditionAbstract>(c.ConditionData)
                                               : new List<DTSConditionAbstract>()
                                       })
                                       .ToList();

            return conditions;
        }

        public DTSNodeSaveData GetNodeDetailByNodeID(string nodeID)
        {
            var node = _graphView.graphElements
                .OfType<DTSNode>()
                .FirstOrDefault(n => n.NodeID == nodeID);

            if (node == null)
            {
                Debug.LogWarning($"[DTSReadRuntime] No node found with NodeID: {nodeID}");
                return null;
            }

            var nodeData = new DTSNodeSaveData
            {
                NodeID = node.NodeID,
                Name = node.DialogueName,
                Text = node.Text,
                GroupID = node.Group != null ? node.Group.ID : null,
                Group = node.Group != null ? new DTSGroupSaveData
                {
                    GroupID = node.Group.ID,
                    GroupName = node.Group.Title,
                    Position = node.Group.GetPosition().position,
                    // NodeSaveDatas = new List<DTSNodeSaveData>(),
                    // ConditionSaveDatas = new List<DTSConditionSaveData>()
                } : null,
                DialogueType = node.DialogueType,
                HasConditions = node.HaveConditions,
                Position = node.GetPosition().position,
                Choices = node.Choices?.ToList() ?? new List<DTSChoiceSaveData>(),
                Conditions = GetConditionByNodeID(node.NodeID),
                IsStartingNode = node.IsStartingNode(),
                HasTalked = node.HasTalked,
                ActiveKey = node.ActiveKey ?? new List<string>()
            };

            Debug.Log($"[DTSReadRuntime] Retrieved node details for NodeID: {nodeID}, Name: {nodeData.Name}, HasConditions: {nodeData.HasConditions}, Choices: {nodeData.Choices.Count}, Conditions: {nodeData.Conditions.Count}");
            return nodeData;
        }

        // New method to get a single group by GroupID
        public DTSGroupSaveData GetGroupByID(string groupID)
        {
            var group = _graphView.graphElements
                .OfType<DTSGroup>()
                .FirstOrDefault(g => g.ID == groupID);

            if (group == null)
            {
                Debug.LogWarning($"[DTSReadRuntime] No group found with GroupID: {groupID}");
                return null;
            }

            var groupData = new DTSGroupSaveData
            {
                GroupID = group.ID,
                GroupName = group.Title,
                Position = group.GetPosition().position,
                HasADialogueTalked = group.HasADialogueTalked
            };

            Debug.Log($"[DTSReadRuntime] Retrieved group details for GroupID: {groupID}, Name: {groupData.GroupName}");
            return groupData;
        }

        // New method to get all nodes where HasTalked is false
        public List<DTSNodeSaveData> GetAllNodeByHasTalked()
        {
            var allChoices = GetAllChoiceInformation();
            var nodes = _graphView.graphElements
                .OfType<DTSNode>()
                .Where(n => !n.HasTalked)
                .Select(n => new DTSNodeSaveData
                {
                    NodeID = n.NodeID,
                    Name = n.DialogueName,
                    Text = n.Text,
                    GroupID = n.Group != null ? n.Group.ID : null,
                    Group = n.Group != null ? new DTSGroupSaveData
                    {
                        GroupID = n.Group.ID,
                        GroupName = n.Group.Title,
                        Position = n.Group.GetPosition().position,
                    } : null,
                    DialogueType = n.DialogueType,
                    HasConditions = n.HaveConditions,
                    Position = n.GetPosition().position,
                    Choices = allChoices.Where(c => c.NodeID == n.NodeID).ToList(),
                    Conditions = GetConditionByNodeID(n.NodeID),
                    IsStartingNode = n.IsStartingNode(),
                    HasTalked = n.HasTalked,
                    ActiveKey = n.ActiveKey ?? new List<string>()
                })
                .ToList();

            Debug.Log($"[DTSReadRuntime] Retrieved {nodes.Count} nodes with HasTalked = false");
            return nodes;
        }

        public List<DTSGroupSaveData> GetAllGroupByHasTalked()
        {
            var groups = _graphView.graphElements
                .OfType<DTSGroup>()
                .Where(g => !g.HasADialogueTalked)
                .Select(g => new DTSGroupSaveData
                {
                    GroupID = g.ID,
                    GroupName = g.Title,
                    Position = g.GetPosition().position,
                    HasADialogueTalked = g.HasADialogueTalked
                })
                .ToList();

            Debug.Log($"[DTSReadRuntime] Retrieved {groups.Count} groups with HasADialogueTalked = false");
            return groups;
        }
    }
}

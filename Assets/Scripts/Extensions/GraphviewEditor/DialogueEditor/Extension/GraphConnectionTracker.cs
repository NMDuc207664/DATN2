using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DATN2.GraphviewEditor.DialogueEditor.Extension
{
    public class GraphConnectionTracker
    {
        private readonly Dictionary<string, List<string>> graphConnections = new();

        public void AddConnection(string choiceID, string nodeID)
        {
            if (!graphConnections.ContainsKey(choiceID))
                graphConnections[choiceID] = new List<string>();

            if (!graphConnections[choiceID].Contains(nodeID))
                graphConnections[choiceID].Add(nodeID);

            Debug.Log($"[Connect] Choice {choiceID} leads to Node {nodeID}");
            LogChoiceConnections(choiceID);
        }

        public void RemoveConnection(string choiceID, string nodeID)
        {
            if (graphConnections.ContainsKey(choiceID))
            {
                graphConnections[choiceID].Remove(nodeID);

                if (graphConnections[choiceID].Count == 0)
                    graphConnections.Remove(choiceID);

                Debug.Log($"[Disconnect] Choice {choiceID} no longer leads to Node {nodeID}");
                LogChoiceConnections(choiceID);
            }
        }

        public void LogChoiceConnections(string choiceID)
        {
            if (graphConnections.ContainsKey(choiceID))
            {
                string connectedNodes = string.Join(", ", graphConnections[choiceID]);
                Debug.Log($"Choice {choiceID} has connected: [{connectedNodes}]");
            }
            else
            {
                Debug.Log($"Choice {choiceID} has connected: []");
            }
        }
        /// <summary>
        /// Được gọi khi tạo edge mới
        /// </summary>
        public bool HandleEdgeCreated(Edge edge)
        {
            bool changed = false;

            if (edge.output.node is DTSConditionNode conditionNode &&
                edge.input.node is DTSNode targetNode)
            {
                conditionNode.SetParentNode(targetNode);
                Debug.Log($"[Connect] ConditionNode {conditionNode.NodeID} connected to Node {targetNode.NodeID}");
                changed = true;
            }

            if (edge.output.node is DTSNode fromNode &&
                edge.input.node is DTSNode toNode &&
                edge.output.userData is DTSChoiceSaveData choiceData)
            {
                AddConnection(choiceData.ChoiceID, toNode.NodeID);
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Được gọi khi xóa element (edge/node/…)
        /// </summary>
        public bool HandleElementRemoved(GraphElement element)
        {
            bool changed = false;

            if (element is Edge edge)
            {
                if (edge.output.node is DTSConditionNode conditionNode &&
                    edge.input.node is DTSNode targetNode)
                {
                    if (conditionNode.ParentNode == targetNode)
                    {
                        conditionNode.SetParentNode(null);
                        Debug.Log($"[Disconnect] ConditionNode {conditionNode.NodeID} disconnected from Node {targetNode.NodeID}");
                        changed = true;
                    }
                }

                if (edge.output.node is DTSNode &&
                    edge.input.node is DTSNode toNode &&
                    edge.output.userData is DTSChoiceSaveData choiceData)
                {
                    RemoveConnection(choiceData.ChoiceID, toNode.NodeID);
                    changed = true;
                }
            }

            return changed;
        }
        public Dictionary<string, List<string>> GetAllConnections()
        {
            return new Dictionary<string, List<string>>(graphConnections);
        }
    }

}
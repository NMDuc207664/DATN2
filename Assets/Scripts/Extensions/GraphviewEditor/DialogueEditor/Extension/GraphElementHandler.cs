
using System;
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DATN2.GraphviewEditor.DialogueEditor.Extension
{
    public class GraphElementHandler
    {
        private readonly DTSGraphView graphView;

        public GraphElementHandler(DTSGraphView view)
        {
            graphView = view;
        }

        public void HandleDeleteSelection()
        {
            Type groupType = typeof(DTSGroup);

            List<DTSGroup> groupsToDelete = new();
            List<DTSNode> nodesToDelete = new();
            List<DTSConditionNode> conditionNodesToDelete = new();

            foreach (GraphElement selectedElement in graphView.selection)
            {
                if (selectedElement is DTSNode node)
                {
                    nodesToDelete.Add(node);
                    continue;
                }

                if (selectedElement is DTSConditionNode conditionNode)
                {
                    conditionNodesToDelete.Add(conditionNode);
                    continue;
                }

                if (selectedElement.GetType() == groupType)
                {
                    groupsToDelete.Add((DTSGroup)selectedElement);
                }
            }

            // Xóa group và phần tử trong group
            foreach (var groupToDelete in groupsToDelete)
            {
                var groupNodes = groupToDelete.containedElements.OfType<DTSNode>().ToList();
                var groupConditionNodes = groupToDelete.containedElements.OfType<DTSConditionNode>().ToList();

                groupToDelete.RemoveElements(groupNodes);
                foreach (var node in groupNodes)
                    graphView.RemoveElement(node);

                groupToDelete.RemoveElements(groupConditionNodes);
                foreach (var conditionNode in groupConditionNodes)
                    graphView.RemoveElement(conditionNode);

                graphView.RemoveElement(groupToDelete);
            }

            // Xóa node thường
            foreach (var nodeToDelete in nodesToDelete)
            {
                if (nodeToDelete != null)
                {
                    graphView.RemoveElement(nodeToDelete);
                    nodeToDelete.DisconnectAllPorts();
                }
            }

            // Xóa condition node
            foreach (var conditionNodeToDelete in conditionNodesToDelete)
            {
                if (conditionNodeToDelete != null)
                {
                    graphView.RemoveElement(conditionNodeToDelete);
                    conditionNodeToDelete.DisconnectAllPorts();
                }
            }

            // graphView.DebugLogElementCounts();
            graphView.validator.ValidateAll(graphView);
        }

        public void HandleElementAddedToGroup(Group group, IEnumerable<GraphElement> elements)
        {
            if (group is DTSGroup dtsGroup)
            {
                foreach (var element in elements)
                {
                    if (element is DTSNode node)
                    {
                        node.Group = dtsGroup;
                        Debug.Log($"[Group] Node {node.NodeID} added to Group {dtsGroup.Title}");
                    }
                    else if (element is DTSConditionNode conditionNode)
                    {
                        conditionNode.Group = dtsGroup;
                        Debug.Log($"[Group] ConditionNode {conditionNode.NodeID} added to Group {dtsGroup.Title}");
                    }
                }
            }
        }

        public void HandleElementRemovedFromGroup(Group group, IEnumerable<GraphElement> elements)
        {
            if (group is DTSGroup dtsGroup)
            {
                foreach (var element in elements)
                {
                    if (element is DTSNode node)
                    {
                        node.Group = null;
                        Debug.Log($"[Group] Node {node.NodeID} removed from Group {dtsGroup.Title}");
                    }
                    else if (element is DTSConditionNode conditionNode)
                    {
                        conditionNode.Group = null;
                        Debug.Log($"[Group] ConditionNode {conditionNode.NodeID} removed from Group {dtsGroup.Title}");
                    }
                }
            }
        }
    }

}
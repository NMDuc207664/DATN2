using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DATN2.GraphviewEditor.DialogueEditor.Extension
{
    public class CheckConditionForNode
    {
        private readonly HashSet<DTSBaseNode> errorNodes = new HashSet<DTSBaseNode>();
        private void MarkNodeError(DTSBaseNode node, string reason, bool debug)
        {
            if (node == null) return;

            errorNodes.Add(node);
            node.mainContainer.style.backgroundColor = new Color(1f, 0f, 0f);
            if (debug)
            {
                Debug.Log($"[Error] {node.DialogueName}: {reason}");
            }
        }

        private void ClearNodeError(DTSBaseNode node)
        {
            if (node == null) return;

            // Chỉ reset nếu node này không còn trong errorNodes
            if (!errorNodes.Contains(node))
            {
                node.mainContainer.style.backgroundColor = node.defaultBackgroundColor;
            }
        }

        private void BeginValidationRound()
        {
            errorNodes.Clear(); // reset danh sách lỗi trước khi chạy loạt check mới
        }
        public void CheckForDuplicateNames(DTSGraphView graphView)
        {
            BeginValidationRound();

            var nodes = graphView.graphElements.OfType<DTSBaseNode>().ToList();
            var groups = graphView.graphElements.OfType<DTSGroup>().ToList();

            var nodeNameGroups = nodes.GroupBy(n => n.DialogueName).Where(g => g.Count() > 1).ToList();
            var groupNameGroups = groups.GroupBy(g => g.title).Where(g => g.Count() > 1).ToList();

            bool hasDuplicateNodeNames = nodeNameGroups.Any();
            bool hasDuplicateGroupNames = groupNameGroups.Any();

            foreach (var node in nodes)
            {
                if (nodeNameGroups.Any(g => g.Key == node.DialogueName))
                    MarkNodeError(node, "Duplicate Node Name", false);
                else
                    ClearNodeError(node);
            }

            foreach (var group in groups)
            {
                if (groupNameGroups.Any(g => g.Key == group.title))
                    group.style.backgroundColor = new Color(1f, 0f, 0f);
                else
                    group.style.backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            }

            const string KEY = "duplicate_names";
            if (hasDuplicateNodeNames || hasDuplicateGroupNames)
                graphView.editorWindow.SetWarning(KEY, "Duplicate names detected. Please rename Nodes/Groups before saving.");
            else
                graphView.editorWindow.ClearWarning(KEY);
        }

        // =========================
        // 🟡 Condition Connections
        // =========================
        public void CheckConditionConnections(DTSGraphView graphView)
        {
            BeginValidationRound();
            var allNodes = graphView.graphElements.OfType<DTSNode>().ToList();
            var nodesWithConditions = allNodes.Where(n => n.HaveConditions).ToList();
            var invalidNodes = nodesWithConditions
                .Where(node => node.ConditionChildren == null || node.ConditionChildren.Count == 0)
                .ToList();
            foreach (var node in allNodes)
            {
                if (nodesWithConditions.Contains(node) && invalidNodes.Contains(node))
                {
                    // Node có HasCondition = true nhưng không có ConditionChildren
                    MarkNodeError(node, "Missing ConditionChildren", false);
                }
                else
                {
                    // Node không có lỗi liên quan đến điều kiện (hoặc HasCondition = false)
                    ClearNodeError(node);
                }
            }

            const string WARNING_KEY = "condition_connections_missing";
            if (invalidNodes.Count > 0)
            {
                string warningMessage =
                    "These nodes have 'Has Conditions = true' but no ConditionNodes assigned:\n" +
                    string.Join(", ", invalidNodes.Select(n => n.DialogueName));

                graphView.editorWindow.SetWarning(WARNING_KEY, warningMessage);
            }
            else
            {
                graphView.editorWindow.ClearWarning(WARNING_KEY);
            }
        }

        // =========================
        // 🟡 Orphan Condition Nodes
        // =========================
        public void CheckOrphanConditionNodes(DTSGraphView graphView)
        {
            BeginValidationRound();
            var conditionNodes = graphView.graphElements.OfType<DTSConditionNode>().ToList();
            var orphanConditions = conditionNodes.Where(cn => cn.ParentNode == null).ToList();

            foreach (var node in conditionNodes)
            {
                if (orphanConditions.Contains(node))
                    MarkNodeError(node, "Orphan ConditionNode (no parent)", false);
                else
                    ClearNodeError(node);
            }

            const string WARNING_KEY = "orphan_condition_nodes";
            if (orphanConditions.Count > 0)
            {
                string warningMessage =
                    "These ConditionNodes are not connected to any parent Node:\n" +
                    string.Join(", ", orphanConditions.Select(cn => cn.DialogueName));

                graphView.editorWindow.SetWarning(WARNING_KEY, warningMessage);
            }
            else
            {
                graphView.editorWindow.ClearWarning(WARNING_KEY);
            }
        }
        public void ValidateAll(DTSGraphView graphView)
        {
            BeginValidationRound();
            CheckForDuplicateNames(graphView);
            CheckConditionConnections(graphView);
            CheckOrphanConditionNodes(graphView);
        }
    }
}
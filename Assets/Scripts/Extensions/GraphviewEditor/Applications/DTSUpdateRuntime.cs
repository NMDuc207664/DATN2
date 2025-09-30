using System.Linq;
using DATN2.GraphviewEditor.Data.SaveModal;
using UnityEngine;

namespace DATN2.GraphviewEditor.Applications
{
    public static class DTSUpdateRuntime
    {
        // Updates the HasTalked status for all nodes in the graph and groups that contain talked nodes
        public static void UpdateGraphHasTalked(DTSGraphSaveDataSO dialogueGraph, bool hasTalked)
        {
            foreach (var node in dialogueGraph.Nodes)
            {
                node.HasTalked = hasTalked;
            }

            // Update HasADialogueTalked for groups based on their nodes
            foreach (var group in dialogueGraph.Groups)
            {
                bool groupHasTalked = hasTalked; // If all nodes are set to hasTalked, group follows
                if (!hasTalked)
                {
                    // Only check nodes if we're setting HasTalked to false
                    groupHasTalked = dialogueGraph.Nodes.Any(n => n.GroupID == group.GroupID && n.HasTalked);
                }
                group.HasADialogueTalked = groupHasTalked;
            }

            Debug.Log($"[DTSUpdateRuntime] Updated graph '{dialogueGraph.GraphName}' HasTalked to {hasTalked}");
        }

        // Updates the HasTalked status for a specific node by NodeID and its associated group
        public static void UpdateNodeHasTalked(DTSGraphSaveDataSO dialogueGraph, string nodeID, bool hasTalked)
        {
            var node = dialogueGraph.Nodes.FirstOrDefault(n => n.NodeID == nodeID);
            if (node == null)
            {
                Debug.LogWarning($"[DTSUpdateRuntime] No node found with NodeID: {nodeID}");
                return;
            }

            // Only proceed if the HasTalked status is changing
            if (node.HasTalked != hasTalked)
            {
                node.HasTalked = hasTalked;

                // Update the group's HasADialogueTalked status if the node belongs to a group
                if (!string.IsNullOrEmpty(node.GroupID))
                {
                    var group = dialogueGraph.Groups.FirstOrDefault(g => g.GroupID == node.GroupID);
                    if (group != null)
                    {
                        if (hasTalked)
                        {
                            // If node is now talked, group is immediately talked
                            group.HasADialogueTalked = true;
                        }
                        else
                        {
                            // If node is now not talked, check if any other nodes in the group are talked
                            group.HasADialogueTalked = dialogueGraph.Nodes.Any(n => n.GroupID == group.GroupID && n.HasTalked);
                        }
                    }
                }

                Debug.Log($"[DTSUpdateRuntime] Updated node '{node.Name}' (NodeID: {nodeID}) HasTalked to {hasTalked}");
            }
        }
    }
}
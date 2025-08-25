using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.DialogueEditor;
using DATN2.GraphviewEditor.DialogueEditor.Extension;
using UnityEditor;
using UnityEngine;

namespace DATN2.GraphviewEditor.Applications
{
    public static class DTSSaveSO
    {
        public static void SaveGraph(DTSGraphView graphView, string graphFileName)
        {
            // Step 1: Delete existing graph data
            DTSRemoveSO.DeleteGraphData(graphFileName);

            // Step 2: Initialize reader and gather data
            var reader = new DTSReadRuntime();
            reader.Initialize(graphView);

            // Step 3: Create folder structure
            string containerFolderPath = $"Assets/Resources/Dialogues/{graphFileName}";
            CreateStaticFolder(graphFileName, containerFolderPath);

            // Step 4: Gather data from graph
            var groups = reader.GetAllGroupInformation();
            var nodes = reader.GetAllDialogueNodeInformation();
            var choices = reader.GetAllChoiceInformation();
            var conditions = reader.GetAllConditionNodeInformation();
            var connections = reader.GetAllConnectionInformation();

            // Step 5: Save all components
            SaveGraphSO(graphFileName, groups, nodes, choices, conditions);
            SaveGroups(groups, containerFolderPath);
            var nodeMap = SaveNodes(nodes, containerFolderPath);
            SaveConditions(nodeMap, conditions, groups, containerFolderPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[SaveGraph] {graphFileName} saved successfully!");
        }

        #region SAVE SECTIONS

        private static void SaveGraphSO(
            string graphFileName,
            List<DTSGroupSaveData> groups,
            List<DTSNodeSaveData> nodes,
            List<DTSChoiceSaveData> choices,
            List<DTSConditionSaveData> conditions)
        {
            var graphSO = ScriptableObject.CreateInstance<DTSGraphSaveDataSO>();
            graphSO.Initialize(graphFileName);
            graphSO.Groups = groups;
            graphSO.Nodes = nodes;
            graphSO.Choices = choices;
            graphSO.Conditions = conditions;

            CreateOrReplaceAsset(graphSO, $"Assets/Resources/Graphs/{graphFileName}_Graph.asset");
        }

        private static void SaveGroups(List<DTSGroupSaveData> groups, string containerFolderPath)
        {
            foreach (var group in groups)
            {
                var groupSO = ScriptableObject.CreateInstance<DTSDialogueGroupSO>();
                groupSO.Initialize(group.GroupName);

                CreateOrReplaceAsset(groupSO,
                    $"{containerFolderPath}/Groups/{group.GroupName}.asset");
            }
        }

        private static Dictionary<string, DTSDialogueSO> SaveNodes(
            List<DTSNodeSaveData> nodes,
            string containerFolderPath)
        {
            Dictionary<string, DTSDialogueSO> nodeMap = new();

            foreach (var node in nodes)
            {
                var dialogueSO = ScriptableObject.CreateInstance<DTSDialogueSO>();
                // Khởi tạo với danh sách Choices rỗng
                dialogueSO.Initialize(node.Name, node.Text,
                    new List<DTSChoiceSaveData>(),
                    node.DialogueType, node.IsStartingNode, node.HasConditions);
                string dialoguePath;

                //case GroupId
                if (node.GroupID != null)
                {
                    string groupName = node.Group.GroupName;
                    CreateFolder($"{containerFolderPath}/Groups", groupName);
                    // CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");
                    dialoguePath = $"{containerFolderPath}/Groups/{groupName}/{node.Name}.asset";
                    CreateOrReplaceAsset(dialogueSO, dialoguePath);
                }
                else
                {
                    CreateOrReplaceAsset(dialogueSO,
                     $"{containerFolderPath}/Global/Dialogues/{node.Name}.asset");
                }

                nodeMap[node.NodeID] = dialogueSO;

                // Sao chép toàn bộ thông tin của Choices, bao gồm ChoiceID
                foreach (var choice in node.Choices)
                {
                    dialogueSO.Choices.Add(new DTSChoiceSaveData
                    {
                        ChoiceID = choice.ChoiceID, // Thêm ChoiceID
                        Text = choice.Text,
                        NodeID = node.NodeID,
                        ConnectedNodeIDs = choice.ConnectedNodeIDs != null
                            ? new List<string>(choice.ConnectedNodeIDs)
                            : new List<string>()
                    });
                }
                EditorUtility.SetDirty(dialogueSO);
            }
            return nodeMap;
        }

        private static void SaveConditions(
            Dictionary<string, DTSDialogueSO> nodeMap,
            List<DTSConditionSaveData> conditions,
            List<DTSGroupSaveData> groups,
            string containerFolderPath)
        {
            foreach (var cond in conditions)
            {
                if (!nodeMap.ContainsKey(cond.ParentNodeID))
                    continue;

                var parentSO = nodeMap[cond.ParentNodeID];
                string parentName = parentSO.DialogueName;

                string condFolder;

                // Case Group
                if (!string.IsNullOrEmpty(cond.GroupID))
                {
                    var groupData = groups.Find(g => g.GroupID == cond.GroupID);
                    string groupName = groupData != null ? groupData.GroupName : "UnknownGroup";
                    string parentFolder = $"{containerFolderPath}/Groups/{groupName}";
                    condFolder = $"{parentFolder}/{parentName}_Conditions";

                    CreateFolder($"{containerFolderPath}/Groups", groupName);
                    CreateFolder(parentFolder, $"{parentName}_Conditions");
                }
                else
                {
                    // Case Global
                    condFolder = $"{containerFolderPath}/Global/Dialogues/{parentName}_Conditions";
                    CreateFolder($"{containerFolderPath}/Global/Dialogues", $"{parentName}_Conditions");
                }

                // Create container SO for ConditionNode
                var containerSO = ScriptableObject.CreateInstance<DTSConditionSO>();
                containerSO.Initialize(cond.NodeID, cond.DialogueType);

                // Copy list Abstract from cond.Conditions
                if (cond.Conditions != null)
                {
                    foreach (var abstractSO in cond.Conditions)
                    {
                        if (abstractSO != null && !containerSO.Conditions.Contains(abstractSO))
                        {
                            containerSO.Conditions.Add(abstractSO);
                        }
                    }
                }

                string path = $"{condFolder}/{cond.DialogueName}_Condition.asset";
                CreateOrReplaceAsset(containerSO, path);

                // Add to DialogueSO (avoid duplicates)
                if (!parentSO.Conditions.Contains(containerSO))
                {
                    parentSO.Conditions.Add(containerSO);
                    EditorUtility.SetDirty(parentSO);
                }
            }
        }

        #region HELPERS

        private static void CreateOrReplaceAsset(ScriptableObject so, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(path);
            so.name = assetName;
            if (existing == null)
            {
                AssetDatabase.CreateAsset(so, path);
            }
            else
            {
                EditorUtility.CopySerialized(so, existing);
                EditorUtility.SetDirty(existing);
            }
        }

        private static void CreateFolder(string parent, string newFolder)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{newFolder}"))
            {
                AssetDatabase.CreateFolder(parent, newFolder);
            }
        }

        private static void CreateStaticFolder(string graphFileName, string containerFolderPath)
        {
            CreateFolder("Assets", "Resources");
            CreateFolder("Assets/Resources", "Graphs");
            CreateFolder("Assets/Resources", "Dialogues");
            CreateFolder("Assets/Resources/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }

        #endregion
    }
}
#endregion
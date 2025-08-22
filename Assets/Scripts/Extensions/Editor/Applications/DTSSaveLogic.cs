// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using DATN2.Editor.Data.SaveModal;
// using DATN2.Editor.Data.SaveModal.SO;
// using DATN2.Editor.DialogueEditor;
// using DATN2.Editor.DialogueSystem;
// using UnityEditor;
// using UnityEngine;

// namespace DATN2.Editor.Applications
// {
//     public static class DTSSaveLogic
//     {
//         private static DTSGraphView _graphView;
//         private static string graphFileName;
//         private static string containerFolderPath;
//         private static List<DTSGroup> groups;
//         private static List<DTSNode> nodes;
//         private static List<DTSConditionNode> conditions;
//         private static Dictionary<string, DTSDialogueGroupSO> createDialogueGroup;
//         private static Dictionary<string, DTSDialogueSO> createDialogue;
//         private static Dictionary<string, DTSConditionSO> createCondition;
//         public static void Initialize(DTSGraphView graphView, string graphName)// idont know why have to Initialize
//         {
//             _graphView = graphView;
//             graphFileName = graphName;
//             containerFolderPath = $"Assets/Resources/Dialogues/{graphFileName}";// nơi lưu toàn bộ graph

//             groups = new List<DTSGroup>();
//             nodes = new List<DTSNode>();
//             conditions = new List<DTSConditionNode>();

//             createDialogueGroup = new Dictionary<string, DTSDialogueGroupSO>();
//             createDialogue = new Dictionary<string, DTSDialogueSO>();
//             createCondition = new Dictionary<string, DTSConditionSO>();
//         }

//         #region Save Method
//         public static void Save()
//         {
//             CreateStaticFolder();
//             GetElementsFromGraphView();
//             DTSGraphSaveDataSO graphData = CreateScriptableSavingData<DTSGraphSaveDataSO>("Assets/Resources/Graphs", $"{graphFileName}Graph");//tạo file graph
//             graphData.Initialize(graphFileName);//tạo file graph
//             DTSDialogueContainerSO dialogueContainer = CreateScriptableSavingData<DTSDialogueContainerSO>(containerFolderPath, graphFileName);//tạo file container
//             SaveGroups(graphData, dialogueContainer);
//             SaveNodes(graphData, dialogueContainer);
//             SaveConditions(graphData, dialogueContainer);//lưu node vào file graph

//             UpdateDialoguesChoicesConnections();
//             UpdateDialoguesConditionConnections();

//             SaveAsset(graphData);
//             SaveAsset(dialogueContainer);//lưu file container

//         }
//         #endregion
//         #region Create Static Folder
//         private static void CreateStaticFolder()
//         {
//             // Tạo theo thứ tự từ thư mục gốc đến thư mục con
//             CreateFolder("Assets", "Resources");          // Tạo Resources trước
//             CreateFolder("Assets/Resources", "Graphs");   // Tạo Graphs folder
//             CreateFolder("Assets/Resources", "Dialogues"); // Tạo Dialogues folder
//             CreateFolder("Assets/Resources/Dialogues", graphFileName); // Tạo folder chứa graph
//             CreateFolder(containerFolderPath, "Global");   // Bên ngoài group  
//             CreateFolder(containerFolderPath, "Groups");   // Bên trong group
//             CreateFolder($"{containerFolderPath}/Global", "Dialogues"); // Bên trong Global
//             CreateFolder($"{containerFolderPath}/Groups", "DialoguesGroup"); // Bên trong Groups
//         }

//         public static void CreateFolder(string path, string folderName)
//         {
//             if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
//             {
//                 return;
//             }
//             // Đảm bảo parent directory tồn tại
//             if (!AssetDatabase.IsValidFolder(path))
//             {
//                 Debug.LogError($"Parent directory does not exist: {path}");
//                 return;
//             }
//             AssetDatabase.CreateFolder(path, folderName);
//         }
//         #endregion
//         #region Get Elements From GraphView && Create Scriptable Saving Data
//         public static void GetElementsFromGraphView()
//         {
//             Type groupType = typeof(DTSGroup);
//             _graphView.graphElements.ForEach(graphElement =>
//             {
//                 if (graphElement is DTSNode node)
//                 {
//                     nodes.Add(node);
//                 }
//                 if (graphElement.GetType() == groupType)
//                 {
//                     DTSGroup group = (DTSGroup)graphElement;
//                     groups.Add(group);
//                     return;
//                 }
//                 if (graphElement is DTSConditionNode condition)
//                 {
//                     conditions.Add(condition);
//                 }
//             });
//         }
//         // public static T CreateScriptableSavingData<T>(string path, string assetName) where T : ScriptableObject//generic type for object SO
//         //                                                                                                        //phải decleare như này vì có nhiều loại SO trong editor và cần dùng chung thay vì phân biệt dẫn tới phải viết nhiều method
//         // {
//         //     string fullPath = $"{path}/{assetName}.asset";
//         //     T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);
//         //     if (asset == null)
//         //     {
//         //         asset = ScriptableObject.CreateInstance<T>();
//         //         AssetDatabase.CreateAsset(asset, fullPath);
//         //     }
//         //     return asset;
//         // }
//         public static T CreateScriptableSavingData<T>(string path, string assetName) where T : ScriptableObject
//         {
//             // Đảm bảo thư mục tồn tại trước khi tạo asset
//             if (!AssetDatabase.IsValidFolder(path))
//             {
//                 Debug.LogError($"Directory does not exist: {path}. Please ensure all required folders are created first.");

//                 // Thử tạo thư mục nếu chưa có (backup solution)
//                 string[] pathParts = path.Replace("Assets/", "").Split('/');
//                 string currentPath = "Assets";

//                 for (int i = 0; i < pathParts.Length; i++)
//                 {
//                     string nextPath = $"{currentPath}/{pathParts[i]}";
//                     if (!AssetDatabase.IsValidFolder(nextPath))
//                     {
//                         AssetDatabase.CreateFolder(currentPath, pathParts[i]);
//                         Debug.Log($"Created missing folder: {nextPath}");
//                     }
//                     currentPath = nextPath;
//                 }

//                 AssetDatabase.Refresh();
//             }

//             string fullPath = $"{path}/{assetName}.asset";
//             T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);

//             if (asset == null)
//             {
//                 asset = ScriptableObject.CreateInstance<T>();

//                 try
//                 {
//                     AssetDatabase.CreateAsset(asset, fullPath);
//                     AssetDatabase.Refresh();
//                     Debug.Log($"Successfully created asset: {fullPath}");
//                 }
//                 catch (UnityException ex)
//                 {
//                     Debug.LogError($"Failed to create asset at {fullPath}: {ex.Message}");
//                     throw;
//                 }
//             }

//             return asset;
//         }
//         public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
//         {
//             string fullPath = $"{path}/{assetName}.asset";

//             T asset = LoadAsset<T>(path, assetName);

//             if (asset == null)
//             {
//                 asset = ScriptableObject.CreateInstance<T>();

//                 AssetDatabase.CreateAsset(asset, fullPath);
//             }

//             return asset;
//         }
//         public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
//         {
//             string fullPath = $"{path}/{assetName}.asset";

//             return AssetDatabase.LoadAssetAtPath<T>(fullPath);
//         }
//         #endregion

//         private static void SaveGroups(DTSGraphSaveDataSO graphData, DTSDialogueContainerSO dialogueContainer)
//         {
//             List<string> groupNames = new List<string>();

//             foreach (DTSGroup group in groups)
//             {
//                 SaveGroupToGraph(group, graphData);
//                 SaveGroupToScriptableObject(group, dialogueContainer);

//                 groupNames.Add(group.title);
//             }

//             UpdateOldGroups(groupNames, graphData);
//         }
//         private static void SaveGroupToGraph(DTSGroup group, DTSGraphSaveDataSO graphData)
//         {
//             DTSGroupSaveData groupData = new DTSGroupSaveData()
//             {
//                 GroupID = group.ID,
//                 GroupName = group.title,
//                 Position = group.GetPosition().position
//             };
//             graphData.Groups.Add(groupData);
//         }
//         private static void SaveGroupToScriptableObject(DTSGroup group, DTSDialogueContainerSO dialogueContainer)
//         {
//             string groupName = group.title;
//             CreateFolder($"{containerFolderPath}/Groups", groupName);
//             CreateFolder($"{containerFolderPath}/Groups/{groupName}", "DialoguesGroup");
//             DTSDialogueGroupSO groupSO = CreateScriptableSavingData<DTSDialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);
//             groupSO.Initialize(groupName);
//             dialogueContainer.DialogueGroups.Add(groupSO, new List<DTSDialogueSO>());
//             SaveAsset(groupSO);
//         }

//         private static void SaveAsset(UnityEngine.Object asset)
//         {
//             EditorUtility.SetDirty(asset);
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//         }
//         private static void SaveNodes(DTSGraphSaveDataSO graphData, DTSDialogueContainerSO dialogueContainer)
//         {
//             SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();//key = name of group, value = name of node in side that group
//             List<string> ungroupedNodeNames = new List<string>();
//             foreach (DTSNode node in nodes)
//             {
//                 SaveNodeToGraph(node, graphData);
//                 SaveNodeToScriptableObject(node, dialogueContainer);

//                 if (node.Group != null)
//                 {
//                     groupedNodeNames.AddItem(node.Group.title, node.DialogueName);

//                     continue;
//                 }

//                 ungroupedNodeNames.Add(node.DialogueName);
//             }
//             UpdateDialoguesChoicesConnections();//new
//             UpdateOldGroupedNodes(groupedNodeNames, graphData);
//             UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);//new
//         }
//         private static void SaveNodeToGraph(DTSNode node, DTSGraphSaveDataSO graphData)
//         {
//             DTSNodeSaveData nodeData = new DTSNodeSaveData()
//             {
//                 NodeID = node.NodeID,
//                 Name = node.DialogueName,
//                 Choices = node.Choices,
//                 Text = node.Text,
//                 GroupID = node.Group?.ID,
//                 DialogueType = node.DialogueType,
//                 Position = node.GetPosition().position
//             };
//             graphData.Nodes.Add(nodeData);
//         }
//         private static void SaveConditions(DTSGraphSaveDataSO graphData, DTSDialogueContainerSO dialogueContainer)
//         {
//             foreach (DTSConditionNode condition in conditions)
//             {
//                 SaveConditionToGraph(condition, graphData);
//                 SaveConditionToScriptableObject(condition, dialogueContainer);
//             }
//         }
//         private static void SaveConditionToGraph(DTSConditionNode condition, DTSGraphSaveDataSO graphData)
//         {
//             DTSConditionSaveData conditionData = new DTSConditionSaveData()
//             {
//                 NodeID = condition.NodeID,
//                 ParentNodeID = condition.ParentNode.NodeID,
//                 DialogueType = condition.DialogueType,
//                 Position = condition.GetPosition().position,
//                 Conditions = new List<DTSConditionSO>()
//             };

//             // Tìm node cha trong graphData để add
//             DTSNodeSaveData parentNode = graphData.Nodes.Find(n => n.NodeID == condition.ParentNode.NodeID);
//             if (parentNode != null)
//             {
//                 if (parentNode.Conditions == null)
//                     parentNode.Conditions = new List<DTSConditionSaveData>();

//                 parentNode.Conditions.Add(conditionData);
//                 parentNode.HasConditions = true;
//             }
//         }
//         private static void SaveConditionToScriptableObject(DTSConditionNode condition, DTSDialogueContainerSO dialogueContainer)
//         {
//             // Thư mục conditions nằm trong node
//             string nodeFolderPath = $"{containerFolderPath}/Global/Dialogues/{condition.ParentNode.DialogueName}";
//             CreateFolder(nodeFolderPath, "Conditions");

//             string conditionFolderPath = $"{nodeFolderPath}/Conditions";
//             DTSConditionSO conditionSO = CreateScriptableSavingData<DTSConditionSO>(conditionFolderPath, condition.DialogueName);

//             conditionSO.Initialize(condition.DialogueName, condition.NodeID);

//             // Lưu vào dialogueContainer (nếu cần tra cứu nhanh khi load)
//             if (!dialogueContainer.Conditions.ContainsKey(condition.ParentNode.NodeID))
//             {
//                 dialogueContainer.Conditions.Add(condition.ParentNode.NodeID, new List<DTSConditionSO>());
//             }
//             dialogueContainer.Conditions[condition.ParentNode.NodeID].Add(conditionSO);
//         }


//         /////////////
//         private static void SaveNodeToScriptableObject(DTSNode node, DTSDialogueContainerSO dialogueContainer)//new method
//         {
//             DTSDialogueSO dialogue;
//             if (node.Group != null)
//             {
//                 dialogue = CreateAsset<DTSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

//                 dialogueContainer.DialogueGroups.AddItem(createDialogueGroup[node.Group.ID], dialogue);
//             }
//             else
//             {
//                 dialogue = CreateAsset<DTSDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);

//                 dialogueContainer.UngroupedDialogues.Add(dialogue);
//             }

//             dialogue.Initialize(
//                 node.DialogueName,
//                 node.Text,
//                 ConvertNodeChoicesToDialogueChoices(node.Choices),
//                 node.DialogueType,
//                 node.IsStartingNode(),
//                 node.HaveConditions
//             );

//             createDialogue.Add(node.NodeID, dialogue);
//             SaveAsset(dialogue);
//         }

//         private static List<DTSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DTSChoiceSaveData> nodeChoices)
//         {
//             List<DTSDialogueChoiceData> dialogueChoices = new List<DTSDialogueChoiceData>();
//             foreach (DTSChoiceSaveData nodeChoice in nodeChoices)
//             {
//                 DTSDialogueChoiceData choiceData = new DTSDialogueChoiceData()
//                 {
//                     Text = nodeChoice.Text
//                 };
//                 dialogueChoices.Add(choiceData);
//             }
//             return dialogueChoices;
//         }

//         public static void RemoveAsset(string path, string assetName)
//         {
//             AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
//         }
//         public static void RemoveFolder(string path)
//         {
//             FileUtil.DeleteFileOrDirectory($"{path}.meta");
//             FileUtil.DeleteFileOrDirectory($"{path}/");
//         }
//         private static void RemoveNodeAssets(string basePath, string nodeName)
//         {
//             // Delete main asset
//             RemoveAsset(basePath, nodeName);

//             // Delete node folder (including Conditions)
//             string nodeFolderPath = $"{basePath}/{nodeName}";
//             if (AssetDatabase.IsValidFolder(nodeFolderPath))
//             {
//                 string fullSystemPath = Path.Combine(Application.dataPath, nodeFolderPath.Replace("Assets/", ""));
//                 if (Directory.Exists(fullSystemPath))
//                 {
//                     Directory.Delete(fullSystemPath, true);
//                 }
//                 AssetDatabase.Refresh();
//             }
//         }
//         private static void UpdateOldGroups(List<string> currentGroupNames, DTSGraphSaveDataSO graphData)
//         {
//             if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
//             {
//                 List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

//                 foreach (string groupToRemove in groupsToRemove)
//                 {
//                     RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
//                 }
//             }

//             graphData.OldGroupNames = new List<string>(currentGroupNames);
//         }
//         // private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DTSGraphSaveDataSO graphData)
//         // {
//         //     if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
//         //     {
//         //         foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
//         //         {
//         //             List<string> nodesToRemove = new List<string>();

//         //             if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
//         //             {
//         //                 nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
//         //             }

//         //             foreach (string nodeToRemove in nodesToRemove)
//         //             {
//         //                 RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
//         //             }
//         //         }
//         //     }

//         //     graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
//         // }
//         private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DTSGraphSaveDataSO graphData)
//         {
//             if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
//             {
//                 foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
//                 {
//                     List<string> nodesToRemove = new List<string>();

//                     if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
//                     {
//                         nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
//                     }
//                     else
//                     {
//                         nodesToRemove = oldGroupedNode.Value;
//                     }

//                     foreach (string nodeToRemove in nodesToRemove)
//                     {
//                         RemoveNodeAssets($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
//                     }
//                 }
//             }
//             graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
//         }

//         // private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DTSGraphSaveDataSO graphData)
//         // {
//         //     if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
//         //     {
//         //         List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

//         //         foreach (string nodeToRemove in nodesToRemove)
//         //         {
//         //             RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
//         //         }
//         //     }

//         //     graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
//         // }
//         private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DTSGraphSaveDataSO graphData)
//         {
//             if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
//             {
//                 List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

//                 foreach (string nodeToRemove in nodesToRemove)
//                 {
//                     RemoveNodeAssets($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
//                 }
//             }

//             graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
//         }
//         private static void UpdateDialoguesChoicesConnections()
//         {
//             foreach (DTSNode node in nodes)
//             {
//                 DTSDialogueSO dialogue = createDialogue[node.NodeID];

//                 for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
//                 {
//                     DTSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

//                     if (string.IsNullOrEmpty(nodeChoice.NodeID))
//                     {
//                         continue;
//                     }

//                     dialogue.Choices[choiceIndex].NextDialogue = createDialogue[nodeChoice.NodeID];

//                     SaveAsset(dialogue);
//                 }
//             }
//         }
//         // private static void UpdateOldNodeConditions(SerializableDictionary<string, List<string>> currentNodeConditions, DTSGraphSaveDataSO graphData)
//         // {
//         //     if (graphData.OldNodeConditions != null && graphData.OldNodeConditions.Count != 0)
//         //     {
//         //         foreach (KeyValuePair<string, List<string>> oldNodeCond in graphData.OldNodeConditions)
//         //         {
//         //             string parentName = oldNodeCond.Key;
//         //             bool parentExists = nodes.Any(n => n.DialogueName == parentName);

//         //             List<string> condsToRemove = new List<string>();

//         //             if (parentExists)
//         //             {
//         //                 if (currentNodeConditions.ContainsKey(parentName))
//         //                 {
//         //                     condsToRemove = oldNodeCond.Value.Except(currentNodeConditions[parentName]).ToList();
//         //                 }
//         //                 else
//         //                 {
//         //                     condsToRemove = oldNodeCond.Value;
//         //                 }

//         //                 DTSNode parent = nodes.Find(n => n.DialogueName == parentName);
//         //                 string basePath = parent.Group != null
//         //                     ? $"{containerFolderPath}/Groups/{parent.Group.Title}/Dialogues"
//         //                     : $"{containerFolderPath}/Global/Dialogues";

//         //                 foreach (string condToRemove in condsToRemove)
//         //                 {
//         //                     string condPath = $"{basePath}/{parentName}/Conditions/{condToRemove}.asset";
//         //                     AssetDatabase.DeleteAsset(condPath);
//         //                 }
//         //             }
//         //             // If parent doesn't exist, conditions are already removed via node folder deletion
//         //         }
//         //     }
//         // }
//         private static void UpdateDialoguesConditionConnections()
//         {
//             foreach (DTSNode node in nodes)
//             {
//                 if (node.HaveConditions && node.ConditionChildren != null && node.ConditionChildren.Count > 0)
//                 {
//                     DTSDialogueSO dialogue = createDialogue[node.NodeID];
//                     // Assuming DTSDialogueSO has a List<DTSConditionSO> Conditions field
//                     dialogue.Conditions = new List<DTSConditionSO>();

//                     foreach (DTSConditionNode conditionNode in node.ConditionChildren)
//                     {
//                         if (createCondition.ContainsKey(conditionNode.NodeID))
//                         {
//                             dialogue.Conditions.Add(createCondition[conditionNode.NodeID]);
//                         }
//                     }

//                     SaveAsset(dialogue);
//                 }
//             }
//         }
//     }
// }
// using UnityEditor;
// using UnityEngine;
// using DATN2.GraphviewEditor.Inspectors;
// using DATN2.GraphviewEditor.Data.SaveModal.SO;
// using DATN2.GraphviewEditor.Inspectors.Handler;
// using System.Collections.Generic;
// using DATN2.GraphviewEditor.Data.SaveModal;

// namespace DATN2.GraphviewEditor.Style
// {
//     [CustomPropertyDrawer(typeof(DTSDialogue))]
//     public class DTSDialogueDrawer : PropertyDrawer
//     {
//         private bool foldout = true;

//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);

//             // Foldout header
//             Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//             foldout = EditorGUI.Foldout(foldoutRect, foldout, label, true);

//             if (foldout)
//             {
//                 EditorGUI.indentLevel++;
//                 float yPos = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                 // Dialogue Graph
//                 SerializedProperty dialogueGraphProp = property.FindPropertyRelative("dialogueGraph");
//                 Rect graphRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
//                 EditorGUI.PropertyField(graphRect, dialogueGraphProp);
//                 yPos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                 DTSGraphSaveDataSO currentDialogueGraph = (DTSGraphSaveDataSO)dialogueGraphProp.objectReferenceValue;

//                 if (currentDialogueGraph == null)
//                 {
//                     Rect helpRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight * 2);
//                     EditorGUI.HelpBox(helpRect, "Select a Dialogue Container to see more options.", MessageType.Info);
//                     EditorGUI.indentLevel--;
//                     EditorGUI.EndProperty();
//                     return;
//                 }

//                 // Filters
//                 SerializedProperty groupedDialoguesProp = property.FindPropertyRelative("groupedDialogues");
//                 SerializedProperty startingDialoguesOnlyProp = property.FindPropertyRelative("startingDialoguesOnly");

//                 Rect groupedRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
//                 EditorGUI.PropertyField(groupedRect, groupedDialoguesProp);
//                 yPos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                 Rect startingRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
//                 EditorGUI.PropertyField(startingRect, startingDialoguesOnlyProp);
//                 yPos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                 // Get graph name
//                 string graphName = currentDialogueGraph.GraphName;
//                 if (graphName.EndsWith("_Graph"))
//                 {
//                     graphName = graphName.Substring(0, graphName.Length - "_Graph".Length);
//                 }

//                 SerializedProperty dialogueGroupProp = property.FindPropertyRelative("dialogueGroup");
//                 SerializedProperty dialogueProp = property.FindPropertyRelative("dialogue");
//                 SerializedProperty selectedGroupIndexProp = property.FindPropertyRelative("selectedDialogueGroupIndex");
//                 SerializedProperty selectedDialogueIndexProp = property.FindPropertyRelative("selectedDialogueIndex");

//                 string dialogueFolderPath = $"Assets/Resources/Dialogues/{graphName}";
//                 DTSDialogueGroupSO dialogueGroup = null;
//                 List<string> dialogueNames;

//                 if (groupedDialoguesProp.boolValue)
//                 {
//                     // Draw Group Selection
//                     List<string> dialogueGroupNames = DTSGroupInspectorHandler.GetGroupNames(currentDialogueGraph);

//                     if (dialogueGroupNames.Count == 0)
//                     {
//                         Rect helpRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight * 2);
//                         EditorGUI.HelpBox(helpRect, "There are no Dialogue Groups in this Container.", MessageType.Warning);
//                         EditorGUI.indentLevel--;
//                         EditorGUI.EndProperty();
//                         return;
//                     }

//                     // Group popup
//                     Rect groupPopupRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
//                     int currentGroupIndex = selectedGroupIndexProp.intValue;
//                     currentGroupIndex = Mathf.Clamp(currentGroupIndex, 0, dialogueGroupNames.Count - 1);

//                     int newGroupIndex = EditorGUI.Popup(groupPopupRect, "Dialogue Group", currentGroupIndex, dialogueGroupNames.ToArray());
//                     if (newGroupIndex != currentGroupIndex)
//                     {
//                         selectedGroupIndexProp.intValue = newGroupIndex;
//                         selectedDialogueIndexProp.intValue = 0;
//                     }

//                     yPos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                     dialogueGroup = DTSGroupInspectorHandler.LoadDialogueGroup(graphName, dialogueGroupNames[newGroupIndex]);
//                     dialogueGroupProp.objectReferenceValue = dialogueGroup;

//                     if (dialogueGroup != null)
//                     {
//                         dialogueNames = currentDialogueGraph.GetGroupedDialogueNames(dialogueGroup, startingDialoguesOnlyProp.boolValue);
//                         dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}";
//                     }
//                     else
//                     {
//                         dialogueNames = new List<string>();
//                     }
//                 }
//                 else
//                 {
//                     dialogueNames = currentDialogueGraph.GetUngroupedDialogueNames(startingDialoguesOnlyProp.boolValue);
//                     dialogueFolderPath += "/Global/Dialogues";
//                 }

//                 // Draw Dialogue Selection
//                 if (dialogueNames.Count == 0)
//                 {
//                     Rect helpRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight * 2);
//                     EditorGUI.HelpBox(helpRect, "No dialogues found with current filters.", MessageType.Info);
//                 }
//                 else
//                 {
//                     Rect dialoguePopupRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
//                     int currentDialogueIndex = selectedDialogueIndexProp.intValue;
//                     currentDialogueIndex = Mathf.Clamp(currentDialogueIndex, 0, dialogueNames.Count - 1);

//                     int newDialogueIndex = EditorGUI.Popup(dialoguePopupRect, "Dialogue", currentDialogueIndex, dialogueNames.ToArray());
//                     selectedDialogueIndexProp.intValue = newDialogueIndex;

//                     yPos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                     // Load the dialogue
//                     DTSDialogueSO selectedDialogue = DTSDialogueInspectorHandler.LoadDialogue(dialogueFolderPath, dialogueNames[newDialogueIndex]);
//                     dialogueProp.objectReferenceValue = selectedDialogue;

//                     // Update conditions
//                     if (selectedDialogue != null)
//                     {
//                         SerializedProperty conditionProp = property.FindPropertyRelative("condition");
//                         conditionProp.ClearArray();
//                         conditionProp.arraySize = selectedDialogue.Conditions.Count;
//                         for (int i = 0; i < selectedDialogue.Conditions.Count; i++)
//                         {
//                             conditionProp.GetArrayElementAtIndex(i).objectReferenceValue = selectedDialogue.Conditions[i];
//                         }
//                     }
//                 }

//                 EditorGUI.indentLevel--;
//             }

//             EditorGUI.EndProperty();
//         }

//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             if (!foldout)
//                 return EditorGUIUtility.singleLineHeight;

//             float height = EditorGUIUtility.singleLineHeight; // Foldout
//             height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Dialogue Graph

//             SerializedProperty dialogueGraphProp = property.FindPropertyRelative("dialogueGraph");
//             if (dialogueGraphProp.objectReferenceValue == null)
//             {
//                 height += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing; // Help box
//                 return height;
//             }

//             height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Grouped dialogues
//             height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Starting dialogues only

//             SerializedProperty groupedDialoguesProp = property.FindPropertyRelative("groupedDialogues");
//             if (groupedDialoguesProp.boolValue)
//             {
//                 height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Group popup
//             }

//             height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Dialogue popup or help

//             return height;
//         }
//     }
// }
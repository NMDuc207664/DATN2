using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.Inspectors.Handler;
using DATN2.GraphviewEditor.Style;
using UnityEditor;
using UnityEngine;
namespace DATN2.GraphviewEditor.Inspectors
{
    [CustomEditor(typeof(DTSDialogue))]
    public class DTSInspector : Editor
    {
        private SerializedProperty dialogueGraphProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;
        private SerializedProperty conditionProperty;
        /* Filters */
        private SerializedProperty groupedDialoguesProperty;
        private SerializedProperty startingDialoguesOnlyProperty;

        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;
        private string graphName { get; set; }

        private void OnEnable()
        {
            serializedObject.Update();
            dialogueGraphProperty = serializedObject.FindProperty("dialogueGraph");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");
            conditionProperty = serializedObject.FindProperty("condition");

            groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
            startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");

            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDialogueGraphArea();
            DTSGraphSaveDataSO currentDialogueGraph = (DTSGraphSaveDataSO)dialogueGraphProperty.objectReferenceValue;

            if (currentDialogueGraph == null)
            {
                StopDrawing("Select a Dialogue Container to see the rest of the Inspector.");
                return;
            }

            DrawFiltersArea();
            bool currentGroupedDialoguesFilter = groupedDialoguesProperty.boolValue;
            bool currentStartingDialoguesOnlyFilter = startingDialoguesOnlyProperty.boolValue;

            List<string> dialogueNames;
            string dialogueInfoMessage;

            graphName = currentDialogueGraph.GraphName;
            if (graphName.EndsWith("_Graph"))
            {
                graphName = graphName.Substring(0, graphName.Length - "_Graph".Length);
            }
            string dialogueFolderPath = $"Assets/Resources/Dialogues/{graphName}";
            DTSDialogueGroupSO dialogueGroup = null;
            if (currentGroupedDialoguesFilter)
            {
                // Get actual group names from Groups list, not dialogue names
                List<string> dialogueGroupNames = DTSGroupInspectorHandler.GetGroupNames(currentDialogueGraph);

                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("There are no Dialogue Groups in this Dialogue Container.");
                    return;
                }

                dialogueGroup = DTSGroupInspectorHandler.DrawGroupArea(
                selectedDialogueGroupIndexProperty,
                dialogueGroupProperty,
                dialogueGroupNames,
                graphName
                );

                // Only proceed if we have a valid group selected
                if (dialogueGroup != null)
                {
                    dialogueNames = currentDialogueGraph.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);
                    dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}";
                }
                else
                {
                    dialogueNames = new List<string>();
                }

                dialogueInfoMessage = "There are no" + (currentStartingDialoguesOnlyFilter ? " Starting" : "") + " Dialogues in this Dialogue Group.";
            }
            else
            {
                dialogueNames = currentDialogueGraph.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += "/Global/Dialogues";
                dialogueInfoMessage = "There are no" + (currentStartingDialoguesOnlyFilter ? " Starting" : "") + " Ungrouped Dialogues in this Dialogue Container.";
            }

            // Draw the dialogue selection area
            DTSDialogueSO selectedDialogue = DTSDialogueInspectorHandler.DrawDialogueArea(
                selectedDialogueIndexProperty,
                dialogueProperty,
                dialogueNames,
                dialogueFolderPath,
                dialogueInfoMessage
            );
            if (selectedDialogue != null)
            {
                // conditionProperty.ClearArray();
                // conditionProperty.arraySize = selectedDialogue.Conditions.Count;

                // for (int i = 0; i < selectedDialogue.Conditions.Count; i++)
                // {
                //     conditionProperty.GetArrayElementAtIndex(i).objectReferenceValue = selectedDialogue.Conditions[i];
                // }
                bool needsUpdate = conditionProperty.arraySize != selectedDialogue.Conditions.Count;
                if (!needsUpdate)
                {
                    for (int i = 0; i < conditionProperty.arraySize; i++)
                    {
                        if (conditionProperty.GetArrayElementAtIndex(i).objectReferenceValue != selectedDialogue.Conditions[i])
                        {
                            needsUpdate = true;
                            break;
                        }
                    }
                }

                if (needsUpdate)
                {
                    conditionProperty.ClearArray();
                    conditionProperty.arraySize = selectedDialogue.Conditions.Count;
                    for (int i = 0; i < selectedDialogue.Conditions.Count; i++)
                    {
                        conditionProperty.GetArrayElementAtIndex(i).objectReferenceValue = selectedDialogue.Conditions[i];
                    }
                }
            }

            DTSConditionInspectorHandler.DrawConditionsArea(
             conditionProperty,
             selectedDialogue,
             graphName,
             currentGroupedDialoguesFilter,
             dialogueGroup != null ? dialogueGroup.GroupName : null
            );
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawDialogueGraphArea()
        {
            DTSInspectorUtility.DrawHeader("Dialogue Container");

            dialogueGraphProperty.DrawPropertyField();

            DTSInspectorUtility.DrawSpace();
        }
        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            DTSInspectorUtility.DrawHelpBox(reason, messageType);

            DTSInspectorUtility.DrawSpace();

            DTSInspectorUtility.DrawHelpBox("You need to select a Dialogue for this component to work properly at Runtime!", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }
        private void DrawFiltersArea()
        {
            DTSInspectorUtility.DrawHeader("Filters");

            groupedDialoguesProperty.DrawPropertyField();
            startingDialoguesOnlyProperty.DrawPropertyField();

            DTSInspectorUtility.DrawSpace();
        }


    }
}
// 2200622814
// 11311001
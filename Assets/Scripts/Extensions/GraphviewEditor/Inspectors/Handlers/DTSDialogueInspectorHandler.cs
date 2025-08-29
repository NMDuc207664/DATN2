using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.Style;
using UnityEditor;
using UnityEngine;
namespace DATN2.GraphviewEditor.Inspectors.Handler
{
    public static class DTSDialogueInspectorHandler
    {
        public static DTSDialogueSO DrawDialogueArea(
                  SerializedProperty selectedDialogueIndexProp,
                  SerializedProperty dialogueProp,
                  List<string> dialogueNames,
                  string dialogueFolderPath,
                  string dialogueInfoMessage)
        {
            DTSInspectorUtility.DrawHeader("Dialogue");

            if (dialogueNames == null || dialogueNames.Count == 0)
            {
                DTSInspectorUtility.DrawHelpBox(dialogueInfoMessage, MessageType.Info);
                DTSInspectorUtility.DrawSpace();
                return null;
            }

            if (selectedDialogueIndexProp.intValue >= dialogueNames.Count)
            {
                selectedDialogueIndexProp.intValue = 0;
            }

            int newSelectedIndex = DTSInspectorUtility.DrawPopup(
                "Dialogue", selectedDialogueIndexProp.intValue, dialogueNames.ToArray());

            if (newSelectedIndex != selectedDialogueIndexProp.intValue)
            {
                selectedDialogueIndexProp.intValue = newSelectedIndex;
                dialogueProp.objectReferenceValue = null;
            }

            string selectedDialogueName = dialogueNames[selectedDialogueIndexProp.intValue];
            DTSDialogueSO selectedDialogue = LoadDialogue(selectedDialogueName, dialogueFolderPath);

            if (selectedDialogue != null)
            {
                dialogueProp.objectReferenceValue = selectedDialogue;
                DTSInspectorUtility.DrawDisabledFields(() => dialogueProp.DrawPropertyField());
            }
            else
            {
                DTSInspectorUtility.DrawHelpBox($"Could not load dialogue: {selectedDialogueName}", MessageType.Warning);
            }

            DTSInspectorUtility.DrawSpace();
            return selectedDialogue;
        }

        private static DTSDialogueSO LoadDialogue(string dialogueName, string dialogueFolderPath)
        {
            string resourcePath = dialogueFolderPath.Replace("Assets/Resources/", "");
            string fullResourcePath = $"{resourcePath}/{dialogueName}";

            DTSDialogueSO dialogue = Resources.Load<DTSDialogueSO>(fullResourcePath);

            if (dialogue == null)
            {
                Debug.LogWarning($"[DTSDialogueInspectorHelper] Failed to load dialogue at Resources/{fullResourcePath}.asset");
            }

            return dialogue;
        }
    }
}
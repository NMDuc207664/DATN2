using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.Style;
using UnityEditor;
using UnityEngine;
namespace DATN2.GraphviewEditor.Inspectors.Handler
{
    public static class DTSGroupInspectorHandler
    {
        public static DTSDialogueGroupSO DrawGroupArea(
                        SerializedProperty selectedGroupIndexProp,
                        SerializedProperty dialogueGroupProp,
                        List<string> groupNames,
                        string graphName)
        {
            DTSInspectorUtility.DrawHeader("Dialogue Group");

            if (selectedGroupIndexProp.intValue >= groupNames.Count)
            {
                selectedGroupIndexProp.intValue = 0;
            }

            int newSelectedIndex = DTSInspectorUtility.DrawPopup(
                "Group", selectedGroupIndexProp.intValue, groupNames.ToArray());

            if (newSelectedIndex != selectedGroupIndexProp.intValue)
            {
                selectedGroupIndexProp.intValue = newSelectedIndex;
                dialogueGroupProp.objectReferenceValue = null;
            }

            string selectedGroupName = groupNames[selectedGroupIndexProp.intValue];
            DTSDialogueGroupSO selectedGroup = LoadDialogueGroup(graphName, selectedGroupName);

            if (selectedGroup != null)
            {
                dialogueGroupProp.objectReferenceValue = selectedGroup;
                DTSInspectorUtility.DrawDisabledFields(() => dialogueGroupProp.DrawPropertyField());
            }
            else
            {
                DTSInspectorUtility.DrawHelpBox($"Could not load group: {selectedGroupName}", MessageType.Warning);
            }

            DTSInspectorUtility.DrawSpace();
            return selectedGroup;
        }

        private static DTSDialogueGroupSO LoadDialogueGroup(string graphName, string groupName)
        {
            string resourcePath = $"Dialogues/{graphName}/Groups/{groupName}";
            DTSDialogueGroupSO group = Resources.Load<DTSDialogueGroupSO>(resourcePath);

            if (group == null)
            {
                Debug.LogWarning($"[DTSGroupInspectorHelper] Failed to load group at Resources/{resourcePath}.asset");
            }

            return group;
        }

        public static List<string> GetGroupNames(DTSGraphSaveDataSO graphData)
        {
            List<string> groupNames = new List<string>();

            foreach (var group in graphData.Groups)
            {
                if (!string.IsNullOrEmpty(group.GroupName))
                {
                    groupNames.Add(group.GroupName);
                }
            }

            return groupNames;
        }
    }
}
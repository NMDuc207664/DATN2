using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.Style;
using UnityEditor;
using UnityEngine;

namespace DATN2.GraphviewEditor.Inspectors.Handler
{
    public static class DTSConditionInspectorHandler
    {
        public static List<DTSConditionSO> DrawConditionsArea(
            SerializedProperty conditionProp,
            DTSDialogueSO dialogue,
            string graphName,
            bool isGrouped,
            string groupName = null)
        {
            DTSInspectorUtility.DrawHeader("Conditions");

            // 🔹 0. Display condition property as read-only
            if (conditionProp != null)
            {
                DTSInspectorUtility.DrawDisabledFields(() => conditionProp.DrawPropertyField());
                DTSInspectorUtility.DrawSpace();
            }

            List<DTSConditionSO> loadedConditions = new List<DTSConditionSO>();

            // 🔹 1. Conditions attached directly in DialogueSO
            if (dialogue != null && dialogue.Conditions != null && dialogue.Conditions.Count > 0)
            {
                foreach (var cond in dialogue.Conditions)
                {
                    if (cond == null) continue;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Condition (Attached)", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Name:", cond.ConditionName);
                    EditorGUILayout.LabelField("Type:", cond.GetType().Name);
                    EditorGUILayout.EndVertical();

                    loadedConditions.Add(cond);
                }
            }
            else
            {
                DTSInspectorUtility.DrawHelpBox("This dialogue has no attached conditions.", MessageType.Info);
            }

            // 🔹 2. Load all ConditionSO from Resources folder
            if (dialogue != null)
            {
                string basePath;
                if (isGrouped && !string.IsNullOrEmpty(groupName))
                {
                    basePath = $"Dialogues/{graphName}/Groups/{groupName}/{dialogue.DialogueName}_Conditions";
                }
                else
                {
                    basePath = $"Dialogues/{graphName}/Global/Dialogues/{dialogue.DialogueName}_Conditions";
                }

                var allConditions = Resources.LoadAll<DTSConditionSO>(basePath);

            }

            DTSInspectorUtility.DrawSpace();
            return loadedConditions;
        }
    }
}
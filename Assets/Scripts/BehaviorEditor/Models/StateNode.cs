using UnityEditor;
using UnityEngine;
namespace DATN2.Scripts.BehaviorEditor.Models
{
    public class StateNode : BaseNode
    {
        bool collapse;
        public string dialogId;
        public TextAsset inkFile;
        public State.State currentState;
        public override void DrawWindow()
        {
            // Collapse toggle and window resizing
            if (!collapse)
            {
                windowRect.height = 300;
            }
            else
            {
                windowRect.height = 100;
            }
            collapse = EditorGUILayout.Toggle("Collapse", collapse);

            // Ink file selection
            EditorGUILayout.LabelField("Ink File", EditorStyles.boldLabel);
            TextAsset newInkFile = EditorGUILayout.ObjectField("Ink File", inkFile, typeof(TextAsset), false) as TextAsset;

            // Optional: Validate that the selected file is an .ink file
            if (newInkFile != inkFile)
            {
                if (newInkFile == null || newInkFile.name.EndsWith(".ink"))
                {
                    inkFile = newInkFile;
                }
                else
                {
                    Debug.LogWarning("Please select a valid .ink file.");
                }
            }

            // Existing State field (optional: remove if not needed)
            if (currentState == null)
            {
                EditorGUILayout.LabelField("Add state to modify");
            }
            else
            {
                currentState = EditorGUILayout.ObjectField("State", currentState, typeof(State.State), false) as State.State;
            }

            // Optional: Display dialogId if still needed
            dialogId = EditorGUILayout.TextField("Dialog ID", dialogId);
        }
        public override void DrawCurves() { }
    }
}
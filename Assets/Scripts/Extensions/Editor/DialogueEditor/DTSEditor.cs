using System;
using System.Collections.Generic;
using System.IO;
using DS.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DATN2.Editor.DialogueEditor
{
    public class DTSEditor : EditorWindow
    {
        private DTSGraphView graphView;

        private readonly string defaultFileName = "DialoguesFileName";

        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;
        private VisualElement warningContainer;
        private readonly SerializableDictionary<string, string> warnings = new SerializableDictionary<string, string>();

        [MenuItem("DTS/TESTING")]
        public static void Open()
        {
            GetWindow<DTSEditor>("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
            AddWarningContainer();
        }

        private void AddGraphView()
        {
            graphView = new DTSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }


        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DSElementUtility.CreateButton("Save", () => Save());

            // Button loadButton = DSElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DSElementUtility.CreateButton("Reset", () => ResetGraph());

            miniMapButton = DSElementUtility.CreateButton("Minimap", () => ToggleMiniMap());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            // toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);

            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(defaultFileName);
        }

        private void ToggleMiniMap()
        {
            graphView.ToggleMiniMap();

            miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }

        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        // public void ShowWarning(string message)
        // {
        //     warningContainer.Clear();
        //     var helpBox = new HelpBox(message, HelpBoxMessageType.Error);
        //     warningContainer.Add(helpBox);
        // }

        // public void HideWarning()
        // {
        //     warningContainer.Clear();
        // }
        private void AddWarningContainer()
        {
            warningContainer = new VisualElement();
            warningContainer.style.marginTop = 4;
            rootVisualElement.Add(warningContainer);
        }
        public void SetWarning(string key, string message)
        {
            warnings[key] = message;   // cập nhật/ghi đè theo key
            UpdateWarnings();
        }

        public void ClearWarning(string key)
        {
            if (warnings.Remove(key))
                UpdateWarnings();
        }

        private void UpdateWarnings()
        {
            warningContainer.Clear();

            if (warnings.Count > 0)
            {
                foreach (var kv in warnings)
                {
                    var helpBox = new HelpBox(kv.Value, HelpBoxMessageType.Error);
                    warningContainer.Add(helpBox);
                }
                DisableSaving();
            }
            else
            {
                EnableSaving();
            }
        }
    }
}
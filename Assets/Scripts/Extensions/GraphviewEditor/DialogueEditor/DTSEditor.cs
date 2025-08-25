using System;
using System.Collections.Generic;
using System.IO;
using DATN2.GraphviewEditor.Applications;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Style;
using DATN2.GraphviewEditor.Style.Components;
using DS.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.GraphviewEditor.DialogueEditor
{
    public class DTSEditor : EditorWindow
    {
        private DTSGraphView graphView;
        private DTSReadRuntime readRuntime;

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
            AddExtension();
            AddToolbar();
            AddStyles();
            AddWarningContainer();
        }
        private void OnDisable()//cần test thêm
        {
            if (graphView != null)
            {
                graphView.Dispose();
                rootVisualElement.Remove(graphView);
                graphView = null;
            }
        }

        private void AddGraphView()
        {
            graphView = new DTSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }
        private void AddExtension()
        {
            readRuntime = new DTSReadRuntime();
            readRuntime.Initialize(graphView);
        }


        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DTSElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DTSElementUtility.CreateButton("Save", () => Save());

            Button loadButton = DTSElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DTSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DTSElementUtility.CreateButton("Reset", () => ResetGraph());

            miniMapButton = DTSElementUtility.CreateButton("Minimap", () => ToggleMiniMap());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);

            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }
        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Resources/Graphs", "asset");
            Debug.Log(Path.GetFileNameWithoutExtension(filePath).Replace("_Graph", ""));
            DTSLoadGraphSO.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DTSLoadGraphSO.Load();

            // if (string.IsNullOrEmpty(filePath))
            // {
            //     return;
            // }

            // DTSLoadSO.LoadGraph(graphView, filePath);
            // UpdateFileName(Path.GetFileNameWithoutExtension(filePath).Replace("_Graph", ""));
        }
        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

                return;
            }
            DTSSaveSO.SaveGraph(graphView, fileNameTextField.value);


            // readRuntime.GetAllChoiceInformation();
            // foreach (var choice in allChoices)
            // {
            //     Debug.Log($"Choice {choice.ChoiceID} (Node {choice.NodeID}) -> Connected to: {string.Join(", ", choice.ConnectedNodeIDs)}");
            // }


            // DTSSaveLogic.Initialize(graphView, fileNameTextField.value);
            // DTSSaveLogic.Save();
            // readRuntime.Initialize(graphView);
            // readRuntime.DebugGraph();
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
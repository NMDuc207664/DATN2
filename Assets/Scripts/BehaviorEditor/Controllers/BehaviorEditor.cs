using System.Collections.Generic;
using DATN2.Assets.Scripts.BehaviorEditor.Interfaces;
using DATN2.Scripts.BehaviorEditor.Models;
using UnityEditor;
using UnityEngine;
using VContainer;

namespace DATN2.Assets.Scripts.BehaviorEditor.Controllers
{
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        static List<BaseNode> windows = new List<BaseNode>();
        Vector3 MousePosition;
        bool makeTransition = false;
        bool clickedOnWindow;
        BaseNode selectedNode;
        [Inject]
        private readonly IEditorInteractionService _editorInteractionService;

        #endregion

        #region Init
        [MenuItem("Behavior Editor/Dialog Graph")]

        static void ShowEditor()
        {
            // Get or create the EditorWindow
            BehaviorEditor editor = GetWindow<BehaviorEditor>();
            editor.minSize = new Vector2(1000, 600);

            // Create a VContainer scope for the EditorWindow
            DependencyInjector.ConfigureEditorDependencies(editor, windows);
        }

        #endregion

        #region GUI Methods
        private void OnGUI()
        {
            Event e = Event.current;
            MousePosition = e.mousePosition;
            UserInput(e);
            DrawWindow();
            if (makeTransition && selectedNode != null)
            {
                Rect mouseRect = new Rect(MousePosition.x, MousePosition.y, 10, 10);
                DrawNodeCurve(selectedNode.windowRect, mouseRect);
                Repaint();
            }
        }
        private void OnEnable()
        {

        }
        void DrawWindow()
        {
            BeginWindows();
            foreach (BaseNode n in windows)
            {
                n.DrawCurves();
            }
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodesWindow, windows[i].windowTitle);
            }
            EndWindows();
        }

        void DrawNodesWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        void UserInput(Event e)
        {
            if (e.button == 1 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    RightClick(e);
                }
            }

            if (e.button == 0 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {

                }
            }
            else if (e.button == 0 && e.type == EventType.MouseDown && makeTransition)
            {
                BaseNode targetNode = _editorInteractionService.FindNodeAtPosition(e.mousePosition);
                if (targetNode != null && !targetNode.Equals(selectedNode))
                {
                    _editorInteractionService.SetInput(targetNode, selectedNode, e.mousePosition);
                    makeTransition = false;
                    selectedNode = null;
                }
                if (targetNode == null)
                {
                    makeTransition = false;
                    selectedNode = null;
                }
                e.Use();
            }
            else if (e.button == 0 && e.type == EventType.MouseDown && !makeTransition)
            {
                BaseNode clickedNode = _editorInteractionService.FindNodeAtPosition(e.mousePosition);
                if (clickedNode != null)
                {
                    BaseNode nodeToChange = clickedNode.ClickedOnInput(e.mousePosition);
                    if (nodeToChange != null)
                    {
                        selectedNode = nodeToChange;
                        makeTransition = true;
                    }
                }
            }
        }

        void RightClick(Event e)
        {
            clickedOnWindow = _editorInteractionService.IsNodeClicked(e, windows, out selectedNode);
            if (!clickedOnWindow)
            {
                ShowAddNodeMenu(e);
            }
            else
            {
                ShowModifyNodeMenu(e);
            }
        }

        private void ShowModifyNodeMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            if (selectedNode is StateNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Make Transistion"), false,
                () => _editorInteractionService.HandleNodeInteraction(selectedNode, Action.makeTransition, MousePosition));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false,
                () => _editorInteractionService.HandleNodeInteraction(selectedNode, Action.deleteNode, MousePosition));
            }

            menu.ShowAsContext();
            e.Use();
        }

        void ShowAddNodeMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add State"), false, () => _editorInteractionService.HandleNodeInteraction(null, Action.addState, MousePosition));
            menu.AddItem(new GUIContent("Add Condition"), false, () => _editorInteractionService.HandleNodeInteraction(null, Action.addCondition, MousePosition));
            menu.AddItem(new GUIContent("Add Timeline"), false, () => _editorInteractionService.HandleNodeInteraction(null, Action.addTimeLine, MousePosition));
            menu.AddSeparator("");

            //menu.AddItem(new GUIContent("Add Transition Node"), false, ContextCallBack, UserAction.addTransitionNode);
            //menu.AddItem(new GUIContent("Delete Node"), false, ContextCallBack, UserAction.deleteNode);

            menu.ShowAsContext();
            e.Use();
        }

        public static void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadow = new Color(0, 0, 0, .06f);
            for (int i = 0; i < 1; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 5);
            }
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.cyan, null, 1);

        }

        #endregion
        #region Helper Method

        #endregion
    }
}
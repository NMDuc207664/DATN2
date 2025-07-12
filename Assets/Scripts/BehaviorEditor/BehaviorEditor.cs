using System.Collections.Generic;
using DATN2.Assets.Scripts.Manager;
using UnityEditor;
using UnityEngine;
namespace DATN2.Scripts.BehaviorEditor
{
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        static List<BaseNode> windows = new List<BaseNode>();
        Vector3 MousePosition;
        bool makeTransition;
        bool clickedOnWindow;
        BaseNode selectedNode;
        public enum UserAction
        {
            addState,
            addTransitionNode,
            deleteNode,
            commentNode
        }
        #endregion

        #region Init
        [MenuItem("Behavior Editor/Editor")]
        static void ShowEditor()
        {
            BehaviorEditor editor = EditorWindow.GetWindow<BehaviorEditor>();
            editor.minSize = new Vector2(1000, 600);
        }
        #endregion

        #region GUI Methods
        private void OnGUI()
        {
            Event e = Event.current;
            MousePosition = e.mousePosition;
            UserInput(e);
            DrawWindow();
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
        }

        void RightClick(Event e)
        {
            selectedNode = null;
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = windows[i];
                    break;
                }
            }
            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        private void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            if (selectedNode is StateNode)
            {
                StateNode stateNode = (StateNode)selectedNode;
                if (stateNode.currentState != null)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Add Transistion"), false, ContextCallBack, UserAction.addTransitionNode);
                }
                else
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("Add Transistion"));
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserAction.deleteNode);
            }
            if (selectedNode is CommentNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserAction.deleteNode);
            }
            menu.ShowAsContext();
            e.Use();
        }

        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add State"), false, ContextCallBack, UserAction.addState);
            //menu.AddItem(new GUIContent("Add Transition Node"), false, ContextCallBack, UserAction.addTransitionNode);
            //menu.AddItem(new GUIContent("Delete Node"), false, ContextCallBack, UserAction.deleteNode);
            menu.AddItem(new GUIContent("Add Comment"), false, ContextCallBack, UserAction.commentNode);
            menu.ShowAsContext();
            e.Use();
        }
        void ContextCallBack(object o)
        {
            UserAction a = (UserAction)o;
            switch (a)
            {

                case UserAction.addState:
                    StateNode stateNode = new StateNode
                    {
                        windowRect = new Rect(MousePosition.x, MousePosition.y, 200, 300),
                        windowTitle = "State"
                    };
                    windows.Add(stateNode);
                    break;
                case UserAction.addTransitionNode:
                    if (selectedNode is StateNode)
                    {
                        StateNode from = (StateNode)selectedNode;
                        Transition transition = from.AddTransition();
                        AddTransitionNode(from.currentState.transitions.Count, transition, from);
                    }
                    break;
                case UserAction.deleteNode:
                    if (selectedNode != null)
                    {
                        windows.Remove(selectedNode);
                    }
                    break;
                case UserAction.commentNode:
                    CommentNode commentNode = new CommentNode
                    {
                        windowRect = new Rect(MousePosition.x, MousePosition.y, 200, 300),
                        windowTitle = "Comment"
                    };
                    windows.Add(commentNode);
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region Helper Method

        public static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from)
        {
            Rect fromRect = from.windowRect;
            fromRect.x += 50;
            float targetY = fromRect.y - fromRect.height;
            if (from.currentState != null)
            {
                targetY += (index * 100);
            }
            fromRect.y = targetY;
            TransitionNode transitionNode = CreateInstance<TransitionNode>();
            transitionNode.Init(from, transition);
            transitionNode.windowRect = new Rect(fromRect.x + 300, fromRect.y + (fromRect.height * .7f), 200, 100);
            transitionNode.windowTitle = "Condition Check";
            windows.Add(transitionNode);
            return transitionNode;
        }
        // public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        // {
        //     Vector3 startPos = new Vector3(
        //         (left) ? start.x + start.width : start.x,
        //          start.y + (start.height * .5f),
        //          0
        //     );
        //     Vector3 endPos = new Vector3(
        //         end.x + (end.width * .5f),
        //          end.y + (end.height * .5f),
        //          0
        //     );
        //     Vector3 startTan = startPos + Vector3.right * 50;
        //     Vector3 endTan = endPos + Vector3.left * 50;
        //     Color shadow = new Color(0, 0, 0, 1);
        //     for (int i = 0; i < 1; i++)
        //     {
        //         Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * .5f);
        //     }

        //     Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 1);
        // }

        public static void ClearWindowsFromList(List<BaseNode> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                //      if (windows.Contains(l[i]))
                //        windows.Remove(l[i]);
                //reset
            }

        }
        #endregion
    }
}
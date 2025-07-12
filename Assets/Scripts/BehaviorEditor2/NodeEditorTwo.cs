using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class NodeEditorTwo : EditorWindow
{
    private List<BaseNodeTwo> windows = new List<BaseNodeTwo>();
    private Vector2 mousePosition;

    private BaseNodeTwo selectedNode;

    private bool makeTransitionMode = false;

    [MenuItem("Window Behavior/Node Editor")]
    static void ShowEditor()
    {
        NodeEditorTwo editor = EditorWindow.GetWindow<NodeEditorTwo>();
    }

    void OnGUI()
    {
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;
        mousePosition = mousePos;

        if (e.button == 1 && !makeTransitionMode)
        {
            if (e.type == EventType.MouseDown)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (!clickedOnWindow)
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Add State"), false, ContextCallBack, "inputNode");
                    menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallBack, "outputNode");
                    menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallBack, "calcNode");
                    menu.AddItem(new GUIContent("Add Comparison Node"), false, ContextCallBack, "compNode");
                    menu.ShowAsContext();
                    e.Use();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallBack, "makeTransition");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallBack, "deleteNode");
                    menu.ShowAsContext();
                    e.Use();
                }

            }
        }
        else if (e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode)
        {

            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }

            }
            if (clickedOnWindow && !windows[selectIndex].Equals(selectedNode))
            {
                windows[selectIndex].SetInputTwo((BaseInputNodeTwo)selectedNode, mousePos);
                makeTransitionMode = false;
                selectedNode = null;
            }
            if (!clickedOnWindow)
            {
                makeTransitionMode = false;
                selectedNode = null;
            }
            e.Use();
            //}


        }
        else if (e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode)
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }

            }
            if (clickedOnWindow)
            {
                BaseInputNodeTwo nodeToChange = windows[selectIndex].ClickedOnInput(mousePos);
                if (nodeToChange != null)
                {
                    selectedNode = nodeToChange;
                    makeTransitionMode = true;
                }
            }
        }
        if (makeTransitionMode && selectedNode != null)
        {
            Rect mouseRect = new Rect(mousePos.x, mousePos.y, 10, 10);
            NodeEditorTwo.DrawNodeCurveTwo(selectedNode.windowRect, mouseRect);
            Repaint();
        }
        foreach (BaseNodeTwo n in windows)
        {
            n.DrawCurvesTwo();
        }
        BeginWindows();

        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
        }

        EndWindows();
    }

    private void DrawNodeWindow(int id)
    {
        windows[id].DrawWindowTwo();
        GUI.DragWindow();
    }

    void ContextCallBack(object obj)
    {
        string clb = obj.ToString();
        if (clb.Equals("inputNode"))
        {
            InputNodeTwo inputNode = new InputNodeTwo();
            inputNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 150);
            windows.Add(inputNode);

        }
        else if (clb.Equals("outputNode"))
        {
            OutputNodeTwo outputNode = new OutputNodeTwo();
            outputNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100);
            windows.Add(outputNode);
        }
        else if (clb.Equals("calcNode"))
        {
            CalcNodeTwo calcNode = new CalcNodeTwo();
            calcNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100);
            windows.Add(calcNode);
        }
        else if (clb.Equals("compNode"))
        {
            CompareNodeTwo compNode = new CompareNodeTwo();
            compNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100);
            windows.Add(compNode);
        }
        else if (clb.Equals("deleteNode"))
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePosition))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }
            if (clickedOnWindow)
            {
                BaseNodeTwo selNode = windows[selectIndex];
                windows.RemoveAt(selectIndex);
                foreach (BaseNodeTwo node in windows)
                {
                    node.NodeDeleted(selNode);
                }
            }
        }
        else if (clb.Equals("makeTransition"))
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePosition))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }
            if (clickedOnWindow)
            {
                selectedNode = windows[selectIndex];
                makeTransitionMode = true;

            }
        }
    }
    public static void DrawNodeCurveTwo(Rect start, Rect end)
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
}

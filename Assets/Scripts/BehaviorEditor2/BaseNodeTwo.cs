using UnityEditor;
using UnityEngine;

public abstract class BaseNodeTwo : ScriptableObject
{
    public Rect windowRect;
    public bool hasInput = false;
    public string windowTitle = "";
    public virtual void DrawWindowTwo()
    {
        windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    }
    public abstract void DrawCurvesTwo();
    public virtual void SetInputTwo(BaseInputNodeTwo input, Vector2 clickPos)
    {

    }
    public virtual void NodeDeleted(BaseNodeTwo Node) { }
    public virtual BaseInputNodeTwo ClickedOnInput(Vector2 pos)
    {
        return null;
    }

}

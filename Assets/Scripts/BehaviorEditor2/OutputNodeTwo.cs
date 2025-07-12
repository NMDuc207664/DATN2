using UnityEngine;

public class OutputNodeTwo : BaseNodeTwo
{
    private string Result = "";
    private BaseInputNodeTwo inputNode;
    private Rect inputNodeRect;
    public OutputNodeTwo()
    {
        windowTitle = "Output Node";
        hasInput = true;
    }
    public override void DrawCurvesTwo()
    {
        if (inputNode)
        {
            Rect rect = windowRect;
            rect.x += inputNodeRect.x;
            rect.y += inputNodeRect.y + inputNodeRect.height;
            rect.width = 1;
            rect.height = 1;

            NodeEditorTwo.DrawNodeCurveTwo(inputNode.windowRect, rect);
        }
    }
    public override void DrawWindowTwo()
    {
        base.DrawWindowTwo();
        Event e = Event.current;
        string inputTitle = "None";
        if (inputNode)
        {
            inputTitle = inputNode.windowTitle;
        }
        GUILayout.Label("Input 1: " + inputTitle);
        if (e.type == EventType.Repaint)
        {
            inputNodeRect = GUILayoutUtility.GetLastRect();
        }

        GUILayout.Label("Result: " + Result);
    }

    public override void NodeDeleted(BaseNodeTwo node)
    {
        if (node.Equals(inputNode))
        {
            inputNode = null;
        }
    }
    public override BaseInputNodeTwo ClickedOnInput(Vector2 pos)
    {
        BaseInputNodeTwo retVal = null;
        pos.x -= windowRect.x;
        pos.y -= windowRect.y;
        if (inputNodeRect.Contains(pos))
        {
            retVal = inputNode;
            inputNode = null;
        }
        return retVal;
    }

    public override void SetInputTwo(BaseInputNodeTwo input, Vector2 clickPos)
    {

        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (inputNodeRect.Contains(clickPos))
        {
            inputNode = input;
        }
    }

}

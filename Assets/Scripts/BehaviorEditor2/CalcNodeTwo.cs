using UnityEditor;
using UnityEngine;

public class CalcNodeTwo : BaseInputNodeTwo
{
    private BaseInputNodeTwo input1;
    private Rect input1Rect;

    private BaseInputNodeTwo input2;
    private Rect input2Rect;

    private CalculationType calculationType;
    public enum CalculationType
    {
        Addition,
        Substraction,
        Multiplication,
        Division
    }
    public CalcNodeTwo()
    {
        windowTitle = "Calculation Node";
        hasInput = true;
    }

    public override void DrawWindowTwo()
    {
        base.DrawCurvesTwo();
        Event e = Event.current;
        calculationType = (CalculationType)EditorGUILayout.EnumPopup("Calculation Type: ", calculationType);
        string input1Title = "None";

        if (input1)
        {
            input1Title = input1.getResult();
        }

        GUILayout.Label("Input 1: " + input1Title);
        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }

        string input2Title = "None";

        if (input2)
        {
            input2Title = input1.getResult();
        }

        GUILayout.Label("Input 2: " + input2Title);
        if (e.type == EventType.Repaint)
        {
            input2Rect = GUILayoutUtility.GetLastRect();
        }
    }

    public override void SetInputTwo(BaseInputNodeTwo input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
        {
            input1 = input;
        }
        else if (input2Rect.Contains(clickPos))
        {
            input2 = input;
        }
    }

    public override void DrawCurvesTwo()
    {
        if (input1)
        {
            Rect rect = windowRect;
            rect.x += input1Rect.x;
            rect.y += input1Rect.y + input2Rect.height / 2;
            rect.width = 1;
            rect.height = 1;

            NodeEditorTwo.DrawNodeCurveTwo(input1.windowRect, rect);
        }
        if (input2)
        {
            Rect rect = windowRect;
            rect.x += input2Rect.x;
            rect.y += input2Rect.y + input2Rect.height / 2;
            rect.width = 1;
            rect.height = 1;

            NodeEditorTwo.DrawNodeCurveTwo(input2.windowRect, rect);
        }
    }
    public override string getResult()
    {
        float input1Value = 0;
        float input2Value = 0;

        if (input1)
        {
            string input1Raw = input1.getResult();
            float.TryParse(input1Raw, out input1Value);
        }

        if (input2)
        {
            string input2Raw = input2.getResult();
            float.TryParse(input2Raw, out input2Value);
        }
        string result = "False";
        switch (calculationType)
        {
            case CalculationType.Addition:
                result = (input1Value + input2Value).ToString();
                break;
            case CalculationType.Substraction:
                result = (input1Value - input2Value).ToString();
                break;
            case CalculationType.Multiplication:
                result = (input1Value * input2Value).ToString();
                break;
            case CalculationType.Division:
                result = (input1Value / input2Value).ToString();
                break;
                // default:
                //     return "None";
        }
        return result;
    }

    public override BaseInputNodeTwo ClickedOnInput(Vector2 pos)
    {
        BaseInputNodeTwo retVal = null;

        pos.x -= windowRect.x;
        pos.y -= windowRect.y;

        if (input1Rect.Contains(pos))
        {
            retVal = input1;
            input1 = null;
        }
        else if (input2Rect.Contains(pos))
        {
            retVal = input2;
            input2 = null;
        }
        return retVal;
    }

    public override void NodeDeleted(BaseNodeTwo node)
    {
        if (node.Equals(input1))
        {
            input1 = null;
        }
        if (node.Equals(input2))
        {
            input2 = null;
        }
    }
}

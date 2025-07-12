using UnityEditor;
using UnityEngine;


public class InputNodeTwo : BaseInputNodeTwo
{
    private InputType inputType;
    public enum InputType
    {
        Number,
        Randomized
    }
    private string randomFrom = "";
    private string randomTo = "";
    private string inputValue = "";
    public InputNodeTwo()
    {
        windowTitle = "Input Node";
    }

    public override void DrawWindowTwo()
    {
        base.DrawWindowTwo();
        inputType = (InputType)EditorGUILayout.EnumPopup("Input Type: ", inputType);
        if (inputType == InputType.Number)
        {
            inputValue = EditorGUILayout.TextField("Input Value: ", inputValue);
        }
        else if (inputType == InputType.Randomized)
        {
            randomFrom = EditorGUILayout.TextField("Random From: ", randomFrom);
            randomTo = EditorGUILayout.TextField("Random To: ", randomTo);
            if (GUILayout.Button("Calculate random"))
            {
                CalculateRandom();
            }
        }
    }

    public override void DrawCurvesTwo()
    {
        base.DrawCurvesTwo();
    }

    private void CalculateRandom()
    {
        float rFrom = 0;
        float rTo = 0;
        float.TryParse(randomFrom, out rFrom);
        float.TryParse(randomTo, out rTo);

        int randFrom = (int)(rFrom * 10);
        int randTo = (int)(rTo * 10);

        int selected = UnityEngine.Random.Range(randFrom, randTo + 1);
        float selectedValue = selected / 10f;

        // inputValue = UnityEngine.Random.Range(rFrom, rTo).ToString();
        inputValue = selectedValue.ToString();
    }

    public override string getResult()
    {
        return inputValue.ToString();
    }
}

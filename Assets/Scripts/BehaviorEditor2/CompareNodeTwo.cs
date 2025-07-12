using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CompareNodeTwo : BaseInputNodeTwo
{
   private ComparareNodeType comparisionType;

   public enum ComparareNodeType
   {
      Equal,
     Greater,
     Less,
   }

   private BaseInputNodeTwo input1;
   private Rect input1Rect;

   private BaseInputNodeTwo input2;
   private Rect input2Rect;

   private string comparedText ="";

   public CompareNodeTwo(){
    windowTitle = "Compare Node";
    hasInput = true;
   }

   public override void DrawWindowTwo()
   {
       base.DrawWindowTwo();
       Event e = Event.current;

       comparisionType = (ComparareNodeType)EditorGUILayout.EnumPopup("Comparision Type: ", comparisionType);
       string input1Title = "None"; 

       if (input1)
       {
           input1Title = input1.getResult();
       }

       GUILayout.Label("Input 1: " + input1Title);

       if(e.type == EventType.Repaint)
       {
        input1Rect = GUILayoutUtility.GetLastRect();
       }

       string input2Title = "None"; 

       if (input2)
       {
           input2Title = input2.getResult();
       }

       GUILayout.Label("Input 2: " + input2Title);

       if(e.type == EventType.Repaint)
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
        switch (comparisionType)
        {
            case ComparareNodeType.Equal:
               if(input1Value == input2Value){
                result = "True";
               }
                break;
            case ComparareNodeType.Greater:
                if(input1Value > input2Value){
                    result = "True";
                }
                break;
            case ComparareNodeType.Less:
                if (input1Value < input2Value)
                {
                    result = "True";
                }
                break;
           
                // default:
                //     return "None";
        }
        return result;
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
}

using UnityEngine;

namespace DATN2.Scripts.BehaviorEditor.Models
{
    public class TimelineNode : BaseNode
    {
        private ConditionNode _conditionNodeInput1;
        private Rect input1Rect;
        private StateNode _stateNodeInput2;
        private Rect input2Rect;

        public TimelineNode()
        {
            windowTitle = "Cutsence";
            hasInput = true;
        }
        public override void DrawWindow()
        {
            base.DrawWindow();
            Event e = Event.current;
            string input1Title = "None";
            string input2Title = "None";
            if (_conditionNodeInput1)
            {
                input1Title = _conditionNodeInput1.windowTitle;
            }
            if (_stateNodeInput2)
            {
                input2Title = _stateNodeInput2.windowTitle;
            }
            GUILayout.Label("Input 1: " + input1Title);
            GUILayout.Label("Input 2: " + input2Title);
            if (e.type == EventType.Repaint)
            {
                input1Rect = GUILayoutUtility.GetLastRect();
            }
            if (e.type == EventType.Repaint)
            {
                input2Rect = GUILayoutUtility.GetLastRect();
            }
        }
        public override void DrawCurves()
        {
            if (_conditionNodeInput1)
            {
                Rect rect = windowRect;
                rect.x += input1Rect.x;
                rect.y += input1Rect.y + input2Rect.height / 2;
                rect.width = 1;
                rect.height = 1;

                // BehaviorEditor.DrawNodeCurve(_conditionNodeInput1.windowRect, rect);
            }
            if (_stateNodeInput2)
            {
                Rect rect = windowRect;
                rect.x += input2Rect.x;
                rect.y += input2Rect.y + input2Rect.height / 2;
                rect.width = 1;
                rect.height = 1;

                // BehaviorEditor.DrawNodeCurve(_stateNodeInput2.windowRect, rect);
            }
        }
    }
}
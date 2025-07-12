using UnityEditor;
using UnityEngine;
using DATN2.Assets.Scripts.BehaviorEditor.Controllers;
namespace DATN2.Scripts.BehaviorEditor.Models
{
    public class ConditionNode : BaseNode
    {
        private ConditionType _conditionType;
        private bool collapse;
        private int numberOfInput = 1;// có thể fix sau
        //ở đây sẽ có private 1 cái scriptable object cho điều kiện nữa
        private StateNode _stateNode; // nơi mà nó sẽ nhận input, cái này sẽ còn bàn thêm
        private Rect inputNodeRect;
        public ConditionNode()
        {
            windowTitle = "Condition";
            hasInput = true;
        }
        public override void DrawWindow()
        {
            base.DrawWindow();
            if (!collapse)
            {
                windowRect.height = 300;
            }
            else
            {
                windowRect.height = 100;
            }
            collapse = EditorGUILayout.Toggle("Collapse", collapse);

            Event e = Event.current;
            string inputTitle = "None";
            if (_stateNode)
            {
                inputTitle = _stateNode.windowTitle;
            }
            GUILayout.Label("Input: " + inputTitle);
            if (e.type == EventType.Repaint)
            {
                inputNodeRect = GUILayoutUtility.GetLastRect();
            }

            _conditionType = (ConditionType)EditorGUILayout.EnumPopup("Condition Type", _conditionType);
            if (_conditionType == ConditionType.actionType)
            {
                numberOfInput = EditorGUILayout.IntField("Number of Input: ", numberOfInput);
                //ở dưới này là số ô để nhập scriptable object
            }
            if (_conditionType == ConditionType.EmotionType)
            {
                //scriptable duy nhất ở đây (sẽ tạo 1 scriptable object để lưu trữ điều kiện emotion chắc thế)
            }
        }
        public override void DrawCurves()//sửa cái này cho đẹp lên dễ nhìn hơn
        {
            if (_stateNode)
            {
                Rect rect = windowRect;
                rect.x += inputNodeRect.x;
                rect.y += inputNodeRect.y + inputNodeRect.height;
                rect.width = 1;
                rect.height = 1;

                // BehaviorEditor.DrawNodeCurve(_stateNode.windowRect, rect);
            }
        }

        public override BaseNode ClickedOnInput(Vector2 pos)
        {
            BaseNode retVal = null;
            pos.x -= windowRect.x;
            pos.y -= windowRect.y;
            if (inputNodeRect.Contains(pos))
            {
                retVal = _stateNode;
                _stateNode = null;
            }
            return retVal;
        }

        public override void SetInput(BaseNode input, Vector2 clickPos)
        {

            clickPos.x -= windowRect.x;
            clickPos.y -= windowRect.y;

            if (inputNodeRect.Contains(clickPos))
            {
                _stateNode = (StateNode)input;
            }
        }


    }
}

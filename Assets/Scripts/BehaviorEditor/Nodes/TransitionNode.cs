using DATN2.Assets.Scripts.BehaviorEditor.State;
using DATN2.Assets.Scripts.Manager;
using UnityEditor;
using UnityEngine;

namespace DATN2.Scripts.BehaviorEditor
{
    public class TransitionNode : BaseNode
    {
        public Transition targetTransition;
        public StateNode _enterNode;
        public StateNode _exitNode;

        public void Init(StateNode enterState, Transition transition)
        {
            _enterNode = enterState;
            targetTransition = transition;
        }

        public override void DrawWindow()
        {
            if (targetTransition == null)
            {
                return;
            }

            EditorGUILayout.LabelField("");
            targetTransition.condition = (Condition)EditorGUILayout.ObjectField(targetTransition.condition, typeof(Condition), false);

            if (targetTransition.condition == null)
            {
                EditorGUILayout.LabelField("No condition!");
            }
            else
            {
                targetTransition.disable = EditorGUILayout.Toggle("Disable", targetTransition.disable);
            }
        }

        public override void DrawCurves()
        {
            if (_enterNode)
            {
                Rect rect = windowRect;
                rect.y += windowRect.height * .5f;
                rect.width = 1;
                rect.height = 1;

                // BehaviorEditor.DrawNodeCurve(rect, _enterNode.windowRect, true, Color.cyan);
            }
        }

    }
}
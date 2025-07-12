using DATN2.Assets.Scripts.Manager;
using UnityEditor;
namespace DATN2.Scripts.BehaviorEditor
{
    public class StateNode : BaseNode
    {
        bool collapse;
        public State.State currentState;
        public override void DrawWindow()
        {
            if (currentState == null)
            {
                EditorGUILayout.LabelField("Add state to modify");
            }
            else
            {
                if (!collapse)
                {
                    windowRect.height = 300;
                }
                else
                {
                    windowRect.height = 100;
                }
                collapse = EditorGUILayout.Toggle(" ", collapse);

            }
            currentState = EditorGUILayout.ObjectField(currentState, typeof(State.State), false) as State.State;
        }
        public override void DrawCurves() { }
        public Transition AddTransition()
        {
            return currentState.AddTransition();
        }
    }
}
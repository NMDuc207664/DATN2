using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.Editor
{
    public class DACEditorWindow : EditorWindow
    {
        [MenuItem("DAC/Cutscene Controller Graph")]
        public static void Open()
        {
            GetWindow<DACEditorWindow>("DAC Editor Window");
        }
        void OnEnable()//co the switch qua createGUi
        {
            AddGraphView();

            AddStyle();
        }
        void AddGraphView()
        {
            DACGraphView graphview = new DACGraphView();
            graphview.StretchToParentSize();//phong to graphview = editor
            rootVisualElement.Add(graphview);
        }
        void AddStyle()
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/DialogSystem/Windows/DACVariables.uss");

            if (styleSheet != null)
            {
                rootVisualElement.styleSheets.Add(styleSheet);
            }
            else
            {
                Debug.LogError("Failed to load DACVariables.uss - Check path: Assets/Scripts/Editor/DialogSystem/Windows/DACVariables.uss");
            }
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }

}

using UnityEditor;
using UnityEngine;

namespace EW.windows
{
    public class DACEditorWindow : EditorWindow
    {
        [MenuItem("DAC/Cutscene Controller Graph")]
        public static void ShowExample()
        {
            DACEditorWindow editorWindow = GetWindow<DACEditorWindow>();
            editorWindow.titleContent = new GUIContent("Cutscene Controller Graph");
        }
    }

}

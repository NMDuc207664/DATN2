using UnityEngine;

namespace DATN2.Scripts.BehaviorEditor
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect windowRect; //tạo khung cửa sổ
        public string windowTitle;
        //view
        public virtual void DrawWindow() { }
        public virtual void DrawCurves() { }

    }

}

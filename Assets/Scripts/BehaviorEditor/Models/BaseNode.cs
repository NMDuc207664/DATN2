using System;
using UnityEngine;
namespace DATN2.Scripts.BehaviorEditor.Models
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect windowRect;
        public string windowTitle;
        public bool hasInput = false;
        public virtual void DrawWindow() { }
        public virtual void DrawCurves() { }

        public virtual BaseNode ClickedOnInput(Vector2 mousePosition)
        {

            return null;
        }

        public virtual void SetInput(BaseNode sourceNode, Vector2 mousePosition)
        {

        }
    }

}
using System;
using UnityEngine;
namespace DATN2.Scripts.BehaviorEditor.Models
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect windowRect; //tạo khung cửa sổ
        public string windowTitle;
        //view
        public virtual void DrawWindow() { }
        public virtual void DrawCurves() { }

        internal BaseNode ClickedOnInput(Vector2 mousePosition)
        {
            // Check if an input point (e.g., a connection point) was clicked
            // Return this node if it supports being a source for transitions
            return this;
        }

        internal void SetInput(BaseNode sourceNode, Vector2 mousePosition)
        {
            // Implement logic to connect this node to sourceNode
            throw new NotImplementedException();
        }
    }

}
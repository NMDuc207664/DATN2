using DATN2.Scripts.BehaviorEditor.Models;
using UnityEngine;

namespace DATN2.Assets.Scripts.BehaviorEditor.Interfaces
{
    public interface INodeService
    {
        public void AddStateNode(Vector2 mousePosition);
        public void DeleteNode(BaseNode node);
        public BaseNode FindNodeAtPosition(Vector2 mousePosition);
        public void AddTransitionNode(BaseNode selectedNode);
        public void AddTimeLine();
        public void AddCondition();
        void SetInput(BaseNode targetNode, BaseNode sourceNode, Vector2 mousePosition);

    }
}
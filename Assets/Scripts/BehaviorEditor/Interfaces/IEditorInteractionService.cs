using System.Collections.Generic;
using DATN2.Scripts.BehaviorEditor.Models;
using UnityEngine;

namespace DATN2.Assets.Scripts.BehaviorEditor.Interfaces
{
    public interface IEditorInteractionService
    {
        void HandleNodeInteraction(BaseNode selectedNode, Action action, Vector3 mousePosition);
        bool IsNodeClicked(Event e, List<BaseNode> windows, out BaseNode selectedNode);
        BaseNode FindNodeAtPosition(Vector2 mousePosition);
        void SetInput(BaseNode targetNode, BaseNode sourceNode, Vector2 mousePosition);
    }
}
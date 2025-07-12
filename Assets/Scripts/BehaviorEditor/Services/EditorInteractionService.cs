using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.BehaviorEditor.Interfaces;
using DATN2.Scripts.BehaviorEditor.Models;
using UnityEngine;
using VContainer;

namespace DATN2.Assets.Scripts.BehaviorEditor.Services
{
    public class EditorInteractionService : IEditorInteractionService
    {
        private readonly INodeService _nodeService;

        public EditorInteractionService(INodeService nodeService)
        {
            _nodeService = nodeService;
        }

        public BaseNode FindNodeAtPosition(Vector2 mousePosition)
        {
            return _nodeService.FindNodeAtPosition(mousePosition);
        }

        public void HandleNodeInteraction(BaseNode selectedNode, Action action, Vector3 mousePosition)
        {
            switch (action)
            {
                case Action.addState:
                    _nodeService.AddStateNode(mousePosition);
                    break;
                case Action.addTransitionNode:
                    _nodeService.AddTransitionNode(selectedNode);
                    break;
                case Action.deleteNode:
                    _nodeService.DeleteNode(selectedNode);
                    break;
                case Action.addTimeLine:
                    _nodeService.AddTimeLine();
                    break;
                case Action.addCondition:
                    _nodeService.AddCondition();
                    break;
            }
        }

        public bool IsNodeClicked(Event e, List<BaseNode> windows, out BaseNode selectedNode)
        {
            selectedNode = null;
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    selectedNode = windows[i];
                    return true;
                }
            }
            return false;
        }

        public void SetInput(BaseNode targetNode, BaseNode sourceNode, Vector2 mousePosition)
        {
            _nodeService.SetInput(targetNode, sourceNode, mousePosition);
        }
    }
}


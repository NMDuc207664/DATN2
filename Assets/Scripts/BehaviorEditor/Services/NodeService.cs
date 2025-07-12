using System.Collections.Generic;
using DATN2.Assets.Scripts.BehaviorEditor.Interfaces;
using DATN2.Scripts.BehaviorEditor.Models;
using UnityEngine;

public class NodeService : INodeService
{
    private List<BaseNode> _windows;
    public NodeService(List<BaseNode> windows)
    {
        _windows = windows;
    }
    public void AddCondition()
    {
        throw new System.NotImplementedException();
    }

    public void AddStateNode(Vector2 mousePosition)
    {
        StateNode stateNode = ScriptableObject.CreateInstance<StateNode>();
        stateNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 300);
        stateNode.windowTitle = "State";
        _windows.Add(stateNode);
    }

    public void AddTimeLine()
    {
        throw new System.NotImplementedException();
    }

    public void AddTransitionNode(BaseNode selectedNode)
    {
        Debug.Log("Adding transition node for: " + selectedNode.windowTitle);
    }

    public void DeleteNode(BaseNode node)
    {
        if (node != null)
        {
            _windows.Remove(node);
            ScriptableObject.DestroyImmediate(node, true); // Clean up ScriptableObject
        }
    }

    public BaseNode FindNodeAtPosition(Vector2 mousePosition)
    {
        foreach (var node in _windows)
        {
            if (node.windowRect.Contains(mousePosition))
            {
                return node;
            }
        }
        return null;
    }

    public void SetInput(BaseNode targetNode, BaseNode sourceNode, Vector2 mousePosition)
    {
        if (targetNode != null && sourceNode != null)
        {
            targetNode.SetInput(sourceNode, mousePosition);
        }
    }
}

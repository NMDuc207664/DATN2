using System;
using DATN2.Editor.DialogueEditor;
using DATN2.Editor.DialogueSystem;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.Editor.DialogueEditor.Extension
{
    public static class GraphElementFactory
    {
        public static DTSBaseNode CreateNode(DTSGraphView graphView, string nodeName, DTSDialogueType dialogueType, Vector2 position, bool skipDraw = false)
        {
            Type nodeType = Type.GetType($"DATN2.Editor.DialogueSystem.DTS{dialogueType}Node");
            DTSBaseNode node = (DTSBaseNode)Activator.CreateInstance(nodeType);
            node.Initialize(nodeName, graphView, position);

            if (!skipDraw)
            {
                node.Draw();
            }

            return node;

        }

        public static DTSGroup CreateGroup(DTSGraphView graphView, string title, Vector2 position)
        {
            return new DTSGroup(title, position, graphView);
        }
    }
}
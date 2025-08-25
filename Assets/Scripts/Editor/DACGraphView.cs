using System;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using DATN2.GraphviewEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.GraphviewEditor
{
    public class DACGraphView : GraphView
    {
        public DACGraphView()
        {
            AddGridBackGround();
            AddManipulators();
            AddStyles();
        }
        //cho phép 2 node liên kết với nhau
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach((port) =>
            {//các trường hợp không thể tự kết nối với bản thân
                // if (startPort == port)
                // {
                //     return;
                // }
                // if (startPort.node == port.node)
                // {
                //     return;
                // }
                // if (startPort.direction == port.direction)
                // {
                //     return;
                // }
                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        void AddGridBackGround()
        {
            GridBackground gridBackGround = new GridBackground();
            gridBackGround.StretchToParentSize();
            Insert(0, gridBackGround);// gridbackground có thứ tự 0 khi khởi tạo (layer sorting order)
        }

        void AddStyles()
        {
            StyleSheet graphViewStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/DialogSystem/Windows/DACStyleSheet.uss");
            StyleSheet nodeStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/DialogSystem/Windows/DACNodeStyle.uss");

            if (graphViewStyleSheet != null)
            {
                styleSheets.Add(graphViewStyleSheet);
            }
            else
            {
                Debug.LogError("Failed to load DACStyleSheet.uss - Check path: Assets/Scripts/Editor/DialogSystem/Windows/DACStyleSheet.uss");
            }

            if (nodeStyleSheet != null)
            {
                styleSheets.Add(nodeStyleSheet);
            }
            else
            {
                Debug.LogError("Failed to load DACNodeStyle.uss - Check path: Assets/Scripts/Editor/DialogSystem/Windows/DACNodeStyle.uss");
            }
        }
        void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(CreateGroupContextualMenu());
            this.AddManipulator(new ContentDragger());

            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextualMenu("Add Node(Single Choice)", DACDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node(Multiple Choice)", DACDialogueType.MultipleChoice));

        }
        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(

            menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("DialoguedGroup", actionEvent.eventInfo.localMousePosition))));

            return contextualMenuManipulator;
        }


        private IManipulator CreateNodeContextualMenu(string actionTitle, DACDialogueType dialogType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(

            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition, dialogType))));
            return contextualMenuManipulator;
            // contextualMenuManipulator.addItem(new MenuItem("Add Node", (a) => { CreateNode(); }), 0);//0 is the priority of the item, the higher the number the higher the priority of the item
        }
        private Group CreateGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group()
            {
                title = title,
            };
            group.SetPosition(new Rect(localMousePosition, Vector2.zero));
            return group;
        }
        private DACNode CreateNode(Vector2 position, DACDialogueType dialogType)
        {
            Type nodeType = Type.GetType($"DATN2.GraphviewEditor.Nodes.{dialogType}Node");//DS.Elements là namespace
            DACNode node = (DACNode)Activator.CreateInstance(nodeType);
            node.Initialize(position);
            node.Draw();
            // AddElement(node);
            return node;
        }
    }
}
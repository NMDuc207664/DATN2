using System;
using System.Collections.Generic;
using System.Linq;
using DATN2.Editor.DialogueSystem;
using DATN2.Editor.DialogueSystem.Enum;
using DS.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace DATN2.Editor.DialogueEditor
{
    public class DTSGraphView : GraphView
    {
        private DTSEditor editorWindow;
        private MiniMap miniMap;
        public DTSGraphView(DTSEditor dsEditor)
        {
            editorWindow = dsEditor;
            AddManipulators();
            AddGridBackground();
            AddMiniMap();
            AddMiniMapStyles();
            AddStyles();
            OnElementsDeleted();
            graphViewChanged = OnGraphViewChanged;
        }
        public void DebugLogElementCounts()
        {
            int groupCount = graphElements.OfType<DTSGroup>().Count();
            int nodeCount = graphElements.OfType<DTSNode>().Count();
            int conditionNodeCount = graphElements.OfType<DTSConditionNode>().Count();

            Debug.Log($"[Graph Stats] Groups: {groupCount} | Nodes: {nodeCount} | ConditionNodes: {conditionNodeCount}");
        }
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DTSGroup);

                List<DTSGroup> groupsToDelete = new List<DTSGroup>();
                List<DTSNode> nodesToDelete = new List<DTSNode>();
                List<DTSConditionNode> conditionNodesToDelete = new List<DTSConditionNode>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is DTSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement is DTSConditionNode conditionNode)
                    {
                        conditionNodesToDelete.Add(conditionNode);
                        continue;
                    }

                    if (selectedElement.GetType() != groupType)
                    {
                        continue;
                    }

                    DTSGroup group = (DTSGroup)selectedElement;

                    groupsToDelete.Add(group);
                }

                foreach (DTSGroup groupToDelete in groupsToDelete)
                {
                    List<DTSNode> groupNodes = new List<DTSNode>();
                    List<DTSConditionNode> groupConditionNodes = new List<DTSConditionNode>();

                    foreach (GraphElement groupElement in groupToDelete.containedElements)
                    {
                        if (groupElement is DTSNode groupNode)
                        {
                            groupNodes.Add(groupNode);
                            continue;
                        }

                        if (groupElement is DTSConditionNode groupConditionNode)
                        {
                            groupConditionNodes.Add(groupConditionNode);
                            continue;
                        }
                    }

                    // Xóa node thường trong nhóm
                    groupToDelete.RemoveElements(groupNodes);
                    foreach (var node in groupNodes)
                    {
                        // node.DisconnectAllPorts();
                        RemoveElement(node);
                    }

                    // Xóa condition node trong nhóm
                    groupToDelete.RemoveElements(groupConditionNodes);
                    foreach (var conditionNode in groupConditionNodes)
                    {
                        // conditionNode.DisconnectAllPorts();
                        RemoveElement(conditionNode);
                    }

                    // Xóa group
                    RemoveElement(groupToDelete);
                }


                foreach (DTSNode nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete != null)
                    {
                        RemoveElement(nodeToDelete);
                        nodeToDelete.DisconnectAllPorts();
                    }
                }
                foreach (DTSConditionNode conditionNodeToDelete in conditionNodesToDelete)
                {
                    if (conditionNodeToDelete != null)
                    {
                        RemoveElement(conditionNodeToDelete);
                        conditionNodeToDelete.DisconnectAllPorts();
                    }
                }
                DebugLogElementCounts();
                CheckForDuplicateNames();
            };
        }
        // Wrap AddElement để gọi CheckForDuplicateNames
        public void AddElementCustom(GraphElement element)
        {
            AddElement(element);
            DebugLogElementCounts();
            CheckForDuplicateNames();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }
                if (startPort.node is DTSConditionNode && startPort.direction == Direction.Output)
                {
                    // Chỉ cho phép kết nối với "Condition Port" của DTSNode
                    if (port.node is DTSNode && port.direction == Direction.Input && port.portName == "Condition Port")
                    {
                        compatiblePorts.Add(port);
                    }
                }
                else
                {
                    // Các trường hợp khác (ví dụ: DTSNode output -> DTSNode input)
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }
        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    // Lấy port xuất phát (ConditionNode output)
                    if (edge.output.node is DTSConditionNode conditionNode &&
                        edge.input.node is DTSNode targetNode)
                    {
                        conditionNode.SetParentNode(targetNode);
                        Debug.Log($"[Connect] ConditionNode {conditionNode.NodeID} parent set to Node {targetNode.NodeID}");
                    }
                }
            }

            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        if (edge.output.node is DTSConditionNode conditionNode &&
                            edge.input.node is DTSNode targetNode)
                        {
                            // Ngắt kết nối => clear parent
                            if (conditionNode.ParentNode == targetNode)
                            {
                                conditionNode.SetParentNode(null);
                                Debug.Log($"[Disconnect] ConditionNode {conditionNode.NodeID} parent cleared");
                            }
                        }
                    }
                }
            }

            return change;
        }
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DTSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DTSDialogueType.MultipleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Condition)", DTSDialogueType.Condition));
            // this.AddManipulator(CreateConditionNodeContextualMenu("Add Node (Condition)"));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DTSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                           menuEvent => menuEvent.menu.AppendAction(actionTitle,
                           actionEvent => AddElementCustom(CreateNode("Node Name", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                       );

            return contextualMenuManipulator;
        }
        // private IManipulator CreateConditionNodeContextualMenu(string actionTitle)
        // {
        //     ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
        //                    menuEvent => menuEvent.menu.AppendAction(actionTitle,
        //                    actionEvent => AddElementCustom(CreateConditionNode("ConditionName", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        //                );

        //     return contextualMenuManipulator;
        // }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                           menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElementCustom(CreateGroup("Dialogue Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                       );

            return contextualMenuManipulator;
        }

        public DTSGroup CreateGroup(string title, Vector2 position)
        {
            DTSGroup group = new DTSGroup(title, position, this);
            return group;
        }


        public DTSBaseNode CreateNode(string nodeName, DTSDialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"DATN2.Editor.DialogueSystem.DTS{dialogueType}Node");//để lấy type của node kết thừa từ dsnode
            DTSBaseNode node = (DTSBaseNode)Activator.CreateInstance(nodeType);
            node.Initialize(nodeName, this, position);
            node.Draw();
            // AddElement(node);

            return node;
        }
        public DTSConditionNode CreateConditionNode(string nodeName, Vector2 position, bool shouldDraw = true)
        {
            DTSConditionNode node = new DTSConditionNode();
            node.Initialize(nodeName, this, position);
            node.Draw();
            // AddElement(node);
            // CheckForDuplicateNames();
            return node;
        }
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }


        private void AddStyles()
        {
            this.AddStyleSheets(
                "DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss"
            );
        }


        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }
        private void AddMiniMap()
        {
            miniMap = new MiniMap()
            {
                anchored = true
            };

            miniMap.SetPosition(new Rect(15, 50, 200, 180));

            Add(miniMap);

            miniMap.visible = false;
        }
        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
        }
        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));
            CheckForDuplicateNames();

            // groups.Clear();
            // groupedNodes.Clear();
            // ungroupedNodes.Clear();

            // NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }
        public void CheckForDuplicateNames()
        {
            var nodes = graphElements.ToList().OfType<DTSBaseNode>().ToList();
            var nodeNameGroups = nodes.GroupBy(n => n.DialogueName).Where(g => g.Count() > 1).ToList();
            bool hasDuplicateNodeNames = nodeNameGroups.Any();

            var groups = graphElements.ToList().OfType<Group>().ToList();
            var groupNameGroups = groups.GroupBy(g => g.title).Where(g => g.Count() > 1).ToList();
            bool hasDuplicateGroupNames = groupNameGroups.Any();

            foreach (var node in nodes)
            {
                if (nodeNameGroups.Any(g => g.Key == node.DialogueName))
                {
                    node.SetErrorStyle(new Color(1f, 0f, 0f));
                }
                else
                {
                    node.ResetStyle();
                }
            }


            foreach (var group in groups)
            {
                if (groupNameGroups.Any(g => g.Key == group.title))
                {
                    group.style.backgroundColor = new Color(1f, 0f, 0f);
                }
                else
                {
                    group.style.backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
                }
            }

            if (hasDuplicateNodeNames || hasDuplicateGroupNames)
            {
                editorWindow.DisableSaving();
                editorWindow.ShowWarning("Duplicate names detected. Please rename Nodes/Groups before saving.");
            }
            else
            {
                editorWindow.EnableSaving();
                editorWindow.HideWarning();
            }
        }
    }
}
//mai viết checking xem số node có trong graph
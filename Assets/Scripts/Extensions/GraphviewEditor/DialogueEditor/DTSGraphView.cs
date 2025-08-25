using System;
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.DialogueEditor.Extension;
using DATN2.GraphviewEditor.DialogueSystem;
using DATN2.GraphviewEditor.DialogueSystem.Enum;
using DATN2.GraphviewEditor.Style.Components;
using DS.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace DATN2.GraphviewEditor.DialogueEditor
{
    public class DTSGraphView : GraphView
    {
        public DTSEditor editorWindow;
        public CheckConditionForNode validator;
        public GraphElementHandler elementHandler;
        public GraphConnectionTracker connectionTracker;
        private MiniMap miniMap;
        private SerializableDictionary<string, List<string>> GraphConnections = new();
        public DTSGraphView(DTSEditor dsEditor)
        {
            editorWindow = dsEditor;
            validator = new CheckConditionForNode();
            AddManipulators();
            AddGridBackground();
            AddMiniMap();
            AddMiniMapStyles();
            AddStyles();
            OnElementsDeleted();
            graphViewChanged += OnGraphViewChanged;
            // elementsAddedToGroup += OnElementsAddedToGroup;
            // elementsRemovedFromGroup += OnElementsRemovedFromGroup;

            elementHandler = new GraphElementHandler(this);
            connectionTracker = new GraphConnectionTracker();

            deleteSelection = (op, ask) => elementHandler.HandleDeleteSelection();
            elementsAddedToGroup += elementHandler.HandleElementAddedToGroup;
            elementsRemovedFromGroup += elementHandler.HandleElementRemovedFromGroup;
        }
        public void Dispose()//cứ để để test nhé
        {
            elementsAddedToGroup -= elementHandler.HandleElementAddedToGroup;
            elementsRemovedFromGroup -= elementHandler.HandleElementRemovedFromGroup;
            graphViewChanged = null; // nếu bạn có assign delegate
            Debug.Log("Dispose");
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Nếu click vào Edge thì không add option Delete
            if (evt.target is Edge)
            {
                evt.menu.ClearItems(); // Xóa hết menu
                return;
            }

            base.BuildContextualMenu(evt);
        }
        // public void DebugLogElementCounts()
        // {
        //     int groupCount = graphElements.OfType<DTSGroup>().Count();
        //     int nodeCount = graphElements.OfType<DTSNode>().Count();
        //     int conditionNodeCount = graphElements.OfType<DTSConditionNode>().Count();

        //     Debug.Log($"[Graph Stats] Groups: {groupCount} | Nodes: {nodeCount} | ConditionNodes: {conditionNodeCount}");
        // }
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DTSGroup);
                //có thể phải sửa chỗ này
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
                //DebugLogElementCounts();
                validator.CheckForDuplicateNames(this);
                validator.CheckConditionConnections(this);
                validator.CheckOrphanConditionNodes(this);
            };
        }

        // Wrap AddElement để gọi CheckForDuplicateNames
        public void AddElementCustom(GraphElement element)
        {
            AddElement(element);
            //DebugLogElementCounts();
            validator.CheckForDuplicateNames(this);
            // validator.CheckConditionConnections(this);
            // validator.CheckOrphanConditionNodes(this);
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
            bool needsCheck = false;

            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    // Giao cho connectionTracker quản lý
                    if (connectionTracker.HandleEdgeCreated(edge))
                    {
                        needsCheck = true;
                    }
                }
            }

            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    // Giao cho connectionTracker quản lý
                    if (connectionTracker.HandleElementRemoved(element))
                    {
                        needsCheck = true;
                    }
                }
            }

            if (needsCheck)
            {
                validator.CheckForDuplicateNames(this);
                validator.CheckConditionConnections(this);
                validator.CheckOrphanConditionNodes(this);
            }

            return change;
        }
        public void AddManipulators()
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

        public IManipulator CreateNodeContextualMenu(string actionTitle, DTSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                           menuEvent => menuEvent.menu.AppendAction(actionTitle,
                           actionEvent => AddElementCustom(GraphElementFactory.CreateNode(this, "Node Name", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                       );

            return contextualMenuManipulator;
        }

        public IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                           menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElementCustom(GraphElementFactory.CreateGroup(this, "Dialogue Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                       );

            return contextualMenuManipulator;
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
            validator.CheckForDuplicateNames(this);
            validator.CheckConditionConnections(this);
            validator.CheckOrphanConditionNodes(this);
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }
    }
}
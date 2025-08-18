using System;
using System.Collections.Generic;
using System.Linq;
using DATN2.Editor.DialogueEditor;
using DATN2.Editor.DialogueSystem.Enum;
using DATN2.Editor.Style;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DS.Utilities;
using UnityEditor.UIElements;
using DATN2.Editor.Data.SaveModal.SO;
namespace DATN2.Editor.DialogueSystem
{
    public class DTSConditionNode : DTSBaseNode
    {
        public DTSNode ParentNode { get; set; }
        public List<DTSConditionSO> ConditionData { get; set; }
        // public DSDialogueType DialogueType { get; set; }

        public DTSConditionType ConditionType { get; set; }
        private readonly List<VisualElement> conditionFieldContainers = new List<VisualElement>();
        private Label parentNodeLabel;

        public override void Initialize(string nodeName, DTSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);
            DialogueType = DTSDialogueType.Condition;
            ConditionType = DTSConditionType.Float;
            HasInputPort = false;
        }

        // public void SetParentNode(DTSNode parent)
        // {
        //     ParentNode = parent;
        //     if (parentNodeLabel != null)
        //     {
        //         parentNodeLabel.text = parent != null ? $"Parent: {parent.DialogueName}" : "Parent: None";
        //     }
        // }
        public void SetParentNode(DTSNode parent)
        {
            if (ParentNode != null)
            {
                ParentNode.ConditionChildren?.Remove(this);
            }

            ParentNode = parent;

            if (ParentNode != null)
            {
                if (ParentNode.ConditionChildren == null)
                    ParentNode.ConditionChildren = new List<DTSConditionNode>();

                if (!ParentNode.ConditionChildren.Contains(this))
                    ParentNode.ConditionChildren.Add(this);
            }

            if (parentNodeLabel != null)
            {
                parentNodeLabel.text = parent != null ? $"Parent: {parent.DialogueName}" : "Parent: None";
            }
        }
        public override void Draw()
        {
            base.Draw();
            VisualElement conditionWrapper = new VisualElement();
            conditionWrapper.AddToClassList("ds-condition-wrapper");
            /* CONDITION TYPE FIELD */
            // EnumField conditionTypeField = new EnumField("Condition Type", ConditionType);
            // conditionTypeField.RegisterValueChangedCallback(evt =>
            // {
            //     ConditionType = (DTSConditionType)evt.newValue;
            // });
            // mainContainer.Add(conditionTypeField);

            /* ADD CONDITION BUTTON */
            Button addConditionButton = DTSElementUtility.CreateButton("Add Condition", () =>
            {
                AddConditionField(null);
            });
            addConditionButton.AddToClassList("ds-node__button");
            mainContainer.Add(addConditionButton);

            /* PARENT NODE LABEL */
            parentNodeLabel = new Label("Parent: None");
            parentNodeLabel.AddToClassList("ds-node__parent-label");
            conditionWrapper.Add(parentNodeLabel);

            /* OUTPUT PORT */
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);

            /* INITIALIZE CONDITION FIELDS FROM EXISTING DATA */
            if (ConditionData != null)
            {
                foreach (var condition in ConditionData)
                {
                    AddConditionField(condition, conditionWrapper);
                }
            }
            mainContainer.Add(conditionWrapper);
            RefreshExpandedState();
            RefreshPorts();
        }

        private void AddConditionField(DTSConditionSO initialValue, VisualElement parentContainer = null)
        {
            // Đảm bảo ConditionData không null
            if (ConditionData == null)
            {
                ConditionData = new List<DTSConditionSO>();
            }
            // Tạo container cho ObjectField và nút xóa
            VisualElement conditionContainer = new VisualElement();
            conditionContainer.AddToClassList("condition-field-container"); // Thêm class để style nếu cần
            conditionContainer.style.flexDirection = FlexDirection.Row; // Đặt layout ngang

            // Tạo ObjectField cho ScriptableObject
            ObjectField conditionField = new ObjectField
            {
                objectType = typeof(DTSConditionSO),
                allowSceneObjects = false,
                value = initialValue
            };
            conditionField.style.flexGrow = 1f; // Chiếm không gian tối đa
            conditionField.RegisterValueChangedCallback(evt =>
             {
                 int index = conditionFieldContainers.IndexOf(conditionContainer);
                 if (index >= 0)
                 {
                     if (index < ConditionData.Count)
                     {
                         ConditionData[index] = evt.newValue as DTSConditionSO;
                     }
                     else
                     {
                         ConditionData.Add(evt.newValue as DTSConditionSO);
                     }
                 }
             });


            // Tạo nút xóa
            Button deleteConditionButton = DTSElementUtility.CreateButton("X", () =>
             {
                 int index = conditionFieldContainers.IndexOf(conditionContainer);
                 if (index >= 0)
                 {
                     if (ConditionData != null && index < ConditionData.Count)
                     {
                         ConditionData.RemoveAt(index);
                     }
                     conditionFieldContainers.RemoveAt(index);
                     mainContainer.Remove(conditionContainer);
                 }
             });
            deleteConditionButton.AddToClassList("ds-node__button");

            // Thêm ObjectField và nút xóa vào container
            conditionContainer.Add(conditionField);
            conditionContainer.Add(deleteConditionButton);

            // Thêm container vào mainContainer và danh sách theo dõi
            if (parentContainer != null)
            {
                parentContainer.Add(conditionContainer);   // ✅ thêm vào wrapper
            }
            else
            {
                mainContainer.Add(conditionContainer);     // fallback
            }
            conditionFieldContainers.Add(conditionContainer);

            // Cập nhật ConditionData nếu initialValue không null
            if (initialValue != null && !ConditionData.Contains(initialValue))
            {
                ConditionData.Add(initialValue);
            }
            else if (initialValue == null)
            {
                ConditionData.Add(null); // Đồng bộ với field mới
            }
        }
    }
}
// AddConditionField thêm null vào
//  ConditionData khi tạo field mới, nhưng không có cơ chế nhắc người dùng gán SO hợp lệ, có thể dẫn đến lỗi khi lưu.
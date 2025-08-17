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
namespace DATN2.Editor.DialogueSystem
{
    public class DTSNode : DTSBaseNode
    {
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public bool HaveConditions { get; set; }
        public List<DTSConditionNode> ConditionChildren { get; set; }

        public override void Initialize(string nodeName, DTSGraphView dsGraphView, Vector2 position)
        {


            base.Initialize(nodeName, dsGraphView, position);
            Choices = new List<string>();
            Text = "Dialogue text.";
            HaveConditions = false;
        }

        public override void Draw()
        {
            base.Draw();

            /* EXTENSION CONTAINER */
            Toggle haveConditionsToggle = new Toggle("Has Conditions?")
            {
                value = HaveConditions
            };
            haveConditionsToggle.RegisterValueChangedCallback(evt =>
            {
                HaveConditions = evt.newValue;
                ToggleConditionPort(HaveConditions);

            });


            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            Foldout textFoldout = DTSElementUtility.CreateFoldout("Dialogue Text");

            TextField textTextField = DTSElementUtility.CreateTextField(Text);
            textTextField.style.width = StyleKeyword.Auto; // Tự tính theo nội dung
            textTextField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Length > 75)
                {
                    textTextField.multiline = true;
                }
                else
                {
                    textTextField.multiline = false;
                }
            });
            // textTextField.multiline = true;
            // textTextField.style.whiteSpace = WhiteSpace.Normal;
            // textTextField.style.flexWrap = Wrap.NoWrap; // Bắt đầu không wrap
            textTextField.AddClasses(
               "ds-node__text-field",
               "ds-node__quote-text-field"
            );


            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            customDataContainer.Add(haveConditionsToggle);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

        public void ToggleConditionPort(bool enable)
        {
            if (enable)
            {
                Port conditionPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                conditionPort.name = "Condition Port"; // Đặt tên để dễ tìm khi xóa
                conditionPort.portName = "Condition Port";
                conditionPort.userData = "Condition Port";
                inputContainer.Add(conditionPort);
            }
            else
            {
                // Xóa cổng Condition nếu tồn tại
                var conditionPort = inputContainer.Children()
                    .FirstOrDefault(x => x.name == "Condition Port");
                if (conditionPort != null)
                {
                    inputContainer.Remove(conditionPort);
                }
            }

            RefreshExpandedState();
            RefreshPorts();
            graphView.CheckConditionConnections();
        }
    }
}

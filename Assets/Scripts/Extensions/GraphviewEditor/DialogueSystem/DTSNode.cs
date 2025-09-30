using System;
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.DialogueEditor;
using DATN2.GraphviewEditor.DialogueSystem.Enum;
using DATN2.GraphviewEditor.Style;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DS.Utilities;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Style.Components;
namespace DATN2.GraphviewEditor.DialogueSystem
{
    public class DTSNode : DTSBaseNode
    {
        public List<DTSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public List<string> ActiveKey { get; set; }
        public bool HaveConditions { get; set; }
        public bool HasTalked { get; set; }
        public List<DTSConditionNode> ConditionChildren { get; set; }

        public override void Initialize(string nodeName, DTSGraphView dsGraphView, Vector2 position)
        {


            base.Initialize(nodeName, dsGraphView, position);
            Choices = new List<DTSChoiceSaveData>();
            Text = "Dialogue text.";
            ActiveKey = new List<string>();
            HaveConditions = false;
            HasTalked = false;
        }

        public override void Draw()
        {
            base.Draw();

            /* EXTENSION CONTAINER */
            Toggle haveConditionsToggle = new Toggle("Has Conditions?")
            {
                value = HaveConditions
            };
            Toggle hasTalkedToggle = new Toggle("Has Talked?")
            {
                value = HasTalked
            };
            // haveConditionsToggle.RegisterValueChangedCallback(evt =>
            // {
            //     HaveConditions = evt.newValue;
            //     ToggleConditionPort(HaveConditions);

            // });
            haveConditionsToggle.RegisterCallback<MouseOverEvent>(evt =>
            {
                if (HaveConditions)
                {
                    var conditionPort = inputContainer.Children()
                        .FirstOrDefault(x => x.name == "Condition Port") as Port;
                    if (conditionPort != null && conditionPort.connected)
                    {
                        haveConditionsToggle.SetEnabled(false);
                        haveConditionsToggle.tooltip = "Cannot uncheck 'Has Conditions' while Condition Port is connected. Disconnect all conditions first.";
                    }
                    else
                    {
                        haveConditionsToggle.SetEnabled(true);
                        haveConditionsToggle.tooltip = "";
                    }
                }
                else
                {
                    haveConditionsToggle.SetEnabled(true);
                    haveConditionsToggle.tooltip = "";
                }
            });

            haveConditionsToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == false)
                {
                    var conditionPort = inputContainer.Children()
                        .FirstOrDefault(x => x.name == "Condition Port") as Port;
                    if (conditionPort != null && conditionPort.connected)
                    {
                        haveConditionsToggle.value = true; // Ngăn không cho bỏ tích
                        graphView.editorWindow.SetWarning("cannot_uncheck_has_conditions",
                            $"Cannot uncheck 'Has Conditions' for node '{DialogueName}'. Disconnect all conditions first.");
                        return;
                    }
                }

                HaveConditions = evt.newValue;
                ToggleConditionPort(HaveConditions);
                graphView.editorWindow.ClearWarning("cannot_uncheck_has_conditions");
            });
            hasTalkedToggle.RegisterValueChangedCallback(evt =>
            {
                HasTalked = evt.newValue;

                if (Group != null && Group is DTSGroup dtsGroup)
                {
                    dtsGroup.UpdateHasDialogueTalkedStatus();
                }
            });

            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            Foldout textFoldout = DTSElementUtility.CreateFoldout("Dialogue Text");

            TextField textTextField = DTSElementUtility.CreateTextField(Text);
            textTextField.style.width = StyleKeyword.Auto; // Tự tính theo nội dung

            textTextField.RegisterValueChangedCallback(evt =>
            {
                Text = evt.newValue;
                if (evt.newValue.Length > 75)
                {
                    textTextField.multiline = true;
                }
                else
                {
                    textTextField.multiline = false;
                }
            });
            textTextField.AddClasses(
               "ds-node__text-field",
               "ds-node__quote-text-field"
            );

            Foldout activeKeyFoldout = DTSElementUtility.CreateFoldout("Active Keys");

            // Render lại list ActiveKey
            if (ActiveKey == null)
                ActiveKey = new List<string>();

            for (int i = 0; i < ActiveKey.Count; i++)
            {
                int index = i;
                TextField keyField = new TextField($"Key {i + 1}")
                {
                    value = ActiveKey[i]
                };

                // keyField.style.flexGrow = 1;
                // keyField.style.minWidth = 150; // hoặc to hơn nếu muốn
                // keyField.style.maxWidth = 400; // tránh quá dài
                keyField.RegisterValueChangedCallback(evt =>
                {
                    ActiveKey[index] = evt.newValue;
                });

                Button removeButton = new Button(() =>
                {
                    ActiveKey.RemoveAt(index);
                    activeKeyFoldout.Remove(keyField.parent); // xóa cả row
                })
                { text = "X" };

                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.Add(keyField);
                row.Add(removeButton);

                activeKeyFoldout.Add(row);
            }

            Button addButton = new Button(() =>
            {
                ActiveKey.Add(string.Empty);
                // force refresh (simple cách: redraw lại node hoặc add row mới)
                TextField newKeyField = new TextField($"Key {ActiveKey.Count}")
                {
                    value = ""
                };
                int newIndex = ActiveKey.Count - 1;
                newKeyField.RegisterValueChangedCallback(evt =>
                {
                    ActiveKey[newIndex] = evt.newValue;
                });

                Button removeButton = new Button(() =>
                {
                    ActiveKey.RemoveAt(newIndex);
                    activeKeyFoldout.Remove(newKeyField.parent);
                })
                { text = "X" };

                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.Add(newKeyField);
                row.Add(removeButton);
                activeKeyFoldout.Add(row);

            })
            { text = "+ Add Key" };

            activeKeyFoldout.Add(addButton);
            customDataContainer.Add(activeKeyFoldout);


            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            customDataContainer.Add(haveConditionsToggle);
            customDataContainer.Add(hasTalkedToggle);
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
            graphView.validator.CheckConditionConnections(graphView);
        }
        public bool IsStartingNode()
        {
            Port inputPort = (Port)inputContainer.Children().Where(p => p.name != "Condition Port").First();

            return !inputPort.connected;
        }
    }
}

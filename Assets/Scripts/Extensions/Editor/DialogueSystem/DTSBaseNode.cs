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
    public class DTSBaseNode : Node
    {
        public string NodeID { get; set; }
        public string DialogueName { get; set; }
        public DTSDialogueType DialogueType { get; set; }
        public DTSGroup Group { get; set; }
        protected DTSGraphView graphView;
        public Color defaultBackgroundColor;
        public bool HasInputPort { get; set; }

        public virtual void Initialize(string nodeName, DTSGraphView dsGraphView, Vector2 position)
        {
            NodeID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            SetPosition(new Rect(position, Vector2.zero));
            graphView = dsGraphView;
            HasInputPort = true;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (HasInputPort)
            {
                evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            }
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }
        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DTSElementUtility.CreateTextField(DialogueName);
            dialogueNameTextField.RegisterValueChangedCallback(evt =>
                     {
                         DialogueName = evt.newValue;
                         graphView.validator.CheckForDuplicateNames(graphView); // Kiểm tra trùng tên khi thay đổi
                     });
            titleContainer.Insert(0, dialogueNameTextField);
            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );
            //do dialougeNameTextField là 1 array nên phải insert 0 là a[0]
            if (HasInputPort)
            {
                /* INPUT CONTAINER */
                Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                inputPort.portName = "Dialogue Connection";
                inputContainer.Add(inputPort);

            }
            RefreshExpandedState();
        }

        public virtual void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public virtual void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        public virtual void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }


        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }
                graphView.DeleteElements(port.connections);
            }
        }
    }
}

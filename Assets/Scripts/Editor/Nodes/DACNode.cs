using System;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.GraphviewEditor.Nodes
{
    public class DACNode : Node
    {
        // public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DACDialogueType DialogueType { get; set; }
        public virtual void Initialize(Vector2 position)
        {
            // ID = Guid.NewGuid().ToString();
            DialogueName = "DialogueName";
            Choices = new List<string>();
            Text = "Dialogue text.";
            DialogueType = DACDialogueType.SingleChoice;

            SetPosition(new Rect(position, Vector2.zero));
            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
            // mainContainer.style.backgroundColor = new StyleColor(Color.red);
            // extensionContainer.style.backgroundColor = new StyleColor(Color.green);
            Debug.Log(mainContainer.ClassListContains("ds-node__main-container"));
        }
        public virtual void Draw()
        {
            /*TitleContainer*/
            TextField dialogueNameTextField = new TextField()
            {
                value = DialogueName,
                // multiline = true
            };
            dialogueNameTextField.AddToClassList("ds-node__text-field");
            dialogueNameTextField.AddToClassList("ds-node__text-field__hidden");
            dialogueNameTextField.AddToClassList("ds-node__filename-text-field");
            titleContainer.Insert(0, dialogueNameTextField);
            //input container
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            // xây dựng đường nối capacity là port này có thể nhận bao nhiêu input
            //The type parameter is used to restrict entering ports for certain types, let's say your port is of bool type, only connections from other bool ports can be made.
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);

            //Extension
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            Foldout textFoldout = new Foldout()//collapse able
            {
                text = "Dialogue Text"
            };
            TextField textTextField = new TextField()
            {
                value = Text
            };
            textTextField.AddToClassList("ds-node__text-field");
            textTextField.AddToClassList("ds-node__quote-text-field");

            textFoldout.Add(textTextField);
            // inputContainer.Add(textFoldout);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);

            //RefeshExpandedState call refeshport itself so we don't need to call it ourselve
            RefreshExpandedState();
        }
    }
}
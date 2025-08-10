using DATN2.Assets.Scripts.Modals.Enum;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace DATN2.Editor.Nodes
{
    public class MultipleChoiceNode : DACNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
            DialogueType = DACDialogueType.MultipleChoice;
            Choices.Add("New Choice");
        }
        public override void Draw()
        {
            base.Draw();
            Button addChoiceButton = new Button()
            {
                text = "+"
            };
            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                choicePort.portName = "";
                Button deleteChoiceButton = new Button()
                {
                    text = "X"
                };
                TextField choiceTextField = new TextField()
                {
                    value = choice
                };
                choiceTextField.AddToClassList("ds-node__textfield");
                choicePort.Add(deleteChoiceButton);
                choicePort.Add(choiceTextField);
                inputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }
    }
}
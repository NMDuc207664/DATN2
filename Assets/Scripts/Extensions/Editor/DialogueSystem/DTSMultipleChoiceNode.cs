using DATN2.Editor.DialogueEditor;
using DATN2.Editor.DialogueSystem.Enum;
using DATN2.Editor.Style;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace DATN2.Editor.DialogueSystem
{
    public class DTSMultipleChoiceNode : DTSNode
    {
        public override void Initialize(string nodeName, DTSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DTSDialogueType.MultipleChoice;

            // DSChoiceSaveData choiceData = new DSChoiceSaveData()
            // {
            //     Text = "New Choice"
            // };

            // Choices.Add(choiceData);
            Choices.Add("New Choice");
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */
            Button addChoiceButton = DTSElementUtility.CreateButton("Add Choice", () =>
            {
                string newChoice = "New Choice";
                Choices.Add(newChoice);

                Port choicePort = CreateChoicePort(newChoice);
                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */
            foreach (string choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(string choiceText)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

            choicePort.portName = choiceText;
            choicePort.userData = choiceText;

            Button deleteChoiceButton = DTSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                    return;

                if (choicePort.connected)
                    graphView.DeleteElements(choicePort.connections);

                Choices.Remove(choiceText);
                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = DTSElementUtility.CreateTextField(choiceText, null, callback =>
            {
                int index = Choices.IndexOf(choiceText);
                if (index >= 0)
                    Choices[index] = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }
    }
}
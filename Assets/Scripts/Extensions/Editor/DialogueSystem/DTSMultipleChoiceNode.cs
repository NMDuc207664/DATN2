using System;
using DATN2.Editor.Data.SaveModal;
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

            DTSChoiceSaveData choiceData = new DTSChoiceSaveData()
            {
                ChoiceID = Guid.NewGuid().ToString(),
                Text = "New Choice"
            };

            Choices.Add(choiceData);
            // Choices.Add("New Choice");
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */
            Button addChoiceButton = DTSElementUtility.CreateButton("Add Choice", () =>
            {
                DTSChoiceSaveData choiceData = new DTSChoiceSaveData()
                {
                    ChoiceID = Guid.NewGuid().ToString(),
                    Text = "New Choice"
                };
                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);
                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */
            foreach (var choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

            // choicePort.portName = choice.Text;
            choicePort.userData = userData;
            DTSChoiceSaveData choiceData = (DTSChoiceSaveData)userData;
            int index = Choices.IndexOf(choiceData);
            choicePort.portName = $"Choice {index}";

            Button deleteChoiceButton = DTSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                    return;

                if (choicePort.connected)
                    graphView.DeleteElements(choicePort.connections);

                Choices.Remove(choiceData);
                graphView.RemoveElement(choicePort);

                for (int i = 0; i < Choices.Count; i++)
                {
                    if (outputContainer[i] is Port port)
                        port.portName = $"Choice {i}";
                }
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = DTSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                // int index = Choices.IndexOf(choiceText);
                // if (index >= 0)
                //     Choices[index] = callback.newValue;
                choiceData.Text = callback.newValue;

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
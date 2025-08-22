using System;
using DATN2.Editor.Data.SaveModal;
using DATN2.Editor.DialogueEditor;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DATN2.Editor.DialogueSystem
{
    public class DTSSingleChoiceNode : DTSNode
    {
        public override void Initialize(string nodeName, DTSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DTSDialogueType.SingleChoice;
            DTSChoiceSaveData choiceData = new DTSChoiceSaveData()
            {
                ChoiceID = Guid.NewGuid().ToString(),
                NodeID = this.NodeID,
                Text = "New Choice"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            // foreach (DSChoiceSaveData choice in Choices)
            // {
            //     Port choicePort = this.CreatePort(choice.Text);

            //     choicePort.userData = choice;

            //     outputContainer.Add(choicePort);
            // }
            foreach (var choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                choicePort.userData = choice;
                choicePort.portName = choice.Text;
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }
    }
}
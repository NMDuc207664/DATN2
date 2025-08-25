using DATN2.Assets.Scripts.Modals.Enum;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DATN2.GraphviewEditor.Nodes
{
    public class SingleChoiceNode : DACNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
            DialogueType = DACDialogueType.SingleChoice;
            Choices.Add("Next Dialogue");
        }

        public override void Draw()
        {
            base.Draw();
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                choicePort.portName = choice;
                inputContainer.Add(choicePort);

            }
            RefreshExpandedState();
        }
    }
}
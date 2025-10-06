// using System;
// using System.Collections.Generic;
// using DATN2.Assets.Scripts.Data;
// using DATN2.Assets.Scripts.Logics.Controllers;
// using DATN2.Assets.Scripts.Logics.Interface.NPC;
// using DATN2.GraphviewEditor.Applications;
// using UnityEngine;

// public class QDialogueService : IQdialogueService
// {
//     DTSReadSO reader = new DTSReadSO();
//     List<string> connectedNodes = new List<string>();
//     public void NextDialogueAsync(QuestDataSO questDataSO, Action onComplete = null)
//     {
//         foreach (var quest in questDataSO.quests)
//         {
//             if (quest.dialogues == null || quest.dialogues.dialogueGraph == null)
//             {
//                 Debug.Log("[Trigger] DialogueGraph is NULL, skip.");
//                 continue;
//             }



//             var nextDialogue = reader.GetNodeByID(quest.dialogues.dialogueGraph, DialogueManager.Instance.nextNodeID[0]);
//             connectedNodes.Clear();
//             DialogueManager.Instance.UpdateDialogue(nextDialogue.Text);
//             // Lấy ConnectedNodeIDs từ Choices

//             if (nextDialogue.Choices != null && nextDialogue.Choices.Count > 0)
//             {
//                 foreach (var choice in nextDialogue.Choices)
//                 {
//                     if (choice.ConnectedNodeIDs != null && choice.ConnectedNodeIDs.Count > 0)
//                     {
//                         connectedNodes.AddRange(choice.ConnectedNodeIDs);
//                     }
//                 }
//             }

//             // Update nextNodeID mới
//             DialogueManager.Instance.UpdateNextNodeID(connectedNodes);
//         }

//         onComplete?.Invoke();
//     }

//     public void StartDialogueAsync(QuestDataSO questDataSO, Action onComplete = null)
//     {
//         questDataSO = KeyGameStateManager.Instance.GetQuestData(questDataSO.Key);
//         foreach (var quest in questDataSO.quests)
//         {
//             if (quest.dialogues == null || quest.dialogues.dialogueGraph == null)
//             {
//                 Debug.Log("[Trigger] DialogueGraph is NULL, skip.");
//                 continue;
//             }

//             // Khởi tạo read runtime từ GraphSaveDataSO

//             //reader.GetInformationFromGraph(quest.dialogues.dialogueGraph);

//             var startingDialogue = reader.GetInformFromNode(quest.dialogues.dialogue);

//             DialogueManager.Instance.UpdateDialogue(startingDialogue.Dialogue);
//             DialogueManager.Instance.UpdateNextNodeID(startingDialogue.NextNodeId);
//             // reader.GetNodeByID(quest.dialogues.dialogueGraph, "b76a53bc-3120-440b-93b0-b17fa1f85e54");

//         }
//         onComplete?.Invoke();
//     }
// }
using System;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.GraphviewEditor.Applications;
using UnityEngine;

public class QDialogueService : IQdialogueService
{
    DTSReadSO reader = new DTSReadSO();
    List<string> connectedNodes = new List<string>();

    public void NextDialogueAsync(QuestDataSO questDataSO, string nodeId, Action onComplete = null)
    {
        foreach (var quest in questDataSO.quests)
        {
            if (quest.dialogues == null || quest.dialogues.dialogueGraph == null)
            {
                Debug.Log("[Trigger] DialogueGraph is NULL, skip.");
                continue;
            }

            var nextDialogue = reader.GetNodeByID(quest.dialogues.dialogueGraph, nodeId);
            connectedNodes.Clear();
            DialogueManager.Instance.UpdateDialogue(nextDialogue.Text);

            // Lấy ConnectedNodeIDs từ Choices
            if (nextDialogue.Choices != null && nextDialogue.Choices.Count > 0)
            {
                foreach (var choice in nextDialogue.Choices)
                {
                    if (choice.ConnectedNodeIDs != null && choice.ConnectedNodeIDs.Count > 0)
                    {
                        connectedNodes.AddRange(choice.ConnectedNodeIDs);
                    }
                }
            }

            // Update nextNodeID mới (giả sử single branch, nếu multi thì cần UI chọn)
            DialogueManager.Instance.UpdateNextNodeID(connectedNodes.Count > 0 ? connectedNodes : null);
        }

        onComplete?.Invoke();
    }

    public void StartDialogueAsync(QuestDataSO questDataSO, Action onComplete = null)
    {
        questDataSO = KeyGameStateManager.Instance.GetQuestData(questDataSO.Key);
        foreach (var quest in questDataSO.quests)
        {
            if (quest.dialogues == null || quest.dialogues.dialogueGraph == null)
            {
                Debug.Log("[Trigger] DialogueGraph is NULL, skip.");
                continue;
            }

            var startingDialogue = reader.GetInformFromNode(quest.dialogues.dialogue);
            DialogueManager.Instance.StartDialogue(startingDialogue.Dialogue, startingDialogue.NextNodeId, questDataSO, onComplete);
        }
    }
}
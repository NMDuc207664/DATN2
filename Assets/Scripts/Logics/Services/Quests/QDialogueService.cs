
// using System;
// using System.Collections.Generic;
// using DATN2.Assets.Scripts.Data;
// using DATN2.Assets.Scripts.Logics.Controllers;
// using DATN2.Assets.Scripts.Logics.Interface.NPC;
// using DATN2.GraphviewEditor.Applications;
// using UnityEngine;

// public class QDialogueService : IQdialogueService
// {
//     public event Action OnDialogueComplete;
//     DTSReadSO reader = new DTSReadSO();
//     List<string> connectedNodes = new List<string>();

//     public void NextDialogueAsync(QuestDataSO questDataSO, string nodeId, Action onComplete = null)
//     {
//         foreach (var quest in questDataSO.quests)
//         {
//             if (quest.dialogues == null || quest.dialogues.dialogueGraph == null)
//             {
//                 Debug.Log("[Trigger] DialogueGraph is NULL, skip.");
//                 continue;
//             }

//             var nextDialogue = reader.GetNodeByID(quest.dialogues.dialogueGraph, nodeId);
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

//             // Update nextNodeID mới (giả sử single branch, nếu multi thì cần UI chọn)
//             DialogueManager.Instance.UpdateNextNodeID(connectedNodes.Count > 0 ? connectedNodes : null);
//         }

//         onComplete?.Invoke();
//     }

//     public void StartDialogueAsync(QuestDataSO questDataSO, Action onComplete = null, int questIndex = 0)
//     {
//         questDataSO = KeyGameStateManager.Instance.GetQuestData(questDataSO.Key);
//         if (questDataSO.quests.Count <= questIndex)
//         {
//             Debug.LogError($"[QDialogueService] Quest index {questIndex} out of range!");
//             return;
//         }
//         var quest = questDataSO.quests[questIndex];

//         var startingDialogue = reader.GetInformFromNode(quest.dialogues.dialogue);
//         DialogueManager.Instance.StartDialogue(startingDialogue.Dialogue, startingDialogue.NextNodeId, questDataSO, () =>
//         {
//             onComplete?.Invoke();
//             OnDialogueComplete?.Invoke();
//         });
//     }

//     public void StartDefaultDialogue(QuestDataSO questDataSO, int questIndex)
//     {
//         // Validate questDataSO
//         if (questDataSO == null)
//         {
//             Debug.LogError("[QDialogueService] QuestDataSO is null!");
//             return;
//         }

//         questDataSO = KeyGameStateManager.Instance.GetQuestData(questDataSO.Key);

//         // Validate quest index
//         if (questIndex < 0 || questIndex >= questDataSO.quests.Count)
//         {
//             Debug.LogError($"[QDialogueService] Invalid quest index {questIndex}. QuestData has {questDataSO.quests.Count} quests");
//             return;
//         }

//         var quest = questDataSO.quests[questIndex];

//         // Validate default dialogue
//         if (!quest.HasDefaultDialogue)
//         {
//             Debug.LogWarning($"[QDialogueService] Quest[{questIndex}] doesn't have default dialogue enabled");
//             return;
//         }

//         if (quest.defaultDialogue == null || quest.defaultDialogue.dialogue == null)
//         {
//             Debug.LogError($"[QDialogueService] Quest[{questIndex}] default dialogue is null!");
//             return;
//         }

//         Debug.Log($"[QDialogueService] Starting default dialogue from quest[{questIndex}]");

//         // Start default dialogue (không có callback vì nó có thể lặp lại)
//         var startingDialogue = reader.GetInformFromNode(quest.defaultDialogue.dialogue);

//         // Chạy dialogue với callback rỗng hoặc null để không trigger quest complete
//         DialogueManager.Instance.StartDialogue(
//             startingDialogue.Dialogue,
//             startingDialogue.NextNodeId,
//             questDataSO,
//             null  // Không có callback -> dialogue kết thúc tự nhiên, có thể trigger lại
//         );
//     }

// }

using System;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.Assets.Scripts.Modals.Enum;
using DATN2.GraphviewEditor.Applications;
using UnityEngine;

public class QDialogueService : IQdialogueService
{
    public event Action OnDialogueComplete;
    DTSReadSO reader = new DTSReadSO();
    List<string> connectedNodes = new List<string>();

    public void NextDialogueAsync(QuestDataSO questDataSO, string nodeId, Action onComplete = null)
    {
        foreach (var quest in questDataSO.quests)
        {
            if (quest.dialogues == null || quest.dialogues.dialogueGraph == null)
            {
                Debug.Log("[QDialogueService] DialogueGraph is NULL, skip.");
                continue;
            }

            var nextDialogue = reader.GetNodeByID(quest.dialogues.dialogueGraph, nodeId);
            connectedNodes.Clear();
            DialogueManager.Instance.UpdateDialogue(nextDialogue.Text);

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

            DialogueManager.Instance.UpdateNextNodeID(connectedNodes.Count > 0 ? connectedNodes : null);
        }

        onComplete?.Invoke();
    }

    public void StartDialogueAsync(QuestDataSO questDataSO, Action onComplete = null, int questIndex = 0)
    {
        questDataSO = KeyGameStateManager.Instance.GetQuestData(questDataSO.Key);

        if (questDataSO.quests.Count <= questIndex)
        {
            Debug.LogError($"[QDialogueService] Quest index {questIndex} out of range!");
            return;
        }

        var quest = questDataSO.quests[questIndex];
        var startingDialogue = reader.GetInformFromNode(quest.dialogues.dialogue);

        DialogueManager.Instance.StartDialogue(startingDialogue.Dialogue, startingDialogue.NextNodeId, questDataSO, () =>
        {
            // Gọi callback trước
            onComplete?.Invoke();

            // Sau đó invoke event
            OnDialogueComplete?.Invoke();

            Debug.Log($"[QDialogueService] Dialogue completed for quest[{questIndex}]");
        });
    }

    public void StartDefaultDialogue(QuestDataSO questDataSO, int questIndex)
    {
        if (questDataSO == null)
        {
            Debug.LogError("[QDialogueService] QuestDataSO is null!");
            return;
        }

        questDataSO = KeyGameStateManager.Instance.GetQuestData(questDataSO.Key);

        if (questIndex < 0 || questIndex >= questDataSO.quests.Count)
        {
            Debug.LogError($"[QDialogueService] Invalid quest index {questIndex}. QuestData has {questDataSO.quests.Count} quests");
            return;
        }

        var quest = questDataSO.quests[questIndex];

        if (!quest.HasDefaultDialogue)
        {
            Debug.LogWarning($"[QDialogueService] Quest[{questIndex}] doesn't have default dialogue enabled");
            return;
        }

        if (quest.defaultDialogue == null || quest.defaultDialogue.dialogue == null)
        {
            Debug.LogError($"[QDialogueService] Quest[{questIndex}] default dialogue is null!");
            return;
        }

        Debug.Log($"[QDialogueService] Starting default dialogue from quest[{questIndex}]");

        var startingDialogue = reader.GetInformFromNode(quest.defaultDialogue.dialogue);

        // Default dialogue không trigger OnDialogueComplete event
        DialogueManager.Instance.StartDialogue(
            startingDialogue.Dialogue,
            startingDialogue.NextNodeId,
            questDataSO,
            () =>
            {
                OnDialogueComplete?.Invoke();
                KeyGameStateManager.Instance.AddOrChangeGameState(InGameActionType.None);
            }
        );
    }
}
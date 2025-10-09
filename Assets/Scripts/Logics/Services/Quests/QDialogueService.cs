
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
        DialogueManager.Instance.StartDialogue(startingDialogue.Dialogue, startingDialogue.NextNodeId, questDataSO, onComplete);
    }

}
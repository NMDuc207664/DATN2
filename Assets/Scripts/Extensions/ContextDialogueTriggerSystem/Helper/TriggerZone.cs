using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public List<ContextSO> availableContexts;
    [SerializeField] private string listenEventName; // Tên event mà TriggerZone này lắng nghe (ví dụ "TalkedToNPC_A")
    [SerializeField] private string triggerEventNameOnEnd; // Tên event để trigger khi dialogue end (ví dụ cho NPC A: "TalkedToNPC_A")
    [SerializeField] private List<ContextSO> contextsToSendOnEnd;
    private void Start()
    {
        if (DialogueEventManager.Instance != null)
        {
            DialogueEventManager.Instance.OnDialogueEnded += HandleDialogueEnded;
        }
    }

    private void OnDestroy()
    {
        if (DialogueEventManager.Instance != null)
        {
            DialogueEventManager.Instance.OnDialogueEnded -= HandleDialogueEnded;
        }
    }

    private void HandleDialogueEnded(string eventName, object data)
    {
        if (eventName != listenEventName) return; // Chỉ xử lý nếu event khớp

        // Xử lý data: Giả sử data là List<ContextSO> mới
        if (data is List<ContextSO> newContexts)
        {
            UpdateContexts(newContexts);
            Debug.Log($"TriggerZone {gameObject.name} updated contexts!");
        }
        // Hoặc nếu data là string flag, set flag và reload contexts dựa trên logic
        else if (data is string flagName)
        {
            PlayerData.Instance.SetFlag(flagName, true);
            // Reload contexts dựa trên flag nếu cần (ví dụ filter contexts)
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialogue();
            QuestManager.questManager.AddQuestItems("Leave Town", 1);
        }
    }

    private void TriggerDialogue()
    {
        // Tìm context phù hợp (check condition của context)
        ContextSO selectedContext = null;
        foreach (var context in availableContexts)
        {
            if (context.condition == null || context.condition.Evaluate())
            {
                selectedContext = context;
                break; // Chọn cái đầu tiên phù hợp
            }
        }

        if (selectedContext == null)
        {
            Debug.Log("No suitable context found.");
            return;
        }

        // Từ context, chọn dialogue (tạm thời chọn cái đầu tiên, vì không check condition trong dialogue)
        if (selectedContext.possibleDialogues == null || selectedContext.possibleDialogues.Count == 0)
        {
            Debug.Log("No dialogues in selected context.");
            return;
        }

        DTSDialogueSO selectedDialogue = selectedContext.possibleDialogues[0]; // Chọn cái đầu tiên

        // Debug ra Text (hoặc integrate với UI)
        Debug.Log(selectedDialogue.Text);
        //EndDialogue("TalkedToNPC_A", new List<ContextSO> { /* contexts mới */ }); // Ví dụ trigger event
        // Tự động end dialogue và trigger event (không cần choice)
        if (!string.IsNullOrEmpty(triggerEventNameOnEnd) && contextsToSendOnEnd != null)
        {
            DialogueEventManager.Instance.TriggerDialogueEnded(triggerEventNameOnEnd, contextsToSendOnEnd);
        }
    }

    // Method để thay đổi contexts dynamically
    public void UpdateContexts(List<ContextSO> newContexts)
    {
        availableContexts = newContexts;
    }
    // Method để trigger event khi dialogue end (gọi từ UI hoặc logic dialogue của bạn)
    public void EndDialogue(string eventName, object data)
    {
        DialogueEventManager.Instance.TriggerDialogueEnded(eventName, data);
    }
}
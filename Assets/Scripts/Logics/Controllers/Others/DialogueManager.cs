using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;
using DATN2.Assets.Scripts.Data;
using System;
using DATN2.Assets.Scripts.Logics.Interface.NPC; // Add this for QuestDataSO

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; set; }
    public List<string> nextNodeID = new List<string>();
    public string dialogue;
    public TextMeshProUGUI dialogueText;
    private bool isDialogueActive = false;
    private QuestDataSO currentQuestData; // Lưu để reuse khi next
    private Action onDialogueComplete; // Callback khi hết

    [Inject]
    private readonly IQdialogueService _questDialogueService;

    void Awake()
    {
        if (_questDialogueService == null)
        {
            Debug.LogError("[DialogueManager] QDialogueService is null.");
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            ProceedNextDialogue();
        }
    }

    public void StartDialogue(string initialText, List<string> initialNextIds, QuestDataSO questData, Action onComplete)
    {
        currentQuestData = questData;
        onDialogueComplete = onComplete;
        UpdateDialogue(initialText);
        UpdateNextNodeID(initialNextIds);
        isDialogueActive = true;
        dialogueText.gameObject.SetActive(true); // Show UI
        dialogueText.text = dialogue; // Display text
    }

    public void UpdateNextNodeID(List<string> nodeIDs)
    {
        nextNodeID.Clear();
        if (nodeIDs != null)
        {
            nextNodeID.AddRange(nodeIDs);
            Debug.Log($"[DialogueManager] NextNodeIDs set: {string.Join(", ", nextNodeID)}");
        }
    }

    public void UpdateDialogue(string text)
    {
        dialogue = text;
        dialogueText.text = text; // Update UI text
        Debug.Log($"[DialogueManager] Dialogue set: {text}");
    }

    private void ProceedNextDialogue()
    {
        if (nextNodeID.Count == 0)
        {
            // Hết dialogue -> đóng
            isDialogueActive = false;
            dialogueText.gameObject.SetActive(false); // Hide UI
            dialogue = null;
            onDialogueComplete?.Invoke(); // Call PassKey hoặc callback
            currentQuestData = null;
            onDialogueComplete = null;
            return;
        }

        // Lấy nextId (giả sử single, nếu multi cần chọn)
        string nextId = nextNodeID[0];
        nextNodeID.RemoveAt(0);

        // Gọi next với currentQuestData và nextId
        _questDialogueService.NextDialogueAsync(currentQuestData, nextId, () =>
        {
            Debug.Log("[DialogueManager] Next dialogue loaded.");
            dialogueText.text = dialogue; // Ensure update UI
        });
    }
}
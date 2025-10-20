using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;
using DATN2.Assets.Scripts.Data;
using System;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.Assets.Scripts.Logics.Controllers; // Add this for QuestDataSO

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; set; }
    public List<string> nextNodeID = new List<string>();
    public string dialogue;
    public TextMeshProUGUI dialogueText;
    public bool isDialogueActive = false;
    private QuestDataSO currentQuestData; // Lưu để reuse khi next
    private Action onDialogueComplete; // Callback khi hết
    private Coroutine autoAdvanceCoroutine; // To manage auto-advance
    private Coroutine typingCoroutine;
    public bool isTyping { get; private set; } = false;
    [Header("Dialogue Settings")]
    public float delayBetweenSentences = 1f;
    public float typingSpeed = 0.05f; // thời gian giữa mỗi ký tự
    public bool skipTypingOnKey = true; // bấm E để skip typing

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
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ProceedNextDialogue();
            }
        }

        // if (isDialogueActive)
        // {
        //     KeyGameStateManager.Instance.SetLockMouseInput(true);
        // }
        // else if (!isDialogueActive)
        // {
        //     KeyGameStateManager.Instance.SetLockMouseInput(false);
        // }
    }

    public void StartDialogue(string initialText, List<string> initialNextIds, QuestDataSO questData, Action onComplete)
    {
        currentQuestData = questData;
        onDialogueComplete = onComplete;

        isDialogueActive = true;
        dialogueText.gameObject.SetActive(true); // Show UI
        UpdateDialogue(initialText);
        UpdateNextNodeID(initialNextIds);

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

        // Dừng tất cả coroutines cũ
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        if (autoAdvanceCoroutine != null)
            StopCoroutine(autoAdvanceCoroutine);

        // Bắt đầu hiệu ứng gõ mới
        typingCoroutine = StartCoroutine(TypeSentence(dialogue));
    }
    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        dialogueText.text = dialogue; // Show full text immediately

        // Bắt đầu đếm auto-advance sau khi skip
        StartAutoAdvance();
    }

    private void ProceedNextDialogue()
    {
        // Dừng auto-advance nếu đang chạy
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (nextNodeID.Count == 0)
        {
            // Hết dialogue -> đóng
            EndDialogue();
            return;
        }

        // Lấy nextId
        string nextId = nextNodeID[0];
        nextNodeID.RemoveAt(0);

        // Load dialogue tiếp theo
        _questDialogueService.NextDialogueAsync(currentQuestData, nextId, () =>
        {
            Debug.Log("[DialogueManager] Next dialogue loaded.");
            // Không gọi AutoAdvance ở đây, để TypeSentence tự gọi khi xong
        });
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;
        dialogueText.gameObject.SetActive(false);
        dialogue = null;

        // Dừng tất cả coroutines
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        onDialogueComplete?.Invoke();
        currentQuestData = null;
        onDialogueComplete = null;
    }

    private void StartAutoAdvance()
    {
        if (autoAdvanceCoroutine != null)
            StopCoroutine(autoAdvanceCoroutine);

        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceDialogue());
    }

    private IEnumerator AutoAdvanceDialogue()
    {
        yield return new WaitForSeconds(delayBetweenSentences);

        if (isDialogueActive && !isTyping) // Chỉ advance nếu không đang typing
        {
            ProceedNextDialogue();
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Typing xong
        isTyping = false;
        typingCoroutine = null;

        // Bắt đầu đếm auto-advance
        StartAutoAdvance();
    }
}
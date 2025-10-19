using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using VContainer;

namespace DATN2.GraphviewEditor.Runtime
{
    public class DTSDialogueTriggerZone : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField] private string keyToTrigger = "Map_1_OpeningS_Q1";
        [SerializeField] private string mapKey = "Map1";
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] public string interactPrompt = "E: Interact";

        private Dictionary<string, IQuestService> _questControllers;
        private IQdialogueService _dialogueService;
        private Transform _player;
        private bool _inRange;
        public bool autoActivate = false;
        private bool _hasActivated = false;
        private DefaultDialogue _defaultDialogue;
        private BoxCollider _col;
        private bool _isDialogueRunning = false;

        void Awake()
        {
            _col = GetComponent<BoxCollider>();
            _defaultDialogue = GetComponent<DefaultDialogue>();
        }

        public void SetQuestControllers(Dictionary<string, IQuestService> questControllers)
        {
            _questControllers = questControllers;
            Debug.Log($"[DTSTriggerZone:{gameObject.name}] QuestControllers injected successfully");
        }

        public void SetDialogueService(IQdialogueService dialogueService)
        {
            _dialogueService = dialogueService;
            //_dialogueService.OnDialogueComplete += OnDialogueComplete;
            // Debug.Log($"[DTSTriggerZone:{gameObject.name}] DialogueService injected successfully");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (autoActivate && !_hasActivated)
            {
                TryActivate();
                _hasActivated = true;
            }

        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (!autoActivate && Input.GetKeyDown(KeyCode.E))
            {
                TryActivate();
                _hasActivated = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _hasActivated = false;

        }

        private void TryActivate()
        {
            // Kiểm tra nếu có DefaultDialogue đã được setup
            if (_defaultDialogue != null && _defaultDialogue._questData != null)
            {
                Debug.Log($"[DTSTriggerZone:{gameObject.name}] Triggering default dialogue");
                TriggerDefaultDialogue();
                return;
            }

            // Nếu không có default dialogue, chạy quest bình thường
            if (_questControllers == null)
            {
                Debug.LogError("_questControllers is null! DTSTriggerZone was not injected properly.");
                return;
            }

            if (!KeyGameStateManager.Instance.currentKeysActive.Contains(keyToTrigger))
            {
                KeyGameStateManager.Instance.ActivateKey(keyToTrigger);

                if (_questControllers.TryGetValue(mapKey, out var quest))
                {
                    quest.ActivateQuest(keyToTrigger);
                }
                else
                {
                    Debug.LogWarning($"No quest controller found for mapKey '{mapKey}' on trigger '{gameObject.name}'");
                }
            }
        }

        private void TriggerDefaultDialogue()
        {
            if (_dialogueService == null)
            {
                Debug.LogError($"[DTSTriggerZone:{gameObject.name}] DialogueService is null!");
                return;
            }

            if (_defaultDialogue == null || _defaultDialogue._questData == null)
            {
                Debug.LogWarning($"[DTSTriggerZone:{gameObject.name}] No default dialogue setup");
                return;
            }
            if (_isDialogueRunning)
            {
                return;
            }
            // InGameControlStateManager.Instance.SetState(InGameActionType.OnDialogue);
            _isDialogueRunning = true;

            if (_isDialogueRunning == true)
            {
                _dialogueService.StartDefaultDialogue(_defaultDialogue._questData, _defaultDialogue._questIndex);
            }
            _dialogueService.OnDialogueComplete += () =>
            {
                Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                // OnDialogueComplete();
            };
        }

        private void OnDialogueComplete()
        {
            _isDialogueRunning = false;
            Debug.Log($"[DTSTriggerZone:{gameObject.name}] Dialogue complete → can trigger again");
        }

        // public void SetKey(string key)
        // {
        //     keyToTrigger = key;
        // }
    }
}
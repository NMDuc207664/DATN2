using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using TMPro;

namespace DATN2.GraphviewEditor.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class DTSDialogueTriggerZone : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField] private string keyToTrigger = "Map_1_OpeningS_Q1";
        [SerializeField] private string mapKey = "Map1";
        [SerializeField] private float interactDistance = 5f;

        [Header("Prompt Settings")]
        [SerializeField] private Transform promptCanvas; // assign the World Space Canvas here
        [SerializeField] private TextMeshProUGUI promptText; // assign TMP text from Canvas
        [SerializeField] private string interactPrompt = "E: Interact";
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 2f, 0f);
    [Header("Screen-style Settings")]
    [Tooltip("When true the prompt will scale with distance so it appears a constant size on the screen")]
    [SerializeField] private bool scaleWithDistance = true;
    [Tooltip("Multiplier used when scaling the prompt with distance. Tweak for desired screen size.")]
    [SerializeField] private float scaleFactor = 0.12f;
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 2f;
    [Tooltip("If true, the prompt will only rotate around Y so it stays upright (good for pitched cameras)")]
    [SerializeField] private bool billboardYOnly = true;

        private Dictionary<string, IQuestService> _questControllers;
        private Transform _player;
        private bool _inRange;

        public void SetQuestControllers(Dictionary<string, IQuestService> questControllers)
        {
            _questControllers = questControllers;
            Debug.Log($"[DTSTriggerZone:{gameObject.name}] QuestControllers injected successfully");
        }

        void Start()
        {
            if (promptText != null)
            {
                // center the prompt text on the canvas and enable auto-sizing so it looks like UI
                promptText.alignment = TextAlignmentOptions.Center;
                promptText.enableAutoSizing = true;
                promptText.text = interactPrompt;
            }

            if (promptCanvas != null)
            {
                // ensure canvas is world-space so it can be positioned in the scene like a screen
                var canvas = promptCanvas.GetComponentInParent<UnityEngine.Canvas>();
                if (canvas != null)
                {
                    canvas.renderMode = UnityEngine.RenderMode.WorldSpace;
                }

                // hide initially and place above the NPC
                promptCanvas.gameObject.SetActive(false);
                promptCanvas.position = transform.position + promptOffset;

                // initial scale so the prompt looks like a ui element
                if (scaleWithDistance && Camera.main != null)
                {
                    var dist = Vector3.Distance(Camera.main.transform.position, promptCanvas.position);
                    var s = Mathf.Clamp(dist * scaleFactor, minScale, maxScale);
                    promptCanvas.localScale = Vector3.one * s;
                }
            }
        }

        void Update()
        {
            if (_player == null)
            {
                var pgo = GameObject.FindWithTag("Player");
                if (pgo != null) _player = pgo.transform;
            }

            if (_player == null) return;

            var dist = Vector3.Distance(_player.position, transform.position);
            _inRange = dist <= interactDistance;

            if (promptCanvas != null)
            {
                // keep the prompt positioned above the NPC so it overlaps the NPC
                promptCanvas.position = transform.position + promptOffset;

                // scale with distance so it appears as a flat UI element on the screen
                if (scaleWithDistance && Camera.main != null)
                {
                    var dist = Vector3.Distance(Camera.main.transform.position, promptCanvas.position);
                    var s = Mathf.Clamp(dist * scaleFactor, minScale, maxScale);
                    promptCanvas.localScale = Vector3.one * s;
                }

                promptCanvas.gameObject.SetActive(_inRange && !KeyGameStateManager.Instance.currentKeysActive.Contains(keyToTrigger));
            }

            if (_inRange && Input.GetKeyDown(KeyCode.E))
                TryActivate();
        }

        void LateUpdate()
        {
            // Make prompt face camera
            if (promptCanvas != null && Camera.main != null)
            {
                promptCanvas.LookAt(Camera.main.transform);
                promptCanvas.Rotate(0, 180f, 0); // flip text
            }
        }

        private void TryActivate()
        {
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
    }
}

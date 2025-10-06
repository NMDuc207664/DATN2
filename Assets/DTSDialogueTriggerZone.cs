using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

namespace DATN2.GraphviewEditor.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class DTSDialogueTriggerZone : MonoBehaviour
    {
        [SerializeField]
        private string keyToTrigger = "Map_1_OpeningS_Q1";
        [SerializeField] private string mapKey = "Map1";
        private DialogueTriggerType triggerType;
        private Dictionary<string, IQuestService> _questControllers;
        public void SetQuestControllers(Dictionary<string, IQuestService> questControllers)
        {
            this._questControllers = questControllers;
            Debug.Log($"[DTSTriggerZone:{gameObject.name}] QuestControllers injected successfully");
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
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
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.Inspectors;
using UnityEngine;

namespace DATN2.Assets.Scripts.Data.Runtime
{
    [Serializable]
    public class QuestRuntime
    {

        // Used to track the initial and current position of NPCs
        [SerializeField] public SerializableDictionary<string, Vector3> currentNPCPosition;
        [SerializeField] public SerializableDictionary<string, Vector3> targetPosition;

        // Dialogue System Integration
        [Header("Dialogue Settings")]
        [SerializeField] public DialogueTriggerType triggerType;
        [SerializeField] public DTSDialogue dialogues;


        [Header("Default Dialogue Settings")]
        public bool HasDefaultDialogue;
        [SerializeField] public DTSDialogue defaultDialogue;
    }
}
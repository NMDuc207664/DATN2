using System;
using System.Collections.Generic;
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
        [SerializeField] public SerializableDictionary<string, Transform> currentNPCPosition;
        [SerializeField] public List<Vector3> targetPosition;

        // Dialogue System Integration
        [Header("Dialogue Settings")]
        [SerializeField] public DTSDialogue dialogue;


        [Header("Default Dialogue Settings")]
        public bool HasDefaultDialogue;
        [SerializeField] public DTSDialogue defaultDialogue;
    }
}
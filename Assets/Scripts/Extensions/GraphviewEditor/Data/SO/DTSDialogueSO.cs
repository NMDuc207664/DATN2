using System.Collections.Generic;
using DATN2.GraphviewEditor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.GraphviewEditor.Data.SaveModal.SO
{
    public class DTSDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField][field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DTSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public List<DTSConditionSO> Conditions { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public List<string> ActiveKey { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        [field: SerializeField] public bool HasConditions { get; set; }
        [field: SerializeField] public bool HasTalked { get; set; }
        public void Initialize(string dialogueName, string text, List<DTSChoiceSaveData> choices, DTSDialogueType dialogueType, bool isStartingDialogue, bool hasConditions, bool hasTalked, List<string> key)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            ActiveKey = key;
            IsStartingDialogue = isStartingDialogue;
            HasConditions = hasConditions;
            HasTalked = hasTalked;
            Conditions = new List<DTSConditionSO>();
        }
    }
}
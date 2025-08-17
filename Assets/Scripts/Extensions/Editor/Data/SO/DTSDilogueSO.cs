using System.Collections.Generic;
using DATN2.Editor.DialogueSystem.Enum;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal.SO
{
    public class DTSDilogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField][field: TextArea] public string Text { get; set; }
        [field: SerializeField] public List<string> Choices { get; set; }
        [field: SerializeField] public DTSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        [field: SerializeField] public bool HasConditions { get; set; }
        public void Initialize(string dialogueName, string text, List<string> choices, DTSDialogueType dialogueType, bool isStartingDialogue, bool hasConditions)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
            HasConditions = hasConditions;
        }
    }
}
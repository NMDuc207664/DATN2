using System;
using DATN2.Editor.Data.SaveModal.SO;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DTSDialogueSO NextDialogue { get; set; }
    }
}


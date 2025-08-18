using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal.SO
{
    public class DTSDialogueContainerSO : ScriptableObject
    {
        public string FileName { get; set; }
        public List<DTSDialogueSO> Dialogues { get; set; }
        public SerializableDictionary<DTSDialogueGroupSO, List<DTSDialogueSO>> DialogueGroups { get; set; }
        public void Initialize(string fileName)
        {
            FileName = fileName;
            Dialogues = new List<DTSDialogueSO>();
            DialogueGroups = new SerializableDictionary<DTSDialogueGroupSO, List<DTSDialogueSO>>();
        }
    }
}
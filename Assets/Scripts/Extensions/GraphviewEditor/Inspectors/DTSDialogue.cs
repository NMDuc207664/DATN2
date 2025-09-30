using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using UnityEngine;
using UnityEngine.Events;
namespace DATN2.GraphviewEditor.Inspectors
{
    [CreateAssetMenu(fileName = "DialogueConfig", menuName = "Dialogue/Dialogue Config")]
    public class DTSDialogue : ScriptableObject
    {
        [SerializeField] public DTSGraphSaveDataSO dialogueGraph;
        [SerializeField] public DTSDialogueGroupSO dialogueGroup;
        [SerializeField] public DTSDialogueSO dialogue;
        [SerializeField] public List<DTSConditionSO> condition;

        /* Filters */
        [SerializeField] public bool groupedDialogues;
        [SerializeField] public bool startingDialoguesOnly;

        [SerializeField] public int selectedDialogueGroupIndex;
        [SerializeField] public int selectedDialogueIndex;

        public static event UnityAction<DTSDialogueSO> OnAnyDialogueFinished;
        public void OnDialogueFinished()
        {
            if (dialogue != null)
            {
                dialogue.HasTalked = true;
                OnAnyDialogueFinished?.Invoke(dialogue);

            }
        }
    }
}
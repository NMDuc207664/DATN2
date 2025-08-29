using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using UnityEngine;
namespace DATN2.GraphviewEditor.Inspectors
{
    public class DTSDialogue : MonoBehaviour
    {
        [SerializeField] private DTSGraphSaveDataSO dialogueGraph;
        [SerializeField] private DTSDialogueGroupSO dialogueGroup;
        [SerializeField] private DTSDialogueSO dialogue;
        [SerializeField] private List<DTSConditionSO> condition;

        /* Filters */
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;

        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
    }
}
using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using TMPro;
using UnityEngine;

public class DialogueDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    // [SerializeField] private DSDialogueSO startingDialogue;
    private DSDialogueSO currentDialogue;

    private void Start()
    {
        dialogueBox.SetActive(false);
        // if (startingDialogue != null)
        // {
        //     StartDialogue(startingDialogue);
        // }
    }

    public void StartDialogue(DSDialogueSO dialogue)
    {
        dialogueBox.SetActive(true);
        currentDialogue = dialogue;
        DisplayCurrentDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (currentDialogue.Choices != null && currentDialogue.Choices.Count > 0)
        {
            var nextDialogue = currentDialogue.Choices[0].NextDialogue;
            if (nextDialogue != null)
            {
                currentDialogue = nextDialogue;
                DisplayCurrentDialogue();
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayCurrentDialogue()
    {
        dialogueText.text = currentDialogue.Text;
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
        currentDialogue = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using DS;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DSDialogue dialogue;
    [SerializeField] private DialogueDisplayer displayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            displayer.StartDialogue(dialogue.dialogue);
        }
    }
}

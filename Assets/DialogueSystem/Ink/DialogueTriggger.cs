using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] GameObject visualCue;
    [Header("Ink Json")]
    [SerializeField] TextAsset inkJSON;
    private bool playerInRange;
    private void Awake()
    {
        playerInRange = false;
        // visualCue.SetActive(false);
    }
    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            // visualCue.SetActive(true);//only work on 2D
            // if (playerInput == "E")//example
            // {
            //     DialogueManager.GetInstance().EnterDialougueMode(inkJSON);
            // }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DialogueManager.GetInstance().EnterDialougueMode(inkJSON);
            }

        }
        // else
        // {
        //     visualCue.SetActive(false);
        // }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
            // DialogueManager.GetInstance().EnterDialougueMode(inkJSON);
            // if (playerInput == "E")//example
            // {
            //     DialogueManager.GetInstance().EnterDialougueMode(inkJSON);
            // }
        }
    }
    private void OnTriggerExit(Collider collider)
    {

    }

}

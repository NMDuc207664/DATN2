using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    public bool inTrigger = false;
    public List<int> availableQuestIDs = new List<int>();
    public List<int> receivableQuestIDs = new List<int>();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && Input.GetKeyDown(KeyCode.T))
        {

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;
        }
    }
}

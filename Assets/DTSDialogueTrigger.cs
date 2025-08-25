using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Applications;
using DATN2.GraphviewEditor.Data.SaveModal;
using UnityEngine;

public class DTSDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DTSGraphSaveDataSO graphSO; // The graph containing the dialogues
    [SerializeField] private string selectedNodeName; // Selected node name from dropdown

    public void TriggerDialogue()
    {
        if (DTSDialogueTest.Instance == null)
        {
            Debug.LogError("[DialogueTrigger] DialogueManager not found! Ensure it's in the scene.");
            return;
        }

        if (graphSO == null)
        {
            Debug.LogError("[DialogueTrigger] GraphSO is null!");
            return;
        }

        if (string.IsNullOrEmpty(selectedNodeName))
        {
            Debug.LogError("[DialogueTrigger] SelectedNodeName is not set!");
            return;
        }

        // Use DTSReadSO to get NodeNames and find the corresponding NodeID
        DTSReadSO reader = new DTSReadSO();
        reader.GetInformationFromGraph(graphSO);
        var nodeData = graphSO.Nodes.FirstOrDefault(n => n.Name == selectedNodeName);
        if (nodeData == null)
        {
            Debug.LogError($"[DialogueTrigger] No node found with name: {selectedNodeName}");
            return;
        }

        string nodeID = nodeData.NodeID;
        Debug.Log($"[DialogueTrigger] Triggering dialogue from Node: {selectedNodeName} (NodeID: {nodeID}) in graph: {graphSO.GraphName}");
        DTSDialogueTest.Instance.StartDialogue(graphSO, nodeID);
    }

    // Optional: Example of how to trigger (e.g., on collision or interaction)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }

    // Public method to access NodeNames for the custom Inspector
    public string[] GetNodeNames()
    {
        if (graphSO == null)
        {
            return new string[0];
        }

        DTSReadSO reader = new DTSReadSO();
        reader.GetInformationFromGraph(graphSO);
        return reader.NodeNames.ToArray();
    }

    // Public method to get/set the selected node name
    public string SelectedNodeName
    {
        get => selectedNodeName;
        set => selectedNodeName = value;
    }

    // Public method to get the GraphSO (for the custom Inspector)
    public DTSGraphSaveDataSO GraphSO => graphSO;
}

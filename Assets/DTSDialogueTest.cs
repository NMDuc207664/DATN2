using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DATN2.GraphviewEditor.Applications;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DTSDialogueTest : MonoBehaviour
{
    //     [SerializeField] private DTSGraphSaveDataSO graphSO; // Assign this in Inspector
    //     [SerializeField] private TextMeshProUGUI dialogueTextUI; // UI for dialogue text
    //     [SerializeField] private Transform choicesContainer; // Panel for choice buttons
    //     [SerializeField] private Button choiceButtonPrefab; // Choice button prefab

    //     private Dictionary<string, DTSDialogueSO> dialogueSOMap = new Dictionary<string, DTSDialogueSO>();
    //     private DTSDialogueSO currentDialogueSO;
    //     private string graphFileName;


    //     private void Start()
    //     {
    //         if (graphSO == null)
    //         {
    //             Debug.LogError("No Graph SO assigned!");
    //             return;
    //         }
    //         graphFileName = graphSO.GraphName;
    //         // DTSReadSO reader = new DTSReadSO();
    //         // reader.GetInformationFromGraph(graphSO);
    //         // reader.GetInformationFromNode(graphSO);
    //         LoadGraphData();
    //         TriggerStartingDialogue();
    //     }

    public static DTSDialogueTest Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI dialogueTextUI; // UI for dialogue text
    [SerializeField] private Transform choicesContainer; // Panel for choice buttons
    [SerializeField] private Button choiceButtonPrefab; // Choice button prefab

    private Dictionary<string, DTSDialogueSO> dialogueSOMap = new Dictionary<string, DTSDialogueSO>();
    private DTSDialogueSO currentDialogueSO;
    private DTSGraphSaveDataSO currentGraphSO;
    private string graphFileName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(DTSGraphSaveDataSO graphSO, string startingNodeID)
    {
        if (graphSO == null)
        {
            Debug.LogError("[DialogueManager] GraphSO is null!");
            return;
        }

        graphFileName = graphSO.GraphName;
        if (string.IsNullOrEmpty(graphFileName))
        {
            Debug.LogError("[DialogueManager] GraphSO.GraphName is empty!");
            return;
        }

        currentGraphSO = graphSO;
        LoadGraphData();

        if (!dialogueSOMap.TryGetValue(startingNodeID, out var startingDialogueSO))
        {
            Debug.LogError($"[DialogueManager] DTSDialogueSO not found for node ID: {startingNodeID}");
            return;
        }

        // Find node data for logging
        var nodeData = graphSO.Nodes.FirstOrDefault(n => n.NodeID == startingNodeID);
        string nodeName = nodeData != null ? nodeData.Name : "Unknown";
        Debug.Log($"[DialogueManager] Starting dialogue: {nodeName} (NodeID: {startingNodeID})");

        DisplayDialogue(startingDialogueSO);
    }

    private void LoadGraphData()
    {
        dialogueSOMap.Clear();
        Debug.Log($"[DialogueManager] Loading graph: {graphFileName}");

        foreach (var nodeData in currentGraphSO.Nodes)
        {
            DTSDialogueSO dialogueSO = LoadDialogueSO(nodeData.Name);
            if (dialogueSO == null)
            {
                Debug.LogWarning($"[DialogueManager] Failed to load DTSDialogueSO for node: {nodeData.Name} (NodeID: {nodeData.NodeID})");
                continue;
            }

            dialogueSOMap.Add(nodeData.NodeID, dialogueSO);
            Debug.Log($"[DialogueManager] Loaded DialogueSO: {dialogueSO.DialogueName} (NodeID: {nodeData.NodeID}, IsStartingDialogue: {dialogueSO.IsStartingDialogue})");
        }

        Debug.Log($"[DialogueManager] Loaded {dialogueSOMap.Count} dialogues for graph: {graphFileName}");
    }

    private DTSDialogueSO LoadDialogueSO(string nodeName)
    {
        // Try Global path
        string globalPath = $"Dialogues/{graphFileName}/Global/Dialogues/{nodeName}";
        DTSDialogueSO dialogueSO = Resources.Load<DTSDialogueSO>(globalPath);
        if (dialogueSO != null)
        {
            Debug.Log($"[DialogueManager] Loaded DTSDialogueSO from: {globalPath}");
            return dialogueSO;
        }

        // Try Group paths
        foreach (var group in currentGraphSO.Groups)
        {
            string groupPath = $"Dialogues/{graphFileName}/Groups/{group.GroupName}/{nodeName}";
            dialogueSO = Resources.Load<DTSDialogueSO>(groupPath);
            if (dialogueSO != null)
            {
                Debug.Log($"[DialogueManager] Loaded DTSDialogueSO from: {groupPath}");
                return dialogueSO;
            }
        }

        return null;
    }

    private void DisplayDialogue(DTSDialogueSO dialogueSO)
    {
        currentDialogueSO = dialogueSO;

        // Display text
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = dialogueSO.Text;
            Debug.Log($"[DialogueManager] Displaying Dialogue: {dialogueSO.DialogueName}, Text: {dialogueSO.Text}");
        }
        else
        {
            Debug.LogWarning("[DialogueManager] DialogueTextUI is not assigned! Logging text: " + dialogueSO.Text);
        }

        // Check conditions
        if (dialogueSO.HasConditions && dialogueSO.Conditions != null && dialogueSO.Conditions.Count > 0)
        {
            bool conditionsMet = true; // Placeholder for condition evaluation
            foreach (var condition in dialogueSO.Conditions)
            {
                // Log condition for debugging; implement actual evaluation logic here
                Debug.Log($"[DialogueManager] Evaluating Condition for {dialogueSO.DialogueName}: {condition.ConditionName}");
                // Placeholder: Replace with actual condition evaluation
                // For example: conditionsMet &= EvaluateCondition(condition);
            }
            if (!conditionsMet)
            {
                Debug.LogWarning($"[DialogueManager] Conditions not met for Dialogue: {dialogueSO.DialogueName}");
                EndDialogue();
                return;
            }
        }
        else
        {
            Debug.Log($"[DialogueManager] No conditions for Dialogue: {dialogueSO.DialogueName}");
        }

        // Display choices
        ClearChoices();
        if (dialogueSO.Choices != null && dialogueSO.Choices.Count > 0)
        {
            Debug.Log($"[DialogueManager] Dialogue: {dialogueSO.DialogueName} has {dialogueSO.Choices.Count} choices");
            for (int i = 0; i < dialogueSO.Choices.Count; i++)
            {
                var choice = dialogueSO.Choices[i];
                var button = Instantiate(choiceButtonPrefab, choicesContainer);
                button.GetComponentInChildren<TextMeshProUGUI>().text = choice.Text;
                int index = i;
                button.onClick.AddListener(() => OnChoiceSelected(index));
                Debug.Log($"[DialogueManager] Added choice button: {choice.Text} (ChoiceID: {choice.ChoiceID})");
            }
        }
        else
        {
            Debug.Log($"[DialogueManager] Dialogue: {dialogueSO.DialogueName} has no choices, ending dialogue");
            EndDialogue();
        }
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        var selectedChoice = currentDialogueSO.Choices[choiceIndex];
        Debug.Log($"[DialogueManager] Selected choice: {selectedChoice.Text} (ChoiceID: {selectedChoice.ChoiceID})");

        if (selectedChoice.ConnectedNodeIDs == null || selectedChoice.ConnectedNodeIDs.Count == 0)
        {
            Debug.Log("[DialogueManager] No connected nodes for this choice, ending dialogue");
            EndDialogue();
            return;
        }

        // Take the first connected node
        var nextNodeID = selectedChoice.ConnectedNodeIDs[0];
        if (dialogueSOMap.TryGetValue(nextNodeID, out var nextDialogueSO))
        {
            // Find node data for logging
            var nodeData = currentGraphSO.Nodes.FirstOrDefault(n => n.NodeID == nextNodeID);
            string nodeName = nodeData != null ? nodeData.Name : "Unknown";
            Debug.Log($"[DialogueManager] Moving to next dialogue: {nextDialogueSO.DialogueName} (NodeID: {nextNodeID}, Name: {nodeName})");
            DisplayDialogue(nextDialogueSO);
        }
        else
        {
            Debug.LogError($"[DialogueManager] DTSDialogueSO not found for next node: {nextNodeID}");
            EndDialogue();
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("[DialogueManager] Cleared all choice buttons");
    }

    private void EndDialogue()
    {
        Debug.Log("[DialogueManager] Dialogue Ended");
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = "";
        }
        ClearChoices();
        currentDialogueSO = null;
        currentGraphSO = null;
        dialogueSOMap.Clear();
    }

}

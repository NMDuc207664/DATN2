using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using DATN2.GraphviewEditor.Inspectors;
using UnityEngine;

namespace DATN2.GraphviewEditor.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class DTSTriggerZone : MonoBehaviour
    {
        [Header("Target Dialogue Component")]
        [SerializeField] private DTSDialogue target;   // Gán cái DTSDialogue trong scene

        [Header("Trigger Settings")]
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;

        [Tooltip("Set Graph (đây là bắt buộc)")]
        [SerializeField] private DTSGraphSaveDataSO graph;

        [Tooltip("Chỉ dùng nếu Grouped = true")]
        [SerializeField] private DTSDialogueGroupSO targetGroup;

        [Tooltip("Chỉ dùng nếu Grouped = false")]
        [SerializeField] private DTSDialogueSO targetDialogue;

        [Header("Context / Repeat Settings")]
        [SerializeField] private List<DTSDialogueSO> repeatableDialogues;
        //[SerializeField] private List<DTSContextConditionSO> contextConditions;

        // Runtime resolved dialogue (chỉ để debug, không config)
        [SerializeField] private DTSDialogueSO resolvedDialogue;

        //         private void OnTriggerEnter(Collider other)
        //         {
        //             if (!other.CompareTag("Player")) return;

        //             DTSDialogueSO chosenDialogue = groupedDialogues ? null : targetDialogue;

        //             bool canTrigger = DialogueContextEvaluator.CanTriggerDialogue(
        //                 chosenDialogue,
        //                 contextConditions,
        //                 repeatableDialogues
        //             );

        //             if (canTrigger)
        //             {
        //                 ApplySettings();
        //             }

        //             ApplySettings();
        //         }

        //         private void ApplySettings()
        //         {
        //             if (target == null) return;
        //             target.dialogueGraph = graph;
        //             target.groupedDialogues = groupedDialogues;
        //             target.startingDialoguesOnly = startingDialoguesOnly;

        //             if (groupedDialogues)
        //             {
        //                 target.dialogueGroup = targetGroup;
        //                 target.dialogue = null; // reset dialogue nếu đang chọn lẻ
        //             }
        //             else
        //             {
        //                 target.dialogue = targetDialogue;
        //                 target.dialogueGroup = null;
        //             }

        // #if UNITY_EDITOR
        //             // Force inspector refresh trong Editor
        //             UnityEditor.EditorUtility.SetDirty(target);
        // #endif
        //         }
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (!other.CompareTag("Player")) return;

        //     // Chọn dialogue từ context evaluator
        //     resolvedDialogue = DialogueContextEvaluator.ResolveDialogue(
        //         groupedDialogues ? null : targetDialogue,
        //         contextConditions,
        //         repeatableDialogues
        //     );

        //     // Nếu tìm được dialogue hợp lệ thì apply
        //     if (resolvedDialogue != null || groupedDialogues)
        //     {
        //         ApplySettings();
        //     }
        // }
    }
}

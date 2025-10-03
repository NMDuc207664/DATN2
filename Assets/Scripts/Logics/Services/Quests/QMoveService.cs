using System;
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using UnityEngine;

public class QMoveService : IQmoveService
{
    // [SerializeField] private GameEventSystem gameEventManager;

    // void Awake()
    // {
    //     // Find GameEventManager if not assigned
    //     if (gameEventManager == null)
    //     {
    //         gameEventManager = FindObjectOfType<GameEventSystem>();
    //         if (gameEventManager == null)
    //         {
    //             Debug.LogError("[NPCController] GameEventManager not found in scene!");
    //         }
    //     }

    //     // Subscribe to key events
    //     gameEventManager.OnKeyActivated += HandleKeyActivated;
    // }

    // void OnDestroy()
    // {
    //     // Unsubscribe to prevent memory leaks
    //     gameEventManager.OnKeyActivated -= HandleKeyActivated;
    // }

    // private void HandleKeyActivated(string key, QuestDataSO questData)
    // {
    //     foreach (var quest in questData.quests)
    //     {
    //         // Handle NPC movement
    //         foreach (var npc in quest.currentNPCPosition)
    //         {
    //             if (npc.Value != null && quest.targetPosition.Count > 0)
    //             {
    //                 MoveNPC(npc.Value, quest.targetPosition[0]);
    //             }
    //         }
    //     }
    // }

    // private void MoveNPC(Transform npc, Vector3 targetPosition)
    // {
    //     while (Vector3.Distance(npc.position, targetPosition) > 0.1f)
    //     {
    //         npc.position = Vector3.MoveTowards(
    //             npc.position,
    //             targetPosition,
    //             Time.deltaTime * 5f
    //         );

    //     }
    //     Debug.Log($"NPC {npc.name} reached target position: {targetPosition}");
    // }
    private readonly MonoBehaviour _coroutineRunner;

    public QMoveService(MonoBehaviour coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void MoveAsync(Transform npc, Vector3 targetPosition, Action onComplete = null)
    {
        _coroutineRunner.StartCoroutine(MoveNPC(npc, targetPosition, onComplete));
    }

    private IEnumerator MoveNPC(Transform npc, Vector3 targetPosition, Action onComplete)
    {
        while (Vector3.Distance(npc.position, targetPosition) > 0.1f)
        {
            npc.position = Vector3.MoveTowards(
                npc.position,
                targetPosition,
                Time.deltaTime * 5f
            );
            yield return null;
        }
        Debug.Log($"NPC {npc.name} reached target position: {targetPosition}");
        onComplete?.Invoke();
    }
}

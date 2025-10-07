using System;
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using UnityEngine;

public class QMoveService : IQmoveService
{
    private List<Vector3> destinations = new List<Vector3>();
    private float stopDistance = 1.5f;       // khoảng cách tính là "đã tới"
    private float waitTimeAtPoint = 0.5f;    // thời gian dừng giữa các điểm
    private float sampleRadius = 25f;
    // private readonly MonoBehaviour _coroutineRunner;

    // public QMoveService(MonoBehaviour coroutineRunner)
    // {
    //     _coroutineRunner = coroutineRunner;
    // }

    // public void MoveAsync(Transform npc, Vector3 targetPosition, Action onComplete = null)
    // {
    //     _coroutineRunner.StartCoroutine(MoveNPC(npc, targetPosition, onComplete));
    // }

    // private IEnumerator MoveNPC(Transform npc, Vector3 targetPosition, Action onComplete)
    // {
    //     while (Vector3.Distance(npc.position, targetPosition) > 0.1f)
    //     {
    //         npc.position = Vector3.MoveTowards(
    //             npc.position,
    //             targetPosition,
    //             Time.deltaTime * 5f
    //         );
    //         yield return null;
    //     }
    //     Debug.Log($"NPC {npc.name} reached target position: {targetPosition}");
    //     onComplete?.Invoke();
    //}
    public void StartMovement(List<Vector3> destinations)
    {
        throw new NotImplementedException();
    }
}

using System;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Interface.NPC
{
    public interface IQmoveService
    {
        void MoveAsync(Transform npc, Vector3 targetPosition, Action onComplete = null);
    }
}
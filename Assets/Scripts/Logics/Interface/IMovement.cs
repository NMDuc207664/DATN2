using System.Collections;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface IMovement
    {
        [RequireGameState(StateType.Ingame)]
        void Move(Vector3 direction, float speed);
        [RequireGameState(StateType.Ingame)]
        void Jump(float height);
        [RequireGameState(StateType.Ingame)]
        IEnumerator PickUp();
    }
}
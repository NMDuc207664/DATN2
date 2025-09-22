using System.Collections;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface IMovement
    {
        [RequireGameState(StateType.Ingame)]
        void Move(Vector3 direction, float speed, bool isGrounded, float airMultiplier = 0.5f);
        [RequireGameState(StateType.Ingame)]
        void Jump(float height);
        [RequireGameState(StateType.Ingame)]
        void PickUp();
        void UpdateDrag(bool isGrounded);
        void SetStepClimbingParameters(float maxStepHeight, float stepRayDistance, LayerMask stepLayerMask, float stepUpForce);
        void SetStepRayTransforms(Transform lowerRay, Transform upperRay);
    }
}
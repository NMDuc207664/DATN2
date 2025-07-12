using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface IMovement
    {
        void Move(Vector3 direction, float speed);
    }
}
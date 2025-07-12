using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer.Unity;

namespace DATN2.Assets.Scripts.Logics.Services
{
    public class MovementService : IMovement
    {
        private readonly Transform _playerTransform;
        public MovementService(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        public void Move(Vector3 direction, float speed)
        {
            _playerTransform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        // public void Start()
        // {
        //     throw new System.NotImplementedException();
        // }
    }
}
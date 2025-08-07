using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Services
{
    public class MovementService : IMovement
    {
        private readonly Transform _playerTransform;
        private readonly Rigidbody _rigidbody;
        public MovementService(Transform playerTransform, Rigidbody rigidbody)
        {
            _playerTransform = playerTransform;
            _rigidbody = rigidbody;

        }

        // [RequireGameState(StateType.Pause)]
        public void Jump(float height)
        {
            if (IsGrounded())
            {
                _rigidbody.AddForce(Vector3.up * height, ForceMode.Impulse);
                Debug.Log("Jumped!");
            }
            // throw new System.NotImplementedException();
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(_rigidbody.position, Vector3.down, 1.1f);
            // throw new System.NotImplementedException();
        }

        // [RequireGameState(StateType.Ingame)]
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
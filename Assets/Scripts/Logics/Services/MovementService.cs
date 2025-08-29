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
                // Debug.Log("Jumped!");
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
            if (direction.magnitude >= 0.1f)
            {
                // Tính hướng di chuyển dựa vào hướng player đang nhìn
                Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
                moveDir.y = 0f; // tránh đi chéo lên trời nếu player nghiêng

                _rigidbody.MovePosition(_rigidbody.position + moveDir.normalized * speed * Time.deltaTime);
            }
        }

        // public void Start()
        // {
        //     throw new System.NotImplementedException();
        // }
    }
}
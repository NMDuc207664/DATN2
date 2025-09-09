using System.Collections;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Services
{
    public class MovementService : IMovement
    {
        private readonly Transform _playerTransform;
        private readonly Rigidbody _rigidbody;
        private readonly Animator _animator;
        public MovementService(Transform playerTransform, Rigidbody rigidbody, Animator animator)
        {
            _playerTransform = playerTransform;
            _rigidbody = rigidbody;
            _animator = animator;
        }

        // [RequireGameState(StateType.Pause)]
        public void Jump(float height)
        {

            _rigidbody.AddForce(Vector3.up * height, ForceMode.Impulse);
            Debug.Log("Jumped!");

        }


        // [RequireGameState(StateType.Ingame)]
        public void Move(Vector3 direction, float speed)
        {
            if (direction.magnitude >= 0.1f)
            {
                // Tính hướng di chuyển dựa vào hướng player đang nhìn
                Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
                moveDir.y = 0f; // tránh đi chéo lên trời nếu player nghiêng
                _animator.SetBool("isWalking", true);
                _rigidbody.MovePosition(_rigidbody.position + moveDir.normalized * speed * Time.deltaTime);
            }
            else
            {
                _animator.SetBool("isWalking", false);
            }
        }

        public IEnumerator PickUp()
        {
            _animator.SetBool("isWalking", false);

            // Kích hoạt animation
            _animator.SetTrigger("isPickUp");
            float pickUpTime = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(pickUpTime);
        }
    }
}
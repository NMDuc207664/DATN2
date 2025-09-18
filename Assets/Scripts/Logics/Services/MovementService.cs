
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Services
{
    public class MovementService : IMovement
    {
        private readonly Transform _playerTransform;
        private readonly Rigidbody _rigidbody;
        private readonly Animator _animator;
        private readonly float _groundDrag = 4f;
        private readonly float _maxJumpVelocity = 12.5f;
        // private readonly float _jumpForce = 12f;

        public MovementService(Dictionary<string, Transform> transforms, Rigidbody rigidbody, Animator animator)
        {
            _playerTransform = transforms["PlayerTransform"];
            _rigidbody = rigidbody;
            _animator = animator;

            // Configure Rigidbody for better physics-based movement
            _rigidbody.drag = _groundDrag;
            _rigidbody.freezeRotation = true; // Prevent physics from rotating the player
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate; // Smooth movement
        }

        public void Jump(float height)
        {
            _rigidbody.drag = 0f;
            _rigidbody.AddForce(Vector3.up * height, ForceMode.Impulse);

        }
        public void UpdateDrag(bool isGrounded)
        {
            _rigidbody.drag = isGrounded ? _groundDrag : 0f;
        }


        public void Move(Vector3 direction, float speed, bool isGrounded, float airMultiplier)
        {
            if (direction.magnitude >= 0.1f)
            {
                _animator.SetBool("isWalking", true);

                Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
                moveDir.y = 0f;
                moveDir.Normalize();

                float multiplier = isGrounded ? 1f : airMultiplier;

                _rigidbody.AddForce(moveDir * speed * 10f * multiplier, ForceMode.Force);
                LimitVelocity();
            }
            else
            {
                _animator.SetBool("isWalking", false);
            }
        }

        public void PickUp()
        {
            // Dừng di chuyển ngang (giữ y velocity cho gravity)
            Vector3 velocity = _rigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            _rigidbody.velocity = velocity;

            // Tắt walking state để vào Idle
            _animator.SetBool("isWalking", false);

            // Kích hoạt animation Interact
            _animator.SetTrigger("isPickUp");

        }
        private void LimitVelocity()
        {
            Vector3 velocity = _rigidbody.velocity;
            // Giới hạn vận tốc theo trục y
            if (Mathf.Abs(velocity.y) > _maxJumpVelocity)
            {
                velocity.y = Mathf.Sign(velocity.y) * _maxJumpVelocity;
                _rigidbody.velocity = velocity;
            }
        }
    }

}
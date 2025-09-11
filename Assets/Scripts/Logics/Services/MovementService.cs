// using System.Collections;
// using DATN2.Assets.Scripts.Logics.Interface;
// using UnityEngine;

// namespace DATN2.Assets.Scripts.Logics.Services
// {
//     public class MovementService : IMovement
//     {
//         private readonly Transform _playerTransform;
//         private readonly Rigidbody _rigidbody;
//         private readonly Animator _animator;
//         public MovementService(Transform playerTransform, Rigidbody rigidbody, Animator animator)
//         {
//             _playerTransform = playerTransform;
//             _rigidbody = rigidbody;
//             _animator = animator;
//         }

//         // [RequireGameState(StateType.Pause)]
//         public void Jump(float height)
//         {

//             _rigidbody.AddForce(Vector3.up * height, ForceMode.Impulse);
//             Debug.Log("Jumped!");

//         }


//         // [RequireGameState(StateType.Ingame)]
//         public void Move(Vector3 direction, float speed)
//         {
//             if (direction.magnitude >= 0.1f)
//             {
//                 // Tính hướng di chuyển dựa vào hướng player đang nhìn
//                 Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
//                 moveDir.y = 0f; // tránh đi chéo lên trời nếu player nghiêng
//                 _animator.SetBool("isWalking", true);
//                 _rigidbody.MovePosition(_rigidbody.position + moveDir.normalized * speed * Time.fixedDeltaTime);
//             }
//             else
//             {
//                 _animator.SetBool("isWalking", false);
//             }
//         }

//         public IEnumerator PickUp()
//         {
//             _animator.SetBool("isWalking", false);

//             // Kích hoạt animation
//             _animator.SetTrigger("isPickUp");
//             float pickUpTime = _animator.GetCurrentAnimatorStateInfo(0).length;
//             yield return new WaitForSeconds(pickUpTime);
//         }
//     }
// }
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

        // Physics parameters
        private readonly float _accelerationForce = 50f;
        private readonly float _maxSpeed = 8f;
        private readonly float _groundDrag = 5f;
        // private readonly float _jumpForce = 12f;

        public MovementService(Transform playerTransform, Rigidbody rigidbody, Animator animator)
        {
            _playerTransform = playerTransform;
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
            Debug.Log("Jumped!");

        }
        public void UpdateDrag(bool isGrounded)
        {
            _rigidbody.drag = isGrounded ? _groundDrag : 0f;
        }

        // public void Move(Vector3 direction, float speed)
        // {
        //     // Tính hướng di chuyển dựa vào hướng player đang nhìn
        //     Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
        //     moveDir.y = 0f; // Chỉ di chuyển trên mặt phẳng ngang
        //     moveDir = moveDir.normalized;

        //     if (direction.magnitude >= 0.1f)
        //     {
        //         _animator.SetBool("isWalking", true);

        //         // Sử dụng AddForce thay vì MovePosition để có physics tự nhiên hơn
        //         Vector3 targetVelocity = moveDir * speed;
        //         Vector3 currentVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

        //         // Tính lực cần thiết để đạt target velocity
        //         Vector3 velocityDifference = targetVelocity - currentVelocity;

        //         // Giới hạn tốc độ tối đa
        //         if (currentVelocity.magnitude < _maxSpeed)
        //         {
        //             _rigidbody.AddForce(velocityDifference * _accelerationForce, ForceMode.Acceleration);
        //         }

        //         // Alternative approach: Sử dụng velocity trực tiếp (smoother but less realistic)
        //         // Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 10f);
        //         // _rigidbody.velocity = new Vector3(newVelocity.x, _rigidbody.velocity.y, newVelocity.z);
        //     }
        //     else
        //     {
        //         _animator.SetBool("isWalking", false);

        //         // Áp dụng drag để dừng lại tự nhiên khi không có input
        //         Vector3 horizontalVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        //         _rigidbody.AddForce(-horizontalVelocity * _groundDrag, ForceMode.Acceleration);
        //     }

        //     // Cập nhật animator speed parameter để animation phù hợp với tốc độ thực tế
        //     float currentSpeed = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z).magnitude;
        //     _animator.SetFloat("Speed", currentSpeed / _maxSpeed); // Normalized speed
        // }
        public void Move(Vector3 direction, float speed)
        {
            if (direction.magnitude >= 0.1f)
            {
                _animator.SetBool("isWalking", true);

                Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
                moveDir.y = 0f;
                moveDir.Normalize();

                // ✅ Set velocity trực tiếp, giữ nguyên Y velocity (gravity, jump)
                Vector3 newVelocity = moveDir * speed;
                newVelocity.y = _rigidbody.velocity.y;
                _rigidbody.velocity = newVelocity;
            }
            else
            {
                _animator.SetBool("isWalking", false);

                // Khi đứng yên, giữ nguyên velocity Y (gravity/jump)
                _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
            }
        }

        public IEnumerator PickUp()
        {
            _animator.SetBool("isWalking", false);

            // Dừng movement bằng cách set velocity về 0 (physics-based)
            Vector3 velocity = _rigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            _rigidbody.velocity = velocity;

            // Kích hoạt animation
            _animator.SetTrigger("isPickUp");

            // Tính toán thời gian animation chính xác hơn
            _animator.Update(0f); // Force update để lấy state mới
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // Đợi cho animation hoàn thành
            float animationLength = stateInfo.length;
            yield return new WaitForSeconds(animationLength);
        }
    }

}
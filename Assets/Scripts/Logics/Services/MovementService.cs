
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

namespace DATN2.Assets.Scripts.Logics.Services
{
    public class MovementService : IMovement
    {
        private readonly Transform _playerTransform;
        private readonly Rigidbody _rigidbody;
        private readonly GameObject _arm;
        private readonly Animator _animator;
        private readonly float _groundDrag = 4f;
        private float _maxStepHeight;
        private float _stepRayDistance;
        private LayerMask _stepLayerMask;
        // private readonly float _jumpForce = 12f;

        public MovementService(Dictionary<string, Transform> transforms, Rigidbody rigidbody, Animator animator, Dictionary<string, GameObject> arm)
        {
            _playerTransform = transforms["PlayerTransform"];
            _rigidbody = rigidbody;
            _animator = animator;
            _arm = arm["Player_arm"];
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
        public void ConfigureStep(float maxStepHeight, LayerMask stepLayerMask)
        {
            _maxStepHeight = maxStepHeight;
            _stepLayerMask = stepLayerMask;
        }


        public void Move(Vector3 direction, float speed, bool isGrounded, float airMultiplier)
        {
            if (direction.magnitude >= 0.1f)
            {
                _animator.SetBool("isWalking", true);

                Vector3 moveDir = _playerTransform.right * direction.x + _playerTransform.forward * direction.z;
                moveDir.y = 0f;
                moveDir.Normalize();

                // Check for step climbing before applying movement
                if (isGrounded)
                {
                    // Ưu tiên StepUp

                    // Chỉ check StepDown khi không stepUp được và đang đi xuống/
                    if (_rigidbody.velocity.y <= 0f && CanStepDown(moveDir))
                    {
                        StepDown(moveDir, speed * 1.25f);
                        return;
                    }
                }

                float multiplier = isGrounded ? 1f : airMultiplier;
                _rigidbody.AddForce(moveDir * speed * 10f * multiplier, ForceMode.Force);
                // LimitVelocity();
            }
            else
            {
                _animator.SetBool("isWalking", false);
            }
        }

        public async void PickUp()
        {
            KeyGameStateManager.Instance.AddOrChangeGameState(InGameActionType.Interact);
            KeyGameStateManager.Instance.SetLockMouseInput(true);
            _arm.SetActive(true);
            // Dừng di chuyển ngang (giữ y velocity cho gravity)
            Vector3 velocity = _rigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            _rigidbody.velocity = velocity;

            // Tắt walking state để vào Idle
            _animator.SetBool("isWalking", false);


            // Kích hoạt animation Interact
            await SwingArmAsync(_arm);

            _arm.SetActive(false);
            KeyGameStateManager.Instance.SetLockMouseInput(false);
            KeyGameStateManager.Instance.AddOrChangeGameState(InGameActionType.None);

        }
        private async Task SwingArmAsync(GameObject arm)
        {
            Quaternion startRot = arm.transform.localRotation;
            Quaternion upRot = Quaternion.Euler(-90f, 0f, -30f);   // tay vung lên
            Quaternion downRot = Quaternion.Euler(40f, 0f, 0f);   // tay vung xuống

            float duration = 0.30f; // thời gian cho mỗi pha
            float elapsed;

            // Vung lên
            elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                arm.transform.localRotation = Quaternion.Slerp(startRot, upRot, t);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }

            await Task.Delay(100); // giữ 0.1s

            // Vung xuống
            elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                arm.transform.localRotation = Quaternion.Slerp(upRot, downRot, t);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }

            // Trả lại vị trí ban đầu
            elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                arm.transform.localRotation = Quaternion.Slerp(downRot, startRot, t);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
        }

        private bool CanStepDown(Vector3 moveDirection)
        {
            if (!_rigidbody) return false;

            Vector3 origin = _playerTransform.position + Vector3.up * 0.1f;
            RaycastHit hit;

            if (Physics.Raycast(origin, Vector3.down, out hit, _maxStepHeight + 0.2f, _stepLayerMask))
            {
                float stepHeight = _playerTransform.position.y - hit.point.y;

                // Chỉ coi là stepDown nếu khoảng cách nằm trong ngưỡng
                if (stepHeight > 0.05f && stepHeight <= _maxStepHeight)
                {
                    // Thêm điều kiện: phía trước không có vật cản cao (tránh nhầm stepUp)
                    if (!Physics.Raycast(origin, moveDirection, 0.3f, _stepLayerMask))
                    {
                        Debug.DrawRay(origin, Vector3.down * (_maxStepHeight + 0.2f), Color.blue);
                        return true;
                    }
                }
            }

            return false;
        }


        private void StepDown(Vector3 moveDirection, float speed)
        {
            RaycastHit hit;
            Vector3 origin = _playerTransform.position + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out hit, _maxStepHeight + 0.2f, _stepLayerMask))
            {
                Vector3 targetPosition = new Vector3(_playerTransform.position.x, hit.point.y, _playerTransform.position.z);

                // Move xuống mượt bằng MovePosition
                Vector3 newPos = Vector3.Lerp(_rigidbody.position, targetPosition, Time.fixedDeltaTime * 10f);
                _rigidbody.MovePosition(newPos);

                // Vẫn giữ momentum đi tới
                Vector3 forwardForce = moveDirection * speed * 5f;
                _rigidbody.AddForce(forwardForce, ForceMode.Force);

                Debug.Log("Step Down Applied");
            }
        }
    }

}
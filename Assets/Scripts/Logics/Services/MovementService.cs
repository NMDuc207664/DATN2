
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

        // Stair climbing parameters (now passed from CharacterController)
        private float _maxStepHeight;
        private float _stepRayDistance;
        private LayerMask _stepLayerMask;
        private float _stepUpForce;
        private readonly float _stepSmoothTime = 0.1f;

        // Cooldown to prevent spamming step climbing
        private float _lastStepTime = 0f;
        private readonly float _stepCooldown = 0.2f;

        // Stair detection transforms (set these in constructor or via method)
        private Transform _stepRayLower;
        private Transform _stepRayUpper;

        public MovementService(Dictionary<string, Transform> transforms, Rigidbody rigidbody, Animator animator)
        {
            _playerTransform = transforms["PlayerTransform"];
            _rigidbody = rigidbody;
            _animator = animator;

            // Try to get step ray transforms if they exist
            if (transforms.ContainsKey("StepRayLower"))
                _stepRayLower = transforms["StepRayLower"];
            if (transforms.ContainsKey("StepRayUpper"))
                _stepRayUpper = transforms["StepRayUpper"];

            // Configure Rigidbody for better physics-based movement
            _rigidbody.drag = _groundDrag;
            _rigidbody.freezeRotation = true;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
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

                // Check for step climbing before applying movement
                if (isGrounded)
                {
                    // Ưu tiên StepUp
                    if (CanStepUp(moveDir))
                    {
                        // StepUp(moveDir, speed);
                        StepUp(moveDir, speed);
                        return;
                    }

                    // Chỉ check StepDown khi không stepUp được và đang đi xuống/
                    if (_rigidbody.velocity.y <= 0f && CanStepDown(moveDir))
                    {
                        StepDown(moveDir, speed * 1.25f);
                        return;
                    }
                }

                float multiplier = isGrounded ? 1f : airMultiplier;
                _rigidbody.AddForce(moveDir * speed * 10f * multiplier, ForceMode.Force);
                LimitVelocity();
            }
            else
            {
                _animator.SetBool("isWalking", false);
            }
        }

        private bool CanStepUp(Vector3 moveDirection)
        {
            // Check cooldown
            if (Time.time - _lastStepTime < _stepCooldown)
                return false;

            // Check velocity to avoid stepping while jumping
            if (_rigidbody.velocity.y > 2f)
                return false;

            // Use step ray transforms if available, otherwise create temporary positions
            Vector3 lowerRayOrigin;
            Vector3 upperRayOrigin;

            if (_stepRayLower != null && _stepRayUpper != null)
            {
                lowerRayOrigin = _stepRayLower.position;
                upperRayOrigin = _stepRayUpper.position;
            }
            else
            {
                // Fallback: create ray positions based on player transform
                lowerRayOrigin = _playerTransform.position + Vector3.up * 0.1f;
                upperRayOrigin = _playerTransform.position + Vector3.up * _maxStepHeight;
            }

            // Cast ray forward at foot level
            RaycastHit lowerHit;
            bool hitLower = Physics.Raycast(lowerRayOrigin, moveDirection, out lowerHit, _stepRayDistance, _stepLayerMask);

            if (!hitLower)
                return false; // No obstacle

            // Cast ray forward at step height level
            RaycastHit upperHit;
            bool hitUpper = Physics.Raycast(upperRayOrigin, moveDirection, out upperHit, _stepRayDistance, _stepLayerMask);

            // If lower ray hits but upper ray doesn't, we can step up
            bool canStep = hitLower && !hitUpper;

            // Additional check: make sure the step height is reasonable
            if (canStep)
            {
                float stepHeight = lowerHit.point.y - (_playerTransform.position.y - 0.5f);
                canStep = stepHeight > 0.05f && stepHeight <= _maxStepHeight;
            }

            // Debug visualization
            Debug.DrawRay(lowerRayOrigin, moveDirection * _stepRayDistance, hitLower ? Color.red : Color.green, 0.1f);
            Debug.DrawRay(upperRayOrigin, moveDirection * _stepRayDistance, hitUpper ? Color.red : Color.green, 0.1f);

            return canStep;
        }

        private void StepUp(Vector3 moveDirection, float speed)
        {
            // Set cooldown
            _lastStepTime = Time.time;

            // Reduce current velocity before stepping
            Vector3 currentVel = _rigidbody.velocity;
            currentVel.y = Mathf.Max(0f, currentVel.y); // Prevent negative velocity
            _rigidbody.velocity = currentVel;

            // Apply gentle upward force
            _rigidbody.AddForce(Vector3.up * _stepUpForce, ForceMode.VelocityChange);

            // Apply forward momentum gently
            Vector3 forwardForce = moveDirection * speed * 2f;
            _rigidbody.AddForce(forwardForce, ForceMode.Force);

            Debug.Log($"Step Up Applied - Force: {_stepUpForce}, Forward: {forwardForce.magnitude}");
        }

        // Alternative step climbing method using position adjustment (more like CMF system)
        private void StepUpSmooth(Vector3 moveDirection, float speed)
        {
            // Cast downward from step height to find the top of the step
            Vector3 rayOrigin = _playerTransform.position + Vector3.up * _maxStepHeight + moveDirection * _stepRayDistance;
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, _maxStepHeight, _stepLayerMask))
            {
                // Calculate target position on top of step
                Vector3 targetPosition = new Vector3(_playerTransform.position.x, hit.point.y, _playerTransform.position.z);

                // Smoothly move to target position
                Vector3 adjustmentVelocity = (targetPosition - _playerTransform.position) / _stepSmoothTime;

                // Apply the adjustment velocity
                Vector3 currentVel = _rigidbody.velocity;
                currentVel.y = Mathf.Max(currentVel.y, adjustmentVelocity.y);
                _rigidbody.velocity = currentVel;

                // Continue forward movement
                Vector3 forwardForce = moveDirection * speed * 5f;
                _rigidbody.AddForce(forwardForce, ForceMode.Force);
            }
        }

        public void PickUp()
        {
            Vector3 velocity = _rigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            _rigidbody.velocity = velocity;

            _animator.SetBool("isWalking", false);
            _animator.SetTrigger("isPickUp");
        }

        private void LimitVelocity()
        {
            Vector3 velocity = _rigidbody.velocity;
            if (Mathf.Abs(velocity.y) > _maxJumpVelocity)
            {
                velocity.y = Mathf.Sign(velocity.y) * _maxJumpVelocity;
                _rigidbody.velocity = velocity;
            }
        }

        // Public method to set step climbing parameters
        public void SetStepClimbingParameters(float maxStepHeight, float stepRayDistance, LayerMask stepLayerMask, float stepUpForce)
        {
            _maxStepHeight = maxStepHeight;
            _stepRayDistance = stepRayDistance;
            _stepLayerMask = stepLayerMask;
            _stepUpForce = stepUpForce;
        }

        // Public method to set step ray transforms
        public void SetStepRayTransforms(Transform lowerRay, Transform upperRay)
        {
            _stepRayLower = lowerRay;
            _stepRayUpper = upperRay;
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
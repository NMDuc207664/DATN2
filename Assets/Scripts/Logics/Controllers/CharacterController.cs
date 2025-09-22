using System;
using System.Collections;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class CharacterController : MonoBehaviour
    {
        [Inject]
        private readonly IMovement _movementService;
        [Inject]
        private readonly IInventoryService _inventoryService;
        [Inject]
        private readonly Animator _animator;
        //     [Inject]
        //     private readonly Rigidbody _rigidbody;
        [SerializeField] private GlobalRaycast _raycastDetector;

        //     [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float _maxHorizontalSpeedOnJump = 2f;
        [SerializeField] private float _slopeSlideForce = 2f;
        //     [SerializeField] private float airMultiplier = 0.4f;
        //     [SerializeField] private Transform groundCheck;
        //     [SerializeField] private LayerMask groundMask;
        //     [SerializeField] private PhysicMaterial slipperyMaterial;
        //     [SerializeField] private BoxCollider _groundCheckCollider;

        //     // Stair climbing components
        //     [Header("Stair Climbing")]
        //     [SerializeField] private Transform stepRayLower;
        //     [SerializeField] private Transform stepRayUpper;
        //     [SerializeField] private float maxStepHeight = 0.3f;
        //     [SerializeField] private float stepRayDistance = 0.5f;
        //     [SerializeField] private LayerMask stepLayerMask = -1;
        //     [SerializeField] private float stepUpForce = 3f; // Moved from MovementService

        //     [SerializeField] private bool debugStepRays = false;
        //     [SerializeField] private bool debugInventory = false;

        bool isPickingUp = false;
        //     private Collider _collider;
        //     private PhysicMaterial _defaultMaterial;

        //     private void Awake()
        //     {
        //         _collider = GetComponentInChildren<Collider>();
        //         _defaultMaterial = _collider.material;

        //         // Setup step ray transforms if they don't exist
        //         SetupStepRayTransforms();

        //         // Configure movement service for stair climbing
        //         _movementService.SetStepClimbingParameters(maxStepHeight, stepRayDistance, stepLayerMask, stepUpForce);
        //         _movementService.SetStepRayTransforms(stepRayLower, stepRayUpper);
        //     }

        //     private void SetupStepRayTransforms()
        //     {
        //         // Create step ray transforms if they don't exist
        //         if (stepRayLower == null)
        //         {
        //             GameObject lowerRayGO = new GameObject("StepRayLower");
        //             lowerRayGO.transform.SetParent(this.transform);
        //             lowerRayGO.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        //             stepRayLower = lowerRayGO.transform;
        //         }

        //         if (stepRayUpper == null)
        //         {
        //             GameObject upperRayGO = new GameObject("StepRayUpper");
        //             upperRayGO.transform.SetParent(this.transform);
        //             upperRayGO.transform.localPosition = new Vector3(0f, maxStepHeight, 0f);
        //             stepRayUpper = upperRayGO.transform;
        //         }

        //         // Update upper ray position based on max step height
        //         stepRayUpper.localPosition = new Vector3(stepRayUpper.localPosition.x, maxStepHeight, stepRayUpper.localPosition.z);
        //     }

        //     private void FixedUpdate()
        //     {
        //         HandleMovementInput();
        //     }

        //     private void LateUpdate()
        //     {
        //         if (debugInventory)
        //         {
        //             _inventoryService.DebugPrintInventory();
        //         }

        //         if (debugStepRays)
        //         {
        //             DrawStepDebugRays();
        //         }
        //     }

        private void Update()
        {
            // HandleJumpInput();
            HandlePickupInput();
            _movementService.UpdateDrag(IsGrounded());
        }

        //     private void DrawStepDebugRays()
        //     {
        //         if (stepRayLower == null || stepRayUpper == null) return;

        //         Vector3 forward = transform.forward;

        //         // Draw lower ray
        //         RaycastHit lowerHit;
        //         bool hitLower = Physics.Raycast(stepRayLower.position, forward, out lowerHit, stepRayDistance, stepLayerMask);
        //         Debug.DrawRay(stepRayLower.position, forward * stepRayDistance, hitLower ? Color.red : Color.green);

        //         // Draw upper ray
        //         RaycastHit upperHit;
        //         bool hitUpper = Physics.Raycast(stepRayUpper.position, forward, out upperHit, stepRayDistance, stepLayerMask);
        //         Debug.DrawRay(stepRayUpper.position, forward * stepRayDistance, hitUpper ? Color.red : Color.green);

        //         // Draw step detection info
        //         if (hitLower && !hitUpper)
        //         {
        //             Debug.DrawLine(stepRayLower.position, lowerHit.point, Color.yellow);
        //             Debug.DrawLine(lowerHit.point, lowerHit.point + Vector3.up * maxStepHeight, Color.cyan);
        //         }
        //     }

        private void HandlePickupInput()
        {
            if (_raycastDetector == null || _raycastDetector.DetectedItem == null) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                var pickup = _raycastDetector.DetectedItem;
                if (_raycastDetector.DetectedItem.itemData.interactTypes.Contains(InteractType.Pickable) && !isPickingUp)
                {
                    StartCoroutine(DoPickup());
                }
                if (_raycastDetector.DetectedItem.itemData.interactTypes.Contains(InteractType.Interatable))
                {
                    pickup.OnInspected();
                }
                if (_raycastDetector.DetectedItem.itemData.interactTypes.Contains(InteractType.Door))
                {
                    pickup.OnInteracted();
                }
            }
        }

        private IEnumerator DoPickup()
        {
            isPickingUp = true;

            var pickup = _raycastDetector.DetectedItem;
            var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
            Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added._amount}");
            pickup.OnPickedUp();

            _movementService.PickUp();
            yield return new WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"));

            isPickingUp = false;
        }

        //     private void HandleMovementInput()
        //     {
        //         if (isPickingUp || _animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"))
        //         {
        //             return;
        //         }

        //         float moveX = Input.GetAxisRaw("Horizontal");
        //         float moveZ = Input.GetAxisRaw("Vertical");
        //         Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        //         GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed, IsGrounded(), airMultiplier);
        //     }

        // private void HandleJumpInput()
        // {
        //     if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        //     {
        //         GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Jump), jumpHeight);
        //     }
        // }

        //     private bool IsGrounded()
        //     {
        //         if (_groundCheckCollider == null) return false;

        //         Vector3 boxCenter = groundCheck.position + _groundCheckCollider.center;
        //         Vector3 boxHalfExtents = _groundCheckCollider.size * 0.5f * groundCheck.localScale.y;
        //         Quaternion boxRotation = groundCheck.rotation;

        //         Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation, groundMask);
        //         bool isGrounded = hits.Length > 0;

        //         if (isGrounded)
        //         {
        //             RaycastHit hit;
        //             float checkDistance = boxHalfExtents.y + 0.1f * transform.localScale.y;
        //             if (Physics.Raycast(boxCenter, Vector3.down, out hit, checkDistance, groundMask))
        //             {
        //                 float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        //                 return slopeAngle < 45f;
        //             }
        //         }

        //         return isGrounded;
        //     }

        //     private void OnCollisionStay(Collision collision)
        //     {
        //         foreach (var contact in collision.contacts)
        //         {
        //             float dot = Vector3.Dot(contact.normal, Vector3.up);
        //             if (dot < 0.5f) // steep wall > ~60°
        //             {
        //                 _collider.material = slipperyMaterial;
        //                 return;
        //             }
        //         }
        //         _collider.material = _defaultMaterial;
        //     }

        //     private void OnCollisionExit(Collision collision)
        //     {
        //         _collider.material = _defaultMaterial;
        //     }

        //     // Public method to update step climbing parameters during runtime
        //     public void UpdateStepClimbingSettings(float newMaxStepHeight, float newStepRayDistance, float newStepUpForce)
        //     {
        //         maxStepHeight = newMaxStepHeight;
        //         stepRayDistance = newStepRayDistance;
        //         stepUpForce = newStepUpForce;

        //         // Update step ray positions
        //         if (stepRayUpper != null)
        //         {
        //             stepRayUpper.localPosition = new Vector3(stepRayUpper.localPosition.x, maxStepHeight, stepRayUpper.localPosition.z);
        //         }

        //         _movementService.SetStepClimbingParameters(maxStepHeight, stepRayDistance, stepLayerMask, stepUpForce);
        //     }
        [Inject]
        private readonly Rigidbody _rigidbody;
        // [Inject]
        // private readonly Collider _collider;
        private CapsuleCollider _capsuleCollider;
        [SerializeField]
        CharacterRegisterManager _characterRegisterManager;
        Vector3 _playerMoveInput = Vector3.zero;
        [SerializeField] float _moveInputMultiplier = 30.0f;
        [SerializeField] float _runMultiplier = 2.0f;
        [SerializeField]
        bool _isGrounded = true;
        [SerializeField][Range(0.0f, 2.0f)] float _groundCheckRadiusMultiplier = 1.0f;
        [SerializeField][Range(-0.9f, 1.05f)] float _groundCheckDistance = 0.05f;
        RaycastHit _groundCheckHit = new RaycastHit();

        [SerializeField]
        float _gravityFallCurrent = -100.0f;
        [SerializeField]
        float _gravityFallMin = -100.0f;
        [SerializeField]
        float _gravityFallMax = -500.0f;
        [SerializeField][Range(-5.0f, -35.0f)] float _gravityFallIncreaseAmount = -20.0f;
        [SerializeField] float _gravityIncreaseTime = 0.05f;
        [SerializeField] float _playerFallTimer = 0.0f;
        [SerializeField] float _gravity = 0.0f;
        [SerializeField] float _maxSlopeAngle = 47.5f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private BoxCollider _groundCheckCollider;


        [Header("Jumping")]
        [SerializeField] float _jumpForceMultiplier = 750.0f;
        [SerializeField] float _continualJumpForceMultiplier = 0.1f;
        [SerializeField] float _jumpTime = 0.175f;
        [SerializeField] float _jumpTimeCounter = 0.0f;
        [SerializeField] float _coyoteTime = 0.15f;
        [SerializeField] float _coyoteTimeCounter = 0.0f;
        [SerializeField] float _jumpBufferTime = 0.2f;
        [SerializeField] float _jumpBufferTimeCounter = 0.0f;
        [SerializeField] bool _playerIsJumping = false;
        [SerializeField] bool _jumpWasPressedLastFrame = false;

        private void Awake()
        {
            _capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        }
        // private void FixedUpdate()
        // {
        //     if (_capsuleCollider == null)
        //     {
        //         Debug.Log("CapsuleCollider not found!");
        //     }
        //     _playerMoveInput = GetMoveInput();
        //     _isGrounded = IsGrounded();
        //     _playerMoveInput.y = PlayerGravity();
        //     _playerMoveInput.y = PlayerJump();
        //     _playerMoveInput = PlayerMove();
        //     _playerMoveInput = PlayerRun();
        //     _playerMoveInput = PlayerSlope();

        //     _playerMoveInput *= _rigidbody.mass;
        //     _rigidbody.AddRelativeForce(_playerMoveInput, ForceMode.Force);
        // }


        // private Vector3 PlayerMove()
        // {
        //     Vector3 calculatorPlayerMove = (new Vector3(_playerMoveInput.x * _moveInputMultiplier, _playerMoveInput.y, _playerMoveInput.z * _moveInputMultiplier));
        //     return calculatorPlayerMove;
        // }

        private Vector3 GetMoveInput()
        {
            return new Vector3(_characterRegisterManager.MoveInput.x, 0.0f, _characterRegisterManager.MoveInput.y);
        }
        // private bool GroundCheck()
        // {
        //     float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
        //     float sphereCastTravelDistance = _capsuleCollider.bounds.extents.y - sphereCastRadius + _groundCheckDistance;
        //     return Physics.SphereCast(_rigidbody.position, sphereCastRadius, Vector3.down, out _groundCheckHit, sphereCastTravelDistance);
        // }
        private bool IsGrounded()
        {
            if (_groundCheckCollider == null) return false;

            Vector3 boxCenter = groundCheck.position + _groundCheckCollider.center;
            Vector3 boxHalfExtents = _groundCheckCollider.size * 0.5f * groundCheck.localScale.y;
            Quaternion boxRotation = groundCheck.rotation;

            Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation, groundMask);
            bool isGrounded = hits.Length > 0;

            if (isGrounded)
            {
                RaycastHit hit;
                float checkDistance = boxHalfExtents.y + 0.1f * transform.localScale.y;
                if (Physics.Raycast(boxCenter, Vector3.down, out hit, checkDistance, groundMask))
                {
                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                    return slopeAngle < 45f;
                }
            }

            return isGrounded;
        }

        // private Vector3 PlayerSlope()
        // {
        //     Vector3 calculatedPlayerMovement = _playerMoveInput;
        //     if (_isGrounded)
        //     {
        //         Vector3 localGroundCheckHitNormal = _rigidbody.transform.InverseTransformDirection(_groundCheckHit.normal);
        //         float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, _rigidbody.transform.up);
        //         if (!(groundSlopeAngle == 0.0f))
        //         {
        //             Quaternion slopeAngleRotation = Quaternion.FromToRotation(_rigidbody.transform.up, localGroundCheckHitNormal);
        //             calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;
        //         }
        //         Debug.DrawRay(_rigidbody.position, _rigidbody.transform.TransformDirection(calculatedPlayerMovement), Color.red, 0.5f);
        //     }
        //     return calculatedPlayerMovement;
        // }
        // private Vector3 PlayerSlope()
        // {
        //     Vector3 calculatedPlayerMovement = _playerMoveInput;
        //     if (_isGrounded)
        //     {
        //         Vector3 localGroundCheckHitNormal = _rigidbody.transform.InverseTransformDirection(_groundCheckHit.normal);
        //         float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, _rigidbody.transform.up);
        //         if (!(groundSlopeAngle == 0.0f))
        //         {
        //             Quaternion slopeAngleRotation = Quaternion.FromToRotation(_rigidbody.transform.up, localGroundCheckHitNormal);
        //             calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;
        //             float relativeSlopeAngle = Vector3.Angle(calculatedPlayerMovement, _rigidbody.transform.up) - 90.0f;
        //             calculatedPlayerMovement += calculatedPlayerMovement * (relativeSlopeAngle / _maxSlopeAngle);

        //             if (groundSlopeAngle > _maxSlopeAngle)
        //             {
        //                 calculatedPlayerMovement.y = groundSlopeAngle * -0.7f;
        //             }
        //         }
        //     }
        //     return calculatedPlayerMovement;
        // }
        // Sửa PlayerSlope để nhận parameter và chỉ xử lý horizontal
        // private Vector3 PlayerSlope(Vector3 movement)
        // {
        //     Vector3 calculatedPlayerMovement = movement;
        //     if (_isGrounded)
        //     {
        //         Vector3 localGroundCheckHitNormal = _rigidbody.transform.InverseTransformDirection(_groundCheckHit.normal);
        //         float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, _rigidbody.transform.up);
        //         if (!(groundSlopeAngle == 0.0f))
        //         {
        //             Quaternion slopeAngleRotation = Quaternion.FromToRotation(_rigidbody.transform.up, localGroundCheckHitNormal);
        //             calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;

        //             // Chỉ áp dụng slope effect cho horizontal movement
        //             float relativeSlopeAngle = Vector3.Angle(new Vector3(calculatedPlayerMovement.x, 0, calculatedPlayerMovement.z), Vector3.up) - 90.0f;
        //             Vector3 slopeEffect = new Vector3(calculatedPlayerMovement.x, 0, calculatedPlayerMovement.z) * (relativeSlopeAngle / _maxSlopeAngle);
        //             calculatedPlayerMovement.x += slopeEffect.x;
        //             calculatedPlayerMovement.z += slopeEffect.z;

        //             if (groundSlopeAngle > _maxSlopeAngle)
        //             {
        //                 // Giảm tác động của steep slope
        //                 calculatedPlayerMovement.x *= 0.3f;
        //                 calculatedPlayerMovement.z *= 0.3f;
        //             }
        //         }
        //     }
        //     return calculatedPlayerMovement;
        // }

        private float PlayerGravity()
        {
            if (_isGrounded)
            {
                _gravity = 0.0f;
                _gravityFallCurrent = _gravityFallMin;
            }
            else
            {
                _playerFallTimer -= Time.fixedDeltaTime;
                if (_playerFallTimer < 0.0f)
                {
                    if (_gravityFallCurrent > _gravityFallMax)
                    {
                        _gravityFallCurrent += _gravityFallIncreaseAmount;
                    }
                    _playerFallTimer = _gravityIncreaseTime;
                    _gravity = _gravityFallCurrent;
                }
            }
            return _gravity;
        }

        private Vector3 PlayerRun()
        {
            Vector3 calculatedPlayerRunSpeed = _playerMoveInput;
            if (_characterRegisterManager.RunIsPress)
            {
                calculatedPlayerRunSpeed.x *= _runMultiplier;
                calculatedPlayerRunSpeed.z *= _runMultiplier;
            }
            return calculatedPlayerRunSpeed;
        }
        private void DrawGroundCheckGizmos()
        {
            if (_capsuleCollider == null || _rigidbody == null)
                return;

            float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
            float sphereCastTravelDistance = _capsuleCollider.bounds.extents.y - sphereCastRadius + _groundCheckDistance;

            Vector3 origin = _rigidbody.position;
            Vector3 end = origin + Vector3.down * sphereCastTravelDistance;

            Gizmos.color = _isGrounded ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(origin, sphereCastRadius);
            Gizmos.DrawWireSphere(end, sphereCastRadius);
            Gizmos.DrawLine(origin, end);
        }
        private void OnDrawGizmos()
        {
            DrawGroundCheckGizmos();
        }

        // private float PlayerJump()
        // {
        //     float calculateJumpInput = _playerMoveInput.y;
        //     SetJumpTimeCounter();
        //     SetCoyoteTimeCounter();
        //     SetJumpBufferCouter();

        //     if (_jumpBufferTimeCounter > 0.0f && !_playerIsJumping && _coyoteTimeCounter > 0.0f)
        //     {
        //         calculateJumpInput = _jumpForceMultiplier;
        //         _playerIsJumping = true;
        //         _jumpBufferTimeCounter = 0.0f;
        //         _coyoteTimeCounter = 0.0f;
        //     }
        //     else if (_characterRegisterManager.JumpIsPress && _playerIsJumping && !_isGrounded && _jumpTimeCounter > 0.0f)
        //     {
        //         calculateJumpInput = _jumpForceMultiplier * _continualJumpForceMultiplier;
        //     }
        //     else if (_playerIsJumping && _isGrounded)
        //     {
        //         _playerIsJumping = false;
        //     }
        //     return calculateJumpInput;
        // }
        // private float PlayerJump()
        // {
        //     float calculateJumpInput = 0f;
        //     SetJumpTimeCounter();
        //     SetCoyoteTimeCounter();
        //     SetJumpBufferCouter();

        //     // Initial jump - chỉ cần nhấn jump một lần
        //     if (_jumpBufferTimeCounter > 0.0f && !_playerIsJumping && _coyoteTimeCounter > 0.0f)
        //     {
        //         calculateJumpInput = _jumpForceMultiplier;
        //         _playerIsJumping = true;
        //         _jumpBufferTimeCounter = 0.0f;
        //         _coyoteTimeCounter = 0.0f;

        //         // Giới hạn horizontal velocity khi jump để tránh phóng xa
        //         Vector3 currentVelocity = _rigidbody.velocity;
        //         Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        //         float maxHorizontalSpeed = 12f; // Giới hạn tốc độ ngang khi jump

        //         if (horizontalVelocity.magnitude > maxHorizontalSpeed)
        //         {
        //             Vector3 limitedHorizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;
        //             _rigidbody.velocity = new Vector3(limitedHorizontalVelocity.x, currentVelocity.y, limitedHorizontalVelocity.z);
        //         }
        //     }
        //     // Variable jump height - không cần JumpIsPress vì nó chỉ true 1 frame
        //     // Thay vào đó dùng jumpTimeCounter để cho phép jump cao hơn
        //     else if (_playerIsJumping && !_isGrounded && _jumpTimeCounter > 0.0f)
        //     {
        //         calculateJumpInput = _jumpForceMultiplier * _continualJumpForceMultiplier * 0.5f; // Giảm continual jump force
        //     }
        //     else if (_playerIsJumping && _isGrounded)
        //     {
        //         _playerIsJumping = false;
        //     }
        //     return calculateJumpInput;
        // }

        private void SetJumpBufferCouter()
        {
            if (!_jumpWasPressedLastFrame && _characterRegisterManager.JumpIsPress)
            {
                _jumpBufferTimeCounter = _jumpBufferTime;
            }
            else if (_jumpBufferTimeCounter > 0.0f)
            {
                _jumpBufferTimeCounter -= Time.fixedDeltaTime;
            }
            _jumpWasPressedLastFrame = _characterRegisterManager.JumpIsPress;
        }

        private void SetCoyoteTimeCounter()
        {
            if (_isGrounded)
            {
                _coyoteTimeCounter = _coyoteTime;
            }
            else
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
            }
        }

        private void SetJumpTimeCounter()
        {
            if (_playerIsJumping && !_isGrounded)
            {
                _jumpTimeCounter -= Time.fixedDeltaTime;
            }
            else
            {
                _jumpTimeCounter = _jumpTime;
            }
        }
        private void FixedUpdate()
        {
            if (_capsuleCollider == null)
            {
                Debug.Log("CapsuleCollider not found!");
                return;
            }

            Vector3 horizontalInput = GetMoveInput();
            _isGrounded = IsGrounded();

            // Xử lý movement ngang (X, Z)
            Vector3 horizontalMovement = PlayerMove(horizontalInput);
            horizontalMovement = PlayerRun(horizontalMovement);
            horizontalMovement = PlayerSlope(horizontalMovement);

            // Xử lý gravity và jump (Y)
            float verticalForce = PlayerGravity() + PlayerJump();

            // Apply forces riêng biệt
            Vector3 finalHorizontalForce = new Vector3(horizontalMovement.x, 0, horizontalMovement.z) * _rigidbody.mass;
            Vector3 finalVerticalForce = new Vector3(0, verticalForce, 0) * _rigidbody.mass;

            _rigidbody.AddRelativeForce(finalHorizontalForce, ForceMode.Force);
            _rigidbody.AddForce(finalVerticalForce, ForceMode.Force); // đổi sang world force

            // Thêm drag cho horizontal movement khi grounded để giảm trượt
            if (_isGrounded && horizontalInput.magnitude < 0.1f)
            {
                Vector3 horizontalVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
                _rigidbody.AddForce(-horizontalVelocity * 10f * _rigidbody.mass, ForceMode.Force);
            }

            // Thêm slope sliding effect
            ApplySlopeSliding();
        }

        // Sửa PlayerMove để chỉ xử lý horizontal movement
        private Vector3 PlayerMove(Vector3 input)
        {
            return new Vector3(input.x * _moveInputMultiplier, 0, input.z * _moveInputMultiplier);
        }

        // Sửa PlayerRun để nhận parameter
        private Vector3 PlayerRun(Vector3 movement)
        {
            Vector3 calculatedPlayerRunSpeed = movement;
            if (_characterRegisterManager.RunIsPress)
            {
                calculatedPlayerRunSpeed.x *= _runMultiplier;
                calculatedPlayerRunSpeed.z *= _runMultiplier;
            }
            return calculatedPlayerRunSpeed;
        }

        // Sửa PlayerSlope để nhận parameter và chỉ xử lý horizontal
        private Vector3 PlayerSlope(Vector3 movement)
        {
            Vector3 calculatedPlayerMovement = movement;
            if (_isGrounded)
            {
                Vector3 localGroundCheckHitNormal = _rigidbody.transform.InverseTransformDirection(_groundCheckHit.normal);
                float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, _rigidbody.transform.up);
                if (!(groundSlopeAngle == 0.0f))
                {
                    Quaternion slopeAngleRotation = Quaternion.FromToRotation(_rigidbody.transform.up, localGroundCheckHitNormal);
                    calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;

                    // Chỉ áp dụng slope effect cho horizontal movement
                    float relativeSlopeAngle = Vector3.Angle(new Vector3(calculatedPlayerMovement.x, 0, calculatedPlayerMovement.z), Vector3.up) - 90.0f;
                    Vector3 slopeEffect = new Vector3(calculatedPlayerMovement.x, 0, calculatedPlayerMovement.z) * (relativeSlopeAngle / _maxSlopeAngle);
                    calculatedPlayerMovement.x += slopeEffect.x;
                    calculatedPlayerMovement.z += slopeEffect.z;

                    if (groundSlopeAngle > _maxSlopeAngle)
                    {
                        // Giảm tác động của steep slope
                        calculatedPlayerMovement.x *= 0.3f;
                        calculatedPlayerMovement.z *= 0.3f;
                    }
                }
            }
            return calculatedPlayerMovement;
        }

        // Sửa PlayerJump để giảm force khi đang di chuyển ngang  
        private float PlayerJump()
        {
            float calculateJumpInput = 0f;
            SetJumpTimeCounter();
            SetCoyoteTimeCounter();
            SetJumpBufferCouter();

            // Initial jump - chỉ cần nhấn jump một lần
            if (_jumpBufferTimeCounter > 0.0f && !_playerIsJumping && _coyoteTimeCounter > 0.0f)
            {
                calculateJumpInput = _jumpForceMultiplier;
                _playerIsJumping = true;
                _jumpBufferTimeCounter = 0.0f;
                _coyoteTimeCounter = 0.0f;

                // Giới hạn horizontal velocity khi jump để tránh phóng xa
                Vector3 currentVelocity = _rigidbody.velocity;
                Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

                if (horizontalVelocity.magnitude > _maxHorizontalSpeedOnJump)
                {
                    Vector3 limitedHorizontalVelocity = horizontalVelocity.normalized * _maxHorizontalSpeedOnJump;
                    _rigidbody.velocity = new Vector3(limitedHorizontalVelocity.x, currentVelocity.y, limitedHorizontalVelocity.z);
                }
            }
            // Variable jump height - không cần JumpIsPress vì nó chỉ true 1 frame
            // Thay vào đó dùng jumpTimeCounter để cho phép jump cao hơn
            else if (_playerIsJumping && !_isGrounded && _jumpTimeCounter > 0.0f)
            {
                calculateJumpInput = _jumpForceMultiplier * _continualJumpForceMultiplier * 0.5f; // Giảm continual jump force
            }
            else if (_playerIsJumping && _isGrounded)
            {
                _playerIsJumping = false;
            }
            return calculateJumpInput;
        }

        // Thêm method mới để xử lý slope sliding
        private void ApplySlopeSliding()
        {
            if (_isGrounded && GetMoveInput().magnitude < 0.1f) // Chỉ slide khi không di chuyển
            {
                RaycastHit hit;
                float checkDistance = 1f;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundMask))
                {
                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                    if (slopeAngle > 10f && slopeAngle <= _maxSlopeAngle)
                    {
                        // Tính toán slide force dựa trên góc dốc
                        float slideForceMultiplier = (slopeAngle - 10f) / (_maxSlopeAngle - 10f);
                        Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                        Vector3 slideForce = slideDirection * _slopeSlideForce * slideForceMultiplier * _rigidbody.mass;

                        _rigidbody.AddForce(slideForce, ForceMode.Force);
                    }
                }
            }
        }
    }
}
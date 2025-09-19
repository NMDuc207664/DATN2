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
        [Inject]
        private readonly Rigidbody _rigidbody;
        [SerializeField] private GlobalRaycast _raycastDetector;

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float airMultiplier = 0.4f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private PhysicMaterial slipperyMaterial;
        [SerializeField] private BoxCollider _groundCheckCollider; // Reference to BoxCollider

        [SerializeField] GameObject stepRayUpper;
        [SerializeField] GameObject stepRayLower;
        [SerializeField] float stepHeight = 0.3f;
        [SerializeField] float stepSmooth = 2f;

        bool isPickingUp = false;
        private Collider _collider;
        private PhysicMaterial _defaultMaterial;


        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            // // mặc định không gán material (dùng friction mặc định của Unity)
            // _collider.material = null;
            _defaultMaterial = _collider.material;
            //stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
        }
        private void FixedUpdate()
        {
            HandleMovementInput();
            StepClimb();

        }
        private void Update()
        {
            HandleJumpInput();

            HandlePickupInput();
            //MoveCamera();
            _movementService.UpdateDrag(IsGrounded());
            Debug.Log(IsGrounded());
            _inventoryService.DebugPrintInventory();
        }
        // private void LateUpdate()
        // {

        // }
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

            // Thực hiện logic nhặt item ngay khi bắt đầu
            var pickup = _raycastDetector.DetectedItem;
            var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
            Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added._amount}");
            pickup.OnPickedUp();

            // Chờ animation "Interact" hoàn tất
            _movementService.PickUp();
            yield return new WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"));

            // Cho phép di chuyển lại sau khi animation hoàn tất
            isPickingUp = false;

        }
        private void HandleMovementInput()
        {
            if (isPickingUp || _animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"))
            {
                return;
            }
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

            // if (direction.magnitude >= 0.1f)
            // {
            GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed, IsGrounded(), airMultiplier);
            //}
        }
        private void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Jump), jumpHeight);
            }
        }
        // private bool IsGrounded()
        // {
        //     bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //     // Thêm kiểm tra raycast để đảm bảo bề mặt không quá dốc
        //     if (isGrounded)
        //     {
        //         RaycastHit hit;
        //         if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance + 0.1f, groundMask))
        //         {
        //             float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        //             // Chỉ coi là mặt đất nếu góc nghiêng < 45 độ
        //             return slopeAngle < 45f;
        //         }
        //     }
        //     return isGrounded;
        // }
        private bool IsGrounded()
        {
            if (_groundCheckCollider == null) return false;

            // Get BoxCollider's world position and size
            Vector3 boxCenter = groundCheck.position + _groundCheckCollider.center;
            Vector3 boxHalfExtents = _groundCheckCollider.size * 0.5f * groundCheck.localScale.y; // Adjust for scale
            Quaternion boxRotation = groundCheck.rotation;

            // Check for overlapping colliders
            Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation, groundMask);
            bool isGrounded = hits.Length > 0;

            Debug.Log($"Ground Check | Hits: {hits.Length} | Box Center: {boxCenter} | Half Extents: {boxHalfExtents}");

            // Slope check using raycast from box center downward
            if (isGrounded)
            {
                RaycastHit hit;
                float checkDistance = boxHalfExtents.y + 0.1f * transform.localScale.y; // Scale-adjusted
                if (Physics.Raycast(boxCenter, Vector3.down, out hit, checkDistance, groundMask))
                {
                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                    Debug.Log($"Slope Angle: {slopeAngle} | Hit Point: {hit.point}");
                    return slopeAngle < 45f; // Only ground if slope < 45°
                }
            }

            return isGrounded;
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                // Nếu mặt phẳng gần đứng (dot với Vector3.up nhỏ)
                float dot = Vector3.Dot(contact.normal, Vector3.up);
                if (dot < 0.5f) // tường dốc > ~60°
                {
                    _collider.material = slipperyMaterial;
                    return;
                }
            }

            // Nếu không có tiếp xúc với tường đứng thì quay về mặc định
            _collider.material = _defaultMaterial;
        }

        private void OnCollisionExit(Collision collision)
        {
            // Rời khỏi va chạm -> reset về mặc định
            _collider.material = _defaultMaterial;
        }
        // private void SpeedControl()
        // {
        //     Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //     // limit velocity if needed
        //     if (flatVel.magnitude > moveSpeed)
        //     {
        //         Vector3 limitedVel = flatVel.normalized * moveSpeed;
        //         rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        //     }
        // }
        // --- Hàm StepClimb mới ---
        void StepClimb()
        {
            RaycastHit hitLower;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
            {
                RaycastHit hitUpper;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
                {
                    _rigidbody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }

            RaycastHit hitLower45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
            {

                RaycastHit hitUpper45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
                {
                    _rigidbody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }

            RaycastHit hitLowerMinus45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
            {

                RaycastHit hitUpperMinus45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
                {
                    _rigidbody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }
        }
    }
}
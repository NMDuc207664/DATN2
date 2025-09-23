using System;
using System.Collections;
using CMF;
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
        [SerializeField] private GlobalRaycast _raycastDetector;

        // [SerializeField] private float moveSpeed = 5f;
        // [SerializeField] private float jumpHeight = 2f;
        // [SerializeField] private float airMultiplier = 0.4f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private PhysicMaterial slipperyMaterial;
        [SerializeField] private float maxStepHeight = 1f;
        public bool _isStair;
        private bool isPickingUp = false;

        private Collider _collider;
        private PhysicMaterial _defaultMaterial;


        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            // // mặc định không gán material (dùng friction mặc định của Unity)
            // _collider.material = null;
            _defaultMaterial = _collider.material;
            // _movementService.ConfigureStep(maxStepHeight, groundMask);
        }
        private void FixedUpdate()
        {

        }
        private void Update()
        {
            //HandleJumpInput();

            HandlePickupInput();
            _isStair = IsStair();
            // _movementService.UpdateDrag(IsGrounded());
            // Debug.Log(isPickingUp);
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

            var pickup = _raycastDetector.DetectedItem;
            var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
            Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added._amount}");
            pickup.OnPickedUp();

            _movementService.PickUp();
            yield return new WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"));

            isPickingUp = false;
        }

        // Thêm method mới để xử lý input di chuyển với step down
        // private void HandleMovementInput()
        // {
        //     if (isPickingUp || _animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"))
        //     {
        //         return;
        //     }

        //     float moveX = Input.GetAxisRaw("Horizontal");
        //     float moveZ = Input.GetAxisRaw("Vertical");
        //     Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        //     GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed, _isStair, airMultiplier);
        // }
        // private void StickToStairs()
        // {
        //     // Vị trí bắt đầu raycast (chút xíu trên chân)
        //     Vector3 origin = groundCheck.position + Vector3.up * 0.1f;

        //     // Độ dài ray để check cầu thang phía dưới
        //     float stairSnapDistance = maxStepHeight + 0.2f;

        //     if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, stairSnapDistance, groundMask))
        //     {
        //         float distanceToGround = hit.distance;

        //         // Nếu khoảng cách nhỏ hơn maxStepHeight thì snap xuống
        //         if (distanceToGround > 0.05f && distanceToGround <= maxStepHeight)
        //         {
        //             // Dịch chuyển xuống ngay sát mặt stair (giữ lại một khoảng nhỏ để tránh kẹt collider)
        //             Vector3 targetPos = new Vector3(transform.position.x, hit.point.y + 0.02f, transform.position.z);
        //             transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * 10f);
        //         }
        //     }
        // }

        // private void HandleMovementInput()
        // {
        //     if (isPickingUp || _animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"))
        //     {
        //         return;
        //     }
        //     float moveX = Input.GetAxisRaw("Horizontal");
        //     float moveZ = Input.GetAxisRaw("Vertical");
        //     Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        //     // if (direction.magnitude >= 0.1f)
        //     // {
        //     GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed, IsGrounded(), airMultiplier);
        //     //}
        // }
        // private void HandleJumpInput()
        // {
        //     if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        //     {
        //         GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Jump), jumpHeight);
        //     }
        // }
        private bool IsStair()
        {
            return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
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


    }
}
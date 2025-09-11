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
        private readonly ICameraService _cameraService;
        [SerializeField] private GlobalRaycast _raycastDetector;

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float sensitivity = 100f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private GameObject head;
        [SerializeField] private PhysicMaterial slipperyMaterial;


        private bool isPickingUp = false;
        // private float _xRotation = 0f;
        // private bool isMenuOpen = false;
        private Collider _collider;
        private PhysicMaterial _defaultMaterial;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            // // mặc định không gán material (dùng friction mặc định của Unity)
            // _collider.material = null;
            _defaultMaterial = _collider.material;
        }
        private void FixedUpdate()
        {
            HandleMovementInput();
            HandleCameraInput();

        }
        private void Update()
        {
            HandleJumpInput();

            HandlePickupInput();
            _movementService.UpdateDrag(IsGrounded());
        }
        private void LateUpdate()
        {

        }
        private void HandlePickupInput()
        {
            if (_raycastDetector == null || _raycastDetector.DetectedItem == null) return;

            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     var pickup = _raycastDetector.DetectedItem;
            //     var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
            //     Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added._amount}");
            //     pickup.OnPickedUp();
            // }
            if (Input.GetKeyDown(KeyCode.E) && !isPickingUp)
            {
                StartCoroutine(DoPickup());
            }
        }
        private IEnumerator DoPickup()
        {
            isPickingUp = true;

            // Ngừng di chuyển khi nhặt

            // Thực hiện logic nhặt item ngay khi bắt đầu
            var pickup = _raycastDetector.DetectedItem;
            var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
            Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added._amount}");
            pickup.OnPickedUp();

            // Chờ hết animation
            yield return StartCoroutine(_movementService.PickUp());

            // Chỉ khi anim kết thúc mới cho di chuyển lại
            isPickingUp = false;

        }
        private void HandleMovementInput()
        {
            if (isPickingUp) return;
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

            // if (direction.magnitude >= 0.1f)
            // {
            GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed);
            //}
        }
        private void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Jump), jumpHeight);
            }
        }
        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        private void HandleCameraInput()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            GameStateInvoker.TryInvoke(_cameraService, nameof(_cameraService.RotateCamera), mouseX, mouseY, sensitivity);
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
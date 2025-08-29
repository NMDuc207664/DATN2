using System;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
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
        private float _xRotation = 0f;

        private void Update()
        {
            HandleMovementInput();
            HandleJumpInput();
            HandleCameraInput();
            HandlePickupInput();
        }

        private void HandlePickupInput()
        {
            if (_raycastDetector == null || _raycastDetector.DetectedItem == null) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                var pickup = _raycastDetector.DetectedItem;
                var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
                Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added.amount}");
                pickup.OnPickedUp();
            }
        }

        private void HandleMovementInput()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

            if (direction.magnitude >= 0.1f)
            {
                GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Move), direction, moveSpeed);
            }
        }
        private void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameStateInvoker.TryInvoke(_movementService, nameof(_movementService.Jump), jumpHeight);
            }
        }
        private void HandleCameraInput()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            GameStateInvoker.TryInvoke(_cameraService, nameof(_cameraService.RotateCamera), _xRotation, mouseX, mouseY, sensitivity);
            // gọi service xoay camera
            // _cameraService.RotateCamera(ref _xRotation, mouseX, mouseY, sensitivity);
        }
    }
}
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
        [SerializeField] private GameObject _arm;

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
            _arm.SetActive(false);
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
                    //StartCoroutine(DoPickup());
                    DoPickup();
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

        private void DoPickup()
        {
            isPickingUp = true;

            var pickup = _raycastDetector.DetectedItem;
            var added = _inventoryService.AddItem(pickup.itemData, pickup.amount);
            Debug.Log($"[Inventory] Nhặt {pickup.amount} {pickup.itemData.itemName}. Tổng: {added._amount}");
            pickup.OnPickedUp();
            _arm.SetActive(true);
            _movementService.PickUp();
            // yield return new WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"));
            // _arm.SetActive(false);
            isPickingUp = false;
        }

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
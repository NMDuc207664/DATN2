
using System.Collections;
using UnityEngine;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Interface;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class DoorInteraction : MonoBehaviour
    {
        [Header("Door Settings")]
        public ItemModel doorData;
        public bool isDoubleDoor;
        public Transform secondDoor;
        public DoorInteraction masterDoor; // thêm tham chiếu tới cửa gốc
        public float openAngle = 90f;
        public float openSpeed = 2f;

        [Tooltip("Nếu > 0 thì cửa sẽ tự đóng sau X giây")]
        public float autoCloseDelay = 0f;
        public bool isOpen = false;
        public string sceneName;
        public Transform pivotPoint;

        private Quaternion closedRotation;
        private Quaternion openRotation;
        private Quaternion secondDoorClosedRotation;
        private Quaternion secondDoorOpenRotation;

        private Coroutine currentCoroutine;

        // [Inject] private IInventoryService inventoryService;
        private IInventoryService inventoryService;
        private ISceneService _sceneService;


        void Start()
        {
            closedRotation = transform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

            if (isDoubleDoor && secondDoor != null)
            {
                secondDoorClosedRotation = secondDoor.localRotation;
                secondDoorOpenRotation = secondDoorClosedRotation * Quaternion.Euler(0, -openAngle, 0);
            }
            if (_sceneService == null)
            {
                Debug.LogError("[DoorInteraction] _sceneService is null! Check VContainer setup.");
            }
        }
        public void SetInventoryService(IInventoryService service, ISceneService sceneService)
        {
            inventoryService = service;
            _sceneService = sceneService;
            Debug.Log($"[DoorInteraction] InventoryService injected for {gameObject.name}");
        }


        public void Interact()
        {
            // Nếu là cửa phụ thì redirect về masterDoor
            if (masterDoor != null)
            {
                masterDoor.Interact();
                return;
            }

            if (doorData.requiresKey)
            {
                if (inventoryService != null && inventoryService.HasItem(doorData.requiredKey))
                {
                    ToggleDoor();
                }
                else
                {
                    Debug.Log($"[Door] Cần chìa khóa {doorData.requiredKey.itemName} để mở {doorData.itemName}");
                }
            }
            else
            {
                ToggleDoor();
            }
            if (sceneName != "" && isOpen)
            {
                StartCoroutine(LoadScene());
            }
        }

        private void ToggleDoor()
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(AnimateDoor());
        }
        public IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(3f);
            _sceneService.LoadScene(sceneName);
        }
        private IEnumerator AnimateDoor()
        {
            Quaternion targetRotation = isOpen ? closedRotation : openRotation;
            Quaternion secondDoorTargetRotation = isOpen ? secondDoorClosedRotation : secondDoorOpenRotation;
            isOpen = !isOpen;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                if (pivotPoint != null)
                {
                    pivotPoint.localRotation = Quaternion.Slerp(pivotPoint.localRotation, targetRotation, t);
                }
                else if (pivotPoint == null)
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
                }

                // transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);

                if (isDoubleDoor && secondDoor != null)
                    secondDoor.localRotation = Quaternion.Slerp(secondDoor.localRotation, secondDoorTargetRotation, t);

                yield return null;
            }

            if (pivotPoint != null)
            {
                pivotPoint.localRotation = targetRotation;
            }
            else
            {
                transform.localRotation = targetRotation;
            }

            if (isDoubleDoor && secondDoor != null)
                secondDoor.localRotation = secondDoorTargetRotation;
            // Nếu cửa vừa mở và có autoCloseDelay > 0 thì chờ và đóng
            if (isOpen && autoCloseDelay > 0f)
            {
                yield return new WaitForSeconds(autoCloseDelay);
                ToggleDoor(); // tự động gọi lại để đóng
            }
        }

    }
}

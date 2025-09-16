// using System.Collections;
// using UnityEngine;
// using DATN2.Assets.Scripts.Data; // để dùng ItemModel
// using DATN2.Assets.Scripts.Logics.Services;
// using VContainer;
// using DATN2.Assets.Scripts.Logics.Interface;

// public class DoorInteraction : MonoBehaviour
// {
//     [Header("Door Settings")]
//     public ItemModel doorData; // ScriptableObject của cửa
//     public bool isDoubleDoor; // Xác định cửa đơn hay cửa đôi
//     public Transform secondDoor; // Cánh cửa thứ hai (nếu là cửa đôi)
//     public float openAngle = 90f; // Góc mở cửa
//     public float openSpeed = 2f; // Tốc độ mở cửa

//     public bool isOpen = false;
//     private Quaternion closedRotation;
//     private Quaternion openRotation;
//     private Quaternion secondDoorClosedRotation;
//     private Quaternion secondDoorOpenRotation;
//     private Coroutine currentCoroutine;

//     [Inject]
//     private IInventoryService inventoryService;

//     void Start()
//     {
//         // Lưu rotation ban đầu của cửa chính
//         closedRotation = transform.rotation;
//         openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

//         // Nếu là cửa đôi, lưu rotation của cánh cửa thứ hai
//         if (isDoubleDoor && secondDoor != null)
//         {
//             secondDoorClosedRotation = secondDoor.rotation;
//             secondDoorOpenRotation = Quaternion.Euler(secondDoor.eulerAngles + new Vector3(0, -openAngle, 0)); // Cánh thứ hai xoay ngược
//         }
//     }

//     public void Interact()
//     {
//         // Kiểm tra nếu cửa cần chìa khóa
//         if (doorData.requiresKey)
//         {
//             if (inventoryService != null && inventoryService.HasItem(doorData.requiredKey))
//             {
//                 ToggleDoor();
//             }
//             else
//             {
//                 Debug.Log($"[Door] Cần chìa khóa {doorData.requiredKey.itemName} để mở {doorData.itemName}");
//             }
//         }
//         else
//         {
//             ToggleDoor();
//         }
//     }

//     private void ToggleDoor()
//     {
//         if (currentCoroutine != null)
//         {
//             StopCoroutine(currentCoroutine);
//         }
//         currentCoroutine = StartCoroutine(AnimateDoor());
//     }

//     private IEnumerator AnimateDoor()
//     {
//         Quaternion targetRotation = isOpen ? closedRotation : openRotation;
//         Quaternion secondDoorTargetRotation = isOpen ? secondDoorClosedRotation : secondDoorOpenRotation;
//         isOpen = !isOpen;

//         float t = 0f;
//         while (t < 1f)
//         {
//             t += Time.deltaTime * openSpeed;
//             // Xoay cửa chính
//             transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

//             // Xoay cánh cửa thứ hai nếu là cửa đôi
//             if (isDoubleDoor && secondDoor != null)
//             {
//                 secondDoor.rotation = Quaternion.Lerp(secondDoor.rotation, secondDoorTargetRotation, t);
//             }

//             yield return null;
//         }

//         // Đảm bảo rotation chính xác khi kết thúc
//         transform.rotation = targetRotation;
//         if (isDoubleDoor && secondDoor != null)
//         {
//             secondDoor.rotation = secondDoorTargetRotation;
//         }
//     }
// }
using System.Collections;
using UnityEngine;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Services;
using VContainer;
using DATN2.Assets.Scripts.Logics.Interface;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public ItemModel doorData;
    public bool isDoubleDoor;
    public Transform secondDoor;
    public DoorInteraction masterDoor; // thêm tham chiếu tới cửa gốc
    public float openAngle = 90f;
    public float openSpeed = 2f;

    public bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion secondDoorClosedRotation;
    private Quaternion secondDoorOpenRotation;

    private Coroutine currentCoroutine;

    // [Inject] private IInventoryService inventoryService;
    private IInventoryService inventoryService;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (isDoubleDoor && secondDoor != null)
        {
            secondDoorClosedRotation = secondDoor.localRotation;
            secondDoorOpenRotation = secondDoorClosedRotation * Quaternion.Euler(0, -openAngle, 0);
        }
        if (inventoryService == null)
        {
            Debug.LogError("[DoorInteraction] inventoryService is null! Check VContainer setup.");
        }
    }
    public void SetInventoryService(IInventoryService service)
    {
        inventoryService = service;
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
    }

    private void ToggleDoor()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(AnimateDoor());
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

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);

            if (isDoubleDoor && secondDoor != null)
                secondDoor.localRotation = Quaternion.Slerp(secondDoor.localRotation, secondDoorTargetRotation, t);

            yield return null;
        }

        transform.localRotation = targetRotation;
        if (isDoubleDoor && secondDoor != null)
            secondDoor.localRotation = secondDoorTargetRotation;
    }
}


using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Logics.Services;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class ItemPickup : MonoBehaviour
    {
        public string displayName;
        public string displayInformation;
        public ItemModel itemData;
        public int amount = 1;

        public void OnPickedUp()
        {
            GlobalRaycast raycastController = FindObjectOfType<GlobalRaycast>();
            if (raycastController != null)
            {
                raycastController.ShowPickupMessage(displayInformation);
            }
            // ẩn hoặc xóa khỏi scene
            // Destroy(gameObject);
        }
        public void OnInspected()
        {
            GlobalRaycast raycastController = FindObjectOfType<GlobalRaycast>();
            if (raycastController != null)
            {
                raycastController.ShowPickupMessage(displayInformation);
            }
        }
        public void OnInteracted()
        {
            if (itemData.interactTypes.Contains(InteractType.Door))
            {
                // Gọi Interact từ DoorInteraction
                var doorInteraction = GetComponent<DoorInteraction>();
                if (doorInteraction != null)
                {
                    doorInteraction.Interact();
                }
                else
                {
                    Debug.LogWarning($"[Door] GameObject {gameObject.name} thiếu component DoorInteraction!");
                }
            }
            else if (itemData.interactTypes.Contains(InteractType.Closet))
            {

            }
        }
    }
}
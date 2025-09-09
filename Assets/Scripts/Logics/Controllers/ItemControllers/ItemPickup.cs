using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
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
            // ẩn hoặc xóa khỏi scene
            Destroy(gameObject);
        }
    }
}
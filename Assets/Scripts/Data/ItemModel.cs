using System;
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEditor;
using UnityEngine;

namespace DATN2.Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Inventory/Item")]
    public class ItemModel : ScriptableObject
    {
        [SerializeField] private string itemId;
        public string ItemId => itemId;

        [SerializeField] public string itemName;
        [SerializeField] public string description;
        [SerializeField] public List<InteractType> interactTypes;

        [Header("Door Settings (Only if InteractType.Door)")]
        [SerializeField] public bool requiresKey;
        [SerializeField] public ItemModel requiredKey; // tham chiếu đến chìa khóa

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Chỉ gán ID một lần khi chưa có
            if (string.IsNullOrEmpty(itemId))
            {
                itemId = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
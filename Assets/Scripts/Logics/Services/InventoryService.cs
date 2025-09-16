using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Data.Runtime;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Services
{
    public class InventoryService : IInventoryService
    {
        private List<ItemRuntimeSerialized> items = new List<ItemRuntimeSerialized>();

        public ItemRuntimeSerialized AddItem(ItemModel item, int amount = 1)
        {
            foreach (var slot in items)
            {
                if (slot._itemData == item)
                {
                    slot._amount += amount;
                    return slot;
                }
            }
            var newItem = new ItemRuntimeSerialized(item, amount);
            items.Add(newItem);
            return newItem;
        }

        public List<ItemRuntimeSerialized> GetAllItems()
        {
            return new List<ItemRuntimeSerialized>(items);
        }

        public ItemRuntimeSerialized GetItemById(string id)
        {
            return items.Find(i => i._itemData.ItemId == id);
        }

        public bool HasItem(ItemModel item, int requiredAmount = 1)
        {
            foreach (var slot in items)
            {
                if (slot._itemData == item && slot._amount >= requiredAmount)
                    return true;
            }
            return false;
        }

        public bool RemoveItem(ItemModel item, int amount = 1)
        {
            var slot = items.Find(i => i._itemData == item);
            if (slot == null) return false;

            if (slot._amount < amount) return false;

            slot._amount -= amount;
            if (slot._amount == 0)
            {
                items.Remove(slot);
            }
            return true;
        }

        public ItemRuntimeSerialized UpdateItems(ItemModel items, int amount)
        {
            throw new System.NotImplementedException();
        }
        public void DebugPrintInventory()
        {
            if (items.Count == 0)
            {
                Debug.Log("[Inventory] Inventory đang rỗng.");
                return;
            }

            Debug.Log("[Inventory] Danh sách item trong túi:");
            foreach (var slot in items)
            {
                string itemName = slot._itemData != null ? slot._itemData.itemName : "NULL";
                Debug.Log($"- {itemName} (x{slot._amount}) | ID: {slot._itemData.ItemId}");
            }
        }
    }
}
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
            throw new System.NotImplementedException();
        }

        public ItemRuntimeSerialized GetItemById(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(string uniqueId, int amount = 1)
        {
            throw new System.NotImplementedException();
        }

        public ItemRuntimeSerialized UpdateItems(ItemModel items, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Data.Runtime;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface IInventoryService
    {
        event System.Action<ItemModel> OnItemAdded;
        ItemRuntimeSerialized AddItem(ItemModel item, int amount = 1);
        bool HasItem(ItemModel item, int requiredAmount = 1);
        bool RemoveItem(ItemModel item, int amount = 1);
        List<ItemRuntimeSerialized> GetAllItems();
        ItemRuntimeSerialized GetItemById(string id);
        ItemRuntimeSerialized UpdateItems(ItemModel items, int amount);
        void DebugPrintInventory();
    }
}
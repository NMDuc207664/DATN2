using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Data.Runtime;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface IInventoryService
    {
        ItemRuntimeSerialized AddItem(ItemModel item, int amount = 1);
        bool RemoveItem(string uniqueId, int amount = 1);
        List<ItemRuntimeSerialized> GetAllItems();
        ItemRuntimeSerialized GetItemById(int id);
        ItemRuntimeSerialized UpdateItems(ItemModel items, int amount);
    }
}
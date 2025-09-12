using System;
using UnityEngine;
namespace DATN2.Assets.Scripts.Data.Runtime
{
    [Serializable]
    public class ItemRuntimeSerialized : MonoBehaviour
    {
        public string uniqueId;
        public ItemModel _itemData;
        public int _amount;

        public ItemRuntimeSerialized(ItemModel itemData, int amount)
        {
            uniqueId = Guid.NewGuid().ToString();
            _itemData = itemData;
            _amount = amount;
        }
    }
}
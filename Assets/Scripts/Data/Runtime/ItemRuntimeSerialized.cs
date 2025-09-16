using System;
using UnityEngine;
namespace DATN2.Assets.Scripts.Data.Runtime
{
    [Serializable]
    public class ItemRuntimeSerialized : MonoBehaviour
    {
        // public string uniqueId;// dòng này để định nghĩa mỗi item trong stack đều có id riêng nhưng không phù hợp với game này
        public ItemModel _itemData;
        public int _amount;

        public ItemRuntimeSerialized(ItemModel itemData, int amount)
        {
            //uniqueId = Guid.NewGuid().ToString();
            _itemData = itemData;
            _amount = amount;
        }
    }
}
using System;
using UnityEngine;
namespace DATN2.Assets.Scripts.Data.Runtime
{
    [Serializable]
    public class ItemRuntimeSerialized : MonoBehaviour
    {
        public string uniqueId;
        public ItemModel itemData;
        public int amount;

        public ItemRuntimeSerialized(ItemModel itemData, int amount)
        {
            this.uniqueId = Guid.NewGuid().ToString();
            this.itemData = itemData;
            this.amount = amount;
        }
    }
}
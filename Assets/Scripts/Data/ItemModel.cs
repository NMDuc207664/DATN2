using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DATN2.Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Inventory/Item")]
    public class ItemModel : ScriptableObject
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string itemName { get; set; }
        public string description { get; set; }
        public int amount { get; set; } = 1;
    }
}
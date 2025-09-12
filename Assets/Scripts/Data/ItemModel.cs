using System;
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

namespace DATN2.Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Inventory/Item")]
    public class ItemModel : ScriptableObject
    {
        [SerializeField] public string itemName;
        [SerializeField] public string description;
        [SerializeField] public List<InteractType> interactTypes;
    }
}
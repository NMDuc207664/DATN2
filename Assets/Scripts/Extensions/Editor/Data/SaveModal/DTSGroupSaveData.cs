using System;
using UnityEngine;
namespace DATN2.Editor.Data.SaveModal
{
    [Serializable]
    public class DTSGroupSaveData
    {
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

    }
}
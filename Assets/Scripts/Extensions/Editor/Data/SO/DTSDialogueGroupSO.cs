using UnityEngine;
namespace DATN2.Editor.Data.SaveModal.SO
{
    public class DTSDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        public void Initialize(string groupName) => GroupName = groupName;
    }
}
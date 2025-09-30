using UnityEngine;
namespace DATN2.GraphviewEditor.Data.SaveModal.SO
{
    public class DTSDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public bool HasADialogueTalked { get; set; }
        public void Initialize(string groupName, bool hasADialogueTalked)
        {
            GroupName = groupName;
            HasADialogueTalked = hasADialogueTalked;
        }
    }
}
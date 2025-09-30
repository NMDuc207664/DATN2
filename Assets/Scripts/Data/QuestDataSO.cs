using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data.Runtime;
using UnityEngine;
namespace DATN2.Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest Data")]

    public class QuestDataSO : ScriptableObject
    {
        public string Key; // sẽ sync với tên file asset
        public List<QuestRuntime> quests = new List<QuestRuntime>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Key != name)
            {
                Key = name;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
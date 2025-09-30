using System.Collections;
using System.Collections.Generic;
using DATN2.GraphviewEditor.Data.SaveModal.SO;
using UnityEngine;

[CreateAssetMenu(fileName = "Context", menuName = "Dialogue System/Context")]
public class ContextSO : ScriptableObject
{
    [SerializeField] public MoneyCondition condition; // Condition để context này được chọn

    public List<DTSDialogueSO> possibleDialogues; // List các dialogue tiềm năng (ví dụ 3 cái)
}

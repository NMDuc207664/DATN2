using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Context", menuName = "Dialogue System/Flag")]
public class ExamplePlayerFlagCondition : ScriptableObject, ICondition
{
    public string flagName; // Tên flag trong player data, ví dụ "hasItemX"
    public bool requiredValue; // Giá trị mong muốn (true/false)

    public bool Evaluate()
    {
        // Giả sử có PlayerData singleton lưu trữ flags
        return PlayerData.Instance.GetFlag(flagName) == requiredValue;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Context", menuName = "Dialogue System/Money")]
public class MoneyCondition : ScriptableObject, ICondition
{
    public int minMoney;
    public int maxMoney = -1; // -1 nghĩa là không giới hạn trên

    public bool Evaluate()
    {
        int playerMoney = PlayerData.Instance.GetMoney();
        return playerMoney >= minMoney && (maxMoney == -1 || playerMoney <= maxMoney);
    }
}

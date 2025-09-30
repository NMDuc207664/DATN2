using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoney : MonoBehaviour
{
    public int money;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerData.Instance.SetMoney(money);
        }
    }
}

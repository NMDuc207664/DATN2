using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();
    private int money = 0; // Thêm biến money

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetFlag(string flagName, bool value)
    {
        flags[flagName] = value;
    }

    public bool GetFlag(string flagName)
    {
        return flags.TryGetValue(flagName, out bool value) ? value : false;
    }
    public int GetMoney()
    {
        return money;
    }
    public void SetMoney(int amount)
    {
        money = amount;
    }
}
using System;
using UnityEngine;

public class DialogueEventManager : MonoBehaviour
{
    public static DialogueEventManager Instance { get; private set; }

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

    // Event khi dialogue kết thúc, truyền tên event và data tùy chỉnh (ví dụ string cho flag, hoặc ContextSO)
    public event Action<string, object> OnDialogueEnded;

    public void TriggerDialogueEnded(string eventName, object data)
    {
        OnDialogueEnded?.Invoke(eventName, data);
    }
}
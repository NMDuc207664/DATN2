using System;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager Instance { get; set; }
    private StateType _currentState;
    // Sự kiện được gọi khi trạng thái thay đổi
    public event Action<StateType> OnStateChanged;
    // Start is called before the first frame update
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
    public void SetState(StateType stateType)
    {
        if (_currentState != stateType)
        {
            _currentState = stateType;
            OnStateChanged?.Invoke(_currentState);
        }
    }

    public string GetCurrentState()
    {
        return _currentState.ToString();
    }
}

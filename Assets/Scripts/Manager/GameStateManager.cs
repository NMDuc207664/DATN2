using System;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager Instance { get; set; }
    private StateType _currentState = StateType.Ingame;
    // Sự kiện được gọi khi trạng thái thay đổi
    public event Action<StateType> OnStateChanged;
    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    // void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }
    private static void Init()
    {
        if (Instance == null)
        {
            var go = new GameObject("GameStateManager");
            Instance = go.AddComponent<GameStateManager>();
            DontDestroyOnLoad(go);
        }
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Instance.SetState(StateType.OnMenu);
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

    public StateType GetCurrentState()
    {
        return _currentState;
    }
}

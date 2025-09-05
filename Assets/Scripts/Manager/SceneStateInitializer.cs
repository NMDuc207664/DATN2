// using DATN2.Assets.Scripts.Modals.Enum;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class SceneStateInitializer : MonoBehaviour
// {
//     [SerializeField] private StateType sceneState;

//     private void Start()
//     {
//         if (SceneManager.GetActiveScene().name == "MainMenu")
//         {
//             sceneState = StateType.OnMenu;
//         }
//         else
//         if (GameStateManager.Instance != null)
//         {
//             GameStateManager.Instance.SetState(sceneState);
//             Debug.Log($"[SceneStateInitializer] Scene started with state: {sceneState}");
//         }
//     }

//     private void StateLoading()
//     {

//     }
// }
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateInitializer : MonoBehaviour
{
    // private void Awake()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDestroy()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     // Gọi StateLoading khi scene load xong
    //     StateLoading(scene.name);
    // }
    // void Update()
    // {
    //     Debug.Log(GameStateManager.Instance.GetCurrentState());
    // }
    // private void StateLoading(string sceneName)
    // {
    //     StateType stateToSet; // mặc định

    //     switch (sceneName)
    //     {
    //         case "MainMenu":
    //             stateToSet = StateType.OnMenu;
    //             break;

    //         case "Testing":
    //             stateToSet = StateType.Ingame;
    //             break;
    //         case "InGameMenu":
    //             stateToSet = StateType.OnMenu;
    //             break;

    //         default:
    //             stateToSet = StateType.OnMenu;
    //             break;
    //     }

    //     if (GameStateManager.Instance != null)
    //     {
    //         GameStateManager.Instance.SetState(stateToSet);
    //         Debug.Log($"[SceneStateInitializer] Scene '{sceneName}' started with state: {stateToSet}");
    //     }
    // }
}

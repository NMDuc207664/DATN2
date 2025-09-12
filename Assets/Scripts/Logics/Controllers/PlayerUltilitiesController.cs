using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class PlayerUltilitiesController : MonoBehaviour
    {
        [Inject]
        private IPlayerUltilitiesService _playerUltilitiesService;
        [Inject]
        private IMenuService _menuService;
        [SerializeField] private GameObject OnLoadMenu;
        [SerializeField] private GameObject OnMenu;

        // Update is called once per frame
        public void Awake()
        {
            GameStateManager.Instance.SetState(StateType.Ingame);
        }
        private void Update()
        {
            HandleEscape();
            HandlePauseToggle();

            Debug.Log(GameStateManager.Instance.GetCurrentState());
        }

        private void HandleEscape()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var currentState = GameStateManager.Instance.GetCurrentState();

                if (currentState == StateType.Ingame)
                {
                    // Chuyển sang menu
                    GameStateInvoker.TryInvoke(_playerUltilitiesService, nameof(_playerUltilitiesService.PauseGame));
                    GameStateManager.Instance.SetState(StateType.OnMenu);
                    _playerUltilitiesService.OpenIngameMenu();
                    OnLoadMenu.SetActive(false);
                    OnMenu.SetActive(true);
                }
                else if (currentState == StateType.OnMenu)
                {
                    // Quay lại game
                    GameStateInvoker.TryInvoke(_playerUltilitiesService, nameof(_playerUltilitiesService.ResumeGame));
                    GameStateManager.Instance.SetState(StateType.Ingame);
                    _playerUltilitiesService.BackToGame();
                }
            }
        }

        private void HandlePauseToggle()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var currentState = GameStateManager.Instance.GetCurrentState();

                if (currentState == StateType.Ingame)
                {
                    GameStateInvoker.TryInvoke(_playerUltilitiesService, nameof(_playerUltilitiesService.PauseGame));
                    GameStateManager.Instance.SetState(StateType.Pause);
                }
                else if (currentState == StateType.Pause)
                {
                    GameStateInvoker.TryInvoke(_playerUltilitiesService, nameof(_playerUltilitiesService.ResumeGame));
                    GameStateManager.Instance.SetState(StateType.Ingame);
                }
            }
        }
    }
}
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class Menu : MonoBehaviour
    {

        [Inject]
        public IMenuService _menuService;
        [Inject]
        public ISaveSlotService _saveSlotService;
        // [SerializeField] private SaveSlotsMenu saveSlotsMenu;
        [SerializeField] private Button newGameButton;
        [SerializeField] public Button continueGameButton;
        [SerializeField] public SaveSlotMenu saveSlotsMenu;
        void Awake()
        {
            // GameStateManager.Instance.SetState(StateType.OnMenu);
        }
        public void OnLoadButtonClicked()
        {
            // saveSlotsMenu.OnLoadSlotClick();
            GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.ToogleLoadSaveSlot));
            saveSlotsMenu.ActiveMenu();
            DeactivateMenu();
        }

        // Similarly for save, delete, etc.
        public void OnNewGameButtonClicked()
        {
            GameStateInvoker.TryInvoke(_menuService, nameof(_menuService.NewGame));
        }
        public void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
        }
        public void ActivateMenu()
        {
            this.gameObject.SetActive(true);
            saveSlotsMenu.DeactivateMenu();
        }


    }
}

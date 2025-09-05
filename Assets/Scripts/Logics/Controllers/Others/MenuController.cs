using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class Menu : MonoBehaviour
    {
        [Inject]
        private SaveAndLoadController _controller;
        [Inject]
        private IMenuService _menuService;
        [Inject]
        private ISaveSlotService _saveSlotService;
        // [SerializeField] private SaveSlotsMenu saveSlotsMenu;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private SaveSlotMenu saveSlotsMenu;

        // public void Awake()
        // {
        //     saveSlotsMenu.ActiveMenu();
        // }
        public void OnLoadButtonClicked(string saveId)
        {
            // saveSlotsMenu.OnLoadSlotClick();
            saveSlotsMenu.ActiveMenu();
            DeactivateMenu();
        }

        // Similarly for save, delete, etc.
        public void OnNewGameButtonClicked(string saveName)
        {
            SceneManager.LoadScene("Testing");
        }
        public void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
        }
        public void ActivateMenu()
        {
            this.gameObject.SetActive(true);
        }

    }
}

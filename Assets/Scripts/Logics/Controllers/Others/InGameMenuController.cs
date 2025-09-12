using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class InGameMenuController : Menu
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveSlotButton;
        public void OnSaveButtonClicked()
        {
            // saveSlotsMenu.OnLoadSlotClick();
            GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.ToogleSaveSlot));
            saveSlotsMenu.ActiveMenu();
            DeactivateMenu();
        }
        public void OnResumeButtonClicked()
        {
            GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.BackToGame));

            transform.parent.gameObject.SetActive(false); // Tắt A (menuUI)
        }

    }
}
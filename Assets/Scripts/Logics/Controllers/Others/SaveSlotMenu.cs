using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class SaveSlotMenu : MonoBehaviour
    {
        [Inject]
        private SaveAndLoadController saveAndLoadController;
        [SerializeField] private Menu menu;
        [Inject]
        private IMenuService _menuService;
        [Inject]
        private ISaveSlotService _saveSlotService;
        private SaveSlot[] saveSlots;
        Dictionary<string, SaveModel> saveDict = new Dictionary<string, SaveModel>();
        // Start is called before the first frame update
        private void Awake()
        {
            saveSlots = GetComponentsInChildren<SaveSlot>();
        }
        private void Start()
        {
            ActiveMenu();
        }

        public async void ActiveMenu()
        {
            // GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.ToogleLoadSaveSlot));
            this.gameObject.SetActive(true);
            // Lấy tất cả saves
            var allSaves = await saveAndLoadController.GetAllSavesAsync();

            // Tạo dictionary để dễ tìm kiếm

            foreach (var save in allSaves)
            {
                saveDict[save.SaveId] = save;
            }

            // Cập nhật từng save slot
            foreach (SaveSlot saveSlot in saveSlots)
            {
                string profileId = saveSlot.GetProfileId();

                // Tìm save tương ứng với profileId
                if (saveDict.TryGetValue(profileId, out SaveModel saveModel))
                {
                    saveSlot.SetData(saveModel);
                }
                else
                {
                    saveSlot.SetData(null); // Không có data
                }
            }
        }
        public void OnBackClick()
        {
            menu.ActivateMenu();
            GameStateInvoker.TryInvoke(_menuService, nameof(_menuService.ToogleMenu));
            this.DeactivateMenu();
        }
        public void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
        }
        public async void OnSaveAndLoadClick(string saveId)
        {
            // if (GameStateManager.Instance.GetCurrentState() == StateType.OnLoad)
            // {
            //     await saveAndLoadController.LoadGameAsync(saveId);
            // }
            // else if (GameStateManager.Instance.GetCurrentState() == StateType.OnSave)
            // {
            //     await saveAndLoadController.SaveNewGameAsync(saveId);
            // }
            await saveAndLoadController.LoadGameAsync(saveId);
        }
    }
}

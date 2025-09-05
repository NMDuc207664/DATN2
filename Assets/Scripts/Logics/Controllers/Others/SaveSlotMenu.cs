using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [Inject]
        private ISaveService _saveService;
        private SaveSlot[] saveSlots;
        private readonly string[] SAVE_SLOTS = { "Save_Slot_1", "Save_Slot_2", "Save_Slot_3" };
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
            this.gameObject.SetActive(true);
            // GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.ToogleLoadSaveSlot));

            // Lấy tất cả saves
            // var allSaves = await saveAndLoadController.GetAllSavesAsync();
            saveDict.Clear();
            foreach (string slotName in SAVE_SLOTS)
            {
                var saveModel = await _saveService.GetSaveBySlotAsync(slotName);
                if (saveModel != null)
                {
                    saveDict[slotName] = saveModel;
                }
                else
                {
                    Debug.Log($"[SaveSlotMenu] No save found for slot: {slotName}");
                }
            }
            await Task.Delay(50);
            // Cập nhật từng save slot
            foreach (SaveSlot saveSlot in saveSlots)
            {
                string slotName = saveSlot.GetSlotName();

                // Kiểm tra slotName có hợp lệ không
                if (!SAVE_SLOTS.Contains(slotName))
                {
                    Debug.LogWarning($"[SaveSlotMenu] Invalid slot name: {slotName}. Must be one of: {string.Join(", ", SAVE_SLOTS)}");
                    saveSlot.SetData(null);
                    continue;
                }

                // Tìm save model cho slot này
                if (saveDict.TryGetValue(slotName, out SaveModel saveModel))
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
            Task.Delay(50);
            this.DeactivateMenu();
        }
        public void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
        }
        public async void OnSaveSlotOneClick()
        {
            await HandleSaveSlotClick(SAVE_SLOTS[0]);
        }

        public async void OnSaveSlotTwoClick()
        {
            await HandleSaveSlotClick(SAVE_SLOTS[1]);
        }

        public async void OnSaveSlotThreeClick()
        {
            await HandleSaveSlotClick(SAVE_SLOTS[2]);
        }
        private async Task HandleSaveSlotClick(string slotName)
        {

            var currentState = GameStateManager.Instance.GetCurrentState();
            if (currentState == StateType.OnSave)
            {
                // Kiểm tra xem slot đã có save chưa
                var existingSave = await saveAndLoadController.SaveSlotOneAsync(slotName, overwrite: false);
                if (existingSave != null)
                {
                    // Nếu đã có save, ghi đè
                    await saveAndLoadController.SaveSlotOneAsync(slotName, overwrite: true);
                    Debug.Log($"[SaveSlotMenu] Overwritten save in slot: {slotName}");
                }
                else
                {
                    // Nếu chưa có save, lưu mới
                    await saveAndLoadController.SaveSlotOneAsync(slotName);
                    Debug.Log($"[SaveSlotMenu] New save created in slot: {slotName}");
                }
                ActiveMenu(); // Cập nhật UI sau khi lưu
            }
            else if (currentState == StateType.OnLoad)
            {
                // Load save từ slot
                var saveModel = await _saveService.GetSaveBySlotAsync(slotName);
                if (saveModel != null)
                {
                    await saveAndLoadController.LoadGameAsync(saveModel.SaveId);
                    GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.LoadGame));
                    Time.timeScale = 1f;
                    this.gameObject.SetActive(false);
                    // if (SceneManager.GetActiveScene().name != "MainMenu")
                    // {
                    //     this.gameObject.SetActive(false);
                    // }
                    // Debug.Log($"[SaveSlotMenu] Loaded save from slot: {slotName}");
                }
                else
                {
                    Debug.LogWarning($"[SaveSlotMenu] No save found in slot: {slotName}");
                }
            }

        }

    }
}


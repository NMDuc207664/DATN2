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
        private bool _isDestroyed = false;
        // Start is called before the first frame update
        private void Awake()
        {
            saveSlots = GetComponentsInChildren<SaveSlot>();
        }
        private void Start()
        {
            ActiveMenu();
        }
        private void OnDestroy()
        {
            _isDestroyed = true;  // Set flag khi destroy
        }
        public async void ActiveMenu()
        {
            if (_isDestroyed) return;  // Early exit nếu đã destroy
            this.gameObject.SetActive(true);
            // GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.ToogleLoadSaveSlot));

            // Lấy tất cả saves
            // var allSaves = await saveAndLoadController.GetAllSavesAsync();
            saveDict.Clear();
            foreach (string slotName in SAVE_SLOTS)
            {
                if (_isDestroyed) return;
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
                if (_isDestroyed) return;
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
            if (_isDestroyed) return;
            menu.ActivateMenu();
            GameStateInvoker.TryInvoke(_menuService, nameof(_menuService.ToogleMenu));
            Task.Delay(50);
            this.DeactivateMenu();
        }
        public void DeactivateMenu()
        {
            if (_isDestroyed || this == null) return;
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
            if (_isDestroyed || saveAndLoadController == null) return;  // Early check

            var currentState = GameStateManager.Instance?.GetCurrentState();  // Safe
            if (currentState == null) return;

            if (currentState == StateType.OnSave)
            {
                // Check existing save (không await ở đây để tránh delay destroy)
                var existingSave = await (saveAndLoadController?.SaveSlotOneAsync(slotName, overwrite: false));
                if (_isDestroyed) return;  // Check sau await

                if (existingSave != null)
                {
                    await (saveAndLoadController?.SaveSlotOneAsync(slotName, overwrite: true));
                    Debug.Log($"[SaveSlotMenu] Overwritten save in slot: {slotName}");
                }
                else
                {
                    await (saveAndLoadController?.SaveSlotOneAsync(slotName));
                    Debug.Log($"[SaveSlotMenu] New save created in slot: {slotName}");
                }

                if (!_isDestroyed)
                    ActiveMenu();  // Chỉ active nếu chưa destroy
            }
            else if (currentState == StateType.OnLoad)
            {
                var saveModel = await (_saveService?.GetSaveBySlotAsync(slotName));
                if (saveModel == null || _isDestroyed)
                {
                    if (saveModel == null)
                        Debug.LogWarning($"[SaveSlotMenu] No save found in slot: {slotName}");
                    return;
                }

                // **QUAN TRỌNG: Di chuyển tất cả code sau LoadGameAsync lên TRƯỚC await**
                // Để tránh chạy sau khi scene load (destroy object)
                GameStateInvoker.TryInvoke(_saveSlotService, nameof(_saveSlotService.LoadGame));  // Gọi trước
                Time.timeScale = 1f;
                this.gameObject.SetActive(false);  // Deactivate trước load

                // Bây giờ mới load (sau khi đã setup xong)
                await (saveAndLoadController?.LoadGameAsync(saveModel.SaveId));

                // Không cần code sau await nữa, vì sau load scene, menu đã inactive/destroy
                Debug.Log($"[SaveSlotMenu] Loaded save from slot: {slotName}");
            }
        }

    }
}


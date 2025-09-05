using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class SaveAndLoadController : MonoBehaviour
    {
        [Inject]
        private readonly ISaveService _saveService;

        [Header("Player Settings")]
        // [SerializeField] private Transform playerTransform;
        [SerializeField] private string defaultSaveName = "QuickSave";
        private readonly string[] SAVE_SLOTS = { "Save_slot_1", "Save_slot_2", "Save_slot_3" };

        public static event System.Action<SaveModel> OnGameSaved;
        public static event System.Action<SaveModel> OnGameLoaded;
        public static event System.Action<List<SaveModel>> OnSaveListUpdated;
        public static event System.Action<string> OnSaveDeleted;

        // private void Start()
        // {
        //     // Tự động tìm player nếu chưa được gán
        //     if (playerTransform == null)
        //     {
        //         var player = GameObject.FindWithTag("Player");
        //         if (player != null)
        //             playerTransform = player.transform;
        //     }
        // }

        #region Public Methods

        /// <summary>
        /// Lưu game với tên mới
        /// </summary>
        public async Task<SaveModel> SaveNewGameAsync(string saveName = null)
        {
            try
            {
                var saveModel = CreateSaveModel(saveName ?? defaultSaveName);
                var result = await _saveService.AddNewSaveAsync(saveModel);

                Debug.Log($"[SaveAndLoadController] New game saved: {result.SaveName} ({result.SaveId})");
                OnGameSaved?.Invoke(result);

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to save new game: {e.Message}");
                return null;
            }
        }


        /// <summary>
        /// Ghi đè save hiện có
        /// </summary>
        public async Task<SaveModel> OverwriteSaveAsync(string saveId, string saveName = null)
        {
            try
            {
                var existingSave = await _saveService.GetSaveByIdAsync(saveId);
                if (existingSave == null)
                {
                    Debug.LogWarning($"[SaveAndLoadController] Save with ID {saveId} not found for overwrite");
                    return null;
                }

                var saveModel = CreateSaveModel(saveName ?? existingSave.SaveName);
                saveModel.SaveId = saveId; // Giữ nguyên ID

                var result = await _saveService.OverwriteSaveAsync(saveModel);

                Debug.Log($"[SaveAndLoadController] Game overwritten: {result.SaveName} ({result.SaveId})");
                OnGameSaved?.Invoke(result);

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to overwrite save: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Load game từ save ID
        /// </summary>
        public async Task<bool> LoadGameAsync(string saveId)
        {
            try
            {
                var saveModel = await _saveService.GetSaveByIdAsync(saveId);
                if (saveModel == null)
                {
                    Debug.LogWarning($"[SaveAndLoadController] Save with ID {saveId} not found");
                    return false;
                }

                return await LoadGameFromModel(saveModel);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to load game: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa save
        /// </summary>
        public async Task<bool> DeleteSaveAsync(string saveId)
        {
            try
            {
                await _saveService.DeleteSaveAsync(saveId);
                Debug.Log($"[SaveAndLoadController] Save deleted: {saveId}");
                OnSaveDeleted?.Invoke(saveId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to delete save: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy tất cả save files
        /// </summary>
        public async Task<List<SaveModel>> GetAllSavesAsync()
        {
            try
            {
                var saves = await _saveService.GetAllSavesAsync();
                OnSaveListUpdated?.Invoke(saves);
                return saves;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to get all saves: {e.Message}");
                return new List<SaveModel>();
            }
        }

        /// <summary>
        /// Kiểm tra có save nào không
        /// </summary>
        public bool HasAnySave()
        {
            return _saveService.AnySave();
        }

        /// <summary>
        /// Quick Save - lưu nhanh
        /// </summary>
        public async Task<SaveModel> QuickSaveAsync()
        {
            return await SaveNewGameAsync("QuickSave_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        }

        /// <summary>
        /// Load save gần nhất
        /// </summary>
        public async Task<bool> LoadLatestSaveAsync()
        {
            try
            {
                var saves = await GetAllSavesAsync();
                if (saves == null || saves.Count == 0)
                {
                    Debug.LogWarning("[SaveAndLoadController] No saves found to load");
                    return false;
                }

                // Tìm save có thời gian gần nhất
                SaveModel latestSave = saves[0];
                DateTime latestTime = DateTime.Parse(latestSave.Time);

                foreach (var save in saves)
                {
                    var saveTime = DateTime.Parse(save.Time);
                    if (saveTime > latestTime)
                    {
                        latestTime = saveTime;
                        latestSave = save;
                    }
                }

                return await LoadGameFromModel(latestSave);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to load latest save: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tạo SaveModel từ trạng thái hiện tại
        /// </summary>
        private SaveModel CreateSaveModel(string saveName)
        {
            Vector3 playerPos = FindObjectOfType<CharacterController>()?.transform.position ?? Vector3.zero;

            return new SaveModel
            {
                SaveName = saveName,
                SceneName = SceneManager.GetActiveScene().name,
                PlayerPosition = playerPos,
                Time = DateTime.Now.ToString("O") // ISO format
            };
        }

        /// <summary>
        /// Load game từ SaveModel
        /// </summary>
        private async Task<bool> LoadGameFromModel(SaveModel saveModel)
        {
            try
            {
                // Nếu scene khác với scene hiện tại, load scene mới
                if (saveModel.SceneName != SceneManager.GetActiveScene().name)
                {
                    Debug.Log($"[SaveAndLoadController] Loading scene: {saveModel.SceneName}");

                    // Load scene async
                    var asyncOperation = SceneManager.LoadSceneAsync(saveModel.SceneName);

                    // Chờ scene load xong
                    while (!asyncOperation.isDone)
                    {
                        await Task.Yield();
                    }

                    // Tìm lại player sau khi load scene
                    await Task.Delay(100); // Đợi một chút để Unity khởi tạo objects
                    // var player = GameObject.FindWithTag("Player");
                    // if (player != null)
                    //     playerTransform = player.transform;
                }

                // Set vị trí player
                // if (playerTransform != null)
                // {
                //     playerTransform.position = saveModel.PlayerPosition;
                //     Debug.Log($"[SaveAndLoadController] Player position set to: {saveModel.PlayerPosition}");
                // }

                Debug.Log($"[SaveAndLoadController] Game loaded successfully: {saveModel.SaveName}");
                OnGameLoaded?.Invoke(saveModel);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveAndLoadController] Failed to load game from model: {e.Message}");
                return false;
            }
        }

        private async void OnApplicationQuit()
        {
            // if (SceneManager.GetActiveScene().name != "MainMenu")
            // {
            //     await SaveNewGameAsync();
            // }
        }

        #endregion

        #region Unity Input Handlers (Optional - for testing)

        // private void Update()
        // {
        //     // Quick Save: F5
        //     if (Input.GetKeyDown(KeyCode.F5))
        //     {
        //         _ = QuickSaveAsync();
        //     }

        //     // Quick Load: F9
        //     if (Input.GetKeyDown(KeyCode.F9))
        //     {
        //         _ = LoadLatestSaveAsync();
        //     }
        // }

        #endregion

        #region Public Sync Wrappers (for UI buttons that don't support async)

        // public void SaveNewGame(string saveName = null)
        // {
        //     _ = SaveNewGameAsync(saveName);
        // }

        // public void OverwriteSave(string saveId, string saveName = null)
        // {
        //     _ = OverwriteSaveAsync(saveId, saveName);
        // }

        // public void LoadGame(string saveId)
        // {
        //     _ = LoadGameAsync(saveId);
        // }

        // public void DeleteSave(string saveId)
        // {
        //     _ = DeleteSaveAsync(saveId);
        // }

        // public void QuickSave()
        // {
        //     _ = QuickSaveAsync();
        // }

        // public void LoadLatestSave()
        // {
        //     _ = LoadLatestSaveAsync();
        // }

        #endregion
    }
}

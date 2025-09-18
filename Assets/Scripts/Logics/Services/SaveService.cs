using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Services
{
    public class SaveService : ISaveService
    {
        private readonly IFileHandler<SaveModel> _fileHandler;
        private readonly Dictionary<string, SaveModel> saves = new Dictionary<string, SaveModel>();

        public SaveService(IFileHandler<SaveModel> fileHandler)
        {
            _fileHandler = fileHandler;
            // LoadAllSavesFromDisk();
        }
        // public SaveModel AddNewSave(SaveModel input)
        // {
        //     var saveModel = new SaveModel
        //     {
        //         SaveId = Guid.NewGuid().ToString(),
        //         SaveName = input.SaveName,
        //         SceneName = input.SceneName,
        //         PlayerPosition = input.PlayerPosition,
        //         Time = DateTime.Now.ToString("O") // lưu ISO string
        //     };

        //     saves[saveModel.SaveId] = saveModel;
        //     _fileHandler.Save(saveModel.SaveId, saveModel);
        //     return saveModel;
        // }
        // public SaveModel OverwriteSave(SaveModel input)
        // {
        //     var saveModel = new SaveModel
        //     {
        //         SaveId = input.SaveId,
        //         SaveName = input.SaveName,
        //         SceneName = input.SceneName,
        //         PlayerPosition = input.PlayerPosition,
        //         Time = DateTime.Now.ToString("O")
        //     };

        //     saves[saveModel.SaveId] = saveModel;
        //     _fileHandler.Save(saveModel.SaveId, saveModel);

        //     return saveModel;
        // }

        // public void DeleteSave(string saveId)
        // {
        //     if (saves.ContainsKey(saveId))
        //     {
        //         saves.Remove(saveId);
        //         _fileHandler.Delete(saveId);
        //     }
        // }
        // public bool AnySave()
        // {
        //     var files = System.IO.Directory.GetFiles(_fileHandler.GetDirectoryPath(), "*.json");
        //     if (files == null || files.Count() == 0)
        //     {
        //         return false;
        //     }
        //     return true;
        // }

        // public List<SaveModel> GetAllSaves()
        // {
        //     var dict = _fileHandler.LoadAll();
        //     Debug.Log($"[SaveService] GetAllSaves found {dict.Count} saves");
        //     return dict.Values.ToList();
        // }

        // public SaveModel GetSaveById(string saveId)
        // {

        //     if (saves.TryGetValue(saveId, out var save))
        //     {
        //         return save;
        //     }

        //     //Debug.Log($"[SaveService] Not in memory, trying to load from file...");
        //     var loaded = _fileHandler.Load(saveId);

        //     if (loaded == null)
        //     {
        //         return null;
        //     }

        //     Debug.Log($"[SaveService] Loaded from file: {loaded.SaveName} ({loaded.SaveId})");
        //     saves[saveId] = loaded;
        //     return loaded;
        // }
        public async Task<SaveModel> AddNewSaveAsync(SaveModel input, string slotName)
        {
            var saveModel = new SaveModel
            {
                SaveId = Guid.NewGuid().ToString(),
                SaveName = input.SaveName,
                SceneName = input.SceneName,
                PlayerPosition = input.PlayerPosition,
                PlayerRotation = input.PlayerRotation,
                PlayerScale = input.PlayerScale,
                Time = DateTime.Now.ToString("O") // ISO string
            };

            saves[saveModel.SaveId] = saveModel;
            await _fileHandler.SaveAsync(slotName, saveModel);

            return saveModel;
        }

        public async Task<SaveModel> OverwriteSaveAsync(SaveModel input, string slotName)
        {
            var saveModel = new SaveModel
            {
                SaveId = input.SaveId,
                SaveName = input.SaveName,
                SceneName = input.SceneName,
                PlayerPosition = input.PlayerPosition,
                PlayerRotation = input.PlayerRotation,
                PlayerScale = input.PlayerScale,
                Time = DateTime.Now.ToString("O")
            };

            saves[saveModel.SaveId] = saveModel;
            await _fileHandler.SaveAsync(slotName, saveModel);

            return saveModel;
        }

        public async Task DeleteSaveAsync(string saveId)
        {
            if (saves.ContainsKey(saveId))
            {
                saves.Remove(saveId);
                await _fileHandler.DeleteAsync(saveId);
            }
        }

        public bool AnySave()
        {
            var files = System.IO.Directory.GetFiles(_fileHandler.GetDirectoryPath(), "*.json");
            return files != null && files.Length > 0;
        }

        public async Task<List<SaveModel>> GetAllSavesAsync()
        {
            var dict = await _fileHandler.LoadAllAsync();
            Debug.Log($"[SaveService] GetAllSavesAsync found {dict.Count} saves");
            return dict.Values.ToList();
        }

        public async Task<SaveModel> GetSaveByIdAsync(string saveId)
        {
            if (saves.TryGetValue(saveId, out var save))
            {
                return save;
            }

            var loaded = await _fileHandler.LoadAsync(saveId);

            if (loaded == null)
            {
                return null;
            }

            Debug.Log($"[SaveService] Loaded from file: {loaded.SaveName} ({loaded.SaveId})");
            saves[saveId] = loaded;
            return loaded;
        }
        public async Task<SaveModel> GetSaveBySlotAsync(string slotName)
        {
            var loaded = await _fileHandler.LoadAsync(slotName);
            if (loaded == null)
            {
                return null;
            }

            Debug.Log($"[SaveService] Loaded from slot: {loaded.SaveName} ({loaded.SaveId}) in slot {slotName}");
            saves[loaded.SaveId] = loaded;
            return loaded;
        }
    }
}

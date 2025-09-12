using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface ISaveService
    {
        Task<SaveModel> AddNewSaveAsync(SaveModel input, string slotName);
        Task<SaveModel> OverwriteSaveAsync(SaveModel input, string slotName);
        Task DeleteSaveAsync(string saveId);
        Task<SaveModel> GetSaveByIdAsync(string saveId);
        Task<List<SaveModel>> GetAllSavesAsync();
        public bool AnySave();
        Task<SaveModel> GetSaveBySlotAsync(string slotName);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Interface
{
    public interface IFileHandler<T> where T : class
    {
        Task SaveAsync(string folderName, T data);
        Task<T> LoadAsync(string folderName);
        Task DeleteAsync(string folderName);
        string GetDirectoryPath();
        Task<Dictionary<string, T>> LoadAllAsync();
    }
}
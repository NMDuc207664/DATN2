using System;
using System.IO;
using UnityEngine;
using DATN2.Assets.Scripts.Logics.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DATN2.Assets.Scripts.Logics.Services
{
    public class JsonFileHandler<T> : IFileHandler<T> where T : class
    {
        private string directoryPath;
        private bool useEncryption;
        private readonly string encryptionCodeWord = "DUCDEPTRAI";
        private readonly string defaultFileName = "GameSave";

        public JsonFileHandler(string directoryPath, bool useEncryption = false)
        {
            this.directoryPath = directoryPath;
            this.useEncryption = useEncryption;
            Debug.Log($"[JsonFileHandler] Save files will be stored in: {directoryPath}");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public async Task SaveAsync(string folderName, T data)
        {
            string folderPath = Path.Combine(directoryPath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, defaultFileName + ".json");
            try
            {
                string json = JsonUtility.ToJson(data, true);
                if (useEncryption) json = EncryptDecrypt(json);

                using (StreamWriter writer = new StreamWriter(fullPath, false))
                {
                    await writer.WriteAsync(json);
                }

                Debug.Log($"[JsonFileHandler] Saved async: {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving file {fullPath}: {e}");
            }
        }

        public async Task<T> LoadAsync(string folderName)
        {
            string folderPath = Path.Combine(directoryPath, folderName);
            string fullPath = Path.Combine(folderPath, defaultFileName + ".json");

            if (!File.Exists(fullPath)) return default;

            try
            {
                string json;
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    json = await reader.ReadToEndAsync();
                }

                if (useEncryption) json = EncryptDecrypt(json);

                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading file {fullPath}: {e}");
                return default;
            }
        }

        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }
            return modifiedData;
        }
        public string GetDirectoryPath()
        {
            return directoryPath;
        }

        public async Task DeleteAsync(string folderName)
        {
            string folderPath = Path.Combine(directoryPath, folderName);
            try
            {
                if (Directory.Exists(folderPath))
                {
                    await Task.Run(() => Directory.Delete(folderPath, true));
                    Debug.Log($"[JsonFileHandler] Deleted folder async: {folderPath}");
                }
                else
                {
                    Debug.LogWarning($"[JsonFileHandler] Folder not found: {folderPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting folder {folderPath}: {e}");
            }
        }

        public async Task<Dictionary<string, T>> LoadAllAsync()
        {
            var dict = new Dictionary<string, T>();
            try
            {
                var directories = Directory.GetDirectories(directoryPath);
                foreach (var dir in directories)
                {
                    string folderName = Path.GetFileName(dir);

                    // chạy LoadAsync cho từng folder
                    var save = await LoadAsync(folderName);
                    if (save != null)
                    {
                        dict[folderName] = save;
                    }
                }

                // Debug.Log($"[JsonFileHandler] LoadAllAsync found {dict.Count} saves");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading all saves: {e}");
            }

            return dict;
        }
    }
}
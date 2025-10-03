using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class KeyGameStateManager : MonoBehaviour
    {
        public static KeyGameStateManager Instance { get; set; }

        [Header("Debug View")]
        [SerializeField] public List<string> currentKeysActive = new List<string>();
        [SerializeField] public List<string> passedKeysUnactive = new List<string>();

        public SerializableDictionary<string, QuestDataSO> allRegisterKeys = new SerializableDictionary<string, QuestDataSO>();
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void RegisterAllKeysForScene(string sceneName)
        {
            // Đường dẫn Resources/Data/SceneName/
            string path = $"Data/{sceneName}";
            Debug.Log($"[KeyGameStateManager] Loading QuestDataSO from path: Resources/{path}");

            QuestDataSO[] quests = Resources.LoadAll<QuestDataSO>(path);

            Debug.Log($"[KeyGameStateManager] Found {quests.Length} QuestDataSO in {path}");

            foreach (var quest in quests)
            {
                Debug.Log($"[KeyGameStateManager] Registering Key: {quest.Key}, Asset name: {quest.name}");

                if (!allRegisterKeys.ContainsKey(quest.Key))
                {
                    allRegisterKeys.Add(quest.Key, quest);
                }
                else
                {
                    Debug.LogWarning($"[KeyGameStateManager] Duplicate Key found: {quest.Key}");
                }
            }
        }

        public void ActivateKey(string key)
        {
            if (!allRegisterKeys.ContainsKey(key))
            {
                Debug.LogWarning($"Key {key} not found!");
                return;
            }

            if (!currentKeysActive.Contains(key))
            {
                currentKeysActive.Add(key);
                Debug.Log($"Activated key: {key}");
            }
        }

        public void PassKey(string key)
        {
            if (currentKeysActive.Contains(key))
            {
                currentKeysActive.Remove(key);
                passedKeysUnactive.Add(key);
                Debug.Log($"Passed key: {key}");
            }
        }

        public QuestDataSO GetQuestData(string key)
        {
            if (allRegisterKeys.TryGetValue(key, out var quest))
                return quest;

            Debug.LogWarning($"QuestData not found for key: {key}");
            return null;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RegisterAllKeysForScene(scene.name);
        }
    }
}
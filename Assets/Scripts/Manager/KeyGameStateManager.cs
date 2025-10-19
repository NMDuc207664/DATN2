using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Modals.Enum;
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

        [Header("GameState")]
        public SerializableDictionary<string, InGameActionType> gameState = new SerializableDictionary<string, InGameActionType>();
        public bool Camera_1 = true;
        public bool Camera_2 = false;
        public bool LockMouseInput = false;
        public bool LimitMouseInput = false;
        public bool LookAtTieuThu = false;
        public bool LookAtGaGiangHo = false;
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
        void Update()
        {

        }
        public void RegisterAllKeysForScene(string sceneName)
        {
            // Đường dẫn Resources/Data/SceneName/
            string path = $"Data/{sceneName}";

            QuestDataSO[] quests = Resources.LoadAll<QuestDataSO>(path);



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


        public void AddOrChangeGameState(InGameActionType newState)
        {
            string key = newState.ToString();

            if (gameState.Count > 0)
            {
                gameState.Clear();
            }

            gameState.Add(key, newState);
        }
        public void SetCamera1Active(bool active)
        {
            Camera_1 = active;
            if (active)
                Camera_2 = false;
        }

        public void SetCamera2Active(bool active)
        {
            Camera_2 = active;
            if (active)
                Camera_1 = false;
        }

        public bool IsInState(InGameActionType state)
        {
            return gameState.ContainsKey(state.ToString()) && gameState[state.ToString()] == state;
        }
        public void SetLockMouseInput(bool lockInput)
        {
            LockMouseInput = lockInput;
            if (lockInput)
                LimitMouseInput = false;
        }
        public void SetLimitMouseInput(bool limitInput)
        {
            LimitMouseInput = limitInput;
            if (limitInput)
                LockMouseInput = false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTracker : MonoBehaviour
{
    public static SceneTracker Instance { get; private set; }

    public string CurrentSceneName { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Gán giá trị khi start
        UpdateSceneInfo(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        // Đăng ký callback khi đổi scene
        SceneManager.sceneLoaded += UpdateSceneInfo;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= UpdateSceneInfo;
        }
    }

    private void UpdateSceneInfo(Scene scene, LoadSceneMode mode)
    {
        CurrentSceneName = scene.name;
        Debug.Log($"[SceneTracker] Current Scene = {CurrentSceneName}");
    }
}


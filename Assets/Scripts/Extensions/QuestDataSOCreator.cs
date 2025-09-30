using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using DATN2.Assets.Scripts.Data;

public static class QuestDataSOCreator
{
    [MenuItem("Assets/Create/Game/Quest Data (Auto)")]
    public static void CreateQuestDataSO()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        string basePath = "Assets/Resources/Data";
        if (!AssetDatabase.IsValidFolder(basePath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Data");
        }

        string sceneFolder = $"{basePath}/{sceneName}";
        if (!AssetDatabase.IsValidFolder(sceneFolder))
        {
            AssetDatabase.CreateFolder(basePath, sceneName);
        }

        string assetPath = $"{sceneFolder}/NewQuestData.asset";

        QuestDataSO asset = ScriptableObject.CreateInstance<QuestDataSO>();
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        Debug.Log($"[QuestDataSOCreator] Created QuestDataSO at {assetPath}");
    }
}

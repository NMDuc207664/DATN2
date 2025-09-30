using UnityEditor;
using DATN2.Assets.Scripts.Data;

public class QuestDataSOAssetPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
            TryUpdateKey(path);

        foreach (string path in movedAssets)
            TryUpdateKey(path);
    }

    private static void TryUpdateKey(string assetPath)
    {
        QuestDataSO questData = AssetDatabase.LoadAssetAtPath<QuestDataSO>(assetPath);
        if (questData != null)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            if (questData.Key != assetName)
            {
                questData.Key = assetName;
                EditorUtility.SetDirty(questData);
                AssetDatabase.SaveAssets();

                UnityEngine.Debug.Log($"[QuestDataSOAssetPostprocessor] Updated Key = {assetName}");
            }
        }
    }
}

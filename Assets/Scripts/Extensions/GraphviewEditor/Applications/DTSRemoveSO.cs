using UnityEditor;
using System.IO;
using UnityEngine;

namespace DATN2.GraphviewEditor.Applications
{
    public static class DTSRemoveSO
    {
        public static void DeleteGraphData(string graphFileName)
        {
            // Define paths for the graph folder and graph ScriptableObject
            string containerFolderPath = $"Assets/Resources/Dialogues/{graphFileName}";
            string graphSOPath = $"Assets/Resources/Graphs/{graphFileName}_Graph.asset";

            // Delete the graph folder and all its contents
            if (AssetDatabase.IsValidFolder(containerFolderPath))
            {
                AssetDatabase.DeleteAsset(containerFolderPath);
                // Debug.Log($"[DeleteGraphData] Deleted folder: {containerFolderPath}");
            }

            // Delete the graph ScriptableObject
            if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(graphSOPath) != null)
            {
                AssetDatabase.DeleteAsset(graphSOPath);
                // Debug.Log($"[DeleteGraphData] Deleted ScriptableObject: {graphSOPath}");
            }

            // Refresh the AssetDatabase to ensure changes are reflected
            AssetDatabase.Refresh();
        }
    }
}
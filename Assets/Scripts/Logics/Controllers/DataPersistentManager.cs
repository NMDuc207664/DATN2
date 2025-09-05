// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DataPersistentManager : MonoBehaviour
// {
//     public static DataPersistentManager Instance { get; private set; }
//     SaveModel saveModel;
//     private void Awake()
//     {
//         if (Instance != null)
//         {
//             Debug.LogError("There is more than one DataPersistentManager in the scene.");
//         }
//         Instance = this;
//     }
//     public void Start()
//     {
//         LoadGame();
//     }
//     public void NewGame()
//     {
//         saveModel = new SaveModel();
//     }
//     public void LoadGame()
//     {
//         //Todo Load any saved Data form a file using the data handler
//         //if no data can be loaded initialize a new game
//         if (saveModel == null)
//         {
//             Debug.Log("No Data Found");
//             NewGame();
//         }
//         //todo push the loaded data to all other script that need it
//     }
//     public void SaveGame()
//     {
//         //todo pass the data to other scripts so they can update it

//         //todo save that data to a file using the data handler
//     }
//     public void OnApplicationQuit()
//     {

//         SaveGame();
//     }
// }

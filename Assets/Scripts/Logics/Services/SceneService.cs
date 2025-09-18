using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace DATN2.Assets.Scripts.Logics.Services
{
    public class SceneService : ISceneService
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
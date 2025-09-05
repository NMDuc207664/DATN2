using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;

public class PlayerUltilitiesService : IPlayerUltilitiesService
{
    [Inject]
    private readonly GameObject _menuUI;


    public void BackToGame()
    {
        _menuUI.SetActive(false);
    }

    public void OpenIngameMenu()
    {
        _menuUI.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}

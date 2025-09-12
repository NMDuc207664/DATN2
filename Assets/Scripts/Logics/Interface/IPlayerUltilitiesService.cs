using DATN2.Assets.Scripts.Modals.Enum;
namespace DATN2.Assets.Scripts.Logics.Interface
{

    public interface IPlayerUltilitiesService
    {
        [RequireGameState(StateType.Ingame, StateType.OnMenu)]
        void PauseGame();

        [RequireGameState(StateType.Pause, StateType.OnMenu)]
        void ResumeGame();
        [RequireGameState(StateType.OnMenu, StateType.OnLoad, StateType.OnSave)]
        void BackToGame();
        [RequireGameState(StateType.Ingame, StateType.Pause)]
        void OpenIngameMenu();
    }
}
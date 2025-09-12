using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine.SceneManagement;
namespace DATN2.Assets.Scripts.Logics.UI_Services
{
    public class MenuService : IMenuService
    {
        public void HideMenu()
        {
            throw new System.NotImplementedException();
        }

        public void NewGame()
        {
            GameStateManager.Instance.SetState(StateType.Ingame);
            SceneManager.LoadScene("Testing");
        }

        public void ShowMenu()
        {
            throw new System.NotImplementedException();
        }

        public void ToogleMenu()
        {
            GameStateManager.Instance.SetState(StateType.OnMenu);
        }

        public void UnToogleMenu()
        {
            GameStateManager.Instance.SetState(StateType.Ingame);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.UI_Services
{
    public class MenuService : IMenuService
    {
        public void HideMenu()
        {
            throw new System.NotImplementedException();
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
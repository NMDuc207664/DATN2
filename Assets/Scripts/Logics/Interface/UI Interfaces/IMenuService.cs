using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces
{
    public interface IMenuService
    {
        void ShowMenu();
        void HideMenu();
        [RequireGameState(StateType.Ingame, StateType.OnLoad, StateType.OnSave)]
        void ToogleMenu();
        [RequireGameState(StateType.OnMenu)]
        void UnToogleMenu();
        void NewGame();
    }
}
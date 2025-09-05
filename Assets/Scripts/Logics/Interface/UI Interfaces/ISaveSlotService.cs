using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces
{
    public interface ISaveSlotService
    {
        void ShowSaveSlots();
        void HideSaveSlots();
        [RequireGameState(StateType.OnMenu)]
        void ToogleLoadSaveSlot();
        [RequireGameState(StateType.OnLoad, StateType.OnSave)]
        void BackToMenu();
        [RequireGameState(StateType.OnMenu)]
        void ToogleSaveSlot();
    }
}
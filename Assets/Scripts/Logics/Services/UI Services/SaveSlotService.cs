using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.UI_Services
{
    public class SaveSlotService : ISaveSlotService
    {
        public void BackToGame()
        {
            GameStateManager.Instance.SetState(StateType.Ingame);
        }

        public void BackToMenu()
        {
            GameStateManager.Instance.SetState(StateType.OnMenu);
        }

        public void HideSaveSlots()
        {
            throw new System.NotImplementedException();
        }

        public void LoadGame()
        {
            GameStateManager.Instance.SetState(StateType.Ingame);
        }

        public void ShowSaveSlots()
        {
            throw new System.NotImplementedException();
        }

        public void ToogleLoadSaveSlot()
        {
            GameStateManager.Instance.SetState(StateType.OnLoad);
        }

        public void ToogleSaveSlot()
        {
            GameStateManager.Instance.SetState(StateType.OnSave);
        }
    }
}
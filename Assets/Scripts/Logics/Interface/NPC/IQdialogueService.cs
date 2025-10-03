using System;
using DATN2.Assets.Scripts.Data;

namespace DATN2.Assets.Scripts.Logics.Interface.NPC
{
    public interface IQdialogueService
    {
        void StartDialogueAsync(QuestDataSO questDataSO, Action onComplete = null);
        void NextDialogueAsync(QuestDataSO questDataSO, string nodeId, Action onComplete = null);
    }
}
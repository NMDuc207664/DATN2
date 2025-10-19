using System;
using DATN2.Assets.Scripts.Data;

namespace DATN2.Assets.Scripts.Logics.Interface.NPC
{
    public interface IQdialogueService
    {
        event Action OnDialogueComplete;
        void StartDialogueAsync(QuestDataSO questDataSO, Action onComplete = null, int questIndex = 0);
        void NextDialogueAsync(QuestDataSO questDataSO, string nodeId, Action onComplete = null);
        void StartDefaultDialogue(QuestDataSO questDataSO, int questIndex);
    }
}
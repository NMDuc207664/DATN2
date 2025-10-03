using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using UnityEngine;
using VContainer;
namespace DATN2.Assets.Scripts.Logics.Quest_Manager
{
    public class Map1OpeningS : MonoBehaviour, IQuestService
    {
        //[Inject] private IQmoveService moveService;
        [SerializeField] private string mapKey = "Map1";
        [Inject] private IQdialogueService dialogueService;
        public string MapKey => mapKey;


        public void ActivateQuest(string key)
        {
            if (key == "Map_1_OpeningS_Q1")
            {
                QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData(key);
                if (questData != null)
                {
                    foreach (var quest in questData.quests)
                    {
                        if (quest.dialogues != null)
                        {
                            dialogueService.StartDialogueAsync(questData, () =>
                            {
                                // KeyGameStateManager.Instance.PassKey("Map_1_OpeningS_Q1");
                            });
                        }
                    }
                }
            }
        }
    }
}
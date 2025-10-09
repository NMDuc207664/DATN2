using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Data.Runtime;
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
        [SerializeField] private NPCMover npc1_Mover;
        [SerializeField] private NPCMover npc2_Mover;
        [Inject] private IQdialogueService dialogueService;
        public string MapKey => mapKey;


        public void ActivateQuest(string key)
        {
            // Quest 1: Dialogue -> NPC1 di chuyển
            if (key == "Map_1_OpeningS_Q1")
            {
                QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData(key);

                // Bước 1: Chạy dialogue
                dialogueService.StartDialogueAsync(questData, () =>
                {
                    // Bước 2: Sau khi dialogue xong, cho NPC1 di chuyển
                    if (npc1_Mover != null)
                    {
                        string[] moveKeys = new[] {
                                        "NPC(s)_Move_Point_1",
                                        "NPC(s)_Move_Point_2",
                                        "NPC(s)_Move_Point_3",
                                        "NPC(s)_Move_Point_4",
                                        "NPC(s)_Move_Point_5",
                                        // "NPC(s)_Move_Point_6",
                                        // "NPC(s)_Move_Point_7",
                                        // "NPC(s)_Move_Point_8"
                        };
                        // string[] moveKeys2 = moveKeys.Concat(new[] { "NPC(s)_Move_Point_9", "NPC(s)_Move_Point_10" }).ToArray();
                        npc1_Mover.OnReachMoveKey += (reachedKey) =>
                                       {
                                           if (reachedKey == "NPC(s)_Move_Point_3")
                                           {
                                               Debug.Log("[Trigger] NPC1 reached Point_3 → Start Dialogue (QuestIndex=1)");
                                               dialogueService.StartDialogueAsync(questData, null, 1);
                                           }
                                       };

                        npc2_Mover.OnReachMoveKey += (reachedKey) =>
                        {
                            if (reachedKey == "NPC(s)_Move_Point_3")
                            {
                                Debug.Log("[Trigger] NPC2 reached Point_3 → Start Dialogue (QuestIndex=1)");
                                dialogueService.StartDialogueAsync(questData, null, 1);
                            }
                        };
                        npc1_Mover.StartMovementWithKeys(key, moveKeys, 0);
                        npc2_Mover.StartMovementWithKeys(key, moveKeys, 0);

                    }
                }, 0);


            }
        }

    }
}
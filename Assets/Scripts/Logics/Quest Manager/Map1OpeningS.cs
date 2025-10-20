using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CMF;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.Assets.Scripts.Modals.Enum;
using DATN2.GraphviewEditor.Runtime;
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
        [Inject] private Dictionary<string, GameObject> _npc;
        private GameObject npc1;
        private GameObject npc2;
        private IInventoryService inventoryService;
        private DefaultDialogue defaultDialogue;

        [SerializeField] private AdvancedWalkerController playerController;
        public string MapKey => mapKey;

        void Start()
        {
            npc1 = _npc["NPCA_GO"];
            npc2 = _npc["NPCB_GO"];

            InjectDialogueServiceToTriggers();
            SetupNPCTypes();
            KeyGameStateManager.Instance.AddOrChangeGameState(InGameActionType.None);
            inventoryService = VContainerResolver.Resolve<IInventoryService>();

            if (inventoryService is IInventoryService invService)
            {
                invService.OnItemAdded += (ItemModel item) =>
               {
                   OnMainDoorKeyHandler(item);
                   defaultDialogue = npc1.GetComponent<DefaultDialogue>();
                   defaultDialogue.Clear();
               };
            }
        }
        private void SetupNPCTypes()
        {
            // Giả sử NPCA_GO là Tiểu Thư, NPCB_GO là Ga Giang Hồ
            // Bạn có thể thay đổi theo logic game của bạn

            if (npc1 != null)
            {
                var trigger1 = npc1.GetComponent<DTSDialogueTriggerZone>();
                if (trigger1 != null)
                {
                    // Set NPC1 là Tiểu Thư (hoặc tùy theo thiết kế của bạn)
                    trigger1.SetNPCType(DTSDialogueTriggerZone.NPCType.TieuThu);
                    //Debug.Log($"[Map1OpeningS] Set NPC1 ({npc1.name}) as TieuThu");
                }
            }

            if (npc2 != null)
            {
                var trigger2 = npc2.GetComponent<DTSDialogueTriggerZone>();
                if (trigger2 != null)
                {
                    // Set NPC2 là Ga Giang Hồ
                    trigger2.SetNPCType(DTSDialogueTriggerZone.NPCType.GaGiangHo);
                    //Debug.Log($"[Map1OpeningS] Set NPC2 ({npc2.name}) as GaGiangHo");
                }
            }
        }
        private void InjectDialogueServiceToTriggers()
        {
            // Tìm tất cả DTSDialogueTriggerZone trong scene hoặc chỉ trong NPCs
            var triggerZones = new List<DTSDialogueTriggerZone>();

            if (npc1 != null)
            {
                var trigger1 = npc1.GetComponent<DTSDialogueTriggerZone>();
                if (trigger1 != null) triggerZones.Add(trigger1);
            }

            if (npc2 != null)
            {
                var trigger2 = npc2.GetComponent<DTSDialogueTriggerZone>();
                if (trigger2 != null) triggerZones.Add(trigger2);
            }

            foreach (var trigger in triggerZones)
            {
                trigger.SetDialogueService(dialogueService);
            }

            Debug.Log($"[Map1OpeningS] Injected DialogueService to {triggerZones.Count} trigger zones");
        }
        void Update()
        {

            // if (inventoryService != null)
            // {
            //     inventoryService.DebugPrintInventory();
            // }
            // else
            // {
            //     Debug.LogWarning("[Map1OpeningS] InventoryService chưa được khởi tạo!");
            // }
        }

        public void ActivateQuest(string key)
        {
            // Quest 1: Dialogue -> NPC1 di chuyển
            if (key == "Map_1_OpeningS_Q1")
            {
                QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData(key);
                KeyGameStateManager.Instance.AddOrChangeGameState(InGameActionType.OnMovieDialogue);

                // SimpleCinemachineLook.Instance.SetFollowHeadRotation(true);
                string[] goToStartPoint_1 = new[] {
                    "StartPoint_Main_Char"
                };
                playerController.StartAutoMoveWithKeys(key, goToStartPoint_1, 0, 3f, true);

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
                                        "NPC(s)_Move_Point_6",
                                        "NPC(s)_Move_Point_7",
                                        "NPC(s)_Move_Point_8"
                        };
                        string[] moveKeys2 = moveKeys.Concat(new[] { "NPC(s)_Move_Point_9", "NPC(s)_Move_Point_10" }).ToArray();
                        npc1_Mover.OnReachMoveKey += (reachedKey) =>
                                       {
                                           if (reachedKey == "NPC(s)_Move_Point_3")
                                           {
                                               Debug.Log("[Trigger] NPC1 reached Point_3 → Start Dialogue (QuestIndex=1)");
                                               dialogueService.StartDialogueAsync(questData, null, 1);
                                           }
                                       };

                        npc1_Mover.StartMovementWithKeys(key, moveKeys, 0);
                        npc2_Mover.StartMovementWithKeys(key, moveKeys2, 0);
                        string[] playerMoveKeys = new[] {
                            "NPC(s)_Move_Point_1",
                            "NPC(s)_Move_Point_2",
                            "NPC(s)_Move_Point_3",
                            "NPC(s)_Move_Point_4",
                            "NPC(s)_Move_Point_5",
                        };

                        playerController.StartAutoMoveWithKeys(key, playerMoveKeys, 0, 3f, true);
                        StartCoroutine(WaitForPlayerMoveEnd(true));
                        npc2_Mover.OnMoveKeysComplete += () =>
                        {
                            OnNPCMoveCompleteHandler("Map_1_OpeningS_Q1", "Map_1_OpeningS_Q2");
                            ActivateQuest("Map_1_OpeningS_Q2");
                        };
                        // npc2_Mover.OnMoveKeysComplete -= () =>
                        // {
                        //     OnNPCMoveCompleteHandler("Map_1_OpeningS_Q1", "Map_1_OpeningS_Q2");
                        //     ActivateQuest("Map_1_OpeningS_Q2");
                        // };
                    }
                }, 0);
                //InGameControlStateManager.Instance.ClearState();

            }


            if (key == "Map_1_OpeningS_Q2")
            {
                QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData(key);

                npc1.GetComponent<BoxCollider>().enabled = true;
                npc2.GetComponent<BoxCollider>().enabled = true;

                dialogueService.StartDialogueAsync(questData, () =>
                {
                    dialogueService.OnDialogueComplete += () =>
                    {
                        var npcMapping = new Dictionary<GameObject, int>
                        {
                { npc1, 1 },
                { npc2, 0 }
                        };

                        foreach (var entry in npcMapping)
                        {
                            var npc = entry.Key;
                            var questIndex = entry.Value;
                            defaultDialogue = npc.GetComponent<DefaultDialogue>();

                            if (defaultDialogue != null)
                            {
                                defaultDialogue.Setup(questData, questIndex);
                            }
                        }

                        // 🔥 THÊM PHẦN NÀY: kiểm tra nếu người chơi có MainDoorKey thì NPC1 di chuyển thêm
                        // if (inventoryService != null)
                        // {
                        //     var allItems = inventoryService.GetAllItems();
                        //     bool hasMainDoorKey = allItems.Any(item =>
                        //         item._itemData != null &&
                        //         item._itemData.itemName == "Main Door Key");

                        //     if (hasMainDoorKey)
                        //     {
                        //         Debug.Log("[Map1OpeningS] Người chơi có MainDoorKey → NPC1 tiếp tục di chuyển tới điểm 12, 13, 14");

                        //         string[] extraMoveKeys = new[]
                        //         {
                        // "NPC(s)_Move_Point_1",
                        //         };

                        //         if (npc1_Mover != null)
                        //         {
                        //             npc1_Mover.StartMovementWithKeys(key, extraMoveKeys, 1);
                        //         }
                        //     }
                        //     else
                        //     {
                        //         Debug.Log("[Map1OpeningS] Người chơi chưa có MainDoorKey → NPC1 không di chuyển tiếp");
                        //     }
                        // }
                        // else
                        // {
                        //     Debug.LogWarning("[Map1OpeningS] InventoryService chưa được khởi tạo!");
                        // }

                        // Có thể gọi PassKey hay ActivateKey kế tiếp ở đây nếu cần
                        // OnDialogueCompleteHandler("Map_1_OpeningS_Q2", "Map_1_OpeningS_Q3");
                    };
                }, 0);

            }
        }
        private void OnMainDoorKeyHandler(ItemModel item)
        {
            if (item.itemName == "Main Door Key")
            {
                string[] moveKeys = new[]
                {
            "NPC(s)_Move_Point_1",
        };

                if (npc1_Mover != null)
                {
                    npc1_Mover.StartMovementWithKeys("Map_1_OpeningS_Q2", moveKeys, 1);
                }
            }
        }
        private IEnumerator WaitForPlayerMoveEnd(bool moveAgain, string passKey = null, string activeKey = null)
        {
            // Đợi cho đến khi playerController dừng auto move
            while (playerController != null && playerController.isAutoMoving)
                yield return null;

            yield return new WaitForSeconds(0.3f); // Đợi thêm 1 chút để chắc chắn dừng hẳn
            // SimpleCinemachineLook.Instance.SetFollowHeadRotation(false);
            if (passKey != null)
                KeyGameStateManager.Instance.PassKey(passKey);
            if (activeKey != null)
                KeyGameStateManager.Instance.ActivateKey(activeKey);
            if (moveAgain)
            {
                KeyGameStateManager.Instance.AddOrChangeGameState(InGameActionType.None);
            }
        }

        private void OnDialogueCompleteHandler(string passKey = null, string activeKey = null)
        {
            if (passKey != null)
                KeyGameStateManager.Instance.PassKey(passKey);
            if (activeKey != null)
                KeyGameStateManager.Instance.ActivateKey(activeKey);
        }
        private void OnNPCMoveCompleteHandler(string passKey = null, string activeKey = null)
        {
            if (passKey != null)
                KeyGameStateManager.Instance.PassKey(passKey);
            if (activeKey != null)
                KeyGameStateManager.Instance.ActivateKey(activeKey);
        }
        private IEnumerator WaitForSecondsCustom(float seconds, System.Action onComplete = null)
        {
            if (seconds > 0)
                yield return new WaitForSeconds(seconds);

            onComplete?.Invoke();
        }

    }
}


using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.Assets.Scripts.Logics.Quest_Manager;
using System.Linq;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class NPCVContainer : LifetimeScope
    {
        [SerializeField] private GameObject _NPCA;
        [SerializeField] private GameObject _NPCB;
        [SerializeField] private GameObject _Player;
        [SerializeField] private Animator _NPCAanimator;
        [SerializeField] private Animator _NPCBanimator;
        [SerializeField] private Animator _Playeranimator;
        protected override void Configure(IContainerBuilder builder)
        {

            if (_NPCA == null || _NPCB == null || _NPCAanimator == null || _NPCBanimator == null)
            {
                var dictGameObject = new Dictionary<string, GameObject>
            {
                { "NPCA_GO", _NPCA },
                { "NPCB_GO", _NPCB },
                {"Player_GO", _Player },
            };

                var dictAnimator = new Dictionary<string, Animator>
            {
                { "NPCA_Animator", _NPCAanimator },
                { "NPCB_Animator", _NPCBanimator },
                {"Player_Animator", _Playeranimator },
            };

                builder.RegisterInstance(dictGameObject);
                builder.RegisterInstance(dictAnimator);
            }



            //builder.RegisterComponentInHierarchy<Logics.Quest_Manager.Map1OpeningS>();
            var questServices = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IQuestService>()
                .ToList();
            var dictQuestServices = new Dictionary<string, IQuestService>();
            foreach (var service in questServices)
            {
                if (!string.IsNullOrEmpty(service.MapKey))
                {
                    dictQuestServices[service.MapKey] = service;
                }
            }


            builder.Register<QDialogueService>(Lifetime.Scoped).As<IQdialogueService>();
            builder.Register<QMoveService>(Lifetime.Scoped).As<IQmoveService>();

            builder.RegisterEntryPoint<TriggerZoneEntryPoint>();
            // builder.RegisterEntryPoint<QuestServiceEntryPoint>();
            builder.RegisterComponentInHierarchy<Map1OpeningS>();
            builder.RegisterComponentInHierarchy<DialogueManager>();
            builder.RegisterInstance(dictQuestServices);
        }

    }
}

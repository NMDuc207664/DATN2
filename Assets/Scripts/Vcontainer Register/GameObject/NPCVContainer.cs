using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Services;
using DATN2.Assets.Scripts.Modals;
using System.Collections.Generic;
using CMF;
using DATN2.Assets.Scripts.Logics.Interface.NPC;
using DATN2.Assets.Scripts.Logics.Quest_Manager;
using System.Linq;
using DATN2.GraphviewEditor.Runtime;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class NPCVContainer : LifetimeScope
    {
        [SerializeField] private Transform _NPCATransform;
        [SerializeField] private Transform _NPCBTransform;
        [SerializeField] private Rigidbody _NPCArigidbody;
        [SerializeField] private Rigidbody _NPCBrigidbody;
        [SerializeField] private Animator _NPCAanimator;
        [SerializeField] private Animator _NPCBanimator;
        protected override void Configure(IContainerBuilder builder)
        {

            if (_NPCATransform == null || _NPCBTransform == null || _NPCArigidbody == null || _NPCBrigidbody == null || _NPCAanimator == null || _NPCBanimator == null)
            {
                var dictTransform = new Dictionary<string, Transform>
            {
                { "NPCA_Transform", _NPCATransform },
                { "NPCB_Transform", _NPCBTransform },
            };
                var dictRigidbody = new Dictionary<string, Rigidbody>
            {
                { "NPCA_Rigidbody", _NPCArigidbody },
                { "NPCB_Rigidbody", _NPCBrigidbody },
            };
                var dictAnimator = new Dictionary<string, Animator>
            {
                { "NPCA_Animator", _NPCAanimator },
                { "NPCB_Animator", _NPCBanimator },
            };

                builder.RegisterInstance(dictTransform);
                builder.RegisterInstance(dictRigidbody);
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

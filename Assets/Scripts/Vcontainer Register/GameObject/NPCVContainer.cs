using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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

}

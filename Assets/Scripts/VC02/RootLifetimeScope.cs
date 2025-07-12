
using VContainer;
using VContainer.Unity;
using UnityEngine;
namespace DATN2.Assets.Scripts.VC02
{
    public class RootLifetimeScope : LifetimeScope
    {
        // protected override void Awake()
        // {
        //     Debug.Log("RootLifetimeScope Awake");
        //     base.Awake();
        // }
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<HelloWorldManager>(Lifetime.Scoped).AsSelf();
        }
    }
}

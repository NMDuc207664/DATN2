using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class InGameMenuVContainer : LifetimeScope
    {
        [SerializeField] private GameObject _menuUI;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_menuUI);
            builder.Register<PlayerUltilitiesService>(Lifetime.Scoped).As<IPlayerUltilitiesService>();

            builder.RegisterComponentInHierarchy<Logics.Controllers.PlayerUltilitiesController>();
        }
    }
}
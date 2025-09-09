using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Services;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class CharacterLifetimeScope : LifetimeScope
    {

        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Animator _animator;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_animator);
            builder.RegisterComponent(_playerTransform);
            builder.RegisterComponent(_rigidbody);
            builder.RegisterComponent(_playerCamera);

            builder.Register<MovementService>(Lifetime.Scoped).As<IMovement>();
            builder.Register<InventoryService>(Lifetime.Scoped).As<IInventoryService>();
            builder.Register<CameraService>(Lifetime.Scoped).As<ICameraService>();
            builder.Register<PlayerUltilitiesService>(Lifetime.Scoped).As<IPlayerUltilitiesService>();
            builder.RegisterComponentInHierarchy<Logics.Controllers.CharacterController>();
        }
    }
}
using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Services;
using System.Collections.Generic;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class CharacterLifetimeScope : LifetimeScope
    {

        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _orientationTransform;
        [SerializeField] private Collider _playerCollider;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_animator);
            builder.RegisterComponent(_playerCollider);
            // builder.RegisterComponent(_playerTransform);
            builder.RegisterComponent(_rigidbody);
            builder.RegisterComponent(_playerCamera);
            var dict = new Dictionary<string, GameObject>
            {
                { "Player", _player },
            };
            var dictTransform = new Dictionary<string, Transform>
            {
                { "OrientationTransform", _orientationTransform },
                { "PlayerTransform", _playerTransform },
            };
            builder.RegisterInstance(dict);
            builder.RegisterInstance(dictTransform);

            builder.Register<MovementService>(Lifetime.Scoped).As<IMovement>();
            builder.Register<InventoryService>(Lifetime.Scoped).As<IInventoryService>();
            builder.Register<CameraService>(Lifetime.Scoped).As<ICameraService>();
            builder.Register<SceneService>(Lifetime.Scoped).As<ISceneService>();
            // builder.Register<PlayerUltilitiesService>(Lifetime.Scoped).As<IPlayerUltilitiesService>();
            //builder.RegisterComponentInHierarchy<DoorInteraction>();
            builder.RegisterComponentInHierarchy<Logics.Controllers.CharacterController>();
            builder.RegisterComponentInHierarchy<Logics.Controllers.CameraController>();

            builder.RegisterEntryPoint<DoorEntryPoint>();
        }
    }
}
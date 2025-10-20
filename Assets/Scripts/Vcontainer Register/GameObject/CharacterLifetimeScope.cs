using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Services;
using DATN2.Assets.Scripts.Modals;
using System.Collections.Generic;
using CMF;
using Cinemachine;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class CharacterLifetimeScope : LifetimeScope
    {

        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private CinemachineVirtualCamera _playerVirtualCamera_1;
        [SerializeField] private Animator _animator_player;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _arm;
        [SerializeField] private Transform _orientationTransform;

        protected override void Configure(IContainerBuilder builder)
        {

            builder.RegisterComponent(_animator_player);
            builder.RegisterComponent(_rigidbody);
            builder.RegisterComponent(_playerCamera);
            var dictCamera = new Dictionary<string, CinemachineVirtualCamera>
            {
                { "Player_camera#1", _playerVirtualCamera_1 },

            };
            var dict = new Dictionary<string, GameObject>
            {
                { "Player", _player },
                { "Player_arm", _arm },
            };
            var dictTransform = new Dictionary<string, Transform>
            {
                { "OrientationTransform", _orientationTransform },
                { "PlayerTransform", _playerTransform },
            };

            //builder.RegisterInstance(dictAnimation);
            builder.RegisterInstance(dict);
            builder.RegisterInstance(dictTransform);
            builder.RegisterInstance(dictCamera);

            builder.Register<MovementService>(Lifetime.Scoped).As<IMovement>();
            builder.Register<InventoryService>(Lifetime.Scoped).As<IInventoryService>();
            builder.Register<CameraService>(Lifetime.Scoped).As<ICameraService>();
            builder.Register<SceneService>(Lifetime.Scoped).As<ISceneService>();
            // builder.Register<PlayerUltilitiesService>(Lifetime.Scoped).As<IPlayerUltilitiesService>();
            builder.RegisterComponentInHierarchy<Logics.Controllers.CharacterController>();
            builder.RegisterComponentInHierarchy<Logics.Controllers.CameraController>();
            builder.RegisterComponentInHierarchy<AdvancedWalkerController>();

            builder.RegisterEntryPoint<DoorEntryPoint>();

            builder.RegisterBuildCallback(container =>
            {
                VContainerResolver.Initialize(container);
            });
        }
    }
}
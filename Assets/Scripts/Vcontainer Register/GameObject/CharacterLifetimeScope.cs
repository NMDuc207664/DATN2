using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Services;

public class CharacterLifetimeScope : LifetimeScope
{

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody _rigidbody;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_playerTransform);
        builder.RegisterComponent(_rigidbody);
        builder.Register<MovementService>(Lifetime.Scoped).As<IMovement>();
        builder.RegisterComponentInHierarchy<DATN2.Assets.Scripts.Logics.Controllers.CharacterController>();




    }
}

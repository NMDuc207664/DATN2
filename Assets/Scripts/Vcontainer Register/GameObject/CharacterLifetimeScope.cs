using DATN2.Assets.Scripts.Logics.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DATN2.Assets.Scripts.Logics.Services;
using System.Collections.Generic;
using DATN2.Scripts.BehaviorEditor.Models;

public class CharacterLifetimeScope : LifetimeScope
{

    [SerializeField] private Transform _playerTransform;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_playerTransform);
        builder.Register<MovementService>(Lifetime.Scoped).As<IMovement>();
        builder.RegisterComponentInHierarchy<DATN2.Assets.Scripts.Logics.Controllers.CharacterController>();




    }
}

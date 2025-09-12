
using VContainer;
using VContainer.Unity;
using UnityEngine;
using DATN2.Assets.Scripts.Logics.Interface;
using DATN2.Assets.Scripts.Logics.Services;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Logics.UI_Services;
using DATN2.Assets.Scripts.Logics.Interface.UI_Interfaces;
using DATN2.Assets.Scripts.Modals.Enum;
namespace DATN2.Assets.Scripts.VContainerRegister
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private bool useEncryption = false;
        [SerializeField] private string savePath;

        protected override void Awake()
        {
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = System.IO.Path.Combine(Application.persistentDataPath, "Saves");
            }
            DontDestroyOnLoad(gameObject);
            Debug.Log("just print");
            base.Awake();
        }
        protected override void Configure(IContainerBuilder builder)
        {
            // builder.RegisterEntryPoint<HelloWorldManager>(Lifetime.Scoped).AsSelf();
            builder.Register<IFileHandler<SaveModel>>(
        _ => new JsonFileHandler<SaveModel>(savePath, useEncryption),
        Lifetime.Singleton
        );
            builder.Register<MenuService>(Lifetime.Scoped).As<IMenuService>();
            builder.Register<SaveSlotService>(Lifetime.Scoped).As<ISaveSlotService>();
            builder.Register<SaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.RegisterComponentInHierarchy<Logics.Controllers.SaveAndLoadController>();
            // builder.RegisterEntryPoint<SaveAndLoadController>(Lifetime.Singleton);

        }
    }
}

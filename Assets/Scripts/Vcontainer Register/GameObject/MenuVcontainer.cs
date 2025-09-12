using DATN2.Assets.Scripts.Logics.Controllers;
using VContainer;
using VContainer.Unity;

public class MenuVcontainer : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<Menu>();
        builder.RegisterComponentInHierarchy<SaveSlotMenu>();
    }
}

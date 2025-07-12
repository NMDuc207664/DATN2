using DATN2.Assets.Scripts;
using VContainer;
using VContainer.Unity;

public class HelloWorldLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<HelloWorldService>(Lifetime.Singleton);//only have one of this service
        builder.RegisterEntryPoint<HelloWorldPresenter>();
        builder.RegisterComponent(GetComponent<HelloWorldModel>());
    }
}

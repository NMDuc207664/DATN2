
using System.Collections.Generic;
using DATN2.Assets.Scripts.BehaviorEditor.Controllers;
using DATN2.Assets.Scripts.BehaviorEditor.Interfaces;
using DATN2.Assets.Scripts.BehaviorEditor.Services;
using DATN2.Scripts.BehaviorEditor.Models;
using VContainer;

public class DependencyInjector
{
    public static void ConfigureEditorDependencies(BehaviorEditor editor, List<BaseNode> windows)
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Register<NodeService>(Lifetime.Singleton).As<INodeService>().WithParameter(windows);
        containerBuilder.Register<EditorInteractionService>(Lifetime.Singleton).As<IEditorInteractionService>();
        var container = containerBuilder.Build();
        container.Inject(editor);
    }
}

using VContainer;
using VContainer.Unity;
using UnityEngine;

public static class VContainerResolver
{
    private static IObjectResolver _resolver;

    public static void Initialize(IObjectResolver resolver)
    {
        _resolver = resolver;
        Debug.Log("[VContainerResolver] Initialized.");
    }

    public static T Resolve<T>() where T : class
    {
        if (_resolver == null)
        {
            Debug.LogError("[VContainerResolver] Resolver chưa được khởi tạo!");
            return null;
        }
        return _resolver.Resolve<T>();
    }
}

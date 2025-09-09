using System.Linq;
using System.Reflection;
using UnityEngine;

public static class GameStateInvoker
{
    public static void TryInvoke<T>(T target, string methodName, params object[] parameters)
    {
        // var method = typeof(T).GetMethod(methodName);
        var method = target.GetType().GetMethod(methodName);
        if (method == null)
        {
            Debug.LogError($"Method {methodName} not found on type {typeof(T).Name}");
            return;
        }

        var attr = method.GetCustomAttribute<RequireGameStateAttribute>();
        if (attr != null)
        {
            var current = GameStateManager.Instance.GetCurrentState();
            if (!attr.AllowedStates.Contains(current))
            {
                Debug.LogWarning($"Cannot invoke {methodName}. Current state is {current}");
                return;
            }
        }

        method.Invoke(target, parameters);
    }
}

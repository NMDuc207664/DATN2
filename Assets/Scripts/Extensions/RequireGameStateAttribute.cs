using System;
using DATN2.Assets.Scripts.Modals.Enum;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RequireGameStateAttribute : Attribute
{
    public StateType[] AllowedStates { get; }

    public RequireGameStateAttribute(params StateType[] allowedStates)
    {
        AllowedStates = allowedStates;
    }
}
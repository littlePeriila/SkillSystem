using System;
using System.Collections.Generic;

/// <summary>位移策略工厂 — 注册并创建策略实例</summary>
public static class DisplacementFactory
{
    private static readonly Dictionary<DisplacementType, Func<IDisplacement>> _creators = new()
    {
        { DisplacementType.Blink,      () => new BlinkDisplacement() },
        { DisplacementType.Dash,       () => new DashDisplacement() },
        { DisplacementType.Parabolic,  () => new ParabolicDisplacement() },
        { DisplacementType.MultiStage, () => new MultiStageDisplacement() },
    };

    public static void Register(DisplacementType type, Func<IDisplacement> creator)
    {
        _creators[type] = creator;
    }

    public static IDisplacement Create(DisplacementType type)
    {
        return _creators.TryGetValue(type, out var creator) ? creator() : null;
    }
}

using System;
using System.Collections.Generic;

/// <summary>Buff 策略工厂 — 注册并缓存所有 IBuffEffect 实例</summary>
public static class BuffFactory
{
    private static readonly Dictionary<BuffType, IBuffEffect> _cache = new Dictionary<BuffType, IBuffEffect>();

    static BuffFactory()
    {
        // 注册所有 buff 策略 — 新增 buff 只需在此注册一行
        Register(new BurnBuffEffect());
        Register(new PoisonBuffEffect());
        Register(new LightBuffEffect());
        Register(new SlowBuffEffect());
        Register(new StunBuffEffect());
        Register(new BeatBackBuffEffect());
        Register(new BeatUpBuffEffect());
        Register(new PullBuffEffect());
        Register(new AddDefenceBuffEffect());
        Register(new RecoverHpBuffEffect());
    }

    public static void Register(IBuffEffect effect)
    {
        _cache[effect.BuffType] = effect;
    }

    public static IBuffEffect Create(BuffType type)
    {
        _cache.TryGetValue(type, out var effect);
        return effect;
    }
}

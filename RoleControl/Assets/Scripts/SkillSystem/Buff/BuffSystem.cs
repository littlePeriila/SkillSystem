using UnityEngine;

/// <summary>Buff 施加入口 — 对外统一接口</summary>
public static class BuffSystem
{
    /// <summary>施加 Buff（已有则刷新持续时间）</summary>
    public static void ApplyBuff(GameObject target, BuffType buffType,
        float duration, float value, float interval, GameObject caster = null)
    {
        // 位移类 buff 打断目标技能施放
        if (buffType == BuffType.BeatBack || buffType == BuffType.BeatUp || buffType == BuffType.Pull)
        {
            var ctrl = target.GetComponent<SkillCastController>();
            if (ctrl != null && ctrl.IsCasting)
                ctrl.Interrupt(InterruptType.Displace);
        }

        var existingBuffs = target.GetComponents<BuffRun>();
        foreach (var existing in existingBuffs)
        {
            if (existing.bufftype == buffType)
            {
                existing.Reset();
                return;
            }
        }

        var buffRun = target.AddComponent<BuffRun>();
        buffRun.InitBuff(buffType, duration, value, interval, caster);
    }

    /// <summary>施加多个 Buff</summary>
    public static void ApplyBuffs(GameObject target, BuffType[] buffTypes,
        float duration, float value, float interval, GameObject caster = null)
    {
        if (buffTypes == null) return;
        foreach (var buff in buffTypes)
            ApplyBuff(target, buff, duration, value, interval, caster);
    }

    /// <summary>施加 Buff 并发布 UI 事件（头像显示 + Buff 图标）</summary>
    public static void ApplyBuffWithEvents(GameObject target, BuffType[] buffTypes,
        float duration, float value, float interval, GameObject caster = null, bool hidePortraits = true)
    {
        if (buffTypes == null || buffTypes.Length == 0) return;

        if (hidePortraits)
            ObserverMa.I.Notify(SkillEventKeys.PortraitsHide);
        ObserverMa.I.Notify(SkillEventKeys.PortraitShow, target);

        ApplyBuffs(target, buffTypes, duration, value, interval, caster);

        foreach (var buff in buffTypes)
        {
            ObserverMa.I.Notify(SkillEventKeys.BuffApplied,
                new BuffAppliedArgs { Target = target, BuffType = buff, Duration = duration });
        }
    }

    /// <summary>对单个目标施加单个 Buff 并发布 UI 事件</summary>
    public static void ApplyBuffWithEvents(GameObject target, BuffType buffType,
        float duration, float value, float interval, GameObject caster = null, bool hidePortraits = true)
    {
        if (hidePortraits)
            ObserverMa.I.Notify(SkillEventKeys.PortraitsHide);
        ObserverMa.I.Notify(SkillEventKeys.PortraitShow, target);

        ApplyBuff(target, buffType, duration, value, interval, caster);

        ObserverMa.I.Notify(SkillEventKeys.BuffApplied,
            new BuffAppliedArgs { Target = target, BuffType = buffType, Duration = duration });
    }
}

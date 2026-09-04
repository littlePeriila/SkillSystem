using System.Collections.Generic;

/// <summary>Buff 图标名称映射 — 从 SkillDeployer 中提取</summary>
public static class BuffIconMapping
{
    private static readonly Dictionary<BuffType, string> _iconNames = new Dictionary<BuffType, string>
    {
        { BuffType.Burn,        "Buff_13" },
        { BuffType.Slow,        "Buff_15" },
        { BuffType.Stun,        "Buff_12" },
        { BuffType.Poison,      "Buff_14" },
        { BuffType.BeatBack,    "Buff_5"  },
        { BuffType.BeatUp,      "Buff_4"  },
        { BuffType.Pull,        "Buff_6"  },
        { BuffType.AddDefence,  "Buff_3"  },
        { BuffType.RecoverHp,   "Buff_7"  },
        { BuffType.Light,       "Buff_8"  },
    };

    public static string GetIconName(BuffType type)
    {
        _iconNames.TryGetValue(type, out var name);
        return name;
    }
}

using UnityEngine;

/// <summary>伤害计算策略接口 — 可替换为暴击、穿透等不同公式</summary>
public interface IDamageCalculator
{
    /// <summary>计算伤害值（0 表示 Miss）</summary>
    float Calculate(GameObject attacker, GameObject target, SkillData skillData);
}

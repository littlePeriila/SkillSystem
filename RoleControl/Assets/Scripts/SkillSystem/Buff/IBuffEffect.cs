using UnityEngine;

/// <summary>Buff 效果策略接口 — 每种 Buff 类型对应一个实现</summary>
public interface IBuffEffect
{
    BuffType BuffType { get; }

    /// <summary>每次 tick 执行的效果</summary>
    /// <param name="caster">施法者（用于方向计算，可为 null）</param>
    void Apply(CharacterStatus target, float value, GameObject caster);

    /// <summary>Buff 移除时的清理（如恢复属性）</summary>
    void OnRemove(CharacterStatus target, float value);

    /// <summary>关联的特效预制体名（null 表示无特效）</summary>
    string FxPrefabName { get; }

    /// <summary>特效挂点：true=角色根节点，false=HitFxPos</summary>
    bool FxOnRoot { get; }
}

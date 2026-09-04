using UnityEngine;

/// <summary>增加防御 — Buff 结束时恢复</summary>
public class AddDefenceBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.AddDefence;
    public string FxPrefabName => "FX_CHAR_Aura";
    public bool FxOnRoot => true;

    public void Apply(CharacterStatus target, float value, GameObject caster)
        => target.defence += value;

    public void OnRemove(CharacterStatus target, float value)
        => target.defence -= value;
}

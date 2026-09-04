using UnityEngine;

/// <summary>点燃 — 持续伤害</summary>
public class BurnBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.Burn;
    public string FxPrefabName => "Skill_32_R_Fly_100";
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
        => target.OnDamage(value, null, true);

    public void OnRemove(CharacterStatus target, float value) { }
}

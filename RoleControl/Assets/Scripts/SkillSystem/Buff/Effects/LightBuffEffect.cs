using UnityEngine;

/// <summary>感电 — 持续伤害</summary>
public class LightBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.Light;
    public string FxPrefabName => "Skill_75_Cast";
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
        => target.OnDamage(value, null, true);

    public void OnRemove(CharacterStatus target, float value) { }
}

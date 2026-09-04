using UnityEngine;

/// <summary>中毒 — 持续伤害</summary>
public class PoisonBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.Poison;
    public string FxPrefabName => "Skill_12_R_Fly_100";
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
        => target.OnDamage(value, null, true);

    public void OnRemove(CharacterStatus target, float value) { }
}

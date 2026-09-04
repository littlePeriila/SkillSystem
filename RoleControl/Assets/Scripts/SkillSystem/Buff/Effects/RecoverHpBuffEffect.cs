using UnityEngine;

/// <summary>回复生命 — 持续治疗</summary>
public class RecoverHpBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.RecoverHp;
    public string FxPrefabName => "FX_Heal_Light_Cast";
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
        => target.OnDamage(-value, null, true);

    public void OnRemove(CharacterStatus target, float value) { }
}

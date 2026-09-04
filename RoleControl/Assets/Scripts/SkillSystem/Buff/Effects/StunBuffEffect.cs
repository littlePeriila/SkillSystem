using UnityEngine;

/// <summary>眩晕 — 打断目标技能施放，持续期间角色无法行动</summary>
public class StunBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.Stun;
    public string FxPrefabName => null;
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
    {
        // 打断目标正在施放的技能
        var ctrl = target.GetComponent<SkillCastController>();
        if (ctrl != null && ctrl.IsCasting)
            ctrl.Interrupt(InterruptType.Stun);
    }

    public void OnRemove(CharacterStatus target, float value) { }
}

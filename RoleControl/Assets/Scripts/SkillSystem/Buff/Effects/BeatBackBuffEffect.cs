using UnityEngine;
using DG.Tweening;

/// <summary>击退 — 向远离施法者方向位移</summary>
public class BeatBackBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.BeatBack;
    public string FxPrefabName => null;
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
    {
        if (caster == null) return;

        Vector3 dir = (target.transform.position - caster.transform.position).normalized;
        dir.y = 0;
        target.transform.DOMove(target.transform.position + dir * value, 0.5f);
    }

    public void OnRemove(CharacterStatus target, float value) { }
}

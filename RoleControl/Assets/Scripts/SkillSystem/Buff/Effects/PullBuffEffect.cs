using UnityEngine;
using DG.Tweening;

/// <summary>拉拽 — 向施法者方向位移</summary>
public class PullBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.Pull;
    public string FxPrefabName => null;
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
    {
        if (caster == null) return;

        Vector3 dir = (caster.transform.position - target.transform.position).normalized;
        dir.y = 0;
        target.transform.DOMove(target.transform.position + dir * value, 0.5f);
    }

    public void OnRemove(CharacterStatus target, float value) { }
}

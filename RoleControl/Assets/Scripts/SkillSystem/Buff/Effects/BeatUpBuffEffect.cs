using UnityEngine;
using DG.Tweening;

/// <summary>击飞 — 向上位移</summary>
public class BeatUpBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.BeatUp;
    public string FxPrefabName => null;
    public bool FxOnRoot => false;

    public void Apply(CharacterStatus target, float value, GameObject caster)
    {
        target.transform.DOMove(target.transform.position + Vector3.up * value, 0.5f);
    }

    public void OnRemove(CharacterStatus target, float value) { }
}

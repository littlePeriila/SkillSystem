using UnityEngine;

/// <summary>减速 — 持续特效挂在角色根节点</summary>
public class SlowBuffEffect : IBuffEffect
{
    public BuffType BuffType => BuffType.Slow;
    public string FxPrefabName => "Skill_21_R_Fly_100";
    public bool FxOnRoot => true;

    public void Apply(CharacterStatus target, float value, GameObject caster)
    {
        // 减速逻辑可扩展为修改 Movement 组件的速度系数
    }

    public void OnRemove(CharacterStatus target, float value) { }
}

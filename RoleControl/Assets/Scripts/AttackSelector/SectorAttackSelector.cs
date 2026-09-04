using UnityEngine;

/// <summary>扇形攻击选择器</summary>
class SectorAttackSelector : BaseAttackSelector
{
    protected override bool IsInAttackRange(SkillData skillData, Transform skillTransform, GameObject target)
    {
        float angle = Vector3.Angle(skillTransform.forward, target.transform.position - skillTransform.position);
        return angle <= skillData.skill.attackAngle / 2f;
    }
}

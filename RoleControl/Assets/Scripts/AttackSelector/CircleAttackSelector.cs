using UnityEngine;

/// <summary>圆形攻击选择器</summary>
class CircleAttackSelector : BaseAttackSelector
{
    // OverlapSphere 已做圆形检测，无需额外判定
    protected override bool IsInAttackRange(SkillData skillData, Transform skillTransform, GameObject target)
        => true;
}

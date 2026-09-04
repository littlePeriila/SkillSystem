using UnityEngine;

/// <summary>线性（矩形）攻击选择器 — 使用局部坐标判定，跟随角色旋转</summary>
class LineAttackSelector : BaseAttackSelector
{
    protected override bool IsInAttackRange(SkillData skillData, Transform skillTransform, GameObject target)
    {
        // 转换到技能发出者的局部坐标系，z=前方距离, x=侧向宽度
        Vector3 localPos = skillTransform.InverseTransformPoint(target.transform.position);
        return Mathf.Abs(localPos.z) <= skillData.skill.attackDisntance
               && Mathf.Abs(localPos.x) <= skillData.skill.attackWidth / 2f;
    }
}

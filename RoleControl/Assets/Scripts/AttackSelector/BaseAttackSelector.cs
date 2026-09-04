using System;
using UnityEngine;

/// <summary>攻击目标选择器基类 — 封装共用逻辑，子类只需实现形状判定</summary>
public abstract class BaseAttackSelector : IAttackSelector
{
    public GameObject[] SelectTarget(SkillData skillData, Transform skillTransform)
    {
        // 1. 球形射线检测
        var colliders = Physics.OverlapSphere(skillTransform.position, skillData.skill.attackDisntance);
        if (colliders == null || colliders.Length == 0) return null;

        // 2. 转换为 GameObject 并按条件过滤
        string[] attTags = skillData.skill.attckTargetTags;
        var array = CollectionHelper.Select<Collider, GameObject>(colliders, p => p.gameObject);
        array = CollectionHelper.FindAll<GameObject>(array,
            p => Array.IndexOf(attTags, p.tag) >= 0
                 && p.GetComponent<CharacterStatus>().HP > 0
                 && IsInAttackRange(skillData, skillTransform, p));

        if (array == null || array.Length == 0) return null;

        // 3. 按距离升序排列
        CollectionHelper.OrderBy<GameObject, float>(array,
            p => Vector3.Distance(skillData.Owner.transform.position, p.transform.position));

        // 4. 根据 attackNum 决定返回数量
        int attackNum = skillData.skill.attackNum;
        if (attackNum == 1)
            return new GameObject[] { array[0] };

        if (attackNum >= array.Length)
            return array;

        // 5. 取前 N 个（修复原代码 targets 未初始化导致 NPE）
        var targets = new GameObject[attackNum];
        Array.Copy(array, targets, attackNum);
        return targets;
    }

    /// <summary>形状判定 — 子类实现（扇形/圆形/线性）</summary>
    protected abstract bool IsInAttackRange(SkillData skillData, Transform skillTransform, GameObject target);
}

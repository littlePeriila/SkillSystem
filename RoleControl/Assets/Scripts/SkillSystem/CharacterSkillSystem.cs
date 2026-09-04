using System;
using UnityEngine;

/// <summary>
/// 角色技能系统 — 技能调度、连击、目标选择、事件发布
/// </summary>
[RequireComponent(typeof(CharacterSkillManager))]
public class CharacterSkillSystem : MonoBehaviour
{
    public CharacterSkillManager chSkillMgr;

    private SkillData currentUseSkill;
    private Transform currentSelectedTarget;
    private SkillCastController _castController;

    public void Start()
    {
        chSkillMgr = GetComponent<CharacterSkillManager>();
        _castController = GetComponent<SkillCastController>();
    }

    /// <summary>
    /// 使用指定技能
    /// </summary>
    /// <param name="skillid">技能编号</param>
    /// <param name="isBatter">是否连击</param>
    public void AttackUseSkill(int skillid, bool isBatter = false)
    {
        // 施放中不可重复施放
        if (_castController != null && _castController.IsCasting) return;

        if (currentUseSkill != null && isBatter)
            skillid = currentUseSkill.skill.nextBatterId;

        currentUseSkill = chSkillMgr.PrepareSkill(skillid);
        if (currentUseSkill == null) return;

        // Select 类型技能：需选中目标
        if ((currentUseSkill.skill.damageType & DamageType.Select) == DamageType.Select)
        {
            var selectedTarget = SelectTarget();
            if (currentUseSkill.skill.attckTargetTags.Contains("Player"))
                selectedTarget = gameObject;

            if (selectedTarget == null) return;

            UpdateSelectedTarget(selectedTarget.transform);

            // Buff 技能：对选中目标施加 Buff
            if ((currentUseSkill.skill.damageType & DamageType.Buff) == DamageType.Buff)
            {
                ApplyBuffToTarget(selectedTarget);
            }
        }

        // 委托 SkillCastController 管理三阶段施放
        chSkillMgr.DeploySkill(currentUseSkill);
    }

    /// <summary>更新选中目标视觉 + 事件发布</summary>
    private void UpdateSelectedTarget(Transform newTarget)
    {
        if (currentSelectedTarget != null)
            currentSelectedTarget.GetComponent<CharacterStatus>().selected.SetActive(false);

        currentSelectedTarget = newTarget;
        currentSelectedTarget.GetComponent<CharacterStatus>().selected.SetActive(true);

        ObserverMa.I.Notify(SkillEventKeys.TargetSelected,
            new TargetSelectedArgs { Target = currentSelectedTarget.gameObject });
    }

    /// <summary>对目标施加 Buff — 通过 BuffSystem 统一处理</summary>
    private void ApplyBuffToTarget(GameObject target)
    {
        if (currentUseSkill.skill.buffType == null) return;

        foreach (var buff in currentUseSkill.skill.buffType)
        {
            bool hidePortraits = !target.CompareTag("Player");
            BuffSystem.ApplyBuffWithEvents(target, buff,
                currentUseSkill.skill.buffDuration,
                currentUseSkill.skill.buffValue,
                currentUseSkill.skill.buffInterval,
                gameObject,
                hidePortraits);
        }
    }

    /// <summary>
    /// 随机选择技能
    /// </summary>
    public void RandomSelectSkill()
    {
        if (chSkillMgr.skills.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, chSkillMgr.skills.Count);
            currentUseSkill = chSkillMgr.PrepareSkill(chSkillMgr.skills[index].skill.skillID);
            if (currentUseSkill == null)
                currentUseSkill = chSkillMgr.skills[0];
        }
    }

    private GameObject SelectTarget()
    {
        var colliders = Physics.OverlapSphere(transform.position, currentUseSkill.skill.attackDisntance);
        if (colliders == null || colliders.Length == 0) return null;

        String[] attTags = currentUseSkill.skill.attckTargetTags;
        var array = CollectionHelper.Select<Collider, GameObject>(colliders, p => p.gameObject);

        array = CollectionHelper.FindAll<GameObject>(array,
            p => Array.IndexOf(attTags, p.tag) >= 0
                 && p.GetComponent<CharacterStatus>().HP > 0 &&
                 Vector3.Angle(transform.forward, p.transform.position - transform.position) <= 90);

        if (array == null || array.Length == 0) return null;

        CollectionHelper.OrderBy<GameObject, float>(array,
            p => Vector3.Distance(transform.position, p.transform.position));
        return array[0];
    }
}

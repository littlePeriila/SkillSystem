using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 技能管理类 — 冷却、SP检查、VFX部署
/// </summary>
[RequireComponent(typeof(CharacterSkillSystem))]
public class CharacterSkillManager : MonoBehaviour
{
    /// <summary>管理所有技能的容器</summary>
    public List<SkillData> skills = new List<SkillData>();

    /// <summary>技能列表配置（Inspector 中指定，为空时使用默认配置）</summary>
    public SkillListConfig skillConfig;

    private CharacterStatus chStatus;
    private SkillCastController _castController;

    /// <summary>默认技能路径（配置为空时的回退方案，保持向后兼容）</summary>
    private static readonly string[] DefaultSkillPaths = { "Skill_1", "Skill_2", "Skill_3", "Skill_4", "Skill_5" };

    private void AddSkill(string path)
    {
        SkillTemp skTemp = Instantiate(Resources.Load<SkillTemp>(path));
        Skill sk = LoadSkill(skTemp);
        SkillData skd = new SkillData { skill = sk };
        skills.Add(skd);
    }

    public void Start()
    {
        chStatus = GetComponent<CharacterStatus>();
        _castController = GetComponent<SkillCastController>();

        // 配置驱动加载：优先使用 Inspector 配置，回退到默认
        var paths = skillConfig != null ? skillConfig.skillPaths : DefaultSkillPaths;
        foreach (var path in paths)
            AddSkill(path);

        // 预加载技能特效预制体到对象池
        foreach (var item in skills)
        {
            if (item.skillPrefab == null && !string.IsNullOrEmpty(item.skill.prefabName))
                item.skillPrefab = LoadFxPrefab("Skill/" + item.skill.prefabName);

            if (item.hitFxPrefab == null && !string.IsNullOrEmpty(item.skill.hitFxName))
                item.hitFxPrefab = LoadFxPrefab("Skill/" + item.skill.hitFxName);
        }
    }

    private GameObject LoadFxPrefab(string path)
    {
        var key = path.Substring(path.LastIndexOf("/") + 1);
        var go = Resources.Load<GameObject>(path);
        GameObjectPool.I.Destory(
            GameObjectPool.I.CreateObject(key, go, transform.position, transform.rotation));
        return go;
    }

    /// <summary>准备技能</summary>
    public SkillData PrepareSkill(int id)
    {
        var skillData = skills.Find(p => p.skill.skillID == id);
        if (skillData != null &&
            chStatus.SP >= skillData.skill.costSP &&
            skillData.coolRemain == 0)
        {
            skillData.Owner = gameObject;
            return skillData;
        }
        return null;
    }

    /// <summary>释放技能（启动冷却 + 委托施放控制器管理时序）</summary>
    public void DeploySkill(SkillData skillData)
    {
        StartCoroutine(CoolTimeDown(skillData));

        // 有 SkillCastController 时走三阶段施放，否则直接执行（向后兼容）
        if (_castController != null)
            _castController.StartCast(skillData);
        else
            DeploySkillInternal(skillData);
    }

    /// <summary>实际执行 VFX 部署（由 SkillCastController 在 Casting 阶段调用）</summary>
    public void ExecuteDeploy(SkillData skillData)
    {
        DeploySkillInternal(skillData);
    }

    /// <summary>技能部署共用逻辑</summary>
    private void DeploySkillInternal(SkillData skillData)
    {
        GameObject tempGo = CreateSkillPrefab(skillData);
        if (tempGo == null) return;

        var deployer = tempGo.GetComponent<SkillDeployer>();
        if (deployer == null)
            deployer = tempGo.AddComponent<SkillDeployer>();

        deployer.skillData = skillData;
        deployer.DeploySkill();

        if ((skillData.skill.damageType & DamageType.Bullet) != DamageType.Bullet)
        {
            float destroyDelay = skillData.skill.durationTime > 0
                ? skillData.skill.durationTime
                : 0.5f;
            GameObjectPool.I.Destory(tempGo, destroyDelay);
        }
    }

    /// <summary>创建技能预制体（处理偏移/发射点）</summary>
    private GameObject CreateSkillPrefab(SkillData skillData)
    {
        if ((skillData.skill.damageType & DamageType.FxOffset) == DamageType.FxOffset)
            return GameObjectPool.I.CreateObject(skillData.skill.prefabName, skillData.skillPrefab,
                transform.position + transform.forward * skillData.skill.fxOffset, transform.rotation);

        if ((skillData.skill.damageType & DamageType.FirePos) == DamageType.FirePos)
            return GameObjectPool.I.CreateObject(skillData.skill.prefabName, skillData.skillPrefab,
                chStatus.FirePos.position, chStatus.FirePos.rotation);

        return null;
    }

    public IEnumerator CoolTimeDown(SkillData skillData)
    {
        skillData.coolRemain = skillData.skill.coolTime;
        while (skillData.coolRemain > 0)
        {
            yield return new WaitForSeconds(0.1f);
            skillData.coolRemain -= 0.1f;
        }
        skillData.coolRemain = 0;
    }

    public float GetSkillCoolRemain(int id)
    {
        var skillData = skills.Find(p => p.skill.skillID == id);
        return skillData != null ? skillData.coolRemain : 0f;
    }

    private Skill LoadSkill(SkillTemp skillTemp)
    {
        Skill sk = skillTemp.skill;
        foreach (var dt in skillTemp.damageType)
            sk.damageType |= dt;
        return sk;
    }
}

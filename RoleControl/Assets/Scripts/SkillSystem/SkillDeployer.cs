using System.Collections;
using UnityEngine;

public class SkillDeployer : MonoBehaviour
{
    private SkillData m_skillData;

    /// <summary>敌人选区策略</summary>
    public IAttackSelector attackTargetSelector;

    private DamageMode damageMode;

    /// <summary>伤害计算器（可替换）</summary>
    private IDamageCalculator damageCalc = new DefaultDamageCalculator();

    /// <summary>要释放的技能</summary>
    public SkillData skillData
    {
        set
        {
            m_skillData = value;
            damageMode = 0;
            if ((skillData.skill.damageType & DamageType.Sector) == DamageType.Sector)
                damageMode = DamageMode.Sector;
            else if ((skillData.skill.damageType & DamageType.Circle) == DamageType.Circle)
                damageMode = DamageMode.Circle;
            else if ((skillData.skill.damageType & DamageType.Line) == DamageType.Line)
                damageMode = DamageMode.Line;

            if (damageMode != 0)
                attackTargetSelector = SelectorFactory.CreateSelector(damageMode);
        }
        get { return m_skillData; }
    }

    /// <summary>技能释放</summary>
    public virtual void DeploySkill()
    {
        if (m_skillData == null) return;
        SelfImpact(m_skillData.Owner);
        if (damageMode != 0)
            StartCoroutine(ExecuteDamage());
    }

    protected virtual IEnumerator ExecuteDamage()
    {
        float attackTimer = 0;

        ResetTargets();
        ApplyBuffsAndNotify();

        do
        {
            ResetTargets();
            if (skillData.attackTargets != null && skillData.attackTargets.Length > 0)
            {
                foreach (var item in skillData.attackTargets)
                    TargetImpact(item);
            }

            yield return new WaitForSeconds(skillData.skill.damageInterval);
            attackTimer += skillData.skill.damageInterval;
        } while (skillData.skill.durationTime > attackTimer);
    }

    private void ResetTargets()
    {
        if (m_skillData == null) return;
        m_skillData.attackTargets = attackTargetSelector.SelectTarget(m_skillData, transform);
    }

    // ===== 共用方法 =====

    /// <summary>对当前目标集合施加 Buff + 发布 UI 事件</summary>
    private void ApplyBuffsAndNotify()
    {
        if (skillData.attackTargets == null || skillData.attackTargets.Length == 0) return;
        if (skillData.skill.buffType == null || skillData.skill.buffType.Length == 0) return;

        foreach (var target in skillData.attackTargets)
        {
            BuffSystem.ApplyBuffWithEvents(target, skillData.skill.buffType,
                skillData.skill.buffDuration, skillData.skill.buffValue, skillData.skill.buffInterval,
                skillData.Owner);
        }
    }

    /// <summary>生成受击特效（默认：挂载到 HitFxPos）</summary>
    private void SpawnHitFx(GameObject target)
    {
        if (skillData.hitFxPrefab == null) return;
        Transform hitFxPos = target.GetComponent<CharacterStatus>().HitFxPos;
        var go = GameObjectPool.I.CreateObject(
            skillData.skill.hitFxName, skillData.hitFxPrefab,
            hitFxPos.position, hitFxPos.rotation);
        go.transform.SetParent(hitFxPos);
        GameObjectPool.I.Destory(go, 2f);
    }

    /// <summary>生成受击特效（碰撞点：优先射线命中点，回退到 HitFxPos）</summary>
    private void SpawnHitFxAtCollision(GameObject target, Collider collider)
    {
        if (skillData.hitFxPrefab == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000);

        if (hit.collider == collider)
        {
            var go = GameObjectPool.I.CreateObject(
                skillData.skill.hitFxName, skillData.hitFxPrefab,
                hit.point, transform.rotation);
            GameObjectPool.I.Destory(go, 2f);
        }
        else
        {
            SpawnHitFx(target);
        }
    }

    /// <summary>伤害结算</summary>
    private void DealDamage(GameObject target)
    {
        var damageVal = damageCalc.Calculate(skillData.Owner, target, skillData);
        target.GetComponent<CharacterStatus>().OnDamage((int)damageVal, skillData.Owner);
    }

    // ===== TargetImpact 重载 =====

    /// <summary>对敌人的影响（持续伤害周期）</summary>
    public virtual void TargetImpact(GameObject goTarget)
    {
        SpawnHitFx(goTarget);
        DealDamage(goTarget);
    }

    /// <summary>碰撞触发目标影响（Bullet 类型技能）</summary>
    public virtual void TargetImpact(GameObject goTarget, Collider collider)
    {
        if (skillData.skill.buffType != null && skillData.skill.buffType.Length > 0)
        {
            BuffSystem.ApplyBuffWithEvents(goTarget, skillData.skill.buffType,
                skillData.skill.buffDuration, skillData.skill.buffValue, skillData.skill.buffInterval,
                skillData.Owner);
        }
        SpawnHitFxAtCollision(goTarget, collider);
        DealDamage(goTarget);
    }

    /// <summary>对自身的影响</summary>
    public virtual void SelfImpact(GameObject goSelf)
    {
        var chStatus = goSelf.GetComponent<CharacterStatus>();
        if (chStatus.SP != 0)
        {
            chStatus.SP -= m_skillData.skill.costSP;
            ObserverMa.I.Notify(SkillEventKeys.SPChanged,
                new ResourceChangedArgs { Target = goSelf, Current = chStatus.SP, Max = chStatus.MaxSP });
        }

        // 自身位移
        DisplacementSystem.Apply(goSelf, m_skillData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((skillData.skill.damageType & DamageType.Bullet) != DamageType.Bullet)
            return;

        if (skillData.skill.attckTargetTags.Contains(other.tag))
        {
            if (skillData.skill.attackNum == 1)
            {
                TargetImpact(other.gameObject, other);
            }
            else
            {
                IAttackSelector selector = new CircleAttackSelector();
                selector.SelectTarget(m_skillData, transform);
                if (skillData.attackTargets != null && skillData.attackTargets.Length > 0)
                {
                    foreach (var item in skillData.attackTargets)
                        TargetImpact(item, other);
                }
            }
            GameObjectPool.I.Destory(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            if (skillData.hitFxPrefab != null)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 1000);
                if (hit.collider != other) return;

                var go = GameObjectPool.I.CreateObject(
                    skillData.skill.hitFxName, skillData.hitFxPrefab,
                    hit.point, other.transform.rotation);
                GameObjectPool.I.Destory(go, 2f);
            }
            GameObjectPool.I.Destory(gameObject);
        }
    }
}

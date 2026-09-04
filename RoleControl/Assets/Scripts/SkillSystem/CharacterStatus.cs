using UnityEngine;

/// <summary>角色状态 — 纯数据 + 伤害逻辑，不含 UI 创建</summary>
public class CharacterStatus : MonoBehaviour
{
    /// <summary>生命</summary>
    public float HP = 100;
    /// <summary>最大生命</summary>
    public float MaxHP = 100;
    /// <summary>当前魔法</summary>
    public float SP = 100;
    /// <summary>最大魔法</summary>
    public float MaxSP = 100;
    /// <summary>伤害基数</summary>
    public float damage = 100;
    /// <summary>命中</summary>
    public float hitRate = 1;
    /// <summary>闪避</summary>
    public float dodgeRate = 1;
    /// <summary>防御</summary>
    public float defence = 10f;
    /// <summary>主技能攻击距离，用于设置AI的攻击范围</summary>
    public float attackDistance = 2;

    /// <summary>受击特效挂点</summary>
    [HideInInspector] public Transform HitFxPos;
    /// <summary>发射点</summary>
    [HideInInspector] public Transform FirePos;
    /// <summary>选中指示器</summary>
    [HideInInspector] public GameObject selected;
    /// <summary>伤害飘字挂点</summary>
    [HideInInspector] public Transform hudPos;

    public virtual void Start()
    {
        selected = TransformHelper.FindChild(transform, "Selected").gameObject;
        HitFxPos = TransformHelper.FindChild(transform, "HitFxPos");
        FirePos = TransformHelper.FindChild(transform, "FirePos");
        hudPos = TransformHelper.FindChild(transform, "HUDPos");
    }

    /// <summary>受击</summary>
    public virtual void OnDamage(float damage, GameObject killer, bool isBuff = false)
    {
        var damageVal = ApplyDamage(damage, killer);

        // 受击打断（仅非 Buff 伤害才触发）
        if (!isBuff)
        {
            var ctrl = GetComponent<SkillCastController>();
            if (ctrl != null && ctrl.IsCasting)
                ctrl.Interrupt(InterruptType.Damage);
        }

        // 事件: 伤害飘字
        ObserverMa.I.Notify(SkillEventKeys.DamageDealt,
            new DamageDealtArgs { Target = gameObject, Attacker = killer, Damage = damageVal, IsBuff = isBuff });

        if (!isBuff)
        {
            ObserverMa.I.Notify(SkillEventKeys.PortraitShow, gameObject);
            ObserverMa.I.Notify(SkillEventKeys.HPChanged,
                new ResourceChangedArgs { Target = gameObject, Current = HP, Max = MaxHP });
        }
    }

    /// <summary>应用伤害</summary>
    public virtual float ApplyDamage(float damage, GameObject killer)
    {
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            Dead(killer);
        }
        return damage;
    }

    /// <summary>死亡</summary>
    public virtual void Dead(GameObject killer)
    {
        Destroy(gameObject, 5f);
    }
}

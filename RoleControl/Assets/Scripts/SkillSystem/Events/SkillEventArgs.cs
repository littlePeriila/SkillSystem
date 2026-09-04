using UnityEngine;

/// <summary>伤害结算事件参数</summary>
public struct DamageDealtArgs
{
    public GameObject Target;
    public GameObject Attacker;
    public float Damage;
    public bool IsBuff;
}

/// <summary>Buff 施加事件参数</summary>
public struct BuffAppliedArgs
{
    public GameObject Target;
    public BuffType BuffType;
    public float Duration;
}

/// <summary>选中目标事件参数</summary>
public struct TargetSelectedArgs
{
    public GameObject Target;
}

/// <summary>HP/SP 变更事件参数</summary>
public struct ResourceChangedArgs
{
    public GameObject Target;
    public float Current;
    public float Max;
}

/// <summary>技能施放事件参数</summary>
public struct SkillCastArgs
{
    public GameObject Caster;
    public int SkillId;
    public SkillPhase Phase;
}

/// <summary>技能被打断事件参数</summary>
public struct SkillInterruptedArgs
{
    public GameObject Caster;
    public int SkillId;
    public InterruptType Source;
    public SkillPhase Phase;
}

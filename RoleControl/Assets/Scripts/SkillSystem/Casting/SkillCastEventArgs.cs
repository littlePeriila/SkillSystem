using UnityEngine;

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

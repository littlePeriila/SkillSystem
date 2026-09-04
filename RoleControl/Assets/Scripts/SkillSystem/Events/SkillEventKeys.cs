/// <summary>技能系统事件 Key 常量，用于 ObserverMa 事件总线</summary>
public static class SkillEventKeys
{
    /// <summary>技能已释放</summary>
    public const string SkillDeployed = "SkillDeployed";

    /// <summary>造成伤害（含飘字、头像刷新）</summary>
    public const string DamageDealt = "DamageDealt";

    /// <summary>Buff 已施加</summary>
    public const string BuffApplied = "BuffApplied";

    /// <summary>选中目标变更</summary>
    public const string TargetSelected = "TargetSelected";

    /// <summary>SP 变更</summary>
    public const string SPChanged = "SPChanged";

    /// <summary>HP 变更</summary>
    public const string HPChanged = "HPChanged";

    /// <summary>显示角色头像</summary>
    public const string PortraitShow = "PortraitShow";

    /// <summary>隐藏所有敌人头像</summary>
    public const string PortraitsHide = "PortraitsHide";

    /// <summary>技能施放开始</summary>
    public const string SkillCastStarted = "SkillCastStarted";

    /// <summary>技能施放完成</summary>
    public const string SkillCastCompleted = "SkillCastCompleted";

    /// <summary>技能被打断</summary>
    public const string SkillInterrupted = "SkillInterrupted";
}

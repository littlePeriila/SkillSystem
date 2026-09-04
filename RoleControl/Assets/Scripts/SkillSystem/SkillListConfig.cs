using System.Collections.Generic;
using UnityEngine;

/// <summary>技能列表配置 — 通过 Inspector 配置技能资源路径，无需修改代码</summary>
[CreateAssetMenu(menuName = "Create SkillListConfig")]
public class SkillListConfig : ScriptableObject
{
    /// <summary>技能资源路径列表（Resources 下的相对路径，如 "Skill_1"）</summary>
    public List<string> skillPaths = new List<string>
    {
        "Skill_1",
        "Skill_2",
        "Skill_3",
        "Skill_4",
        "Skill_5"
    };
}

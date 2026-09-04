using UnityEngine;

/// <summary>默认伤害计算器 — 从 SkillDeployer.CirculateDamage() 提取</summary>
public class DefaultDamageCalculator : IDamageCalculator
{
    public float Calculate(GameObject attacker, GameObject target, SkillData skillData)
    {
        var atkStatus = attacker.GetComponent<CharacterStatus>();
        var defStatus = target.GetComponent<CharacterStatus>();

        // 命中判定
        float rate = atkStatus.hitRate / defStatus.dodgeRate;
        if (rate < 1f)
        {
            int max = (int)(rate * 100);
            int val = Random.Range(0, 100);
            if (val < max)
                return 0; // Miss
        }

        // 伤害公式：普攻伤害 * 防御减免 + 技能固定伤害 * 等级加成
        return atkStatus.damage * (1000f / (1000f + defStatus.defence))
             + skillData.skill.damage * (1f + skillData.level * skillData.skill.damageRatio);
    }
}

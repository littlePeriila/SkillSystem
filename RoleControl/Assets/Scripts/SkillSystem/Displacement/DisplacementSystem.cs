using UnityEngine;

/// <summary>位移系统 — 对外统一施加入口</summary>
public static class DisplacementSystem
{
    /// <summary>对角色施加自身位移</summary>
    public static void Apply(GameObject character, SkillData skillData)
    {
        var skill = skillData.skill;
        if (skill.displacementType == DisplacementType.None) return;

        var executor = character.GetComponent<DisplacementExecutor>();
        if (executor == null)
            executor = character.AddComponent<DisplacementExecutor>();
        if (executor.IsRunning) return;

        var displacement = DisplacementFactory.Create(skill.displacementType);
        if (displacement == null) return;

        var animator = character.GetComponent<Animator>();

        var ctx = new DisplacementContext
        {
            Character = character.transform,
            Rigidbody = character.GetComponent<Rigidbody>(),
            Animator = animator,
            StartPosition = character.transform.position,
            Direction = Vector3.Scale(character.transform.forward, new Vector3(1, 0, 1)).normalized,
            Distance = skill.displacementDistance,
            Height = skill.displacementHeight,
            Speed = skill.displacementSpeed,
            UseRootMotion = animator != null && animator.applyRootMotion,
            Stages = skill.displacementStages,
            StageInterval = skill.displacementStageInterval,
        };

        executor.StartDisplacement(displacement, ctx);
    }
}

using System.Collections;
using System;
using UnityEngine;

/// <summary>技能施放控制器 — 管理前摇→施放→后摇三阶段，处理打断</summary>
[RequireComponent(typeof(CharacterSkillManager))]
public class SkillCastController : MonoBehaviour
{
    /// <summary>当前施放阶段</summary>
    public SkillPhase Phase { get; private set; } = SkillPhase.Idle;

    private SkillData _currentSkill;
    private Coroutine _castCoroutine;
    private Animator _animator;
    private CharacterSkillManager _skillManager;

    /// <summary>施放完成回调</summary>
    public event Action<SkillData> OnCastCompleted;
    /// <summary>被打断回调</summary>
    public event Action<SkillData, InterruptType> OnCastInterrupted;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        _skillManager = GetComponent<CharacterSkillManager>();
    }

    /// <summary>当前是否正在施放技能</summary>
    public bool IsCasting => Phase != SkillPhase.Idle;

    // ===== 对外 API =====

    /// <summary>开始施放技能</summary>
    public void StartCast(SkillData skillData)
    {
        if (Phase != SkillPhase.Idle) return;

        _currentSkill = skillData;
        Phase = SkillPhase.Windup;

        ObserverMa.I.Notify(SkillEventKeys.SkillCastStarted,
            new SkillCastArgs { Caster = gameObject, SkillId = skillData.skill.skillID, Phase = Phase });

        _castCoroutine = StartCoroutine(CastRoutine(skillData));
    }

    /// <summary>打断当前技能施放</summary>
    /// <param name="source">打断来源</param>
    /// <returns>是否打断成功</returns>
    public bool Interrupt(InterruptType source)
    {
        if (Phase == SkillPhase.Idle || Phase == SkillPhase.Casting)
            return false;

        if (!CanInterrupt(source))
            return false;

        var interruptedSkill = _currentSkill;
        var interruptedPhase = Phase;

        if (_castCoroutine != null)
            StopCoroutine(_castCoroutine);

        OnCastInterrupted?.Invoke(interruptedSkill, source);

        ObserverMa.I.Notify(SkillEventKeys.SkillInterrupted,
            new SkillInterruptedArgs
            {
                Caster = gameObject,
                SkillId = interruptedSkill?.skill.skillID ?? 0,
                Source = source,
                Phase = interruptedPhase,
            });

        EndCast();
        return true;
    }

    // ===== 内部逻辑 =====

    private IEnumerator CastRoutine(SkillData skillData)
    {
        var skill = skillData.skill;

        // 阶段 1: 前摇
        if (skill.windupDuration > 0)
        {
            Phase = SkillPhase.Windup;
            PlayAnim(skill.windupAnimName, skill.animtionName);
            yield return new WaitForSeconds(skill.windupDuration);

            if (Phase != SkillPhase.Windup) yield break;
        }

        // 阶段 2: 施放（瞬间）
        Phase = SkillPhase.Casting;
        ExecuteSkill(skillData);

        // 阶段 3: 后摇
        if (skill.recoveryDuration > 0)
        {
            Phase = SkillPhase.Recovery;
            PlayAnim(skill.recoveryAnimName, skill.animtionName);
            yield return new WaitForSeconds(skill.recoveryDuration);

            if (Phase != SkillPhase.Recovery) yield break;
        }

        OnCastCompleted?.Invoke(_currentSkill);

        ObserverMa.I.Notify(SkillEventKeys.SkillCastCompleted,
            new SkillCastArgs { Caster = gameObject, SkillId = skill.skillID, Phase = SkillPhase.Idle });

        EndCast();
    }

    /// <summary>执行实际技能部署 — 委托给 CharacterSkillManager</summary>
    private void ExecuteSkill(SkillData skillData)
    {
        _skillManager.ExecuteDeploy(skillData);
    }

    /// <summary>检查当前阶段是否允许被指定来源打断</summary>
    private bool CanInterrupt(InterruptType source)
    {
        int flag = _currentSkill?.skill.interruptible ?? 0;
        if (flag == 0) return false;

        bool windupOK = (flag & 1) != 0 && Phase == SkillPhase.Windup;
        bool recoveryOK = (flag & 2) != 0 && Phase == SkillPhase.Recovery;

        return windupOK || recoveryOK;
    }

    private void EndCast()
    {
        Phase = SkillPhase.Idle;
        _currentSkill = null;
        _castCoroutine = null;
    }

    private void PlayAnim(string primary, string fallback)
    {
        if (_animator == null) return;
        string anim = !string.IsNullOrEmpty(primary) ? primary : fallback;
        if (!string.IsNullOrEmpty(anim))
            _animator.Play(anim);
    }
}

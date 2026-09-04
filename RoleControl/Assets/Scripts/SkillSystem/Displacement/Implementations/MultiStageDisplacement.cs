using UnityEngine;

/// <summary>多段位移 — 按序执行多个子位移，每段之间有间隔</summary>
public class MultiStageDisplacement : IDisplacement
{
    private int _currentStage;
    private float _stageTimer;
    private bool _inInterval;
    private float _stageDuration;
    private Vector3 _velocity;

    public float Duration { get; private set; }

    public void OnStart(ref DisplacementContext ctx)
    {
        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = false;

        _currentStage = 0;
        _inInterval = false;
        _stageTimer = 0;

        // 总时长 = 各段冲刺时间 + 段间隔
        if (ctx.Stages == null || ctx.Stages.Length == 0)
        {
            Duration = 0.1f;
            return;
        }

        float stageSpeed = ctx.Speed > 0 ? ctx.Speed : 10f;
        _stageDuration = stageSpeed > 0 ? 2f / stageSpeed : 0.1f; // 每段约 2 米
        Duration = ctx.Stages.Length * _stageDuration
                 + (ctx.Stages.Length - 1) * ctx.StageInterval;

        ApplyStage(ctx);
    }

    public void OnUpdate(ref DisplacementContext ctx, float normalizedTime)
    {
        _stageTimer += Time.deltaTime;

        if (_inInterval)
        {
            // 段间间隔中，减速到 0
            if (ctx.Rigidbody != null)
                ctx.Rigidbody.velocity = Vector3.Lerp(
                    ctx.Rigidbody.velocity, Vector3.zero, 0.2f);

            if (_stageTimer >= ctx.StageInterval)
            {
                _inInterval = false;
                _stageTimer = 0;
                _currentStage++;
                if (_currentStage < ctx.Stages.Length)
                    ApplyStage(ctx);
            }
        }
        else
        {
            // 当前段冲刺中
            if (ctx.Rigidbody != null)
                ctx.Rigidbody.velocity = new Vector3(_velocity.x, 0f, _velocity.z);

            if (_stageTimer >= _stageDuration)
            {
                if (_currentStage < ctx.Stages.Length - 1)
                {
                    _inInterval = true;
                    _stageTimer = 0;
                }
            }
        }
    }

    public void OnEnd(ref DisplacementContext ctx)
    {
        if (ctx.Rigidbody != null)
            ctx.Rigidbody.velocity = Vector3.zero;

        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = ctx.UseRootMotion;
    }

    private void ApplyStage(DisplacementContext ctx)
    {
        float stageDistance = ctx.Stages != null && _currentStage < ctx.Stages.Length
            ? ctx.Stages[_currentStage]
            : ctx.Distance;
        _velocity = ctx.Direction * ctx.Speed;
    }
}

using UnityEngine;

/// <summary>冲刺 — 直线匀速移动，通过 Rigidbody.velocity 驱动</summary>
public class DashDisplacement : IDisplacement
{
    private Vector3 _velocity;

    public float Duration { get; private set; }

    public void OnStart(ref DisplacementContext ctx)
    {
        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = false;

        _velocity = ctx.Direction * ctx.Speed;
        Duration = ctx.Speed > 0 ? ctx.Distance / ctx.Speed : 0.1f;

        if (ctx.Rigidbody != null)
            ctx.Rigidbody.velocity = new Vector3(_velocity.x, 0f, _velocity.z);
    }

    public void OnUpdate(ref DisplacementContext ctx, float normalizedTime)
    {
        // 每帧重设 velocity 防止物理衰减
        if (ctx.Rigidbody != null)
            ctx.Rigidbody.velocity = new Vector3(_velocity.x, 0f, _velocity.z);
    }

    public void OnEnd(ref DisplacementContext ctx)
    {
        if (ctx.Rigidbody != null)
            ctx.Rigidbody.velocity = Vector3.zero;

        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = ctx.UseRootMotion;
    }
}

using UnityEngine;

/// <summary>闪现 — 瞬间传送，带碰撞检测防止穿墙</summary>
public class BlinkDisplacement : IDisplacement
{
    private Vector3 _targetPos;

    public float Duration => 0f;

    public void OnStart(ref DisplacementContext ctx)
    {
        // 关闭 root motion
        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = false;

        // 计算目标位置
        Vector3 origin = ctx.StartPosition;
        Vector3 dir = ctx.Direction;
        _targetPos = origin + dir * ctx.Distance;

        // 射线检测阻挡，回退到碰撞点
        if (Physics.Raycast(origin + Vector3.up * 0.5f, dir, out var hit, ctx.Distance))
            _targetPos = hit.point - dir * 0.5f; // 留 0.5 安全距离

        // 保持原 Y
        _targetPos.y = ctx.StartPosition.y;

        ctx.Character.position = _targetPos;
    }

    public void OnUpdate(ref DisplacementContext ctx, float normalizedTime) { }

    public void OnEnd(ref DisplacementContext ctx)
    {
        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = ctx.UseRootMotion;
    }
}

using UnityEngine;

/// <summary>抛物线 — 水平匀速 + 垂直正弦曲线弧形（起跳落地）</summary>
public class ParabolicDisplacement : IDisplacement
{
    private float _startY;
    private Vector3 _horizontalVelocity;

    public float Duration { get; private set; }

    public void OnStart(ref DisplacementContext ctx)
    {
        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = false;

        _startY = ctx.StartPosition.y;
        Duration = ctx.Speed > 0 ? ctx.Distance / ctx.Speed : 0.5f;
        _horizontalVelocity = ctx.Direction * ctx.Speed;
    }

    public void OnUpdate(ref DisplacementContext ctx, float normalizedTime)
    {
        if (ctx.Rigidbody != null)
        {
            // 水平由 velocity 驱动
            ctx.Rigidbody.velocity = new Vector3(
                _horizontalVelocity.x,
                ctx.Rigidbody.velocity.y,
                _horizontalVelocity.z
            );
        }
        else
        {
            // 无 Rigidbody 时直接改 position
            Vector3 horizontal = ctx.StartPosition + ctx.Direction * ctx.Distance * normalizedTime;
            ctx.Character.position = new Vector3(horizontal.x, ctx.Character.position.y, horizontal.z);
        }

        // 垂直正弦曲线：sin(π·t) 在 t=0.5 时达到峰值 Height
        float y = _startY + ctx.Height * Mathf.Sin(Mathf.PI * normalizedTime);
        ctx.Character.position = new Vector3(ctx.Character.position.x, y, ctx.Character.position.z);
    }

    public void OnEnd(ref DisplacementContext ctx)
    {
        if (ctx.Rigidbody != null)
            ctx.Rigidbody.velocity = Vector3.zero;

        // 落地 Y 归位
        ctx.Character.position = new Vector3(
            ctx.Character.position.x, _startY, ctx.Character.position.z);

        if (ctx.Animator != null)
            ctx.Animator.applyRootMotion = ctx.UseRootMotion;
    }
}

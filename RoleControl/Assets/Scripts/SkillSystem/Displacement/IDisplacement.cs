/// <summary>位移策略接口 — 每种位移类型一个实现</summary>
public interface IDisplacement
{
    /// <summary>总持续时间（秒）</summary>
    float Duration { get; }

    /// <summary>开始位移</summary>
    void OnStart(ref DisplacementContext ctx);

    /// <summary>每帧更新
    /// <param name="normalizedTime">0~1 归一化进度</param>
    /// </summary>
    void OnUpdate(ref DisplacementContext ctx, float normalizedTime);

    /// <summary>结束位移（恢复角色控制）</summary>
    void OnEnd(ref DisplacementContext ctx);
}

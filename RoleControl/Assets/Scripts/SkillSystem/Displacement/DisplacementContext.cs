using UnityEngine;

/// <summary>位移运行时上下文 — 传递给策略的执行环境</summary>
public struct DisplacementContext
{
    /// <summary>角色 Transform</summary>
    public Transform Character;
    /// <summary>物理体</summary>
    public Rigidbody Rigidbody;
    /// <summary>动画器</summary>
    public Animator Animator;
    /// <summary>起始位置</summary>
    public Vector3 StartPosition;
    /// <summary>位移方向（水平面归一化）</summary>
    public Vector3 Direction;
    /// <summary>总距离</summary>
    public float Distance;
    /// <summary>高度参数（抛物线峰值高度）</summary>
    public float Height;
    /// <summary>速度参数</summary>
    public float Speed;
    /// <summary>原始 root motion 状态（结束时恢复）</summary>
    public bool UseRootMotion;
    /// <summary>多段位移每段距离</summary>
    public float[] Stages;
    /// <summary>多段位移每段间隔</summary>
    public float StageInterval;
}

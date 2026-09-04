using System;

/// <summary>打断来源类型（位标志）</summary>
[Flags]
public enum InterruptType
{
    None      = 0,
    Damage    = 1,    // 受击打断
    Stun      = 2,    // 眩晕打断
    Displace  = 4,    // 位移打断（被击退/击飞等）
    Cancel    = 8,    // 手动取消（翻滚/跳跃等）
    Any       = ~0,   // 所有来源
}

/// <summary>位移类型</summary>
public enum DisplacementType
{
    None = 0,
    Blink = 1,        // 闪现 — 瞬间传送
    Dash = 2,         // 冲刺 — 直线匀速
    Parabolic = 3,    // 抛物线 — 起跳落地
    MultiStage = 4,   // 多段 — 依次执行多个位移
}

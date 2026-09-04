using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 受击模式
/// </summary>
public enum DamageMode
{
    /// <summary>圆形区</summary>                  
    Circle = 4096,

    /// <summary>扇形区 </summary>
    Sector = 8192,

    /// <summary>线性</summary>
    Line = 16384,
}

/// <summary>
/// Buff类型，可叠加
/// </summary>
public enum BuffType
{
    None,
    Burn = 2,           //点燃
    Slow = 4,           //减速
    Light = 8,          //感电
    Stun = 16,          //眩晕
    Poison = 32,        //中毒
    BeatBack = 64,      //击退
    BeatUp = 128,       //击飞
    Pull = 256,         //拉拽
    AddDefence = 512,
    RecoverHp = 1024,
}

/// <summary>
/// 技能类型，可叠加
/// </summary>
public enum DamageType
{
    
    Bullet = 4,             //特效粒子碰撞伤害
    None = 8,               //无伤害，
    Buff = 32,              //buffSkill
    
    //二选一
    FirePos = 128,          //发射位置点
    FxOffset = 256,         //发射偏移
    
    //四选一
    Circle = 512,          //圈判定
    Sector = 1024,         //扇形判定
    Line = 4096,           //线性判定
    Select = 8192,         //选中才可释放
}

[Serializable]
public class Skill
{
    /// <summary>技能编号</summary>
    public int skillID;

    /// <summary>技能图标</summary>
    public string skillIcon;

    /// <summary>描述</summary>
    public string description;

    /// <summary>技能名称</summary>
    public string name;

    /// <summary>技能类型，可用 | 拼接</summary>>
    [HideInInspector]
    public DamageType damageType;
    
    /// <summary>多次伤害持续时间</summary>
    public float durationTime;
    
    /// <summary>在持续时间内，两次伤害之间的间隔时间</summary>
    public float damageInterval;

    /// <summary> 可以攻击敌人数量</summary>
    public int attackNum;

    /// <summary>伤害比，挂钩英雄属性</summary>
    public float damageRatio;

    /// <summary>固定伤害</summary>
    public float damage;
    
    /// <summary>冷却时间</summary>
    public int coolTime;
    
    // <summary>魔法消耗</summary>
    public int costSP;

    /// <summary>攻击距离，圆形和扇形判定半径，选中半圆半径，线性（矩形）长度</summary>
    public float attackDisntance;

    /// <summary> 线性判定宽度<summary>
    public float attackWidth;
    
    /// <summary>攻击扇形角度</summary>
    public int attackAngle;
    
    /// <summary>伤害判定偏移距离，默认向人物前方偏移，粒子偏移距离</summary>
    public float fxOffset;

    /// <summary>攻击目标的tag</summary>
    public string[] attckTargetTags;
    
    /// <summary>攻击动画名称</summary>
    public string animtionName;
    
    /// <summary> 动画事件延迟生成技能特效</summary>
    public float delayAnimaTime;
    
    /// <summary>预制文件名</summary>
    public string prefabName;
    
    /// <summary>目标受击特效</summary>
    public string hitFxName;

    /// <summary>下一个连击技能编号</summary>
    public int nextBatterId;

    /// <summary>Buff种类</summary>
    public BuffType[] buffType;

    /// <summary>Buff持续时间</summary>
    public float buffDuration;

    /// <summary>Buff生效间隔时间</summary>
    public float buffInterval;
    
    /// <summary>Buff生效间隔时间</summary>
    public float buffValue;

    [Header("位移参数")]
    /// <summary>自身位移类型</summary>
    public DisplacementType displacementType;
    /// <summary>位移总距离</summary>
    public float displacementDistance;
    /// <summary>抛物线高度 / 闪现高度偏移</summary>
    public float displacementHeight;
    /// <summary>位移速度</summary>
    public float displacementSpeed;
    /// <summary>多段位移每段距离（长度=段数）</summary>
    public float[] displacementStages;
    /// <summary>多段位移每段间隔</summary>
    public float displacementStageInterval;

    [Header("施放时序")]
    /// <summary>前摇时间（秒），前摇期间可被打断</summary>
    public float windupDuration;
    /// <summary>后摇时间（秒），后摇期间可被打断</summary>
    public float recoveryDuration;
    /// <summary>前摇动画名（空则使用 animtionName）</summary>
    public string windupAnimName;
    /// <summary>后摇动画名（空则使用 animtionName）</summary>
    public string recoveryAnimName;
    /// <summary>可打断标志（0=不可打断，1=前摇可打断，2=后摇可打断，3=前后摇均可打断）</summary>
    public int interruptible;
}

# 技能系统说明文档

## 1. 概述

本系统是一个基于 Unity 2020.3 的 3D 动作 RPG 技能框架，支持数据驱动的技能配置、多形状攻击选区、可扩展的 Buff 系统、事件驱动的 UI 更新和对象池管理。

### 1.1 核心特性

| 特性 | 说明 |
|------|------|
| 数据驱动技能 | 通过 `ScriptableObject` 配置技能参数，无需修改代码 |
| 多形状选区 | 圆形 / 扇形 / 线性，基于策略模式可扩展 |
| 可插拔 Buff 系统 | 10 种内置 Buff，策略模式 + 工厂，新增仅需 1 个类 + 1 行注册 |
| 事件驱动 UI | 业务层与 UI 层完全解耦，通过事件总线通信 |
| 可替换伤害公式 | `IDamageCalculator` 接口，支持自定义伤害算法 |
| 对象池 | 所有 VFX / 技能预制体通过 `GameObjectPool` 回收复用 |

### 1.2 设计模式总览

| 模式 | 应用位置 |
|------|---------|
| 策略模式 | `IBuffEffect`（Buff 效果）、`IAttackSelector`（攻击选区）、`IDamageCalculator`（伤害计算） |
| 简单工厂 | `BuffFactory`（创建 Buff 策略）、`SelectorFactory`（创建选区策略） |
| 模板方法 | `BaseAttackSelector.SelectTarget()`（共用流程，子类实现 `IsInAttackRange`） |
| 观察者模式 | `ObserverMa`（事件总线，业务层发布 → UI 层订阅） |
| 单例模式 | `SingletonMono<T>`（`GameObjectPool`）、`MonsterMgr`（纯 C# 单例） |
| 对象池模式 | `GameObjectPool`（激活/隐藏回收） |

---

## 2. 目录结构

```
Assets/Scripts/
├── SkillSystem/
│   ├── Skill.cs                        # 技能数据模型（枚举 + Serializable 类）
│   ├── SkillData.cs                    # 技能运行时数据（冷却状态、预制体引用）
│   ├── SkillTemp.cs                    # ScriptableObject 容器（技能配置资产）
│   ├── SkillListConfig.cs              # 技能列表配置（ScriptableObject）
│   ├── CharacterStatus.cs             # 角色状态（HP/SP/防御 + 伤害逻辑）
│   ├── CharacterSkillSystem.cs        # 技能调度入口（连击、动画、事件发布）
│   ├── CharacterSkillManager.cs       # 技能管理（冷却、SP 检查、VFX 部署）
│   ├── SkillDeployer.cs               # 技能部署器（VFX 释放、目标选择、伤害结算）
│   ├── BuffRun.cs                     # Buff 运行时实例（生命周期管理）
│   ├── FxBullet.cs                    # 弹道型技能子弹行为
│   ├── Events/
│   │   ├── SkillEventKeys.cs           # 事件 Key 常量
│   │   └── SkillEventArgs.cs          # 事件参数结构体
│   ├── Buff/
│   │   ├── IBuffEffect.cs              # Buff 效果策略接口
│   │   ├── BuffFactory.cs              # Buff 策略工厂
│   │   ├── BuffSystem.cs              # Buff 统一施加入口
│   │   ├── BuffIconMapping.cs          # Buff 图标名称映射
│   │   └── Effects/
│   │       ├── BurnBuffEffect.cs       # 点燃（持续伤害）
│   │       ├── PoisonBuffEffect.cs     # 中毒（持续伤害）
│   │       ├── LightBuffEffect.cs      # 感电（持续伤害）
│   │       ├── SlowBuffEffect.cs       # 减速
│   │       ├── StunBuffEffect.cs       # 眩晕
│   │       ├── BeatBackBuffEffect.cs   # 击退（DOTween 位移）
│   │       ├── BeatUpBuffEffect.cs     # 击飞（DOTween 位移）
│   │       ├── PullBuffEffect.cs       # 拉拽（DOTween 位移）
│   │       ├── AddDefenceBuffEffect.cs  # 增加防御（结束时恢复）
│   │       └── RecoverHpBuffEffect.cs  # 回复生命
│   └── Damage/
│       ├── IDamageCalculator.cs        # 伤害计算接口
│       └── DefaultDamageCalculator.cs  # 默认伤害公式实现
├── AttackSelector/
│   ├── IAttackSelector.cs              # 选区策略接口
│   ├── BaseAttackSelector.cs           # 选区基类（模板方法）
│   ├── SelectorFactory.cs             # 选区工厂（反射 + 缓存）
│   ├── CircleAttackSelector.cs        # 圆形选区
│   ├── SectorAttackSelector.cs        # 扇形选区
│   └── LineAttackSelector.cs          # 线性选区
├── PlayerControl/
│   ├── CharacterManager.cs            # 输入处理入口
│   ├── ThirdPersonUserControl.cs      # 第三人称输入控制
│   └── ThirdPersonCharacter.cs        # 第三人称角色运动
├── Camera/
│   ├── FreeLookCam.cs                 # 自由视角相机
│   └── ProtectCameraFromWallClip.cs   # 相机穿墙保护
├── UI/
│   ├── UIPortrait.cs                  # 角色头像（HP/MP/Buff 图标）
│   ├── BuffIcon.cs                    # 单个 Buff 图标
│   ├── UISkillBox.cs                  # 技能冷却 UI
│   ├── DamagePopupListener.cs         # 伤害飘字监听器（自动注册）
│   └── PortraitManager.cs            # 头像创建管理器
├── Tool/
│   ├── SingletonMono.cs              # MonoBehaviour 泛型单例基类
│   ├── GameObjectPool.cs             # 对象池
│   ├── ObserverMa.cs                 # 事件总线（观察者模式）
│   ├── CollectionHelper.cs           # 集合操作工具（排序/查找/投影）
│   ├── TransformHelper.cs            # Transform 工具（递归查找子物体）
│   ├── DamagePopup.cs                # 伤害飘字 OnGUI 渲染
│   └── RayTool.cs                    # UI 射线检测工具
├── SkillTemp.cs                       # ScriptableObject（与 SkillSystem/ 下重复，历史遗留）
└── MonsterMgr.cs                     # 敌人头像管理单例
```

---

## 3. 架构设计

### 3.1 整体数据流

```
玩家输入 / AI 调用
      │
      ▼
CharacterSkillSystem.AttackUseSkill(skillId)
      │
      ├─ CharacterSkillManager.PrepareSkill(id)    检查 SP / 冷却
      │       │
      │       └─ SkillData (从 SkillTemp ScriptableObject 加载)
      │
      ├─ SelectTarget()                            选中目标（Select 类型技能）
      │
      ├─ BuffSystem.ApplyBuffWithEvents()          施加 Buff + 发布事件
      │
      ├─ CharacterSkillManager.DeploySkill()       部署技能 VFX
      │       │
      │       ├─ CreateSkillPrefab()              从对象池创建预制体
      │       │
      │       └─ SkillDeployer.DeploySkill()       执行技能逻辑
      │               │
      │               ├─ SelfImpact()              消耗 SP + 事件
      │               │
      │               ├─ IAttackSelector.SelectTarget()  选择攻击目标
      │               │
      │               ├─ BuffSystem.ApplyBuffWithEvents()  施加 Buff
      │               │
      │               └─ IDamageCalculator.Calculate()     伤害结算
      │                       │
      │                       └─ CharacterStatus.OnDamage()
      │                               │
      │                               └─ ObserverMa.Notify()  发布事件
      │
      └─ Animator.Play()                          播放动画

事件总线 (ObserverMa)
      │
      ├─ DamagePopupListener  → 生成伤害飘字
      ├─ UIPortrait           → 刷新 HP/MP/Buff 图标/显隐
      └─ (可扩展更多订阅者)
```

### 3.2 分层职责

```
┌─────────────────────────────────────────────────────────────┐
│  输入层        CharacterManager / UISkillBox                │
│               → 调用 AttackUseSkill(id)                      │
├─────────────────────────────────────────────────────────────┤
│  调度层        CharacterSkillSystem                          │
│               → 连击 / 目标选择 / 动画 / 事件发布            │
├─────────────────────────────────────────────────────────────┤
│  管理层        CharacterSkillManager                         │
│               → 冷却计时 / SP 检查 / 配置加载 / VFX 部署     │
├─────────────────────────────────────────────────────────────┤
│  执行层        SkillDeployer                                 │
│               → 目标选择 / Buff 施加 / 伤害结算 / 受击特效   │
├──────────────┬──────────────┬───────────────────────────────┤
│  策略层       │  策略层       │  策略层                       │
│ IAttackSelector │ IBuffEffect │ IDamageCalculator            │
│ (BaseAttack…) │ (Burn/Slow…) │ (DefaultDamage…)             │
│  SelectorFactory│ BuffFactory │                               │
├──────────────┴──────────────┴───────────────────────────────┤
│  数据层        CharacterStatus (HP/SP/defence)               │
│               Skill / SkillData / SkillTemp                  │
├─────────────────────────────────────────────────────────────┤
│  基础设施      GameObjectPool / ObserverMa / SingletonMono    │
├─────────────────────────────────────────────────────────────┤
│  UI 层         UIPortrait / BuffIcon / UISkillBox            │
│               DamagePopupListener / PortraitManager          │
│               → 订阅事件，自行更新                           │
└─────────────────────────────────────────────────────────────┘
```

---

## 4. 核心模块详解

### 4.1 技能数据模型

#### Skill（技能定义）

`Skill.cs` 中定义了技能的所有静态参数，是一个 `[Serializable]` 类：

| 字段 | 类型 | 说明 |
|------|------|------|
| `skillID` | int | 技能唯一编号 |
| `name` | string | 技能名称 |
| `description` | string | 描述 |
| `skillIcon` | string | 图标资源名 |
| `damageType` | DamageType | 伤害类型（位标志，可组合） |
| `damage` | float | 固定伤害值 |
| `damageRatio` | float | 等级伤害加成系数 |
| `attackNum` | int | 可攻击目标数（1=单体） |
| `attackDisntance` | float | 攻击距离（球形检测半径） |
| `attackAngle` | int | 扇形角度 |
| `attackWidth` | float | 线性宽度 |
| `coolTime` | int | 冷却时间（秒） |
| `costSP` | int | 魔法消耗 |
| `durationTime` | float | 持续伤害时间 |
| `damageInterval` | float | 两次伤害间隔 |
| `fxOffset` | float | 特效偏移距离 |
| `animtionName` | string | 攻击动画名 |
| `delayAnimaTime` | float | 动画延迟释放特效时间 |
| `prefabName` | string | 技能特效预制体名 |
| `hitFxName` | string | 受击特效预制体名 |
| `nextBatterId` | int | 连击下一个技能 ID |
| `buffType` | BuffType[] | Buff 类型数组 |
| `buffDuration` | float | Buff 持续时间 |
| `buffInterval` | float | Buff 生效间隔 |
| `buffValue` | float | Buff 效果值 |

#### DamageType（伤害类型 — 位标志）

```csharp
[Flags]
public enum DamageType
{
    Bullet   = 4,      // 弹道型：碰撞触发伤害
    None     = 8,      // 无伤害
    Buff     = 32,     // Buff 技能（需选中目标）
    FirePos  = 128,    // 从发射点释放
    FxOffset = 256,    // 带偏移释放
    Circle   = 512,    // 圆形判定
    Sector   = 1024,   // 扇形判定
    Line     = 4096,   // 线性判定
    Select   = 8192,   // 需选中目标才能释放
}
```

**组合规则**：
- 判定形状四选一：`Circle` / `Sector` / `Line`（或都不是，仅 Buff）
- 释放位置二选一：`FirePos` / `FxOffset`
- 附加标志：`Bullet`（弹道）、`Select`（需选中）、`Buff`（纯 Buff 技能）

示例：`Skill_1.asset` 中 `damageType = [Bullet, Select]` → 弹道型技能，需选中目标。

#### BuffType（Buff 类型 — 位标志）

```csharp
[Flags]
public enum BuffType
{
    None        = 0,
    Burn        = 2,      // 点燃 — 持续伤害
    Slow        = 4,      // 减速
    Light       = 8,      // 感电 — 持续伤害
    Stun        = 16,     // 眩晕
    Poison      = 32,     // 中毒 — 持续伤害
    BeatBack    = 64,     // 击退
    BeatUp      = 128,    // 击飞
    Pull        = 256,    // 拉拽
    AddDefence  = 512,    // 增加防御
    RecoverHp   = 1024,   // 回复生命
}
```

#### SkillData（运行时状态）

| 字段 | 类型 | 说明 |
|------|------|------|
| `skill` | Skill | 技能静态数据 |
| `Owner` | GameObject | 技能拥有者 |
| `level` | int | 技能等级 |
| `coolRemain` | float | 冷却剩余时间 |
| `attackTargets` | GameObject[] | 当前攻击目标列表 |
| `Activated` | bool | 是否激活 |
| `skillPrefab` | GameObject | 技能特效预制体（运行时加载） |
| `hitFxPrefab` | GameObject | 受击特效预制体（运行时加载） |

#### SkillTemp（ScriptableObject 配置）

```csharp
[CreateAssetMenu(menuName = "Create SkillTemp")]
public class SkillTemp : ScriptableObject
{
    public Skill skill = new Skill();
    public DamageType[] damageType;  // 数组形式，运行时合并为位标志
}
```

在 Unity 菜单中 `Create > Create SkillTemp` 可新建技能配置资产。已有配置位于 `Resources/Skill_1.asset` ~ `Skill_5.asset`。

#### SkillListConfig（技能列表配置）

```csharp
[CreateAssetMenu(menuName = "Create SkillListConfig")]
public class SkillListConfig : ScriptableObject
{
    public List<string> skillPaths = new List<string> { "Skill_1", ..., "Skill_5" };
}
```

挂载到 `CharacterSkillManager.skillConfig` 字段。为空时回退到硬编码默认路径，保持向后兼容。

---

### 4.2 技能调度 — CharacterSkillSystem

**职责**：技能使用入口、连击逻辑、目标选择、动画播放、事件发布。

**核心方法**：

```csharp
// 使用技能（外部调用入口）
public void AttackUseSkill(int skillid, bool isBatter = false)

// 随机选择技能（AI 用）
public void RandomSelectSkill()
```

**AttackUseSkill 流程**：

```
1. 如果 isBatter，取当前技能的 nextBatterId
2. PrepareSkill(id) — 检查 SP 和冷却
3. 如果是 Select 类型：
   a. SelectTarget() — 球形检测 + 前方 90° + HP > 0 + 距离排序
   b. UpdateSelectedTarget() — 切换选中指示器 + 发布事件
   c. 如果是 Buff 类型 → BuffSystem.ApplyBuffWithEvents()
4. DeploySkill() — 委托给 CharacterSkillManager
5. Animator.Play(animtionName)
```

**所需组件**：`CharacterSkillManager`（RequireComponent）

---

### 4.3 技能管理 — CharacterSkillManager

**职责**：技能列表管理、配置加载、冷却计时、SP 检查、VFX 预加载与部署。

**初始化流程**：

```
Start()
  ├─ 读取 skillConfig（或回退到默认路径）
  ├─ 逐个 AddSkill(path) — Resources.Load<SkillTemp> + 合并 DamageType
  └─ 预加载所有特效预制体到对象池
```

**冷却机制**：

```csharp
// 协程式冷却，0.1 秒粒度递减
public IEnumerator CoolTimeDown(SkillData skillData)
```

**VFX 部署**：

```
DeploySkill(skillData)
  ├─ 启动冷却协程
  ├─ 如果 delayAnimaTime != 0 → 延迟部署（Invoke）
  └─ DeploySkillInternal()
       ├─ CreateSkillPrefab() — 根据 FirePos / FxOffset 从对象池创建
       ├─ 获取/添加 SkillDeployer 组件
       ├─ deployer.DeploySkill()
       └─ 非弹道型 → 延迟回收到对象池
```

---

### 4.4 技能部署器 — SkillDeployer

**职责**：VFX 释放、目标选择、Buff 施加、伤害结算、碰撞处理。

**挂载位置**：运行时动态添加到技能 VFX 预制体上。

**核心流程**：

```
DeploySkill()
  ├─ SelfImpact() — 消耗 SP + 发布事件
  └─ ExecuteDamage() 协程
       ├─ ResetTargets() — IAttackSelector.SelectTarget()
       ├─ ApplyBuffsAndNotify() — BuffSystem.ApplyBuffWithEvents()
       └─ 循环（按 damageInterval）:
            ├─ ResetTargets() — 重新选择目标
            ├─ TargetImpact(target) — 受击特效 + 伤害结算
            └─ yield WaitForSeconds(damageInterval)
```

**两个 TargetImpact 重载**：

| 方法 | 触发场景 | 行为 |
|------|---------|------|
| `TargetImpact(GameObject)` | 持续伤害周期 | 受击特效 + 伤害 |
| `TargetImpact(GameObject, Collider)` | Bullet 碰撞 | Buff + 碰撞点特效 + 伤害 |

**碰撞处理**（`OnTriggerEnter`）：

- 击中目标 Tag → 根据 attackNum 单体/群攻 → `TargetImpact` → 回收子弹
- 撞墙 → 射线检测命中点 → 生成受击特效 → 回收子弹

---

### 4.5 攻击选区系统

#### 类结构

```
IAttackSelector (接口)
    └── BaseAttackSelector (抽象基类 — 模板方法)
            ├── CircleAttackSelector
            ├── SectorAttackSelector
            └── LineAttackSelector
```

#### BaseAttackSelector 模板方法

```
SelectTarget(skillData, skillTransform):
  1. Physics.OverlapSphere() — 球形检测
  2. 过滤：Tag 匹配 + HP > 0 + IsInAttackRange()（子类实现）
  3. 按距离升序排列
  4. 根据 attackNum 截取前 N 个
```

#### 各选区判定逻辑

| 选区 | 判定方式 |
|------|---------|
| Circle | `OverlapSphere` 已覆盖，无额外判定 |
| Sector | `Vector3.Angle(forward, dir) <= attackAngle / 2` |
| Line | `InverseTransformPoint` 转局部坐标，`|z| <= distance && |x| <= width/2` |

#### SelectorFactory

通过反射 + 类名约定（`{DamageMode}AttackSelector`）创建策略实例，带缓存。

---

### 4.6 Buff 系统

#### 类结构

```
IBuffEffect (策略接口)
    ├── BurnBuffEffect       — 持续伤害
    ├── PoisonBuffEffect     — 持续伤害
    ├── LightBuffEffect     — 持续伤害
    ├── SlowBuffEffect      — 减速（预留 Movement 扩展）
    ├── StunBuffEffect      — 眩晕（预留输入禁用扩展）
    ├── BeatBackBuffEffect  — 击退（DOTween 位移）
    ├── BeatUpBuffEffect    — 击飞（DOTween 位移）
    ├── PullBuffEffect      — 拉拽（DOTween 位移）
    ├── AddDefenceBuffEffect — 加防（结束时恢复）
    └── RecoverHpBuffEffect — 回血

BuffFactory (工厂)  — 静态构造自动注册所有策略，带缓存
BuffSystem (入口)   — 对外统一 API
BuffRun (运行时)    — 生命周期管理，效果委托给策略
BuffIconMapping     — Buff 图标名称映射
```

#### IBuffEffect 接口

```csharp
public interface IBuffEffect
{
    BuffType BuffType { get; }
    void Apply(CharacterStatus target, float value);      // 每 tick 执行
    void OnRemove(CharacterStatus target, float value);   // Buff 结束清理
    string FxPrefabName { get; }                           // 关联特效（null=无）
    bool FxOnRoot { get; }                                 // 特效挂点（根/HitFxPos）
}
```

#### BuffRun 生命周期

```
InitBuff() → BuffFactory.Create() 获取策略
           → StartCoroutine(Execute())
               │
               ▼
           循环: Apply() + SpawnFx() + WaitForSeconds(interval)
               │
               ▼ (timer >= duration)
           Cleanup() → OnRemove() → Destroy(this)

OnDisable() → Cleanup() (幂等，_cleanedUp 标志防重复)
```

**重复 Buff 处理**：`BuffSystem.ApplyBuff()` 检查已有同类型 Buff → 调用 `Reset()` 刷新计时，不叠加。

**位移类 Buff**：`BeatBack` / `BeatUp` / `Pull` 固定持续 2 秒。

#### BuffSystem API

| 方法 | 说明 |
|------|------|
| `ApplyBuff(target, type, duration, value, interval)` | 施加单个 Buff（无事件） |
| `ApplyBuffs(target, types[], duration, value, interval)` | 施加多个 Buff（无事件） |
| `ApplyBuffWithEvents(target, type, ...)` | 施加单个 Buff + 发布 UI 事件 |
| `ApplyBuffWithEvents(target, types[], ...)` | 施加多个 Buff + 发布 UI 事件 |

---

### 4.7 伤害计算系统

#### IDamageCalculator 接口

```csharp
public interface IDamageCalculator
{
    float Calculate(GameObject attacker, GameObject target, SkillData skillData);
}
```

#### 默认伤害公式

```
命中判定:
  rate = attacker.hitRate / target.dodgeRate
  if rate < 1:
    随机判定是否 Miss（返回 0）

伤害计算:
  damage = attacker.damage × (1000 / (1000 + target.defence))
         + skill.damage × (1 + skill.level × skill.damageRatio)
```

#### 替换示例

```csharp
// 自定义暴击伤害计算器
public class CritDamageCalculator : IDamageCalculator
{
    public float Calculate(GameObject attacker, GameObject target, SkillData skillData)
    {
        // ... 自定义逻辑，含暴击判定
    }
}

// 在 SkillDeployer 中替换
damageCalc = new CritDamageCalculator();
```

---

### 4.8 事件系统

#### ObserverMa 事件总线

基于 `SingletonMono<ObserverMa>`，支持有参/无参事件。

**工作原理**：
1. `Register(key, callback)` — 订阅事件
2. `Notify(key, args)` — 发布事件（延迟到 `LateUpdate` 执行）
3. 执行后清空当前帧的事件队列

**延迟执行设计**：事件在 `LateUpdate` 中统一处理，避免在物理回调（如 `OnTriggerEnter`）中直接操作 UI 导致的时序问题。

#### 事件清单

| Key | 参数类型 | 发布者 | 订阅者 |
|-----|---------|--------|--------|
| `DamageDealt` | `DamageDealtArgs` | `CharacterStatus.OnDamage()` | `DamagePopupListener` |
| `BuffApplied` | `BuffAppliedArgs` | `BuffSystem.ApplyBuffWithEvents()` | `UIPortrait` |
| `PortraitShow` | `GameObject` | `CharacterStatus` / `BuffSystem` / `SkillDeployer` | `UIPortrait` |
| `PortraitsHide` | (无) | `BuffSystem` | `UIPortrait` |
| `HPChanged` | `ResourceChangedArgs` | `CharacterStatus.OnDamage()` | `UIPortrait` |
| `SPChanged` | `ResourceChangedArgs` | `SkillDeployer.SelfImpact()` | `UIPortrait` |
| `TargetSelected` | `TargetSelectedArgs` | `CharacterSkillSystem` | (预留扩展) |
| `SkillDeployed` | (无) | (预留) | (预留扩展) |

#### 事件参数结构体

```csharp
struct DamageDealtArgs     { GameObject Target, Attacker; float Damage; bool IsBuff; }
struct BuffAppliedArgs     { GameObject Target; BuffType BuffType; float Duration; }
struct TargetSelectedArgs  { GameObject Target; }
struct ResourceChangedArgs { GameObject Target; float Current, Max; }
```

---

### 4.9 角色状态 — CharacterStatus

**职责**：纯数据 + 伤害逻辑，不含 UI 创建。

| 字段 | 类型 | 说明 |
|------|------|------|
| `HP` / `MaxHP` | float | 生命值 |
| `SP` / `MaxSP` | float | 魔法值 |
| `damage` | float | 伤害基数 |
| `hitRate` | float | 命中率 |
| `dodgeRate` | float | 闪避率 |
| `defence` | float | 防御值 |
| `attackDistance` | float | AI 攻击距离 |
| `HitFxPos` | Transform | 受击特效挂点（子物体 "HitFxPos"） |
| `FirePos` | Transform | 发射点（子物体 "FirePos"） |
| `selected` | GameObject | 选中指示器（子物体 "Selected"） |
| `hudPos` | Transform | 飘字挂点（子物体 "HUDPos"） |

**伤害处理链**：

```
OnDamage(damage, killer, isBuff)
  ├─ ApplyDamage() — HP -= damage, HP <= 0 时调用 Dead()
  ├─ Notify(DamageDealt) — 伤害飘字
  └─ if !isBuff: Notify(PortraitShow + HPChanged)
```

**Dead()**：`Destroy(gameObject, 5f)` — 延迟 5 秒销毁自身。子类可 override 扩展死亡逻辑（掉落、积分等）。

---

### 4.10 UI 层

#### UIPortrait

订阅事件自行更新，不被动接受调用。

| 订阅事件 | 行为 |
|---------|------|
| `PortraitShow` | 显示头像（移到默认位置） |
| `PortraitsHide` | 隐藏头像（移到屏幕外） |
| `HPChanged` | 更新 HP 滑块 |
| `SPChanged` | 更新 SP 滑块 |
| `BuffApplied` | 添加/刷新 Buff 图标 |

#### BuffIcon

- 首次加载时缓存整张图集（`Resources.LoadAll<Sprite>("BuffIcon/Buff")`）
- 通过 `BuffIconMapping.GetIconName(buffType)` 查找对应精灵名
- 倒计时显示，结束后自动隐藏回池

#### UISkillBox

- 每帧读取 `CharacterSkillManager.skills[i].coolRemain`
- 更新技能图标填充量（`fillAmount = 1 - remain/coolTime`）和倒计时文本

#### DamagePopupListener

- `[RuntimeInitializeOnLoadMethod]` 自动注册到场景
- 订阅 `DamageDealt` 事件 → 创建 `HUD` 预制体 → 挂载到目标 `hudPos`

#### PortraitManager

- 挂载到角色预制体上
- `Start()` 中根据 Tag 创建/绑定头像 UI
- Player → 查找场景中 "HeroHead" 标签对象
- Enemy → `Instantiate("UIEnemyPortrait")` 到 Canvas + 注册到 `MonsterMgr`

---

### 4.11 基础设施

#### GameObjectPool

```
CreateObject(key, prefab, pos, rot) — 从池中取/新建
Destory(go) — 隐藏（非销毁）
Destory(go, delay) — 延迟隐藏
Clear(key) / ClearAll() — 清理
```

#### SingletonMono\<T\>

泛型 MonoBehaviour 单例基类，自动查找/创建实例，`DontDestroyOnLoad` 保护。

#### CollectionHelper

提供泛型数组操作：`OrderBy`、`OrderByDescending`、`Find`、`FindAll`、`Select`，基于委托。

---

## 5. 输入映射

| 按键 | 动作 |
|------|------|
| `WASD` | 移动（相机相对） |
| `Space` | 跳跃 |
| `C` | 蹲伏 |
| `Left Shift` | 行走（半速） |
| `F` | 技能 1（火球） |
| `1` | 技能 2（闪电） |
| `2` | 技能 3 |
| `3` | 技能 4 |
| `4` | 技能 5 |
| `Left Alt` | 切换鼠标显隐 |
| Mouse X/Y | 相机旋转 |

---

## 6. 资源约定

### 6.1 Resources 目录结构

```
Resources/
├── Skill_1.asset ~ Skill_5.asset       # SkillTemp ScriptableObject
├── SkillListConfig.asset               # (可选) 技能列表配置
├── Hero.prefab                         # 玩家预制体
├── FreeLookCameraRig.prefab            # 相机 Rig
├── HUD.prefab                          # 伤害飘字预制体
├── BuffIcon.prefab                     # Buff 图标 UI 元素
├── UIEnemyPortrait.prefab              # 敌人头像 UI
├── BuffIcon/Buff.spriteatlas           # Buff 图标精灵图集
├── Skill/
│   ├── Skill_*_Cast.prefab             # 施法特效
│   ├── Skill_*_Hit.prefab              # 受击特效
│   ├── Skill_*_Fly.prefab              # 弹道特效
│   ├── FX_Heal_Light_Cast.prefab       # 回血特效
│   └── FX_CHAR_Aura.prefab             # 加防特效
└── WALRUSGU.TTF                       # 飘字字体
```

### 6.2 角色预制体子物体约定

角色预制体（如 `Hero.prefab`）需包含以下子物体：

| 子物体名 | 用途 | 对应字段 |
|---------|------|---------|
| `Selected` | 选中指示器（默认隐藏） | `CharacterStatus.selected` |
| `HitFxPos` | 受击特效挂点 | `CharacterStatus.HitFxPos` |
| `FirePos` | 技能发射点 | `CharacterStatus.FirePos` |
| `HUDPos` | 伤害飘字挂点 | `CharacterStatus.hudPos` |

### 6.3 Tag 约定

| Tag | 用途 |
|-----|------|
| `Player` | 玩家角色 |
| `Enemy` | 敌人角色 |
| `Wall` | 墙壁（子弹碰撞检测） |
| `HeroHead` | 玩家头像 UI 对象 |
| `Canvas` | UI 根 Canvas |

---

## 7. 扩展指南

### 7.1 新增技能

1. **创建技能配置**：Unity 菜单 `Create > Create SkillTemp`，保存到 `Resources/Skill_X.asset`
2. **配置参数**：在 Inspector 中设置技能 ID、伤害、冷却、动画名、预制体名等
3. **准备特效预制体**：将 VFX 预制体放入 `Resources/Skill/` 目录
4. **注册到配置列表**：在 `SkillListConfig` 的 `skillPaths` 中添加 `"Skill_X"`（或直接使用默认列表）

> 无需修改任何代码。

### 7.2 新增 Buff 类型

1. **在 `BuffType` 枚举中添加新值**（`Skill.cs`）：
   ```csharp
   public enum BuffType
   {
       // ... 现有值
       Freeze = 2048,  // 新增冰冻
   }
   ```

2. **创建策略类**（`SkillSystem/Buff/Effects/FreezeBuffEffect.cs`）：
   ```csharp
   public class FreezeBuffEffect : IBuffEffect
   {
       public BuffType BuffType => BuffType.Freeze;
       public string FxPrefabName => "FX_Freeze";
       public bool FxOnRoot => false;

       public void Apply(CharacterStatus target, float value)
       {
           // 冰冻逻辑（如修改移动速度、禁用动画）
       }

       public void OnRemove(CharacterStatus target, float value)
       {
           // 解除冰冻
       }
   }
   ```

3. **在 `BuffFactory` 静态构造中注册**：
   ```csharp
   static BuffFactory()
   {
       // ... 现有注册
       Register(new FreezeBuffEffect());
   }
   ```

4. **在 `BuffIconMapping` 中添加图标映射**：
   ```csharp
   { BuffType.Freeze, "Buff_16" },
   ```

> 无需修改 `BuffRun`、`BuffSystem`、`SkillDeployer` 或任何 UI 代码。

### 7.3 新增攻击形状

1. **创建选区类**（`AttackSelector/TriangleAttackSelector.cs`）：
   ```csharp
   public class TriangleAttackSelector : BaseAttackSelector
   {
       protected override bool IsInAttackRange(SkillData skillData, Transform tf, GameObject target)
       {
           // 自定义三角形判定逻辑
           return /* ... */;
       }
   }
   ```

2. **在 `DamageMode` 枚举中添加**（`Skill.cs`）：
   ```csharp
   public enum DamageMode
   {
       Circle = 4096,
       Sector = 8192,
       Line = 16384,
       Triangle = 32768,  // 新增
   }
   ```

> `SelectorFactory` 会通过反射自动发现并创建。在 `SkillTemp` 配置中选择对应 `DamageType` 即可使用。

### 7.4 替换伤害公式

```csharp
// 1. 实现新计算器
public class CritDamageCalculator : IDamageCalculator
{
    public float Calculate(GameObject attacker, GameObject target, SkillData skillData)
    {
        var baseCalc = new DefaultDamageCalculator();
        float damage = baseCalc.Calculate(attacker, target, skillData);
        if (Random.Range(0f, 1f) < 0.3f)  // 30% 暴击
            damage *= 2f;
        return damage;
    }
}

// 2. 在 SkillDeployer 中替换（或通过子类 override）
// SkillDeployer 子类中:
protected override void DeploySkill()
{
    damageCalc = new CritDamageCalculator();
    base.DeploySkill();
}
```

### 7.5 新增 UI 反馈

只需订阅已有事件，不修改业务代码：

```csharp
public class HitFlashEffect : MonoBehaviour
{
    void Start()
    {
        ObserverMa.I.Register(SkillEventKeys.DamageDealt, OnDamageDealt);
    }

    private void OnDamageDealt(object args)
    {
        var e = (DamageDealtArgs)args;
        // 对 e.Target 做闪红效果
    }
}
```

---

## 8. 扩展性总结

| 扩展场景 | 所需步骤 | 需修改的现有文件数 |
|---------|---------|-------------------|
| 新增技能 | 1 个 .asset + 配置路径 | 0 |
| 新增 Buff 类型 | 1 个策略类 + 2 行注册 | 0（仅修改工厂和映射，非业务逻辑） |
| 新增攻击形状 | 1 个选区类 + 1 个枚举值 | 0（工厂自动发现） |
| 替换伤害公式 | 1 个计算器实现 | 0（赋值处替换） |
| 新增 UI 反馈 | 1 个事件订阅者 | 0 |
| 新增死亡逻辑 | override `CharacterStatus.Dead()` | 0 |

---

## 9. 已知限制与未来方向

| 项目 | 当前状态 | 建议 |
|------|---------|------|
| Assembly Definition | 无，全部编译到 Assembly-CSharp | 可按模块拆分 `.asmdef` 提升编译效率 |
| 技能配置 | ScriptableObject 硬存 Resources 路径 | 可迁移到 Addressables 支持热更新 |
| Buff 数值 | 固定 value 参数 | 可扩展为 ScriptableObject 配置的数值曲线 |
| 事件系统 | 自定义 ObserverMa（字符串 key，非类型安全） | 可考虑迁移到 `UnityEvent<T>` 或第三方库 |
| 移动减速 Buff | `SlowBuffEffect.Apply()` 为空 | 需接入 `ThirdPersonCharacter` 移动速度修改 |
| 眩晕 Buff | `StunBuffEffect.Apply()` 为空 | 需接入输入禁用 / 动画状态控制 |
| 测试 | 无 | 建议添加 EditMode / PlayMode 测试 |

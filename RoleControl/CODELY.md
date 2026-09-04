# CODELY.md — RoleControl Skill System

## Project Overview

A 3D action RPG skill system prototype with third-person character control, featuring a data-driven skill framework with multiple attack shapes (circle, sector, line), buff effects, object pooling, and real-time UI feedback (HP/MP bars, buff icons, floating damage numbers).

- **Unity Version**: 2020.3.12f1c1 (Unity China / Tuanjie variant)
- **Render Pipeline**: Built-in (Legacy)
- **Target Platform**: PC / Standalone (uses Standard Assets CrossPlatformInput, mobile-capable)
- **Language**: C# (.NET Standard 2.0 via Unity)
- **Third-Party**: DOTween Pro, Unity Standard Assets (Cameras, CrossPlatformInput, Utility)

## Key Scenes & Entry Point

| Scene | Path | Description |
|-------|------|-------------|
| SampleScene | `Assets/Scenes/SampleScene.unity` | Main and only scene; contains hero, camera rig, enemies, and UI |

## Project Structure

```
Assets/
├── Scenes/
│   └── SampleScene.unity
├── Scripts/
│   ├── SkillSystem/          # Core skill framework
│   ├── AttackSelector/        # Target selection strategies (Strategy + Factory)
│   ├── PlayerControl/         # Third-person character & input
│   ├── Camera/                # Camera controllers
│   ├── UI/                    # HP/MP bars, skill cooldown, buff icons
│   ├── Tool/                 # Utilities (pool, singleton, helpers, observer)
│   ├── SkillTemp.cs           # ScriptableObject for skill data
│   └── MonsterMgr.cs          # Enemy UI portrait manager (plain singleton)
├── Resources/
│   ├── Skill_1..5.asset       # SkillTemp ScriptableObjects (skill definitions)
│   ├── Skill/                 # Skill VFX prefabs (Cast/Hit/Fly variants)
│   ├── Hero.prefab            # Player character prefab
│   ├── FreeLookCameraRig.prefab
│   ├── HUD.prefab             # Damage popup prefab
│   ├── BuffIcon.prefab        # Buff icon UI element
│   ├── UIEnemyPortrait.prefab
│   ├── BuffIcon/              # Buff icon sprite atlas
│   ├── Dependence/            # Asset dependencies (e.g. KnifeDefault.prefab)
│   ├── 1-5.png, e.png, h.png # Skill & portrait icons
│   └── WALRUSGU.TTF           # Font for damage popups
├── Plugins/
│   ├── Demigiant/             # DOTween Pro
│   └── Standard Assets/       # Unity Standard Assets (Cameras, CrossPlatformInput, Utility)
└── ProjectSettings/
```

## Core Architecture

### Skill System Pipeline

```
SkillTemp (ScriptableObject)  →  CharacterSkillManager  →  CharacterSkillSystem  →  SkillDeployer
     Resources/*.asset              loads & manages skills      triggers skill use        deploys VFX & damage
                                                                      ↓
                                                            SelectorFactory
                                                                    ↓
                                                    IAttackSelector (Circle/Sector/Line)
```

**Data flow:**
1. `SkillTemp` ScriptableObjects (`Resources/Skill_1.asset` … `Skill_5.asset`) define all skill parameters
2. `CharacterSkillManager.Start()` loads skill data via `Resources.Load<SkillTemp>()` and preloads VFX prefabs into the object pool
3. `CharacterSkillSystem.AttackUseSkill(skillId)` handles target selection, buff application, and animation
4. `CharacterSkillManager.DeploySkill()` instantiates the skill prefab via `GameObjectPool` and invokes `SkillDeployer.DeploySkill()`
5. `SkillDeployer` selects targets via `IAttackSelector`, calculates damage, applies buffs, and spawns hit effects

### Key Classes

| Class | File | Role |
|-------|------|------|
| `Skill` | `SkillSystem/Skill.cs` | Serializable data model for a skill (damage, cooldown, range, buffs, etc.) |
| `SkillData` | `SkillSystem/SkillData.cs` | Runtime wrapper holding skill + cooldown state + prefab refs |
| `SkillTemp` | `SkillTemp.cs` | `ScriptableObject` container; created via `Create > Create SkillTemp` menu |
| `CharacterSkillManager` | `SkillSystem/CharacterSkillManager.cs` | Manages skill list, SP checks, cooldown timers, prefab deployment |
| `CharacterSkillSystem` | `SkillSystem/CharacterSkillSystem.cs` | Skill usage entry point; target selection, buff application, animation |
| `SkillDeployer` | `SkillSystem/SkillDeployer.cs` | Deploys skill VFX, calculates damage, applies buffs, handles bullet collisions |
| `BuffRun` | `SkillSystem/BuffRun.cs` | Runtime buff execution (DoT, slow, knockback, heal, etc.) using DOTween |
| `FxBullet` | `SkillSystem/FxBullet.cs` | Projectile behavior for bullet-type skills |
| `CharacterStatus` | `SkillSystem/CharacterStatus.cs` | Character stats (HP/SP/defence), damage application, UI portrait binding |

### Attack Selectors (Strategy Pattern)

| Class | Shape | Selection Criteria |
|-------|-------|--------------------|
| `CircleAttackSelector` | Circle | `Physics.OverlapSphere` by `attackDistance` |
| `SectorAttackSelector` | Sector/Cone | Circle + angle check (`attackAngle / 2`) |
| `LineAttackSelector` | Rectangle | Distance + width bounds (`attackWidth / 2`) |

`SelectorFactory` creates selectors by `DamageMode` enum via reflection, with instance caching.

### Buff System

Buffs are defined by `BuffType` flags enum (combinable via `|`):

| Buff | Effect |
|------|--------|
| `Burn`, `Poison`, `Light` | Damage over time |
| `Slow` | Movement slow |
| `Stun` | Stun |
| `BeatBack` | Knockback (DOTween move) |
| `BeatUp` | Knockup (DOTween move) |
| `Pull` | Pull toward caster |
| `AddDefence` | Temporary defence boost |
| `RecoverHp` | Heal over time |

`BuffRun` is added as a component to the target; existing buffs of the same type are refreshed (timer reset) rather than stacked.

### Design Patterns

- **Strategy**: `IAttackSelector` + implementations
- **Simple Factory**: `SelectorFactory` (reflection-based, cached)
- **Singleton**: `SingletonMono<T>` (MonoBehaviour), `MonsterMgr` (plain C# singleton)
- **Observer**: `ObserverMa` (event dispatch in `LateUpdate`)
- **Template Method**: `CharacterStatus.OnDamage()` → `ApplyDamage()`
- **Object Pool**: `GameObjectPool` (activate/deactivate recycling)

## Input Mapping

| Key | Action |
|-----|--------|
| `WASD` | Movement (camera-relative) |
| `Space` | Jump |
| `C` | Crouch |
| `Left Shift` | Walk (half speed) |
| `F` | Skill 1 (火球 / Fireball) |
| `1` | Skill 2 (闪电 / Lightning) |
| `2` | Skill 3 |
| `3` | Skill 4 |
| `4` | Skill 5 |
| `Left Alt` | Toggle cursor visibility |
| Mouse X/Y | Camera orbit (FreeLookCam) |

Skill buttons in `UISkillBox` also call `AttackUseSkill` via `onClick`.

## Damage Formula

```
damage = attacker.damage * (1000 / (1000 + target.defence))
       + skill.damage * (1 + skill.level * skill.damageRatio)
```

Hit/miss check: `hitRate / dodgeRate` ratio with random roll.

## Key Prefabs

| Prefab | Path | Purpose |
|--------|------|---------|
| Hero | `Resources/Hero.prefab` | Player character with all skill components |
| FreeLookCameraRig | `Resources/FreeLookCameraRig.prefab` | Third-person camera rig |
| HUD | `Resources/HUD.prefab` | Floating damage number popup |
| BuffIcon | `Resources/BuffIcon.prefab` | Individual buff icon UI element |
| UIEnemyPortrait | `Resources/UIEnemyPortrait.prefab` | Enemy HP/MP portrait panel |
| Skill_*_Cast/Hit/Fly | `Resources/Skill/*.prefab` | Skill VFX prefabs |

## Tags

`Player`, `Enemy`, `Wall`, `HeroHead`, `Canvas` — used for target filtering and UI lookups.

## Building & Running

### Editor
1. Open the project in Unity 2020.3.12f1c1
2. Open `Assets/Scenes/SampleScene.unity`
3. Press **Play**

### CLI / Batchmode
```bash
Unity -batchmode -quit -projectPath . -logFile build.log
```

No custom build script (`BuildScript.cs` or `Editor/` folder) was found.

### Testing
No test assemblies or test scripts were found in the project.

## Development Conventions

- **Naming**: `PascalCase` for classes and public members; Chinese comments on most fields/methods
- **Folders**: `Scripts/` sub-folders by feature (`SkillSystem/`, `AttackSelector/`, `PlayerControl/`, `Camera/`, `UI/`, `Tool/`)
- **No Assembly Definitions**: All scripts compile into the default `Assembly-CSharp.dll`
- **Resource Loading**: All runtime assets loaded via `Resources.Load()` — no Addressables
- **Object Pooling**: All VFX and skill prefabs managed by `GameObjectPool` (activate/deactivate, not Destroy)
- **ScriptableObject Data**: Skill parameters defined in `SkillTemp` assets under `Resources/`
- **Singletons**: MonoBehaviour singletons extend `SingletonMono<T>`; plain C# singletons use static `I` property
- **Third-party Code**: Standard Assets and DOTween live under `Assets/Plugins/` — avoid modifying

## Package & Dependency List

No `Packages/manifest.json` was found at the project root (legacy Unity project layout). Dependencies are managed as direct asset imports under `Assets/Plugins/`:

| Dependency | Location | Usage |
|------------|----------|-------|
| DOTween Pro | `Assets/Plugins/Demigiant/DOTweenPro/` | Buff movement (knockback, knockup) |
| Unity Standard Assets | `Assets/Plugins/Standard Assets/` | Third-person controller, camera rig, CrossPlatformInput |

## Version-Control Tips

- **Track**: `Assets/`, `ProjectSettings/`, `Scenes/`
- **Ignore**: `Library/`, `Temp/`, `obj/`, `Build/`, `Logs/`, `UserSettings/`
- **No `.gitignore` found** — one should be added (see Unity's recommended `.gitignore` template)
- `.meta` files must be committed (Unity asset linking depends on them)

## TODO / Open Questions

- **No `.gitignore`** — add a standard Unity `.gitignore`
- **No `README`** — project documentation is absent
- **No build script** — no `Editor/BuildScript.cs` for automated builds
- **No tests** — no Play-Mode or Edit-Mode test assemblies
- **`Packages/` folder missing** — project uses legacy direct-import dependencies instead of Package Manager
- **Hardcoded skill IDs** in `CharacterSkillManager.Start()` (`AddSkill("Skill_1")` … `AddSkill("Skill_5")`) — adding new skills requires code changes
- **`LineAttackSelector`** uses world-space X/Z axis checks rather than transform-relative — may behave incorrectly when characters are rotated

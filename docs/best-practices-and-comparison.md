# Unity 2D Indie Game: Project & Code Organization Best Practices
## With Comparison to GeometrySurvivor Current State

---

## Table of Contents

1. [Folder & Asset Organization](#1-folder--asset-organization)
2. [Code Architecture Patterns](#2-code-architecture-patterns)
3. [Assembly Definitions](#3-assembly-definitions)
4. [Scene Organization](#4-scene-organization)
5. [Input Handling](#5-input-handling)
6. [Prefab Workflow](#6-prefab-workflow)
7. [Physics & Collision](#7-physics--collision)
8. [Performance](#8-performance)
9. [ScriptableObject-Based Architecture](#9-scriptableobject-based-architecture)
10. [State Management](#10-state-management)
11. [Separation of Concerns](#11-separation-of-concerns)
12. [Common Pitfalls](#12-common-pitfalls)
13. [Summary Scorecard](#13-summary-scorecard)
14. [Priority Action List](#14-priority-action-list)
15. [Recommended Reading](#15-recommended-reading)

---

## 1. Folder & Asset Organization

### Best Practice

Use a `_Project` (or `_Game`) prefix folder to separate your code from third-party assets. Third-party plugins should live in `Assets/ThirdParty/` to avoid accidental edits and keep the root clean. The underscore sorts your folder to the top of the Assets directory.

```
Assets/
    _Project/
        Art/
            Sprites/
            Animations/
            Tilesets/
            UI/
        Audio/
            Music/
            SFX/
        Data/                      # ScriptableObject asset instances
            Weapons/
            Enemies/
            Events/
        Materials/
        Prefabs/
            Characters/
            Enemies/
            Projectiles/
            VFX/
            UI/
            Environment/
            Camera/
            Managers/
        Scenes/
        Scripts/
            Core/                  # Bootstrapper, events, interfaces
            Player/
            Enemies/
            Weapons/
            Projectiles/
            UI/
            Utils/
        ScriptableObjects/         # SO class definitions (C# scripts)
    ThirdParty/
        Rewired/
```

**Naming rules:**
- Folders: PascalCase (`PlayerCharacters`, `Projectiles`)
- Scripts: PascalCase matching class name (`Weapon.cs`)
- No spaces in file/folder names (breaks CLI tools)

Use empty GameObjects as hierarchy separators in scenes:

```
--- MANAGERS ---
    GameManager
    RewiredInputManager
--- ENVIRONMENT ---
    Tilemap
    Background
--- PLAYER ---
    Player
--- CAMERA ---
    MainCamera
    CinemachineCamera
```

### Current State: GeometrySurvivor

```
Assets/
    Art/kenney tiny town/       # Spaces in folder name
    InputAssets/
    Prefabs/
        Camera/
        Managers/
        PlayerCharacters/
    Rewired/                    # Third-party at Assets root
    Scenes/
    Scripts/
        Input/
        Projectiles/
        Weapons/
        PlayerInputHandler.cs
        Projectile.prefab       # PREFAB IN SCRIPTS FOLDER
    Settings/
```

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Root separation | `_Project/` + `ThirdParty/` | Everything at `Assets/` root | Rewired mixed with project files |
| Asset colocation | Assets grouped by type | Mostly correct | `Projectile.prefab` lives in `Scripts/` |
| Naming | No spaces, PascalCase | `kenney tiny town` has spaces | Minor but causes CLI issues |
| Scene hierarchy | Separator GameObjects | Flat root hierarchy | Everything at root level |
| Third-party isolation | `ThirdParty/Rewired/` | `Assets/Rewired/` | Not isolated |

---

## 2. Code Architecture Patterns

### Best Practice

**Single Responsibility Principle:** Each MonoBehaviour should do one thing. An input reader reads input. A mover moves. A weapon fires. They communicate through events or shared data, not by reaching into each other.

Recommended split for a player character:

```csharp
// Reads raw input, exposes values. Nothing else.
public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveDirection { get; private set; }
    public Vector2 AimDirection { get; private set; }
    public bool IsAttacking { get; private set; }
}

// Consumes input, applies movement. Knows nothing about weapons.
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    private PlayerInput _input;

    void FixedUpdate()
    {
        Vector2 movement = _input.MoveDirection * (_moveSpeed * Time.fixedDeltaTime);
        transform.Translate(movement);
    }
}
```

**Avoid `SendMessage`:** It uses reflection, is 100x slower than a direct call, provides no compile-time safety, and fails silently. Always use `GetComponent<T>()` or direct references.

**Cache `Camera.main`:** Internally it calls `FindGameObjectWithTag("MainCamera")` every time. Cache the reference in `Awake()`.

**Singletons:** Limit to 2-3 truly global services (GameManager, AudioManager). Everything else should communicate through events or direct references.

### Current State: GeometrySurvivor

`PlayerInputHandler.cs` handles input reading, movement, AND attack triggering in one class (48 lines, 3 responsibilities). It has TODO comments acknowledging this should be split.

`Weapon.cs` uses `SendMessage("SetDirection", direction)` to configure projectiles instead of a direct component reference.

`PlayerInputHandler.cs` calls `Camera.main` every FixedUpdate tick without caching.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Single responsibility | One class, one job | PlayerInputHandler does 3 jobs | Needs split into Input/Movement/Combat |
| Inter-component calls | Direct `GetComponent<T>()` or cached refs | `SendMessage` (reflection) | Replace with typed calls |
| Camera reference | Cache in `Awake()` | `Camera.main` every FixedUpdate | Performance and clarity issue |
| Coupling | Events or interfaces | Direct SerializeField references | Acceptable at this scale, but no path to decouple |

---

## 3. Assembly Definitions

### Best Practice

Assembly definitions (`.asmdef` files) split your code into separate compilation units. For a small indie project, add them when script count exceeds ~30 files or compile times become noticeable. They enforce architectural boundaries and speed up iteration.

Recommended minimal split:

```
Scripts/
    Core/
        GeometrySurvivor.Core.asmdef         # Events, interfaces, data types
    Runtime/
        GeometrySurvivor.Runtime.asmdef      # All gameplay code (references Core)
    Editor/
        GeometrySurvivor.Editor.asmdef       # Custom inspectors, editor tools
```

The key benefit even at small scale: Editor code gets properly excluded from builds, and you can't accidentally reference editor-only APIs from runtime code.

### Current State: GeometrySurvivor

No `.asmdef` files exist. All scripts compile into `Assembly-CSharp`. Every change recompiles everything.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Assembly definitions | Add when >30 files or compile times matter | None | Not critical yet (only 4 scripts) |
| Editor isolation | Separate Editor .asmdef | No editor scripts exist yet | No gap yet |

**Verdict:** Not a problem at 4 scripts. Revisit when the project grows.

---

## 4. Scene Organization

### Best Practice

For a survivors-genre game (single continuous arena), use the **persistent manager scene + additive loading** pattern:

```
Scenes/
    Bootstrapper.unity         # Scene 0, loads managers, transitions to menu
    PersistentManagers.unity   # GameManager, AudioManager, Input (loaded additively, never unloaded)
    MainMenu.unity
    Gameplay.unity             # The actual arena
    GameOver.unity
```

The Bootstrapper loads PersistentManagers additively, then loads the first real scene. This is preferred over `DontDestroyOnLoad` because DDOL objects are invisible in the hierarchy and harder to debug.

**Every scene should be independently runnable.** If you press Play in Gameplay.unity, the bootstrapper should detect missing managers and load them. This is critical for iteration speed.

### Current State: GeometrySurvivor

Single scene: `MainScene.unity`. Flat hierarchy with everything at root level. No GameManager. No bootstrapper. The RewiredInputManager uses DontDestroyOnLoad.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Scene split | Bootstrapper + PersistentManagers + Gameplay | Single monolithic scene | Acceptable for prototype |
| Hierarchy | Separator GameObjects by category | Flat root hierarchy | Hard to navigate as it grows |
| Manager lifecycle | Additive persistent scene | DontDestroyOnLoad | Minor, DDOL is fine for now |
| Independent testability | Every scene runnable standalone | Only one scene | N/A |

---

## 5. Input Handling

### Best Practice

Separate input reading from input consumption. Read input in `Update()` (input events are frame-based), store the values, and consume them in `FixedUpdate()` for physics.

Use an interface to abstract input providers so the same character controller can be driven by player input OR AI:

```csharp
public interface IInputProvider
{
    Vector2 MoveDirection { get; }
    Vector2 AimDirection { get; }
    bool IsAttackPressed { get; }
}
```

Pick one input system and use it consistently. Don't mix Rewired with Unity's legacy `Input` class.

### Current State: GeometrySurvivor

`PlayerInputHandler` uses Rewired for movement and attack button, but uses legacy `Input.mousePosition` for mouse position. Input is polled and consumed in the same `FixedUpdate()` call (input events can be missed between fixed timesteps). No input abstraction interface.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Read vs. consume | Read in Update, consume in FixedUpdate | Both in FixedUpdate | Can miss input events |
| Input consistency | One system only | Rewired + legacy Input mixed | Remove legacy Input usage |
| Abstraction | IInputProvider interface | Concrete class, no interface | No path to AI reuse |

---

## 6. Prefab Workflow

### Best Practice

Use **prefab variants** for enemy/weapon types: create a base prefab with shared components, then variants that override specific stats and sprites.

Use **nested prefabs** for composed objects: the Player prefab should contain a Weapon as a nested prefab so weapon changes propagate automatically.

Keep nesting shallow (2-3 levels max). Name prefabs descriptively.

```
Prefabs/
    Enemies/
        EnemyBase.prefab
        EnemyTriangle.prefab        # Variant of EnemyBase
        EnemySquare.prefab          # Variant of EnemyBase
    Projectiles/
        ProjectileBase.prefab
```

### Current State: GeometrySurvivor

5 prefabs total. No variants. No nested prefabs. Projectile prefab is in `Scripts/` instead of `Prefabs/`. Player prefab is monolithic (all components on root).

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Variants | Use for enemy/weapon types | None | Will matter when enemies exist |
| Nested prefabs | Weapon as nested prefab in Player | Weapon is a component, not nested | Minor at this scale |
| Organization | All prefabs in `Prefabs/` by category | Projectile prefab in `Scripts/` | Misplaced asset |
| Composition | Child objects for logical grouping | Flat component structure | Acceptable for now |

---

## 7. Physics & Collision

### Best Practice

**Configure the layer collision matrix.** This is the single biggest optimization for 2D physics. Disable collisions between layers that should never interact:

| | Player | PlayerProjectile | Enemy | EnemyProjectile | Pickup | Environment |
|---|---|---|---|---|---|---|
| **Player** | - | - | YES | YES | YES | YES |
| **PlayerProjectile** | - | - | YES | - | - | YES |
| **Enemy** | YES | YES | - | - | - | YES |
| **EnemyProjectile** | YES | - | - | - | - | YES |
| **Pickup** | YES | - | - | - | - | - |
| **Environment** | YES | YES | YES | YES | - | - |

Key rules:
- Player projectiles don't collide with the player or each other
- Enemies don't collide with each other (common in survivors games; use spatial avoidance instead)
- Pickups only collide with the player

**Use Rigidbody2D for all moving objects**, even kinematic ones. Moving transforms directly bypasses the physics engine and can cause missed collisions.

**Use CircleCollider2D** when possible (cheapest broad-phase check). Use triggers (`isTrigger = true`) for overlap detection (pickups, damage zones).

For projectiles, set `Rigidbody2D.velocity` once instead of translating every frame:

```csharp
public void Launch(Vector2 direction)
{
    transform.up = direction;
    _rb.linearVelocity = direction * _speed;
    // No per-frame update needed. Physics engine handles movement.
}
```

### Current State: GeometrySurvivor

Layers are defined (Player=6, Enemy=7, Projectile=8, Scenario=9) but the collision matrix is fully enabled: every layer collides with every other layer (`ffffffff` for all entries).

Neither the Player nor Projectile use Rigidbody2D. All movement is via `transform.Translate()`, bypassing the physics engine entirely. The Projectile prefab has a BoxCollider2D but no Rigidbody2D, so collision callbacks won't fire.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Layer collision matrix | Selective, optimized | All layers collide with all | No filtering at all |
| Rigidbody2D usage | All moving objects | None use Rigidbody2D | Physics engine bypassed |
| Movement method | `Rigidbody2D.velocity` or `MovePosition` | `transform.Translate()` | Collisions won't work |
| Collider types | CircleCollider2D preferred | BoxCollider2D on projectile | Minor |
| Gravity | Disabled for top-down | Default -9.81 Y gravity | Will affect any Rigidbody2D added |

**This is a critical gap.** Without Rigidbody2D, adding collision-based gameplay (damage, pickups, boundaries) will not work reliably.

---

## 8. Performance

### Best Practice: Object Pooling

In a survivors game, object pooling is non-negotiable. You will spawn hundreds of projectiles, enemies, pickups, and VFX particles. `Instantiate`/`Destroy` causes GC spikes that create visible stutters.

Unity provides a built-in pool since 2021: `UnityEngine.Pool.ObjectPool<T>`. Or use a simple custom pool:

```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _initialSize = 20;
    private Queue<GameObject> _pool = new();

    void Awake()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            var obj = Instantiate(_prefab, transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        var obj = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_prefab, transform);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

**What to pool in a survivors game:** Projectiles (highest priority), enemies, damage numbers, XP gems/pickups, VFX.

### Best Practice: GC Avoidance

- Cache `GetComponent` calls in `Awake()`, never in `Update()`
- Use `CompareTag("Enemy")` instead of `gameObject.tag == "Enemy"` (avoids string allocation)
- Avoid LINQ in hot paths (allocates iterators)
- Pre-allocate lists and arrays; reuse collections
- Cache animator parameter hashes: `static readonly int IsRunning = Animator.StringToHash("IsRunning")`

### Best Practice: Update vs. FixedUpdate

| Method | Use For |
|--------|---------|
| `Update()` | Input polling, UI, visual effects, non-physics logic |
| `FixedUpdate()` | Rigidbody movement, physics forces, physics queries |
| `LateUpdate()` | Camera follow, anything depending on post-Update state |

**Don't mix:** `transform.Translate()` in `FixedUpdate` is contradictory. Either use Rigidbody2D in FixedUpdate, or use transform movement in Update.

### Current State: GeometrySurvivor

`Weapon.Fire()` calls `Instantiate()` every shot with no pooling. Projectiles are never destroyed or returned, so they accumulate indefinitely (at 2 shots/second, that's 120 objects/minute living forever).

`Camera.main` is called every FixedUpdate without caching. `transform.Translate()` is used in FixedUpdate.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Object pooling | Pool all frequently spawned objects | `Instantiate` every shot, no `Destroy` | Memory leak, GC pressure |
| Projectile lifetime | Timer or bounds check, return to pool | None, live forever | Critical: unbounded memory growth |
| Camera caching | `_cam = Camera.main` in Awake | `Camera.main` every FixedUpdate | Unnecessary search each tick |
| Update/FixedUpdate | Match method to movement type | `transform.Translate` in FixedUpdate | Inconsistent with physics model |

---

## 9. ScriptableObject-Based Architecture

### Best Practice

Based on Ryan Hipple's Unite Austin 2017 talk (most-watched Unite video), ScriptableObjects can serve as the backbone of inter-system communication for indie games. Three core patterns:

#### Pattern 1: Data Definitions

Instead of serializing stats directly on components, define them as ScriptableObject assets:

```csharp
[CreateAssetMenu(menuName = "GeometrySurvivor/WeaponData")]
public class WeaponData : ScriptableObject
{
    public GameObject ProjectilePrefab;
    public float Cooldown;
    public float ProjectileSpeed;
    public int Damage;
}
```

Then `Weapon.cs` references a `WeaponData` asset. Creating weapon variants requires zero code — just duplicate the asset and tweak numbers in the Inspector. Designers (or you in 3 months) can balance the game without touching code.

#### Pattern 2: Event Channels

ScriptableObject events decouple systems completely:

```csharp
[CreateAssetMenu(menuName = "Events/VoidEvent")]
public class VoidEvent : ScriptableObject
{
    private readonly List<System.Action> _listeners = new();

    public void Raise() { for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i](); }
    public void Register(System.Action listener) => _listeners.Add(listener);
    public void Unregister(System.Action listener) => _listeners.Remove(listener);
}
```

Create asset instances like `OnPlayerDied`, `OnWaveStarted`, `OnEnemyKilled`. Wire them in the Inspector. The spawner doesn't know the UI exists. The UI doesn't know the spawner exists. Both reference the same SO event.

#### Pattern 3: Runtime Sets

```csharp
[CreateAssetMenu(menuName = "Sets/GameObjectSet")]
public class RuntimeSet : ScriptableObject
{
    private readonly List<GameObject> _items = new();
    public IReadOnlyList<GameObject> Items => _items;
    public void Add(GameObject item) { if (!_items.Contains(item)) _items.Add(item); }
    public void Remove(GameObject item) => _items.Remove(item);
}
```

Enemies register on spawn, unregister on death. Any system that needs enemy references reads the set. No `FindObjectsOfType` needed.

### Why This Matters for Survivors Games

A survivors game has dozens of systems reacting to game state: wave spawner, upgrade UI, damage numbers, sound effects, screen shake, difficulty scaling, XP collection. The SO architecture lets you wire all of this without spaghetti code. Adding a new system (e.g., screen shake on enemy death) requires zero changes to existing code — just subscribe to the existing `OnEnemyKilled` event.

### Current State: GeometrySurvivor

No custom ScriptableObjects. Weapon stats are serialized directly on the `Weapon` component. No event system. No data-driven design. All communication is through direct references and `SendMessage`.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Data definitions | ScriptableObject assets (WeaponData, EnemyData) | Inline SerializeField values | No path to data-driven design |
| Event channels | SO-based decoupled events | Direct references + SendMessage | Systems will become tightly coupled |
| Runtime sets | SO sets for dynamic collections | N/A (no enemies yet) | Will need when enemies exist |
| Balancing workflow | Tweak SO assets, no code changes | Edit component values on prefabs | Less flexible, harder to compare |

---

## 10. State Management

### Best Practice: Game State

Use a simple state machine for top-level game flow:

```csharp
public enum GameState { MainMenu, Playing, Paused, LevelUp, GameOver }

public class GameManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public void ChangeState(GameState newState)
    {
        ExitState(CurrentState);
        CurrentState = newState;
        EnterState(newState);
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing: Time.timeScale = 1f; break;
            case GameState.Paused:
            case GameState.LevelUp: Time.timeScale = 0f; break;
            case GameState.GameOver: Time.timeScale = 0f; break;
        }
    }
}
```

### Best Practice: Entity State Machines

For entity behavior (enemies, bosses), use the interface-based state pattern:

```csharp
public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnFixedUpdate();
    void OnExit();
}

public class StateMachine
{
    private IState _currentState;

    public void ChangeState(IState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public void Update() => _currentState?.OnUpdate();
    public void FixedUpdate() => _currentState?.OnFixedUpdate();
}
```

**When to use it:** Add a state machine when an entity has 3+ distinct behaviors. For basic survivors enemies that just chase and attack on contact, direct logic is fine.

### Current State: GeometrySurvivor

No GameManager. No game state tracking. No state machines. No pause/resume capability. No game-over condition.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Game state | FSM (Menu, Playing, Paused, GameOver) | None | No game loop management |
| Entity state | Interface-based state pattern | N/A (no enemies) | Will need for complex enemies |
| Pause system | `Time.timeScale` controlled by state | None | Missing |

---

## 11. Separation of Concerns

### Best Practice

Full MVC is overkill for indie games. Use a simpler three-layer separation:

**Data (ScriptableObjects / plain C#):** Pure data, no behavior.

```csharp
[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public Sprite Sprite;
    public float MaxHP;
    public float MoveSpeed;
    public int XPValue;
}
```

**Logic (MonoBehaviours):** Behavior, references data, no visual concerns.

```csharp
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData _data;
    private float _currentHP;
    public event System.Action<float, float> OnHealthChanged;

    public void TakeDamage(float amount)
    {
        _currentHP = Mathf.Max(0, _currentHP - amount);
        OnHealthChanged?.Invoke(_currentHP, _data.MaxHP);
    }
}
```

**Presentation (separate MonoBehaviours):** Only handles visuals, subscribes to logic events.

```csharp
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyHealth _health;
    void OnEnable() => _health.OnHealthChanged += UpdateBar;
    void OnDisable() => _health.OnHealthChanged -= UpdateBar;
    private void UpdateBar(float current, float max) { /* update visuals */ }
}
```

**Key test:** Logic components should function with all presentation components removed.

### Current State: GeometrySurvivor

No separation. `PlayerInputHandler` mixes input reading, movement logic, and weapon triggering. `Weapon` mixes firing logic with cooldown UI state. No events between components.

### Comparison

| Aspect | Best Practice | Current State | Gap |
|--------|--------------|---------------|-----|
| Data/Logic/Presentation | Three distinct layers | All mixed together | Significant refactor needed as complexity grows |
| Events for decoupling | C# events or SO events between layers | Direct method calls only | No way to add presentation without modifying logic |

---

## 12. Common Pitfalls

### Pitfalls Present in GeometrySurvivor

| Pitfall | Where | Impact |
|---------|-------|--------|
| **`SendMessage` usage** | `Weapon.cs:24` | 100x slower than direct call, no compile-time safety, fails silently |
| **`Camera.main` in loops** | `PlayerInputHandler.cs:32` | `FindGameObjectWithTag` every FixedUpdate tick |
| **No projectile lifetime** | `Projectile.cs` | Objects fly forever, unbounded memory growth |
| **No object pooling** | `Weapon.cs` | GC spikes from repeated Instantiate, stutters at scale |
| **Mixed input systems** | `PlayerInputHandler.cs` | Rewired for buttons, legacy `Input` for mouse — inconsistent |
| **Prefab in wrong folder** | `Assets/Scripts/Projectile.prefab` | Confusing project layout |
| **`_direction` stored but unused** | `Projectile.cs` | `SetDirection` stores the value but movement always uses `transform.up` (the field is redundant) |
| **Unused import** | `Weapon.cs:2` `using Unity.Collections;` | Dead code |
| **Default gravity for top-down game** | `Physics2DSettings` | -9.81 Y gravity will affect any Rigidbody2D added |

### Pitfalls to Watch For as the Project Grows

| Pitfall | Prevention |
|---------|------------|
| **God class GameManager** | Split: GameManager (state only), SpawnManager, ScoreManager, etc. |
| **FindObjectsOfType in loops** | Use Runtime Sets (SO pattern) or cached references |
| **String-based animator params** | Cache hashes: `static readonly int Hash = Animator.StringToHash("Name")` |
| **Over-scoping** | Define the core loop first, make it fun, then add systems |
| **Premature ECS migration** | MonoBehaviour works fine for hundreds of entities. Profile before migrating. |

---

## 13. Summary Scorecard

| Category | Grade | Notes |
|----------|-------|-------|
| **Folder organization** | C | Mostly okay, but misplaced prefab and third-party not isolated |
| **Naming conventions** | A | Consistent PascalCase classes, `_camelCase` fields |
| **Code architecture** | D | Single-responsibility violated, no abstractions, SendMessage |
| **Assembly definitions** | N/A | Not needed yet at 4 scripts |
| **Scene organization** | C | Single scene is fine for prototype, but flat hierarchy |
| **Input handling** | C- | Good choice (Rewired), but mixed with legacy Input, polled in FixedUpdate |
| **Prefab workflow** | C | Basic structure exists, no variants or nesting |
| **Physics setup** | D | Layers defined but matrix not configured, no Rigidbody2D, wrong gravity |
| **Performance** | F | No pooling, no lifetime management, memory leak |
| **ScriptableObjects** | F | Not used at all |
| **State management** | F | No game state, no manager |
| **Separation of concerns** | D | Everything mixed in monolithic components |

**Overall: Early prototype with solid naming conventions but significant architectural gaps that will become blocking as complexity increases.**

---

## 14. Priority Action List

Ordered by impact and urgency:

### Immediate (before adding any new features)

1. **Add projectile lifetime management** — at minimum `Destroy(gameObject, 5f)` in `Projectile.Awake()`. Without this, every shot leaks memory permanently.

2. **Replace `SendMessage` in `Weapon.cs`** — use `GetComponent<Projectile>()` or store the prefab as a `Projectile` reference directly.

3. **Cache `Camera.main`** — store in a field during `Awake()`, use the cached reference.

4. **Move `Projectile.prefab`** from `Assets/Scripts/` to `Assets/Prefabs/Projectiles/`.

### Short-term (during next feature sprint)

5. **Implement object pooling for projectiles** — use `UnityEngine.Pool.ObjectPool<T>` or a simple custom pool. This is genre-critical for survivors games.

6. **Configure the physics layer collision matrix** — disable unnecessary collisions, set gravity to (0, 0) for top-down.

7. **Add Rigidbody2D to Player and Projectile** — required for collision detection to work. Use Kinematic body type if you want to control movement manually.

8. **Split `PlayerInputHandler`** into PlayerInput (reads input in Update) + PlayerMovement (applies in FixedUpdate) + weapon triggering via events.

### Medium-term (as systems are added)

9. **Create `WeaponData` ScriptableObject** — data-drive weapon stats for easy balancing and variant creation.

10. **Set up SO event channels** — `OnEnemyKilled`, `OnPlayerDied`, `OnWaveStarted` for cross-system communication.

11. **Add a GameManager with state machine** — track Playing/Paused/GameOver states.

12. **Move Rewired to `Assets/ThirdParty/`** — isolate third-party code from project code.

---

## 15. Recommended Reading

- **[Unity E-Book: Level Up Your Code with Design Patterns and SOLID](https://unity.com/resources/design-patterns-solid-ebook)** — Free 150-page guide with Unity-specific examples
- **[Game Programming Patterns by Robert Nystrom](https://gameprogrammingpatterns.com/)** — Free online, the canonical reference for game architecture
- **[Ryan Hipple - Unite Austin 2017: Game Architecture with ScriptableObjects](https://www.youtube.com/watch?v=raQ3iHhE_Kk)** — The foundational talk for SO-based architecture
- **[Unity Official: Architect Code as Your Project Scales](https://unity.com/how-to/how-architect-code-your-project-scales)** — Official guide to scaling Unity codebases
- **[Unity Atoms](https://unity-atoms.github.io/unity-atoms/)** — Production-ready implementation of the SO architecture patterns
- **[UnityHFSM](https://github.com/Inspiaaa/UnityHFSM)** — Lightweight hierarchical state machine library for entity behavior

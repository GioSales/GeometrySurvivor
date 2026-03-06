# ECS Best Practices & Design Patterns (Entitas)

Reference guide for GeometrySurvivor's Entitas-based architecture.

> **TODO:** Research recent Entitas community patterns, GitHub discussions, and real-world project postmortems (2024-2026) to supplement this document with up-to-date practices. Exclude Unity DOTS content.

---

## Table of Contents

1. [Core Principles](#1-core-principles)
2. [Component Design](#2-component-design)
3. [System Design](#3-system-design)
4. [Feature Organization](#4-feature-organization)
5. [Context Separation](#5-context-separation)
6. [Reactive vs Execute Systems](#6-reactive-vs-execute-systems)
7. [Common Patterns](#7-common-patterns)
8. [Anti-Patterns](#8-anti-patterns)
9. [Bridging ECS and Unity](#9-bridging-ecs-and-unity)
10. [Performance Considerations](#10-performance-considerations)
11. [Testing](#11-testing)

---

## 1. Core Principles

### Data-Oriented Thinking

ECS inverts the OOP mindset. Instead of objects that own behavior, you have:

- **Entities** — identity (just an ID with a bag of components)
- **Components** — pure data, no logic
- **Systems** — pure logic, no state

The mental model: components describe *what* an entity *is*, systems describe *what happens* to entities that match a certain shape.

### Composition Over Inheritance

Never model "types" of entities through class hierarchies. An enemy is just an entity with `EnemyComponent`. A boss is an entity with `EnemyComponent` + `BossComponent`. A poisoned boss is `EnemyComponent` + `BossComponent` + `PoisonedComponent`. You build entity archetypes by composing components, not by creating subclasses.

### Single Responsibility

Each system does exactly one thing. If you can't describe what a system does in one short sentence, it's doing too much. `ProjectileMovementSystem` moves projectiles. `LifeTimeSystem` ticks down lifetimes. That's it.

---

## 2. Component Design

### Pure Data Components

Components should be plain data containers. No methods, no logic, no Unity API calls.

```csharp
// Good
[Game]
public sealed class HealthComponent : IComponent
{
    public float current;
    public float max;
}

// Bad — logic in a component
[Game]
public sealed class HealthComponent : IComponent
{
    public float current;
    public float max;
    public float Percentage => current / max;  // Don't do this
    public void TakeDamage(float amount) { ... } // Definitely don't do this
}
```

If you need derived values, compute them in the system that consumes the data.

### Tag / Flag Components

Empty components that act as boolean flags. Very useful for marking entity state.

```csharp
[Game]
public sealed class DestroyedComponent : IComponent { }

[Game]
public sealed class InvulnerableComponent : IComponent { }

[Game]
public sealed class PlayerComponent : IComponent { }
```

Use `entity.isDestroyed = true` (auto-generated for flag components) rather than storing a `bool` field inside a data component. Tag components are cheaper, more expressive, and integrate with Entitas matchers/groups natively.

### Unique / Singleton Components

For entities that should exist exactly once per context. Use the `[Unique]` attribute.

```csharp
[Game, Unique]
public sealed class PlayerComponent : IComponent { }
```

This generates convenient context-level accessors: `gameContext.playerEntity`. Use unique components for: the player, game clock, score, global configuration, camera target.

Don't overuse `[Unique]` — if there could ever be more than one (e.g., supporting co-op later), don't make it unique.

### Relational Components

When entities need to reference other entities, store the entity reference directly.

```csharp
[Game]
public sealed class FollowTargetComponent : IComponent
{
    public GameEntity target;
}
```

Be careful with stale references — always check `target.isEnabled` before accessing. Consider using an entity index or ID instead if references are long-lived or cross-context.

### Component Granularity

Prefer small, focused components over large monolithic ones.

```csharp
// Good — separate concerns
[Game] public sealed class PositionComponent : IComponent { public Vector2 value; }
[Game] public sealed class VelocityComponent : IComponent { public Vector2 value; }
[Game] public sealed class SpeedComponent : IComponent { public float value; }

// Bad — god component
[Game]
public sealed class PhysicsComponent : IComponent
{
    public Vector2 position;
    public Vector2 velocity;
    public float speed;
    public float mass;
    public float drag;
    public bool isKinematic;
}
```

Smaller components mean more precise system matching and better reusability. A `VelocityComponent` can exist on anything that moves — players, enemies, projectiles, particles — without dragging along irrelevant fields.

However, don't split fields that are *always* read and written together. If `x` and `y` of a position are never accessed independently, keep them in one component as a `Vector2`.

### Cleanup Components

Use the `[Cleanup]` attribute for components that should be automatically removed or destroyed after processing.

```csharp
// Destroy the entire entity at end of frame
[Game, Cleanup(CleanupMode.DestroyEntity)]
public sealed class DestroyedComponent : IComponent { }

// Just remove this component at end of frame (entity survives)
[Game, Cleanup(CleanupMode.RemoveComponent)]
public sealed class DamagedThisFrameComponent : IComponent { }
```

This replaces manual cleanup systems and ensures event-like components don't persist.

---

## 3. System Design

### System Types and When to Use Each

| Interface | When to Use |
|-----------|------------|
| `IInitializeSystem` | One-time setup: create initial entities, configure world state |
| `IExecuteSystem` | Per-frame logic: movement, input polling, timers, AI ticks |
| `ReactiveSystem<T>` | Respond to component changes: rendering sync, sound triggers, state transitions |
| `ICleanupSystem` | End-of-frame housekeeping: remove temporary components, destroy dead entities |
| `ITearDownSystem` | Shutdown/dispose: release resources when feature is removed |

### System Structure

Keep systems focused and stateless where possible. Inject contexts and services through the constructor.

```csharp
public sealed class MovementSystem : IExecuteSystem
{
    readonly IGroup<GameEntity> _movers;

    public MovementSystem(Contexts contexts)
    {
        _movers = contexts.game.GetGroup(
            GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Velocity)
        );
    }

    public void Execute()
    {
        foreach (var e in _movers.GetEntities())
        {
            var pos = e.position.value;
            var vel = e.velocity.value;
            e.ReplacePosition(pos + vel * Time.deltaTime);
        }
    }
}
```

### Caching Groups

Always cache groups in the constructor. Never call `GetGroup()` inside `Execute()`.

```csharp
// Good — cached once
readonly IGroup<GameEntity> _enemies;

public EnemySystem(Contexts contexts)
{
    _enemies = contexts.game.GetGroup(GameMatcher.Enemy);
}

// Bad — re-queries every frame
public void Execute()
{
    var enemies = contexts.game.GetGroup(GameMatcher.Enemy); // Don't do this
}
```

Groups in Entitas are live collections that update automatically when entities gain or lose matching components. Caching them is both correct and performant.

### Compound Matchers

Use `AllOf`, `AnyOf`, and `NoneOf` to precisely target entities.

```csharp
// Enemies that are alive and not invulnerable
_targets = contexts.game.GetGroup(
    GameMatcher
        .AllOf(GameMatcher.Enemy, GameMatcher.Health)
        .NoneOf(GameMatcher.Invulnerable, GameMatcher.Destroyed)
);
```

Precise matchers mean your system iterates only over entities it cares about. Never iterate a broad group and then `if`-check components inside the loop.

---

## 4. Feature Organization

Features group related systems into logical modules. They define system execution order within the feature.

### Naming Conventions

Name features after the gameplay domain they own, not after technical concerns.

```csharp
// Good — domain-driven
public sealed class CombatSystems : Feature
{
    public CombatSystems(Contexts contexts) : base("Combat")
    {
        Add(new DamageSystem(contexts));
        Add(new HealthDepletionSystem(contexts));
        Add(new DeathSystem(contexts));
        Add(new InvulnerabilityTimerSystem(contexts));
    }
}

// Acceptable — technical grouping for cross-cutting concerns
public sealed class ViewSystems : Feature { ... }
public sealed class TearDownSystems : Feature { ... }
```

### Ordering Within Features

Systems within a feature execute in the order they are added. This is your primary tool for controlling data flow. Think in terms of a pipeline:

```
Input → Game Logic → Physics → View → Cleanup
```

Within a feature, order so that producers run before consumers:

```csharp
public sealed class CombatSystems : Feature
{
    public CombatSystems(Contexts contexts) : base("Combat")
    {
        // 1. Detect hits (produces DamagedComponent)
        Add(new CollisionDetectionSystem(contexts));
        // 2. Apply damage (consumes DamagedComponent, modifies Health)
        Add(new ApplyDamageSystem(contexts));
        // 3. Check death (consumes Health, produces DestroyedComponent)
        Add(new DeathCheckSystem(contexts));
    }
}
```

### Feature Registration Order

The order features are added to the root system matters. Establish a clear, documented pipeline:

```csharp
// GameController.cs
_systems = new Feature("Root")
    .Add(new InputSystems(contexts))       // 1. Read input
    .Add(new PlayerSystems(contexts))      // 2. Player actions
    .Add(new AISystems(contexts))          // 3. AI decisions
    .Add(new CombatSystems(contexts))      // 4. Combat resolution
    .Add(new MovementSystems(contexts))    // 5. Apply movement
    .Add(new ViewSystems(contexts))        // 6. Sync to rendering
    .Add(new TearDownSystems(contexts));   // 7. Cleanup dead entities
```

---

## 5. Context Separation

Contexts partition your entity world into independent pools. Each context has its own component pool, entity pool, and group indices.

### When to Use Separate Contexts

Use separate contexts when:
- Entities in one domain should never be mixed with another (input events vs game objects)
- You want isolated `[Unique]` singletons per domain
- The domains have fundamentally different lifecycles

Current project contexts and their intended roles:

| Context | Purpose |
|---------|---------|
| `Game` | All gameplay entities: player, enemies, projectiles, pickups, obstacles |
| `Input` | Raw input state: mouse position, button states, axis values |
| `GameState` | High-level game state: score, wave number, game phase, timers |
| `Config` | Static configuration: weapon stats, enemy templates, balance values |

### Cross-Context Communication

Systems can read from multiple contexts. Inject `Contexts` (plural) rather than a single context when a system needs to bridge domains.

```csharp
public sealed class PlayerInputSystem : IExecuteSystem
{
    readonly Contexts _contexts;

    public PlayerInputSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        // Read from Input context
        var mousePos = _contexts.input.leftMouseEntity.mousePosition.value;

        // Write to Game context
        _contexts.game.playerEntity.ReplaceAimDirection(
            (mousePos - _contexts.game.playerEntity.position.value).normalized
        );
    }
}
```

The pattern: Input systems *read* from `Input` context and *write* to `Game` context. View systems *read* from `Game` context and *write* to Unity GameObjects. This creates a clean unidirectional data flow.

---

## 6. Reactive vs Execute Systems

### When to Use ReactiveSystem

Use reactive systems when you want to respond to *changes*, not poll every frame:

- **View synchronization** — update Transform when Position changes
- **Sound effects** — play sound when DamagedThisFrame is added
- **State transitions** — trigger animation when state component changes
- **One-shot events** — respond to "entity was just created" or "entity was just destroyed"

```csharp
public sealed class RenderPositionSystem : ReactiveSystem<GameEntity>
{
    public RenderPositionSystem(Contexts contexts)
        : base(contexts.game) { }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        => context.CreateCollector(GameMatcher.Position);

    protected override bool Filter(GameEntity entity)
        => entity.hasPosition && entity.hasView;

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
            e.view.gameObject.transform.position = e.position.value;
    }
}
```

### When to Use IExecuteSystem

Use execute systems for logic that must run every frame regardless of changes:

- **Input polling** — check input state every frame
- **Timers and cooldowns** — tick down every frame
- **Continuous movement** — apply velocity every frame
- **AI evaluation** — re-evaluate decisions periodically

### Filter Method Best Practices

Always implement `Filter()` in reactive systems. Between the time a component changes and the time the reactive system processes it, the entity may have been modified further (components removed, entity destroyed).

```csharp
protected override bool Filter(GameEntity entity)
    => entity.hasPosition && entity.hasView && !entity.isDestroyed;
```

---

## 7. Common Patterns

### Event Components (One-Frame Components)

For events that should be processed once and then discarded, use components with `[Cleanup(CleanupMode.RemoveComponent)]`.

```csharp
[Game, Cleanup(CleanupMode.RemoveComponent)]
public sealed class DamagedThisFrameComponent : IComponent
{
    public float amount;
    public GameEntity source;
}
```

Systems that care about damage react to it. At the end of the frame, the component is auto-removed. This is Entitas's answer to events — no event bus, no delegates, just data.

### Entity Blueprints / Factory Pattern

Centralize entity creation into factory methods or dedicated systems rather than scattering `CreateEntity()` + `Add...()` chains across multiple systems.

```csharp
public static class EntityFactory
{
    public static GameEntity CreateProjectile(
        Contexts contexts, Vector2 position, Vector2 direction, float speed)
    {
        var e = contexts.game.CreateEntity();
        e.isProjectile = true;
        e.AddPosition(position);
        e.AddVelocity(direction * speed);
        e.AddSprite("projectile_basic");
        e.AddLifeTime(3.0f);
        return e;
    }

    public static GameEntity CreateEnemy(
        Contexts contexts, Vector2 position, string spriteId, float health)
    {
        var e = contexts.game.CreateEntity();
        e.isEnemy = true;
        e.AddPosition(position);
        e.AddHealth(health, health);
        e.AddSprite(spriteId);
        e.AddSpeed(2.0f);
        return e;
    }
}
```

Benefits:
- Single place to change entity composition
- Easy to see all components an archetype needs
- Prevents forgetting a required component

### State Machines via Component Swapping

Model entity state by adding/removing tag components rather than using an enum field.

```csharp
// Instead of:
[Game] public sealed class EnemyStateComponent : IComponent { public EnemyState state; }

// Prefer:
[Game] public sealed class IdleComponent : IComponent { }
[Game] public sealed class ChasingComponent : IComponent { }
[Game] public sealed class AttackingComponent : IComponent { }
[Game] public sealed class FleeingComponent : IComponent { }
```

Then write separate systems per state:

```csharp
// Only processes enemies that are chasing
_chasingEnemies = contexts.game.GetGroup(
    GameMatcher.AllOf(GameMatcher.Enemy, GameMatcher.Chasing)
);
```

Advantages:
- Each system only iterates relevant entities (no switch/if on state)
- Systems are smaller and focused
- Adding new states requires no modification to existing systems

### Entity Index for Fast Lookups

When you need to find entities by a component value (e.g., "find the entity at grid position (3, 5)"), use Entitas entity indices instead of iterating all entities.

```csharp
// Register index at context creation
contexts.game.AddEntityIndex(new PrimaryEntityIndex<GameEntity, Vector2Int>(
    "GridPosition",
    contexts.game.GetGroup(GameMatcher.GridPosition),
    (entity, component) => ((GridPositionComponent)component).value
));

// Fast O(1) lookup
var entity = contexts.game.GetEntityIndex("GridPosition")
    .GetEntity(new Vector2Int(3, 5));
```

Use for: spatial lookups, ID-based lookups, any "find entity where component.field == X" query.

### Request / Response Pattern

For actions that involve multiple systems (e.g., "attack an enemy"), use a request entity:

```csharp
// Request component
[Game, Cleanup(CleanupMode.DestroyEntity)]
public sealed class DamageRequestComponent : IComponent
{
    public GameEntity target;
    public float amount;
    public DamageType type;
}

// Creating a request
var request = contexts.game.CreateEntity();
request.AddDamageRequest(targetEntity, 25f, DamageType.Physical);

// Processing system reacts to DamageRequest
public sealed class ProcessDamageSystem : ReactiveSystem<GameEntity> { ... }
```

The request entity is auto-destroyed after the frame. This decouples the "intent to act" from the "processing of the action" and allows multiple systems to react to the same request.

### Collector Pattern for Buffered Reactions

When you need to process all changes that happened during a frame (not just react to each one individually), use a collector manually:

```csharp
readonly ICollector<GameEntity> _collector;

public SomeSystem(Contexts contexts)
{
    _collector = contexts.game
        .GetGroup(GameMatcher.SomeComponent)
        .CreateCollector(GroupEvent.Added);
}

public void Execute()
{
    if (_collector.count == 0) return;

    // Process all collected entities
    foreach (var e in _collector.collectedEntities)
    {
        if (e.isEnabled && e.hasSomeComponent)
        {
            // ... process
        }
    }
    _collector.ClearCollectedEntities();
}
```

---

## 8. Anti-Patterns

### Logic in Components

Components should never contain methods, properties with logic, or Unity API calls. If you find yourself writing a method on a component, move it to a system.

### Systems with Persistent State

Avoid storing mutable state in system fields (beyond cached groups and injected services). Systems should read state from components and write state back to components. If a system needs a timer, put the timer in a component.

```csharp
// Bad — hidden state in system
public sealed class SpawnSystem : IExecuteSystem
{
    float _timeSinceLastSpawn; // Don't do this

    public void Execute()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn > 2f) { ... }
    }
}

// Good — state lives in a component
[GameState, Unique]
public sealed class SpawnTimerComponent : IComponent { public float elapsed; }
```

When state lives in components: it's visible to other systems, it's serializable, it's debuggable in the Entitas visual debugger.

### God Systems

A system that does many unrelated things is as bad as a god class. If your system has multiple independent loops or lots of branching, split it.

### Overusing Reactive Systems

Reactive systems add overhead (collectors, list allocations). Don't use them for things that change every frame anyway. If every entity's position changes every frame, a reactive `RenderPositionSystem` processes the same entities as an execute system would — but with more overhead. Reactive systems shine when changes are *infrequent* relative to the total entity count.

### Broad Groups with Internal Filtering

```csharp
// Bad — iterating all game entities and checking inside
foreach (var e in contexts.game.GetEntities())
{
    if (e.hasEnemy && e.hasHealth && !e.isDestroyed)
    { ... }
}

// Good — precise matcher
_targets = contexts.game.GetGroup(
    GameMatcher
        .AllOf(GameMatcher.Enemy, GameMatcher.Health)
        .NoneOf(GameMatcher.Destroyed)
);
```

### Direct Cross-System Dependencies

Systems should never call other systems. If System A needs something from System B, the communication channel is always components on entities. System B writes data to a component; System A reads that component. Execution order (set in Features) ensures B runs before A.

### Destroying Entities Immediately

Don't call `entity.Destroy()` in the middle of iteration or in a system that other systems depend on in the same frame. Instead, mark entities with a `DestroyedComponent` and let a cleanup system at the end of the pipeline handle destruction. This project already does this correctly with `[Cleanup(CleanupMode.DestroyEntity)]`.

---

## 9. Bridging ECS and Unity

### The View Layer

Entitas entities are pure data — they don't have GameObjects. You need a bridge layer to synchronize ECS state with Unity's visual representation.

**Pattern: View Component + Reactive Rendering**

```
Entity created with Sprite component
    → AddViewSystem creates a GameObject, stores it in ViewComponent
    → RenderPositionSystem syncs Position → Transform.position
    → RenderSpriteSystem syncs Sprite → SpriteRenderer
    → On entity destruction, cleanup system destroys the GameObject
```

This project already follows this pattern. Key principles:

1. **ECS is the source of truth** — the GameObject/Transform just mirrors ECS state
2. **Never read from GameObjects back into ECS** — data flows one direction: ECS → Unity
3. **Create views reactively** — only when the entity first gets a Sprite/View component
4. **Destroy views in cleanup** — ensure the GameObject is destroyed when the entity is

### Object Pooling for Views

For entities created and destroyed frequently (projectiles, particles, damage numbers), pool the GameObjects:

```csharp
public sealed class AddViewSystem : ReactiveSystem<GameEntity>
{
    readonly ObjectPool _pool;

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var go = _pool.Get(e.sprite.id);
            e.AddView(go);
        }
    }
}

public sealed class DestroyViewSystem : ReactiveSystem<GameEntity>
{
    readonly ObjectPool _pool;

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            if (e.hasView)
            {
                _pool.Return(e.view.gameObject);
                e.RemoveView();
            }
        }
    }
}
```

### Input Bridging

Input is a boundary between Unity and ECS. The pattern:

1. An `IExecuteSystem` reads from the input API (Rewired in this project)
2. It writes the raw values to `Input` context components
3. Subsequent systems in the `Game` context read from `Input` context

This cleanly isolates the input API. If you ever switch from Rewired to another system, you only change one system.

### Physics Integration

For simple physics (this project), handle collision detection in ECS:
- Store collider data in components (`CircleColliderComponent`)
- Run collision detection systems that compare positions + radii
- Write collision results to event components

For complex physics (Unity Physics), let Unity handle it and bridge back:
- MonoBehaviour `OnTriggerEnter2D` callbacks write to ECS components
- Create a `CollisionListenerBehaviour` on view GameObjects that reports collisions back to the entity

---

## 10. Performance Considerations

### Component Access

Entitas generates fast component access. However:

- **Batch Replace calls** — each `Replace` triggers group/collector updates. If replacing multiple components on one entity, consider if a single combined component makes sense for tightly-coupled data
- **Avoid Replace when unchanged** — if you're setting a position to the same value, reactive systems still fire. Add a check if this is a hot path:

```csharp
var newPos = pos + vel * dt;
if (newPos != e.position.value)
    e.ReplacePosition(newPos);
```

### Group Iteration

`group.GetEntities()` returns the internal buffer. This is fast but:
- Don't modify the group (add/remove matching components) during iteration without copying first
- For reactive systems, Entitas handles this automatically with the collected entities list

### Entity Count Awareness

Think about how many entities of each type you expect:

| Entity Type | Expected Count | Performance Strategy |
|-------------|---------------|---------------------|
| Player | 1 | `[Unique]`, direct access |
| Enemies | 10-100s | Standard group iteration |
| Projectiles | 100-1000s | Consider pooling, minimal components |
| Particles/VFX | 1000s+ | May need specialized handling or Unity ParticleSystem |

For very high entity counts (thousands of projectiles), minimize component count per entity and keep systems that iterate them tight.

### Struct vs Class Components

Entitas components are classes by default (reference types). For extremely hot paths with thousands of entities, consider:
- Keeping component data minimal
- Avoiding allocations in component fields (no `new List<>()`, no string concatenation)
- Pre-allocating lists/arrays at init time if needed

---

## 11. Testing

### Unit Testing Systems

One of ECS's biggest advantages: systems are trivially testable because they operate on pure data.

```csharp
[Test]
public void MovementSystemUpdatesPosition()
{
    var contexts = new Contexts();
    var system = new MovementSystem(contexts);

    var entity = contexts.game.CreateEntity();
    entity.AddPosition(Vector2.zero);
    entity.AddVelocity(new Vector2(1, 0));

    // Simulate one frame with known delta time
    system.Execute();

    Assert.AreEqual(new Vector2(1, 0) * Time.deltaTime, entity.position.value);
}
```

### Integration Testing Features

Test entire features as a pipeline:

```csharp
[Test]
public void EnemyDiesWhenHealthReachesZero()
{
    var contexts = new Contexts();
    var systems = new CombatSystems(contexts);
    systems.Initialize();

    var enemy = EntityFactory.CreateEnemy(contexts, Vector2.zero, "test", 10f);
    enemy.AddDamagedThisFrame(10f, null);

    systems.Execute();
    systems.Cleanup();

    Assert.IsTrue(enemy.isDestroyed);
}
```

### Test Isolation

Each test creates fresh `Contexts`. No shared state between tests. No need for mocking in most cases since components are plain data.

---

## Summary: Rules of Thumb

1. **Components are data. Systems are logic. No exceptions.**
2. **One system, one job.** If you can't name it in 3 words, split it.
3. **State lives in components**, not in system fields or static variables.
4. **Use precise matchers** — don't iterate broadly and filter manually.
5. **Cache groups** in constructors. Never allocate in Execute.
6. **Reactive for infrequent changes** (view sync, events). Execute for per-frame logic.
7. **Unidirectional data flow**: Input → Game Logic → View. Never read back from views.
8. **Destroy via marking** (`DestroyedComponent`), not immediate `Destroy()`.
9. **Factory methods** for entity creation. One place to see an archetype's shape.
10. **Feature ordering is your execution model.** Document it. Reason about it.

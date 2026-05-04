# GeometrySurvivor

A 2D survivor-style shooter prototype built with **Unity 6** and **Entitas-CSharp** (Entity-Component-System architecture). Survive waves of geometric enemies in a top-down arena.

## Status

**Early prototype** -- core ECS architecture and basic gameplay systems are in place. Enemy AI, projectiles, and player input are functional.

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| Unity | 6000.3.8f1 | Game engine |
| URP (2D) | 17.3.0 | Rendering pipeline |
| Entitas-CSharp | - | Entity-Component-System framework |
| Rewired | - | Input management |
| Cinemachine | 3.1.5 | Camera system |
| Aseprite Importer | 3.0.1 | Sprite import from Aseprite |

## Architecture

The project uses **Entitas-CSharp** for ECS. Game logic is organized into systems grouped by feature:

```
Assets/_Project/GeoSurvivor/
├── Features/            # System feature groups
│   ├── InputSystems     # Input capture and routing
│   ├── MovementSystems  # Player and enemy movement
│   ├── PlayerSystems    # Player actions (attack, etc.)
│   ├── ViewSystems      # GameObject/view creation and rendering
│   ├── DestroyFxSystems # Destruction effects
│   └── CleanUpSystems   # Entity cleanup and pooling
├── GameComponents/      # ECS component definitions
├── GamePlayer/          # Player-specific components and systems
├── GameSystems/         # Individual systems
│   └── ReactiveSystems/ # Entitas reactive systems
├── Input/               # Input components and systems
├── Enemies/             # Enemy logic
├── Projectiles/         # Projectile logic
├── Weapons/             # Weapon firing logic
├── Managers/            # GameController (ECS entry point)
└── Extensions/          # Context helpers
```

Generated Entitas code lives under `Assets/Generated/`.

### Physics Layers

| Layer | Name |
|-------|------|
| 6 | Player |
| 7 | Enemy |
| 8 | Projectile |
| 9 | Scenario |

## Getting Started

1. Clone the repository
2. Open the project in **Unity 6000.3.8f1** (required version)
3. Open the scene `Assets/_Project/Scenes/MainScene.unity`
4. Press Play

## Scenes

- **MainScene** -- main gameplay scene
- **EntitasTest** -- test/development scene

## Requirements

- Unity 6000.3.8f1
- Rewired (included in `Assets/Plugins/Rewired/`)
- Entitas-CSharp (included in `Assets/Plugins/Entitas/`)

## License

All rights reserved.

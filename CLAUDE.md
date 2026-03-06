# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GeometrySurvivor is an early-stage 2D survivor-style shooter built with **Unity 6** (6000.3.8f1) using the **Universal Render Pipeline (URP)** for 2D rendering. The project is in prototype phase with basic player movement and weapon/projectile systems.

## Build & Development

- **Unity Version:** 6000.3.8f1 (must be opened in Unity Editor)
- **Render Pipeline:** URP 2D (configured in `Assets/Settings/`)
- **IDE:** Visual Studio or JetBrains Rider (both have package support configured)
- **Solution file:** `GeometrySurvivor.sln`
- There are no CLI build scripts; the project is built through the Unity Editor
- Single build scene: `Assets/Scenes/MainScene.unity`

## Architecture

**MonoBehaviour component model** — standard Unity GameObject/Component pattern. There are plans to migrate performance-critical systems (like projectiles) to ECS.

### Input System
- Uses **Rewired** (third-party input manager) — NOT Unity's built-in Input System
- Input constants are auto-generated in `Assets/Scripts/Input/RewiredConsts.cs` (do not hand-edit)
- The Rewired Input Manager lives in `Assets/Prefabs/Managers/RewiredInputManager.prefab` (DontDestroyOnLoad)
- Access input via `ReInput.players.GetSystemPlayer()` and `RewiredConsts.Action.*` constants
- Defined actions: MoveHorizontal, MoveVertical, BasicAttack, Skill1-4

### Core Scripts (`Assets/Scripts/`)
- `PlayerInputHandler.cs` — reads Rewired input, handles movement and attack triggering
- `Weapons/Weapon.cs` — fires projectile prefabs with cooldown, uses `SendMessage("SetDirection")` to configure projectiles
- `Projectiles/Projectile.cs` — base projectile that moves in a set direction each FixedUpdate

### Physics Layers (defined in TagManager)
| Layer | Name |
|-------|------|
| 6 | Player |
| 7 | Enemy |
| 8 | Projectile |
| 9 | Scenario |

### Key Packages
- `com.unity.render-pipelines.universal` 17.3.0 — URP
- `com.unity.cinemachine` 3.1.5 — camera system
- `com.unity.burst` 1.8.27 + `com.unity.collections` 2.6.2 + `com.unity.mathematics` 1.2.6 — performance/ECS foundations
- `com.unity.2d.tilemap` + `com.unity.2d.tilemap.extras` — tilemap support
- `com.unity.2d.aseprite` 3.0.1 — Aseprite sprite import

## Git

- When committing, only include the `Co-Authored-By: Claude` trailer if the commit contains changes actually made by Claude. Remove it from commits that only contain human-authored changes.

## Conventions

- Serialized fields use underscore prefix: `_moveSpeed`, `_cooldown`
- Game logic runs in `FixedUpdate` (player movement, projectile movement)
- Prefabs are organized under `Assets/Prefabs/` by category (Camera, Managers, PlayerCharacters)
- Art assets are in `Assets/Art/` (currently using Kenney Tiny Town pack)

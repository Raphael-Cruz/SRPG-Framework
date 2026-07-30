# SRPG Framework

A tactical turn-based RPG framework built in **Unity (C#)** with a strong focus on **modular architecture**, **data-driven systems**, and **reusable gameplay components**.

Rather than building a single game, this project aims to create a scalable SRPG framework where combat, movement, AI, UI, and gameplay systems remain independent, extensible, and easy to maintain.

---

# Overview

This project is inspired by classic tactical RPGs where positioning, terrain, turn order, and tactical decisions matter as much as character statistics.

The framework emphasizes clean software architecture by separating gameplay systems into independent modules that communicate through well-defined interfaces.

Current development focuses on building the game's core systems before expanding into content creation.

---

# Current Features

## Grid & Navigation

* ✅ Procedural grid generation
* ✅ Tile selection and highlighting
* ✅ 8-direction movement
* ✅ Movement range calculation
* ✅ Tactical cursor and hover feedback
* ✅ Grid-based pathfinding

---

## Unit System

* ✅ ScriptableObject-driven unit data
* ✅ Player and enemy unit support
* ✅ Runtime unit spawning
* ✅ Unit state management
* ✅ Portrait integration

---

## Turn System

* ✅ Initiative-based turn order
* ✅ Turn manager
* ✅ Battle manager
* ✅ Player turn flow
* ✅ Action state management

---

## Combat System

* ✅ Modular combat simulator
* ✅ Combat prediction pipeline
* ✅ Damage calculation
* ✅ Hit chance calculation
* ✅ Defense & Avoid systems
* ✅ Combat modifiers
* ✅ HP prediction gauges
* ✅ Real-time battle preview
* ✅ Extensible combat evaluators

---

## UI

* ✅ Tactical battle preview
* ✅ HP radial gauges
* ✅ Damage prediction overlay
* ✅ Animated page-flipping battle preview
* ✅ Modifier panel framework

---

## AI (Foundation)

* ✅ Utility AI architecture
* ✅ Action generation framework
* ✅ Combat prediction reuse
* ✅ Utility scoring system
* ✅ AI personality profiles
* ✅ Action prediction pipeline

---

# Architecture

The project follows a modular architecture where every system has a single responsibility.

```text
Game
│
├── Input
│   ├── InputManager
│   ├── MouseSelector
│   └── CameraController
│
├── Grid
│   ├── GridManager
│   ├── GridTile
│   ├── Pathfinding
│   └── Range Calculators
│
├── Units
│   ├── Unit
│   ├── UnitData
│   ├── UnitSpawner
│   └── Unit Controllers
│
├── Battle
│   ├── BattleManager
│   ├── TurnManager
│   └── Initiative System
│
├── Combat
│   ├── CombatSimulator
│   ├── CombatResolver
│   ├── CombatPrediction
│   ├── CombatModifiers
│   ├── CombatFacts
│   └── CombatEvaluators
│
├── AI
│   ├── AIActionGenerator
│   ├── IAIAction
│   ├── AIActionOutcome
│   ├── UtilityActionScorer
│   └── AIPersonalityProfile
│
└── UI
    ├── Combat Preview
    ├── HP Gauges
    ├── Modifier Views
    └── Battle Panels
```

Each system is designed to evolve independently while sharing common gameplay data.

---

# Combat Architecture

Combat is entirely simulation-driven.

Instead of directly calculating damage inside gameplay systems, combat flows through a prediction pipeline.

```text
CombatContext
        │
        ▼
CombatSimulator
        │
        ▼
Fact Providers
        │
        ▼
Combat Evaluators
        │
        ▼
Combat Modifiers
        │
        ▼
CombatResolver
        │
        ▼
CombatPrediction
```

This allows combat previews, AI, UI, and gameplay logic to use the exact same simulation results.

---

# AI Architecture

The AI is designed around **Utility AI**, allowing units to evaluate possible actions rather than following scripted behavior trees.

Decision flow:

```text
Observe Battlefield
        │
        ▼
Generate Actions
        │
        ▼
Predict Outcomes
        │
        ▼
Score Utility
        │
        ▼
Choose Best Action
        │
        ▼
Execute
```

Current implementation includes:

* Action generation
* Combat prediction
* Utility scoring
* Personality profiles
* Action outcome prediction

The long-term goal is to support advanced tactical behaviors without rewriting the AI core.

---

# Technologies

| Technology         | Purpose                  |
| ------------------ | ------------------------ |
| Unity              | Game Engine              |
| C#                 | Gameplay Programming     |
| Unity Input System | Player Controls          |
| ScriptableObjects  | Data-driven Architecture |
| Git                | Version Control          |

---

# Development Roadmap

## Phase 1 — Foundation ✅

* Grid generation
* Unit architecture
* Input system
* Camera controls
* Movement system

---

## Phase 2 — Combat Foundation ✅

* Turn management
* Initiative system
* Combat simulator
* Combat prediction
* Battle preview
* HP gauge system
* Modifier framework

---

## Phase 3 — AI Foundation 🚧

* ✅ Utility AI framework
* ✅ Action generation
* ✅ Action prediction
* ✅ Personality profiles
* ⏳ Action selection
* ⏳ Movement actions
* ⏳ Tactical positioning

---

## Phase 4 — Advanced Gameplay

* Character classes
* Skills & abilities
* Equipment
* Status effects
* Terrain interactions
* Objectives
* Campaign progression

---

# Project Goals

This project serves both as a playable tactical RPG prototype and as an exploration of scalable game architecture.

Primary goals:

* Build a complete SRPG framework
* Practice advanced software architecture in Unity
* Create reusable gameplay systems
* Explore modular AI design
* Develop maintainable and extensible combat systems

---

# Developer

**Raphael Cruz**

Full Stack Developer transitioning into gameplay and systems programming.

Areas demonstrated in this project include:

* C#
* Unity
* Object-Oriented Programming
* Gameplay Systems Architecture
* Utility AI Design
* Modular Combat Systems
* Data-Driven Design
* Software Engineering Principles

---

# License

This project is currently developed for educational purposes and as a portfolio piece.

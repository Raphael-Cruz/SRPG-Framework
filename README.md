# SRPG-Framework
A turn-based tactical RPG prototype built in Unity (C#) focused on modular architecture, grid-based combat systems, and data-driven gameplay design.


# Tactical SRPG Project



A turn-based tactical RPG prototype built in **Unity (C#)** focused on modular architecture, grid-based combat systems, and data-driven gameplay design.

The goal of this project is to create a flexible SRPG framework inspired by classic tactical RPGs, featuring strategic movement, unit management, abilities, and combat systems.

---

## 🎮 Overview

This project is a tactical role-playing game prototype where players control units on a grid-based battlefield. Each decision matters: positioning, movement range, abilities, and terrain awareness play an important role in combat.

The project focuses not only on gameplay but also on creating a clean and scalable code architecture.

---

## ✨ Current Features

### Core Systems

✅ Grid-based tactical battlefield
✅ Tile selection and highlighting
✅ Unit placement system
✅ Unit data architecture using ScriptableObjects
✅ Player input system using Unity Input System
✅ Tactical camera controls
✅ Movement range visualization

### Combat Systems

🚧 Turn-based combat system *(in development)*
🚧 Attack range calculation *(in development)*
🚧 Damage and health systems *(in development)*
🚧 Skills and abilities *(in development)*

### Future Systems

* Enemy AI
* Multiple character classes
* Equipment system
* Status effects
* Terrain effects
* Campaign progression
* Save/load system

---

## 🛠️ Technologies

| Technology         | Usage                    |
| ------------------ | ------------------------ |
| Unity              | Game engine              |
| C#                 | Gameplay programming     |
| Unity Input System | Player controls          |
| ScriptableObjects  | Data-driven architecture |
| Git                | Version control          |

---

## 🏗️ Architecture

The project follows a modular approach, separating gameplay systems into independent components.

Example architecture:

```
Game
│
├── Grid System
│   ├── GridManager
│   ├── GridTile
│   └── Pathfinding
│
├── Units
│   ├── Unit
│   ├── UnitData
│   └── UnitController
│
├── Combat
│   ├── TurnManager
│   ├── ActionSystem
│   └── DamageSystem
│
└── Input
    └── InputManager
```

The objective is to keep systems reusable and easy to expand.

---

## 📸 Screenshots

### Tactical Grid

![Grid Screenshot](docs/images/grid.png)

### Unit Selection

![Selection Screenshot](docs/images/selection.png)

---

## 🚧 Development Roadmap

### Phase 1 - Foundation ✅

* [x] Unity project setup
* [x] Grid generation
* [x] Tile selection
* [x] Unit spawning
* [x] Basic unit architecture

### Phase 2 - Combat Foundation

* [x] Turn management
* [x] Movement execution
* [x] Attack system
* [x] Damage calculation
* [x] Action Menu UI 

### Phase 3 - Advanced Gameplay

* [ ] Enemy AI
* [ ] Skills
* [ ] Equipment
* [ ] Status effects
* [ ] Campaign system

---

## 📚 Documentation

Additional documentation can be found in:

```
/Docs
```

Including:

* Game Design Document
* Technical Design Document
* Architecture notes
* Development decisions

---

## 🎯 Project Goals

This project serves as both a game development experiment and a study in software architecture.

Main goals:

* Build a complete SRPG prototype
* Practice advanced Unity and C# patterns
* Develop scalable gameplay systems
* Improve game programming skills

---

## 👤 Developer

**Raphael Cruz**

Full Stack Developer transitioning deeper into game development.

Skills demonstrated in this project:

* C#
* Unity
* Object-oriented programming
* Game architecture
* Systems design

---

## 📄 License

This project is currently for educational and portfolio purposes.

```
```

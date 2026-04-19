# Movement Prototype – Devlog

## Overview

This prototype is part of my Unity learning roadmap, focusing on understanding different movement systems in 2D.

The goal is not to build a complete game, but to explore how movement “feels” and how different approaches affect player control.

---

## Implemented Systems

### Movement Modes

* **Arcade Mode**: Direct velocity control, instant response
* **Platformer Mode**: Enhanced jump system with game-feel improvements
* **Physics Mode**: Force-based movement with inertia

---

### Platformer Features

* Coyote Time
* Jump Buffer
* Variable Jump Height
* Better Falling (faster fall than rise)

---

### Technical Highlights

* Input handled in Update, physics in FixedUpdate
* Ground detection using BoxCast
* Separation of movement logic by mode
* Handling edge cases when switching modes (gravity & velocity reset)

---

## Challenges & Learnings

### 1. Input Timing

Using `wasPressedThisFrame` inside FixedUpdate caused missed inputs.
→ Solved by capturing input in Update and consuming it in FixedUpdate.

### 2. Game Feel vs Physics

Initially, jump felt “floaty”.
→ Learned that gravity scaling is more important than jump force.

### 3. System Design

At first, all features applied to all modes.
→ Refactored to separate Arcade / Platformer / Physics behaviors.

---

## Key Takeaways

* Game feel comes mostly from tuning, not complexity
* Physics-based movement is less controllable but more realistic
* Platformer systems often “assist” the player (coyote time, buffer)

---

## Next Steps

* Build a Combat Prototype
* Apply movement knowledge into real gameplay scenarios

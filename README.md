# Unity Movement Prototype

A 2D movement prototype developed in Unity to explore and compare different character movement systems.

The goal of this project is not to create a complete game, but to understand how different movement approaches affect responsiveness, control, and overall game feel.

## Features

### Movement Modes

#### Arcade Mode

* Direct velocity control
* Instant acceleration and stopping
* Responsive and predictable movement

#### Platformer Mode

* Enhanced platforming movement with game-feel improvements
* Designed to provide forgiving and responsive controls

#### Physics Mode

* Force-based movement using Rigidbody2D
* Momentum and inertia-driven behavior
* More realistic but less precise control

## Platformer Mechanics

* Coyote Time
* Jump Buffer
* Variable Jump Height
* Better Falling

## Technical Highlights

* Input processing separated from physics simulation
* Update / FixedUpdate workflow
* Ground detection using BoxCast
* Modular movement architecture
* Runtime movement mode switching
* Velocity and gravity state handling during mode transitions

## Controls

* A / D : Move
* Space : Jump
* 1 : Arcade Mode
* 2 : Platformer Mode
* 3 : Physics Mode

## Learning Objectives

This project was created to practice:

* Character controller implementation
* Movement system design
* Game feel techniques
* Unity physics fundamentals
* Input handling best practices
* Code organization and separation of responsibilities

## Key Takeaways

* Small adjustments can have a significant impact on game feel
* Platformer movement often relies on player-friendly assistance systems
* Physics-based movement provides realism but reduces precision
* Separating input and physics logic improves reliability and maintainability

## Future Improvements

* Slope handling
* Wall interactions
* Dash mechanics
* Additional movement archetypes

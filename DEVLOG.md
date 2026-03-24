# Movement Prototype – Devlog

## Context
This project is part of Phase 2 - Prototype.

The goal is to explore different movement systems in Unity 2D.

## Features Implemented
- Arcade movement (instant velocity)
- Platformer movement (acceleration-based)
- Better falling
- Coyote time
- Jump buffer
- Variable jump height

## Technical Challenges
- Handling input between Update and FixedUpdate
- Implementing reliable ground detection with BoxCast, visualizing by OnDrawGizmosSelected 
- Managing jump timing (coyote time & jump buffer)

## Observations
- Movement feel depends more on tuning than complexity
- Coyote time significantly improves player experience
- Input timing can easily cause bugs if not handled carefully

## Next Steps
- Tune movement feel
- Implement Physics-based movement mode

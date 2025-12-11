# Titan Soccer 2D Setup Guide

This project is configured for 2D play. Follow the steps below to wire the existing controllers into a scene and validate that touch/mouse input, passing, shooting, AI positioning, and the orthographic camera all work together.

## Prerequisites
- Unity 2021.3 LTS or newer (tested with recent 2021+ builds)
- 2D URP or Built‑in Render Pipeline
- Input System: current scripts use the legacy `Input` API; enable it in **Project Settings ▸ Player ▸ Other Settings ▸ Active Input Handling** (either `Input Manager (Old)` or `Both`).

## Scene Setup
1. **Pitch layer**
   - Create a `Pitch` layer and assign it to your field sprite/tilemap. The `TouchInputController.PitchMask` should include this layer so screen taps raycast onto the field.

2. **Ball**
   - Create a GameObject named `Ball` with `Rigidbody2D` (Dynamic, zero gravity) and a `CircleCollider2D` sized to the sprite.
   - Add **BallController** and configure:
     - `Base Pass Speed`, `Loft Bonus`, `Max Loft Multiplier` for pass tuning.
     - `Base Shot Force`, `Curve Force`, `Spin Duration` for shooting.
     - `Defenders`: assign opposing **PlayerController** references for interception checks.

3. **Players**
   - For each player GameObject:
     - Add `Rigidbody2D` (Dynamic) and a `Collider2D` matching the sprite bounds.
     - Add **PlayerController** and **PlayerAttributes**.
     - Set `PlayerAttributes` values (Pass Power/Accuracy, Shot Power, Spin Control, Speed, Awareness, Stamina, etc.).
     - If using AI, add **AIStateController** and assign a role (e.g., STP, MOO, SĞB). Ensure the team list references your teammates/opponents as needed.

4. **Controlled player**
   - Decide which player the user controls and assign it to the input component below. Ensure its collider is large enough for `OverlapPoint` selection on tap.

5. **Input**
   - Create an empty GameObject `Input` and add **TouchInputController**.
   - Set:
     - `World Camera`: your main Camera.
     - `Controlled Player`: the player to move/receive shot gestures.
     - `Ball`: the scene ball.
     - `Pitch Mask`: the `Pitch` layer.
     - Optional: adjust `Hold Threshold` (default 2s) for lofted passes and `Gesture Min Distance` for shot swipes.

6. **Camera**
   - On the main Camera, add **CameraFollowController**.
   - Ensure the Camera is Orthographic; the script forces orthographic mode on Awake.
   - Assign:
     - `Ball`: the scene ball.
     - `Players`: all visible players (both teams) so density-based zoom can react to clusters.
     - Tune `Follow Distance`, `Smooth Time`, `Min/Max Ortho Size`, `Density Radius`, `Recenter Lerp`, and `Recenter Height Boost` to taste.

7. **Game manager (optional save support)**
   - The provided `GameManager` is a simple singleton for save slot tracking. Drop it onto a bootstrap scene if you plan to manage saves.

## Quick Functional Test
1. Enter Play mode.
2. **Movement:** Click/tap empty pitch – the controlled player should move toward the tapped point with momentum-aware steering.
3. **Passing:**
   - Tap a teammate: sends a ground pass with target prediction.
   - Hold ≥2s on a teammate: sends a lofted pass with distance-scaled lift.
   - Defenders in `BallController.Defenders` will attempt interceptions if close to the lane.
4. **Shooting:** Hold on the controlled player and drag; release to shoot. Fast straight swipes produce low/flat powerful shots; slower/curved swipes add loft/spin.
5. **Camera:** Follows the ball smoothly, zooms based on nearby player density, and recenters quickly when possession changes.

## Notes
- All physics rely on `Rigidbody2D`; ensure 3D physics components are not present on the same objects.
- If using both touch and mouse, the scripts prioritize touch when available.
- For best collision pickup on possession, keep the ball and player colliders slightly overlapping on contact.

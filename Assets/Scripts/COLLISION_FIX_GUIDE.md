# Collision Detection Fix Guide

## Problem
Collision detection between player and enemy vehicles was not working because enemy vehicles were using `transform.position` for movement, which bypasses Unity's physics system.

## Solution Applied
Updated `VehicleMove.cs` to use `Rigidbody.MovePosition()` instead of `transform.position`. This ensures proper physics-based movement and collision detection.

## Required Unity Inspector Settings

### Player GameObject Settings
1. **Box Collider**:
   - Is Trigger: **OFF** (unchecked)
   - Size: Should cover the vehicle model

2. **Rigidbody**:
   - Is Kinematic: **OFF** (unchecked) - PlayerPhysics uses velocity-based movement
   - Use Gravity: **OFF** (unchecked)
   - Collision Detection: **Continuous Dynamic** (for fast-moving player)
   - Constraints:
     - Freeze Position: Y (checked)
     - Freeze Rotation: X, Y, Z (all checked)

3. **Components Required**:
   - PlayerPhysics (handles movement)
   - PlayerHealth
   - PlayerDamageHandler
   - PlayerInvincibility (optional)

### Enemy Vehicle Prefab Settings
1. **Box Collider**:
   - Is Trigger: **OFF** (unchecked)
   - Size: Should cover the vehicle model

2. **Rigidbody**:
   - Is Kinematic: **OFF** (unchecked) - Now uses Rigidbody.MovePosition()
   - Use Gravity: **OFF** (unchecked)
   - Collision Detection: **Continuous** (for moving objects)
   - Constraints:
     - Freeze Position: Y (checked) - Prevents falling through road
     - Freeze Rotation: X, Y, Z (all checked) - Prevents spinning

3. **Components Required**:
   - VehicleMove (now uses physics-based movement)
   - VehicleDamage (stores damage value)

4. **Tag**: Must be set to "Vehicle"

### Layer Collision Matrix
1. Go to: Edit → Project Settings → Physics
2. Ensure "Default" layer collides with itself (checkbox should be checked)

## Why This Works
- `Rigidbody.MovePosition()` moves objects through the physics system
- Physics system can detect collisions between moving objects
- Constraints prevent unwanted movement (falling, spinning)
- Continuous collision detection prevents fast-moving objects from passing through each other

## Testing
1. Start the game
2. Drive the player car into an enemy vehicle
3. Check Console for: `[PlayerDamageHandler] Collision detected with: [VehicleName], Tag: Vehicle`
4. Verify health decreases and UI updates
5. Verify game ends when health reaches zero

## Common Issues

### Issue: Player spawns on barrier
**Cause**: Player collider overlaps with barrier at spawn
**Solution**: Adjust player spawn position in scene or barrier placement

### Issue: Enemies still fly off road
**Cause**: Rigidbody constraints not set correctly
**Solution**: Freeze Position Y and all Rotations on enemy Rigidbody

### Issue: Collisions still not detected
**Cause**: Colliders too small or not covering models
**Solution**: In Scene view, check green wireframe covers the vehicle. Adjust collider size if needed.

# Player GameObject Setup Guide

## Overview

This guide provides step-by-step instructions for adding the health and damage system components to the player GameObject in Unity. These components enable the player vehicle to track health, receive damage from collisions, and trigger game over conditions.

## Prerequisites

Before starting, ensure you have:
- Unity project open with the racing game scene
- Player GameObject present in the scene hierarchy
- PlayerHealth.cs and PlayerDamageHandler.cs scripts compiled without errors

## Setup Steps

### Step 1: Add PlayerHealth Component

1. **Select the Player GameObject**
   - In the Hierarchy window, locate and click on your player GameObject (typically named "Player", "PlayerVehicle", or similar)

2. **Add the PlayerHealth Component**
   - In the Inspector window, click the "Add Component" button at the bottom
   - Type "PlayerHealth" in the search box
   - Click on "Player Health" to add the component

3. **Configure PlayerHealth Settings**
   - In the Inspector, you'll see the PlayerHealth component with the following field:
     - **Max Health**: Set to desired starting health value (default: 100)
   - The `currentHealth` will automatically initialize to `maxHealth` when the game starts

### Step 2: Add PlayerDamageHandler Component

1. **Add the PlayerDamageHandler Component**
   - With the player GameObject still selected
   - Click "Add Component" again
   - Type "PlayerDamageHandler" in the search box
   - Click on "Player Damage Handler" to add the component

2. **Verify Component Dependencies**
   - The PlayerDamageHandler component will automatically detect the PlayerHealth component on the same GameObject
   - No additional configuration is needed for PlayerDamageHandler

### Step 3: Verify Collision Setup

For the damage system to work correctly, the player GameObject must be properly configured for collision detection:

#### 3.1 Verify Collider Configuration

1. **Check for Collider Component**
   - In the Inspector, look for a Collider component (Box Collider, Capsule Collider, Mesh Collider, etc.)
   - If no collider exists, add one:
     - Click "Add Component"
     - Search for the appropriate collider type (e.g., "Box Collider")
     - Add the collider

2. **Enable "Is Trigger" Option**
   - In the Collider component settings, locate the "Is Trigger" checkbox
   - **IMPORTANT**: Check the "Is Trigger" box
   - This enables trigger-based collision detection (OnTriggerEnter) rather than physics-based collisions

3. **Adjust Collider Size** (if needed)
   - Use the collider's size/radius parameters to match your vehicle's dimensions
   - You can visualize the collider bounds in the Scene view (green wireframe)

#### 3.2 Verify Rigidbody Configuration

Unity's trigger system requires at least one of the colliding objects to have a Rigidbody component:

**Option A: Add Rigidbody to Player (Recommended)**
1. Check if player GameObject has a Rigidbody component
2. If not present, add one:
   - Click "Add Component"
   - Search for "Rigidbody"
   - Add the component
3. Configure Rigidbody settings:
   - **Use Gravity**: Depends on your game (typically false for racing games with custom movement)
   - **Is Kinematic**: Set to true if using custom movement scripts (prevents physics from controlling the player)
   - **Constraints**: Freeze rotation axes if needed to prevent unwanted spinning

**Option B: Verify Enemy Vehicles Have Rigidbody**
- If you prefer not to add Rigidbody to the player, ensure all enemy vehicle prefabs have Rigidbody components
- At least one object in each collision must have a Rigidbody for triggers to work

### Step 4: Verify Enemy Vehicle Configuration

For damage to be applied correctly, enemy vehicles must be properly tagged and configured:

1. **Check Enemy Vehicle Tag**
   - Select an enemy vehicle prefab or instance in the scene
   - In the Inspector, look at the "Tag" dropdown at the top
   - Verify the tag is set to "Vehicle"
   - If not, select "Vehicle" from the dropdown (or create the tag if it doesn't exist)

2. **Verify VehicleDamage Component**
   - Ensure enemy vehicles have the VehicleDamage component attached
   - Check that the `damage` field has a value greater than 0 (e.g., 20)

## Verification Checklist

Use this checklist to confirm proper setup:

- [ ] PlayerHealth component added to player GameObject
- [ ] PlayerHealth maxHealth value configured (e.g., 100)
- [ ] PlayerDamageHandler component added to player GameObject
- [ ] Player GameObject has a Collider component
- [ ] Collider "Is Trigger" checkbox is enabled
- [ ] Player GameObject has a Rigidbody component (or enemy vehicles have Rigidbody)
- [ ] Enemy vehicles are tagged with "Vehicle"
- [ ] Enemy vehicles have VehicleDamage component with damage > 0

## Testing the Setup

To verify the setup works correctly:

1. **Enter Play Mode**
   - Click the Play button in Unity Editor

2. **Test Collision Detection**
   - Move the player vehicle to collide with an enemy vehicle
   - Watch the Console window for any error messages

3. **Verify Health Reduction**
   - After collision, check if the player's health decreases
   - You can add a temporary Debug.Log in PlayerHealth.TakeDamage() to see damage values

4. **Test Game Over Condition**
   - Collide with enemy vehicles multiple times until health reaches zero
   - Verify that the game over sequence triggers correctly

## Troubleshooting

### Collisions Not Detected

**Problem**: Player collides with enemy vehicles but no damage is applied

**Solutions**:
- Verify "Is Trigger" is enabled on player's Collider
- Ensure at least one GameObject (player or enemy) has a Rigidbody
- Check that enemy vehicles have the "Vehicle" tag (case-sensitive)
- Verify both objects have Collider components

### Damage Not Applied

**Problem**: Collisions detected but health doesn't decrease

**Solutions**:
- Check that enemy vehicles have VehicleDamage component
- Verify VehicleDamage.damage value is greater than 0
- Check Console for null reference errors
- Ensure PlayerHealth component is on the same GameObject as PlayerDamageHandler

### Game Over Not Triggering

**Problem**: Health reaches zero but game doesn't end

**Solutions**:
- Verify GameLogicController exists in the scene
- Check that GameLogicController.Instance is accessible
- Look for errors in the Console related to GameLogicController
- Ensure PlayerDamageHandler is subscribed to OnHealthDepleted event

## Component Reference

### PlayerHealth Component

**Purpose**: Tracks and manages player vehicle health

**Public Fields**:
- `maxHealth` (float): Maximum health value (configurable in Inspector)

**Events**:
- `OnHealthChanged`: Fired when health changes (used by UI)
- `OnHealthDepleted`: Fired when health reaches zero (triggers game over)

### PlayerDamageHandler Component

**Purpose**: Detects collisions with enemy vehicles and applies damage

**Dependencies**:
- Requires PlayerHealth component on same GameObject
- Requires GameLogicController in scene

**Collision Detection**:
- Uses OnTriggerEnter to detect collisions
- Only processes collisions with objects tagged "Vehicle"
- Retrieves damage value from VehicleDamage component

## Related Documentation

- **Requirements**: See `.kiro/specs/car-health-damage-system/requirements.md`
  - Requirement 1.1: Player Vehicle Health Tracking
  - Requirement 3.4: Collision Detection System

- **Design**: See `.kiro/specs/car-health-damage-system/design.md`
  - Component Design section for technical details
  - Integration Points for system interactions

- **UI Setup**: See `Assets/Scripts/UI/HEALTH_UI_SETUP_GUIDE.md` (if available)
  - Instructions for setting up the health display UI

## Notes

- The PlayerHealth component automatically initializes currentHealth to maxHealth on Start()
- The PlayerDamageHandler component uses [RequireComponent] attribute to ensure PlayerHealth is present
- All health values are stored as floats to support fractional damage
- The system uses Unity's event system for loose coupling between components
- Collision detection uses trigger-based system (OnTriggerEnter) rather than physics collisions

## Support

If you encounter issues not covered in this guide:
1. Check the Unity Console for error messages
2. Review the component scripts for additional comments
3. Verify all prerequisites are met
4. Consult the design document for architectural details

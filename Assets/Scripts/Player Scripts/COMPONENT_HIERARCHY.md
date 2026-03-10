# Component Hierarchy and Data Flow

## Player GameObject Component Structure

```
Player GameObject
│
├── Transform
│   └── Position, Rotation, Scale
│
├── Collider (Box/Capsule/Mesh)
│   ├── Is Trigger: ☑ ENABLED
│   └── Size/Radius: Match vehicle dimensions
│
├── Rigidbody (Recommended)
│   ├── Use Gravity: false
│   ├── Is Kinematic: true
│   └── Constraints: Freeze rotation as needed
│
├── PlayerHealth (Script) ⭐ NEW
│   ├── Max Health: 100
│   ├── Current Health: (runtime)
│   ├── OnHealthChanged event
│   └── OnHealthDepleted event
│
├── PlayerDamageHandler (Script) ⭐ NEW
│   ├── Requires: PlayerHealth
│   ├── Detects: Vehicle collisions
│   └── Triggers: Game over on death
│
└── [Other existing components]
    ├── PlayerController
    ├── PlayerPhysics
    └── PlayerVisual
```

## Enemy Vehicle Component Structure

```
Enemy Vehicle Prefab
│
├── Tag: "Vehicle" ⚠️ REQUIRED
│
├── Collider (any type)
│   └── Collision detection
│
├── Rigidbody (if player doesn't have one)
│   └── Required for trigger detection
│
├── VehicleDamage (Script) ⚠️ REQUIRED
│   └── Damage: 20 (configurable)
│
└── [Other components]
    ├── VehicleMove
    └── EnemyController
```

## Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     COLLISION OCCURS                         │
│              (Player trigger enters Enemy)                   │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         PlayerDamageHandler.OnTriggerEnter()                 │
│                                                               │
│  1. Check if other.CompareTag("Vehicle")                     │
│  2. If not "Vehicle" → Exit (no damage)                      │
│  3. If "Vehicle" → Continue                                  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         Get VehicleDamage Component                          │
│                                                               │
│  vehicleDamage = other.GetComponent<VehicleDamage>()        │
│                                                               │
│  If null → Log warning, Exit                                 │
│  If found → Get damage value                                 │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         PlayerHealth.TakeDamage(amount)                      │
│                                                               │
│  1. Validate damage amount (negative → 0)                    │
│  2. currentHealth -= amount                                  │
│  3. Clamp: 0 <= currentHealth <= maxHealth                   │
│  4. Fire OnHealthChanged event                               │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              OnHealthChanged Event                           │
│                                                               │
│  Listeners:                                                   │
│  • HealthUI.UpdateHealthDisplay()                            │
│    └─> Updates slider and text                              │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         Check if Health <= 0                                 │
│                                                               │
│  If health > 0 → Done                                        │
│  If health <= 0 → Fire OnHealthDepleted event                │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         PlayerDamageHandler.HandleHealthDepleted()           │
│                                                               │
│  GameLogicController.Instance.TriggerGameOver(               │
│      lastCollidedVehicle,                                    │
│      playerGameObject                                        │
│  )                                                            │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    GAME OVER                                 │
└──────────────────────────────────────────────────────────────┘
```

## Event Flow

```
PlayerHealth Events:
│
├── OnHealthChanged(currentHealth, maxHealth)
│   │
│   ├─> HealthUI.UpdateHealthDisplay()
│   │   └─> Updates UI slider value
│   │   └─> Updates UI text display
│   │
│   └─> [Other listeners can subscribe]
│
└── OnHealthDepleted()
    │
    └─> PlayerDamageHandler.HandleHealthDepleted()
        └─> GameLogicController.TriggerGameOver()
            └─> Game ends
```

## Component Dependencies

```
PlayerDamageHandler
    │
    ├─ REQUIRES ──> PlayerHealth (same GameObject)
    │                   │
    │                   └─ Provides: TakeDamage(), events
    │
    ├─ READS ────> VehicleDamage (on enemy vehicles)
    │                   │
    │                   └─ Provides: damage value
    │
    └─ CALLS ────> GameLogicController.Instance
                        │
                        └─ Provides: TriggerGameOver()
```

## Collision Detection Requirements

For Unity's trigger system to work, you need:

```
Collision Detection Requirements:
│
├── At least ONE object must have:
│   └── Rigidbody component
│       ├── Player (recommended) ✓
│       └── OR Enemy vehicles ✓
│
├── At least ONE object must have:
│   └── Collider with "Is Trigger" enabled
│       └── Player collider ✓
│
└── Both objects must have:
    └── Collider component (any type)
        ├── Player: Box/Capsule/Mesh Collider ✓
        └── Enemy: Any Collider ✓
```

## Tag Configuration

```
Unity Tags Required:
│
└── "Vehicle"
    │
    ├─ Applied to: Enemy vehicle GameObjects
    │
    ├─ Used by: PlayerDamageHandler.OnTriggerEnter()
    │   └─> if (other.CompareTag("Vehicle"))
    │
    └─ Purpose: Identify which collisions should apply damage
```

## Setup Order

Recommended order for adding components:

```
1. Add PlayerHealth
   └─> Configure maxHealth value

2. Add PlayerDamageHandler
   └─> Automatically finds PlayerHealth

3. Verify Collider
   └─> Enable "Is Trigger"

4. Add/Verify Rigidbody
   └─> Configure physics settings

5. Test in Play Mode
   └─> Verify collisions work
```

## Inspector View Reference

When properly configured, the Inspector should show:

```
Player GameObject Inspector:
┌─────────────────────────────────────┐
│ Tag: Player                          │
│ Layer: Default                       │
├─────────────────────────────────────┤
│ Transform                            │
│   Position: (0, 0, 0)                │
│   Rotation: (0, 0, 0)                │
│   Scale: (1, 1, 1)                   │
├─────────────────────────────────────┤
│ Box Collider                         │
│   ☑ Is Trigger                       │ ⭐ IMPORTANT
│   Center: (0, 0, 0)                  │
│   Size: (2, 1, 4)                    │
├─────────────────────────────────────┤
│ Rigidbody                            │
│   Mass: 1                            │
│   ☐ Use Gravity                      │
│   ☑ Is Kinematic                     │
├─────────────────────────────────────┤
│ Player Health (Script)               │ ⭐ NEW
│   Max Health: 100                    │
├─────────────────────────────────────┤
│ Player Damage Handler (Script)       │ ⭐ NEW
│   (No public fields)                 │
├─────────────────────────────────────┤
│ [Other Components...]                │
└─────────────────────────────────────┘
```

## Troubleshooting Flow

```
Problem: Collisions not detected
│
├─> Check: Is Trigger enabled?
│   ├─ No → Enable "Is Trigger" on player Collider
│   └─ Yes → Continue
│
├─> Check: Rigidbody present?
│   ├─ No → Add Rigidbody to player or enemies
│   └─ Yes → Continue
│
├─> Check: Both have Colliders?
│   ├─ No → Add Colliders to both objects
│   └─ Yes → Continue
│
└─> Check: Enemy tagged "Vehicle"?
    ├─ No → Set tag to "Vehicle"
    └─ Yes → Check Console for errors
```

## Related Files

- **Setup Guide**: `PLAYER_SETUP_GUIDE.md` - Detailed step-by-step instructions
- **Quick Checklist**: `SETUP_CHECKLIST.md` - Quick reference for experienced users
- **Component Scripts**:
  - `PlayerHealth.cs` - Health management
  - `PlayerDamageHandler.cs` - Collision and damage handling
- **Enemy Scripts**:
  - `VehicleDamage.cs` - Damage configuration on enemies

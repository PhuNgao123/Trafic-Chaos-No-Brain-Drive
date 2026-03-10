# Player Health System - Quick Setup Checklist

## Quick Reference

This is a condensed checklist for experienced Unity developers. For detailed instructions, see `PLAYER_SETUP_GUIDE.md`.

## Player GameObject Setup

### 1. Add Components
```
Player GameObject
├── PlayerHealth (Script)
│   └── Max Health: 100
└── PlayerDamageHandler (Script)
    └── (Auto-detects PlayerHealth)
```

### 2. Collision Configuration
```
Player GameObject
├── Collider (Box/Capsule/Mesh)
│   └── ☑ Is Trigger: ENABLED
└── Rigidbody (Optional but recommended)
    ├── Use Gravity: false (for racing games)
    └── Is Kinematic: true (if using custom movement)
```

### 3. Enemy Vehicle Configuration
```
Enemy Vehicle Prefab
├── Tag: "Vehicle"
├── VehicleDamage (Script)
│   └── Damage: 20
├── Collider (any type)
└── Rigidbody (if player doesn't have one)
```

## Verification Steps

1. ☐ PlayerHealth component added
2. ☐ PlayerDamageHandler component added
3. ☐ Player has Collider with "Is Trigger" enabled
4. ☐ Player or enemies have Rigidbody
5. ☐ Enemy vehicles tagged "Vehicle"
6. ☐ Enemy vehicles have VehicleDamage component
7. ☐ Test collision in Play mode
8. ☐ Verify health decreases on collision
9. ☐ Verify game over triggers at zero health

## Common Issues

| Issue | Solution |
|-------|----------|
| No collision detected | Enable "Is Trigger" on Collider |
| Collision but no damage | Check "Vehicle" tag on enemies |
| No Rigidbody warning | Add Rigidbody to player or enemies |
| Game over not triggering | Verify GameLogicController exists |

## Requirements Satisfied

- ✓ Requirement 1.1: Player Vehicle Health Tracking
- ✓ Requirement 3.4: Collision Detection System

## Next Steps

After completing this setup:
1. Proceed to UI setup (Task 6.2)
2. Configure health display Canvas
3. Run integration tests

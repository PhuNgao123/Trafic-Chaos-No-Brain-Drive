# Design Document

## Introduction

This document provides the technical design for implementing a car health and damage system in the Unity 3D racing game. The design integrates with the existing PlayerController, VehicleDamage, and GameLogicController components.

## Architecture Overview

The system consists of three main components:

1. **PlayerHealth** - Manages the player vehicle's health state
2. **PlayerDamageHandler** - Handles collision detection and damage application
3. **HealthUI** - Displays health information to the player

These components integrate with existing systems:
- PlayerController (collision detection entry point)
- VehicleDamage (damage configuration on enemy vehicles)
- GameLogicController (game over trigger)

## Component Design

### 1. PlayerHealth Component

**Location:** `Assets/Scripts/Player Scripts/PlayerHealth.cs`

**Responsibilities:**
- Track current and maximum health values
- Provide health modification methods
- Enforce health invariants (0 <= health <= maxHealth)
- Notify listeners when health changes

**Public Interface:**
```csharp
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    
    public event System.Action<float, float> OnHealthChanged; // (currentHealth, maxHealth)
    public event System.Action OnHealthDepleted;
    
    public void TakeDamage(float amount);
    public float GetHealthPercentage();
}
```

**Key Design Decisions:**
- Health is stored as float to support fractional damage values
- Events are used for loose coupling with UI and game logic
- Health percentage method simplifies UI calculations

### 2. PlayerDamageHandler Component

**Location:** `Assets/Scripts/Player Scripts/PlayerDamageHandler.cs`

**Responsibilities:**
- Detect collisions with enemy vehicles
- Retrieve damage values from VehicleDamage component
- Apply damage to PlayerHealth
- Trigger game over when health depletes

**Public Interface:**
```csharp
public class PlayerDamageHandler : MonoBehaviour
{
    private PlayerHealth playerHealth;
    
    void OnTriggerEnter(Collider other);
}
```

**Integration Points:**
- Requires PlayerHealth component on same GameObject
- Reads VehicleDamage component from colliding vehicles
- Calls GameLogicController.Instance.TriggerGameOver() on health depletion

### 3. HealthUI Component

**Location:** `Assets/Scripts/UI/HealthUI.cs`

**Responsibilities:**
- Display current health visually
- Update display when health changes
- Remain visible during gameplay

**Public Interface:**
```csharp
public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public UnityEngine.UI.Slider healthSlider;
    public TMPro.TextMeshProUGUI healthText;
    
    void UpdateHealthDisplay(float current, float max);
}
```

**Visual Design:**
- Uses Unity UI Slider for health bar visualization
- Optional TextMeshPro text for numeric display (e.g., "75/100")
- Health bar fills from left to right, color can indicate health level

## Data Flow

```
Collision Occurs
    ↓
PlayerDamageHandler.OnTriggerEnter()
    ↓
Check if collider has "Vehicle" tag
    ↓
Get VehicleDamage component
    ↓
PlayerHealth.TakeDamage(damage)
    ↓
Update currentHealth (enforce invariants)
    ↓
Fire OnHealthChanged event
    ↓
HealthUI.UpdateHealthDisplay()
    ↓
If health <= 0: Fire OnHealthDepleted event
    ↓
GameLogicController.TriggerGameOver()
```

## Integration with Existing Code

### PlayerController Modification

The existing PlayerController.OnTriggerEnter() will be replaced by PlayerDamageHandler. The PlayerController can be simplified or removed if it has no other responsibilities.

**Current Code:**
```csharp
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Vehicle"))
    {
        // Empty - will be replaced by PlayerDamageHandler
    }
}
```

### VehicleDamage (No Changes Required)

The existing VehicleDamage component already provides the damage value:
```csharp
public float damage = 20f;
```

### GameLogicController (No Changes Required)

The existing TriggerGameOver() method will be called when health depletes:
```csharp
public void TriggerGameOver(GameObject collidedVehicle, GameObject player)
```

## UI Layout

### Canvas Hierarchy
```
Canvas (Screen Space - Overlay)
└── HealthPanel
    ├── HealthSlider (UI Slider)
    │   ├── Background
    │   ├── Fill Area
    │   │   └── Fill (colored based on health)
    └── HealthText (TextMeshPro - optional)
```

### Positioning
- Top-left or top-center of screen
- Always visible during gameplay
- Anchored to canvas for resolution independence

## Correctness Properties

### Property 1: Health Invariant
**Type:** Invariant

**Description:** Health must always remain within valid bounds

**Test:** After any damage application, verify: `0 <= currentHealth <= maxHealth`

### Property 2: Damage Application Correctness
**Type:** Metamorphic Property

**Description:** Health after damage equals health before minus damage amount (clamped to 0)

**Test:** 
```
healthBefore = currentHealth
TakeDamage(amount)
healthAfter = currentHealth
Assert: healthAfter == max(0, healthBefore - amount)
```

### Property 3: UI Synchronization
**Type:** Invariant

**Description:** UI display must always reflect current health state

**Test:** After health change event, verify UI shows correct percentage: `slider.value == currentHealth / maxHealth`

### Property 4: Game Over Trigger
**Type:** Event-driven Property

**Description:** Game over must trigger exactly once when health reaches zero

**Test:** 
```
Set health to small value
Apply damage to deplete health
Assert: OnHealthDepleted fired exactly once
Assert: GameLogicController.isGameOver == true
```

### Property 5: Sequential Damage Processing
**Type:** Invariant

**Description:** Multiple rapid collisions must process damage sequentially without race conditions

**Test:**
```
Initial health = 100
Apply damage(30) and damage(40) in same frame
Final health must be 30 (not undefined)
```

## Implementation Notes

### Unity-Specific Considerations

1. **Component Dependencies:**
   - PlayerHealth should be added to the player GameObject
   - PlayerDamageHandler requires PlayerHealth on same GameObject
   - Use [RequireComponent(typeof(PlayerHealth))] attribute

2. **Collision Detection:**
   - Player GameObject must have a Collider with "Is Trigger" enabled
   - Enemy vehicles must have "Vehicle" tag
   - Rigidbody required on at least one colliding object

3. **UI Setup:**
   - Create Canvas if not exists
   - Add HealthUI component to Canvas or child GameObject
   - Assign references in Inspector (playerHealth, healthSlider, healthText)

### Performance Considerations

- Health calculations are simple arithmetic (no performance concerns)
- UI updates only on health change events (not every frame)
- Collision detection uses Unity's built-in physics system

### Error Handling

- Null checks for component references
- Validate damage values (negative damage treated as 0)
- Graceful degradation if UI references missing (log warning, continue gameplay)

## Testing Strategy

### Unit Tests
- Test PlayerHealth invariants with various damage values
- Test health percentage calculations
- Test event firing on health changes

### Integration Tests
- Test collision detection with tagged/untagged objects
- Test damage flow from collision to health to UI
- Test game over trigger on health depletion

### Manual Testing
- Verify UI visibility and readability during gameplay
- Test multiple rapid collisions
- Verify game over sequence triggers correctly

## File Summary

**New Files:**
1. `Assets/Scripts/Player Scripts/PlayerHealth.cs` - Health management
2. `Assets/Scripts/Player Scripts/PlayerDamageHandler.cs` - Collision and damage handling
3. `Assets/Scripts/UI/HealthUI.cs` - UI display component

**Modified Files:**
1. `Assets/Scripts/Player Scripts/PlayerController.cs` - Remove or simplify OnTriggerEnter (functionality moved to PlayerDamageHandler)

**Unity Assets:**
1. Canvas with HealthUI components (created in Unity Editor)
2. UI Slider for health bar
3. TextMeshPro text for numeric display (optional)

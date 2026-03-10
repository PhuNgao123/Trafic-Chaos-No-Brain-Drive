# Implementation Plan: Car Health and Damage System

## Overview

This plan implements a health and damage system for the Unity 3D racing game. The implementation creates three core components (PlayerHealth, PlayerDamageHandler, HealthUI) that integrate with existing systems (PlayerController, VehicleDamage, GameLogicController) to track player health, apply collision damage, and trigger game over conditions.

## Tasks

- [ ] 1. Create PlayerHealth component
  - [x] 1.1 Implement PlayerHealth.cs with health tracking and events
    - Create `Assets/Scripts/Player Scripts/PlayerHealth.cs`
    - Implement health state management (maxHealth, currentHealth)
    - Implement TakeDamage() method with health invariant enforcement (0 <= health <= maxHealth)
    - Implement GetHealthPercentage() method
    - Add OnHealthChanged and OnHealthDepleted events
    - Initialize currentHealth to maxHealth on Start()
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 7.1, 7.2, 7.3_

  - [ ]* 1.2 Write property test for health invariant
    - **Property 1: Health Invariant**
    - **Validates: Requirements 1.4, 7.1, 7.2, 7.3**

  - [ ]* 1.3 Write property test for damage application correctness
    - **Property 2: Damage Application Correctness**
    - **Validates: Requirements 4.3, 4.4**

- [ ] 2. Create PlayerDamageHandler component
  - [x] 2.1 Implement PlayerDamageHandler.cs with collision detection
    - Create `Assets/Scripts/Player Scripts/PlayerDamageHandler.cs`
    - Add [RequireComponent(typeof(PlayerHealth))] attribute
    - Get PlayerHealth reference in Awake() or Start()
    - Implement OnTriggerEnter() to detect collisions with "Vehicle" tag
    - Retrieve VehicleDamage component from colliding object
    - Call PlayerHealth.TakeDamage() with damage amount
    - Subscribe to OnHealthDepleted event and call GameLogicController.Instance.TriggerGameOver()
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 4.1, 4.2, 5.1, 5.2_

  - [ ]* 2.2 Write unit tests for collision detection logic
    - Test collision with tagged vs untagged objects
    - Test null VehicleDamage component handling
    - _Requirements: 3.1, 3.2, 3.3_

- [x] 3. Checkpoint - Verify core damage system
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 4. Create HealthUI component
  - [x] 4.1 Implement HealthUI.cs with UI display logic
    - Create `Assets/Scripts/UI/HealthUI.cs`
    - Add public references for PlayerHealth, UI Slider, and TextMeshProUGUI
    - Subscribe to PlayerHealth.OnHealthChanged event in Start()
    - Implement UpdateHealthDisplay() method to update slider value and text
    - Add null checks for UI references with warning logs
    - _Requirements: 6.1, 6.2, 6.3, 6.4_

  - [ ]* 4.2 Write property test for UI synchronization
    - **Property 3: UI Synchronization**
    - **Validates: Requirements 6.2**

- [ ] 5. Modify PlayerController to remove collision handling
  - [x] 5.1 Update PlayerController.cs
    - Remove or simplify OnTriggerEnter() method (functionality moved to PlayerDamageHandler)
    - Add comment explaining that collision handling is now in PlayerDamageHandler
    - _Requirements: 3.4, 4.2_

- [ ] 6. Integration and setup
  - [x] 6.1 Add components to player GameObject
    - Document steps to add PlayerHealth component to player GameObject in Unity
    - Document steps to add PlayerDamageHandler component to player GameObject
    - Verify player has Collider with "Is Trigger" enabled
    - Verify player has Rigidbody or colliding vehicles have Rigidbody
    - _Requirements: 1.1, 3.4_

  - [x] 6.2 Create UI Canvas and health display
    - Document steps to create Canvas (Screen Space - Overlay) if not exists
    - Document steps to create HealthPanel with UI Slider and TextMeshProUGUI
    - Document steps to add HealthUI component and assign references
    - Position health bar at top-left or top-center with proper anchoring
    - _Requirements: 6.1, 6.3, 6.4_

  - [ ]* 6.3 Write integration tests for complete damage flow
    - Test collision → damage → health update → UI update flow
    - Test game over trigger on health depletion
    - _Requirements: 4.1, 4.2, 4.3, 5.1, 5.2, 6.2_

  - [ ]* 6.4 Write property test for game over trigger
    - **Property 4: Game Over Trigger**
    - **Validates: Requirements 5.1, 5.2**

  - [ ]* 6.5 Write property test for sequential damage processing
    - **Property 5: Sequential Damage Processing**
    - **Validates: Requirements 7.4**

- [x] 7. Final checkpoint - Verify complete system
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Unity setup tasks (6.1, 6.2) require manual work in Unity Editor
- Ensure enemy vehicles have "Vehicle" tag and VehicleDamage component
- Property tests validate universal correctness properties from design document
- Integration happens incrementally - each component can be tested as it's built

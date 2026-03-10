# Health UI Setup Guide

## Overview

This guide provides step-by-step instructions for creating the Canvas and health display UI in Unity. The health UI shows the player's current health as a visual health bar with optional numeric text display.

## Prerequisites

Before starting, ensure you have:
- Unity project open with the racing game scene
- HealthUI.cs script compiled without errors
- PlayerHealth component added to the player GameObject
- TextMeshPro package imported (Unity will prompt if needed)

## UI Hierarchy Structure

The final UI hierarchy will look like this:

```
Canvas (Screen Space - Overlay)
└── HealthPanel
    ├── HealthSlider (UI Slider)
    │   ├── Background
    │   ├── Fill Area
    │   │   └── Fill
    │   └── Handle Slide Area (optional, can be deleted)
    └── HealthText (TextMeshPro - Text UI)
```

## Setup Steps

### Step 1: Create Canvas (if not already present)

1. **Check for Existing Canvas**
   - Look in the Hierarchy window for a GameObject named "Canvas"
   - If a Canvas already exists, skip to Step 2
   - If no Canvas exists, continue with the following steps

2. **Create New Canvas**
   - Right-click in the Hierarchy window
   - Select **UI > Canvas**
   - A Canvas GameObject will be created along with an EventSystem

3. **Configure Canvas Settings**
   - Select the Canvas GameObject in the Hierarchy
   - In the Inspector, locate the Canvas component
   - Set **Render Mode** to "Screen Space - Overlay"
   - This ensures the UI always renders on top of the game view

4. **Verify Canvas Scaler** (optional but recommended)
   - The Canvas should have a Canvas Scaler component
   - Set **UI Scale Mode** to "Scale With Screen Size"
   - Set **Reference Resolution** to your target resolution (e.g., 1920 x 1080)
   - This ensures UI scales properly across different screen sizes

### Step 2: Create HealthPanel Container

1. **Create Empty GameObject**
   - Right-click on the Canvas in the Hierarchy
   - Select **Create Empty**
   - Rename it to "HealthPanel"

2. **Configure HealthPanel RectTransform**
   - Select HealthPanel in the Hierarchy
   - In the Inspector, locate the Rect Transform component

3. **Set Anchor Position** (Top-Left Option)
   - Click the Anchor Presets box (square icon in Rect Transform)
   - Hold **Shift + Alt** (Windows) or **Shift + Option** (Mac)
   - Click the **top-left** preset
   - This anchors the panel to the top-left corner

   **Alternative: Top-Center Option**
   - Hold **Shift + Alt** (Windows) or **Shift + Option** (Mac)
   - Click the **top-center** preset
   - This anchors the panel to the top-center

4. **Position the Panel**
   - Set **Pos X**: 20 (for top-left) or 0 (for top-center)
   - Set **Pos Y**: -20 (positions slightly below the top edge)
   - Set **Width**: 300
   - Set **Height**: 60

### Step 3: Create Health Slider

1. **Add UI Slider**
   - Right-click on HealthPanel in the Hierarchy
   - Select **UI > Slider**
   - Rename it to "HealthSlider"

2. **Configure Slider RectTransform**
   - Select HealthSlider
   - Set **Anchors** to stretch (hold Shift + Alt and click bottom-right stretch preset)
   - Set **Left**: 10
   - Set **Right**: -10
   - Set **Top**: -10
   - Set **Bottom**: 10
   - This makes the slider fill the HealthPanel with 10-pixel padding

3. **Configure Slider Component**
   - In the Inspector, locate the Slider component
   - Set **Min Value**: 0
   - Set **Max Value**: 1
   - Set **Value**: 1 (full health at start)
   - Uncheck **Whole Numbers**
   - Set **Transition**: None (or your preferred visual feedback)

4. **Remove Handle (Optional)**
   - In the Hierarchy, expand HealthSlider
   - Find "Handle Slide Area" child object
   - Right-click and select **Delete**
   - In the Slider component, the Handle Rect field will show "None" (this is fine)
   - Health bars typically don't need draggable handles

5. **Customize Slider Appearance**

   **Background:**
   - Select "Background" under HealthSlider
   - In the Inspector, find the Image component
   - Set **Color**: Dark gray or black (e.g., RGBA: 0, 0, 0, 180)

   **Fill:**
   - Expand "Fill Area" and select "Fill"
   - In the Inspector, find the Image component
   - Set **Color**: Green for healthy (e.g., RGBA: 0, 255, 0, 255)
   - You can change this color dynamically in code based on health percentage

   **Optional - Color Gradient:**
   - To show health status with color (green > yellow > red):
   - You'll need to update the Fill color in HealthUI.cs based on health percentage
   - This is handled in code, not in the Inspector

### Step 4: Create Health Text (Optional)

This step adds numeric health display (e.g., "75/100"). Skip if you only want a visual bar.

1. **Add TextMeshPro Text**
   - Right-click on HealthPanel in the Hierarchy
   - Select **UI > Text - TextMeshPro**
   - If prompted to import TMP Essentials, click "Import TMP Essentials"
   - Rename the text object to "HealthText"

2. **Configure Text RectTransform**
   - Select HealthText
   - Set **Anchors** to stretch (hold Shift + Alt and click bottom-right stretch preset)
   - Set **Left**: 10
   - Set **Right**: -10
   - Set **Top**: -10
   - Set **Bottom**: 10

3. **Configure TextMeshPro Settings**
   - In the Inspector, locate the TextMeshProUGUI component
   - Set **Text**: "100/100" (placeholder)
   - Set **Font Size**: 24 (adjust as needed)
   - Set **Alignment**: Center (both horizontal and vertical)
   - Set **Color**: White or contrasting color
   - Enable **Auto Size** if you want text to scale automatically

4. **Add Text Outline** (optional, for better readability)
   - In TextMeshProUGUI component, expand "Extra Settings"
   - Check **Enable** under Outline
   - Set **Thickness**: 0.2
   - Set **Color**: Black

### Step 5: Add and Configure HealthUI Component

1. **Add HealthUI Script**
   - Select HealthPanel in the Hierarchy
   - In the Inspector, click "Add Component"
   - Type "HealthUI" and select it
   - The HealthUI component will be added

2. **Assign References**
   - In the HealthUI component, you'll see three fields:

   **Player Health:**
   - Click the circle icon next to "Player Health"
   - In the Select Object window, switch to the "Scene" tab
   - Find and select your player GameObject
   - The PlayerHealth component will be automatically selected

   **Health Slider:**
   - Drag the HealthSlider GameObject from the Hierarchy
   - Drop it onto the "Health Slider" field in the Inspector

   **Health Text:**
   - Drag the HealthText GameObject from the Hierarchy
   - Drop it onto the "Health Text" field in the Inspector
   - If you skipped Step 4, leave this field empty (None)

3. **Verify References**
   - Ensure all three fields show the correct references
   - Player Health should show "PlayerHealth (PlayerHealth)"
   - Health Slider should show "HealthSlider (Slider)"
   - Health Text should show "HealthText (TextMeshProUGUI)" or "None"

## Verification Checklist

Use this checklist to confirm proper setup:

- [ ] Canvas exists with Render Mode set to "Screen Space - Overlay"
- [ ] HealthPanel created as child of Canvas
- [ ] HealthPanel anchored to top-left or top-center
- [ ] HealthSlider created with proper min/max values (0 to 1)
- [ ] Slider Background has dark color
- [ ] Slider Fill has green color
- [ ] Slider Handle removed (optional)
- [ ] HealthText created with TextMeshPro (optional)
- [ ] HealthUI component added to HealthPanel
- [ ] PlayerHealth reference assigned in HealthUI
- [ ] HealthSlider reference assigned in HealthUI
- [ ] HealthText reference assigned in HealthUI (if created)

## Testing the UI

To verify the UI works correctly:

1. **Enter Play Mode**
   - Click the Play button in Unity Editor
   - The health bar should appear at the configured position

2. **Verify Initial Display**
   - Health bar should be full (green fill at 100%)
   - Health text should show maximum health (e.g., "100/100")

3. **Test Health Changes**
   - Collide with enemy vehicles to take damage
   - Watch the health bar decrease smoothly
   - Verify the numeric text updates correctly

4. **Test Health Depletion**
   - Continue taking damage until health reaches zero
   - Health bar should be empty
   - Health text should show "0/100" or similar

5. **Check UI Visibility**
   - Ensure the UI remains visible during gameplay
   - Verify it doesn't obstruct important game elements
   - Test on different screen resolutions if possible

## Customization Options

### Positioning Variations

**Top-Left (Default):**
- Anchor: Top-Left
- Pos X: 20, Pos Y: -20

**Top-Center:**
- Anchor: Top-Center
- Pos X: 0, Pos Y: -20

**Top-Right:**
- Anchor: Top-Right
- Pos X: -20, Pos Y: -20

### Color Schemes

**Health-Based Colors:**
- High Health (>70%): Green (0, 255, 0)
- Medium Health (30-70%): Yellow (255, 255, 0)
- Low Health (<30%): Red (255, 0, 0)

To implement dynamic colors, modify HealthUI.cs UpdateHealthDisplay() method.

### Size Adjustments

**Compact Bar:**
- Width: 200, Height: 40
- Font Size: 18

**Large Bar:**
- Width: 400, Height: 80
- Font Size: 32

## Troubleshooting

### UI Not Visible

**Problem**: Health bar doesn't appear in Game view

**Solutions**:
- Verify Canvas Render Mode is "Screen Space - Overlay"
- Check that HealthPanel is a child of Canvas
- Ensure Canvas is enabled in the Hierarchy
- Verify Camera is set correctly in Canvas (if using Camera render mode)

### Health Bar Not Updating

**Problem**: Health bar stays full even when taking damage

**Solutions**:
- Verify PlayerHealth reference is assigned in HealthUI component
- Check Console for null reference errors
- Ensure PlayerHealth component is on the player GameObject
- Verify HealthUI.Start() is subscribing to OnHealthChanged event

### Text Not Displaying

**Problem**: Health text shows as empty or garbled

**Solutions**:
- Ensure TextMeshPro package is imported
- Verify HealthText reference is assigned in HealthUI component
- Check that TextMeshProUGUI component exists on HealthText GameObject
- Set a default font in TextMeshPro settings if using custom fonts

### UI Scaling Issues

**Problem**: UI appears too large or small on different screens

**Solutions**:
- Add Canvas Scaler component to Canvas
- Set UI Scale Mode to "Scale With Screen Size"
- Set Reference Resolution to your target resolution
- Adjust Match slider between Width and Height as needed

## Advanced Customization

### Adding Health Bar Border

1. Right-click on HealthSlider
2. Select **UI > Image**
3. Rename to "Border"
4. Set Image Type to "Sliced" with a border sprite
5. Adjust RectTransform to frame the slider

### Adding Smooth Transitions

To make health changes animate smoothly:
- Modify HealthUI.cs to use Lerp for slider value changes
- Add a coroutine that smoothly transitions from old to new health value
- This creates a more polished visual effect

### Adding Health Icons

1. Create a UI Image next to the health bar
2. Assign a heart or health icon sprite
3. Position it to the left of the health bar
4. This provides visual context for the health display

## Component Reference

### HealthUI Component

**Purpose**: Displays player health visually and updates in real-time

**Public Fields**:
- `playerHealth` (PlayerHealth): Reference to player's health component
- `healthSlider` (Slider): UI slider for visual health bar
- `healthText` (TextMeshProUGUI): Optional text for numeric display

**Behavior**:
- Subscribes to PlayerHealth.OnHealthChanged event
- Updates slider value to match health percentage
- Updates text to show "current/max" format
- Includes null checks for missing references

## Related Documentation

- **Player Setup**: See `Assets/Scripts/Player Scripts/PLAYER_SETUP_GUIDE.md`
  - Instructions for adding PlayerHealth component

- **Requirements**: See `.kiro/specs/car-health-damage-system/requirements.md`
  - Requirement 6: Health UI Display

- **Design**: See `.kiro/specs/car-health-damage-system/design.md`
  - HealthUI Component Design section

## Notes

- The health slider uses normalized values (0 to 1) for the fill percentage
- HealthUI automatically calculates the percentage from PlayerHealth
- The text display is optional and can be omitted for a cleaner look
- UI remains visible during gameplay and updates in real-time
- The system uses Unity's event system for efficient updates (only when health changes)

## Support

If you encounter issues not covered in this guide:
1. Check the Unity Console for error messages
2. Verify all references are assigned in the Inspector
3. Review the HealthUI.cs script for additional comments
4. Ensure PlayerHealth component is properly configured
5. Test in Play Mode to see real-time behavior

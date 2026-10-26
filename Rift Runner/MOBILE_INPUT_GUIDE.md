# Mobile Input Setup - EventTrigger vs Buttons

## Overview
The `MobileInputHandler` uses **EventTrigger** instead of Button.onClick for more responsive, precise control over press/release events. This is crucial for platformer games where holding buttons matters.

---

## Why EventTrigger Over Button.onClick?

| Feature | Button.onClick | EventTrigger |
|---------|---|---|
| Detects Press (Pointer Down) | ❌ No | ✅ Yes |
| Detects Release (Pointer Up) | ❌ No | ✅ Yes |
| Suitable for Jump Hold | ❌ No | ✅ Yes |
| Simplicity | ✅ Yes | ⚠️ Moderate |
| Mobile Responsiveness | ⚠️ Good | ✅ Best |

**For platformers, EventTrigger is essential** because you need to:
1. Detect when the jump button is **pressed** (not just clicked)
2. Detect when it's **released** (for variable jump height)
3. Handle continuous button holds (movement input)

---

## Manual Setup (No Script Auto-Setup)

If MobileInputHandler's auto-setup isn't working, manually add EventTriggers:

### Step 1: Add EventTrigger Component
1. Select your button (e.g., "LeftButton")
2. In Inspector, click **Add Component**
3. Search for and add **EventTrigger**

### Step 2: Add Pointer Down Event
1. In EventTrigger component, click **Add Event Type**
2. Select **Pointer Down**
3. In the callback, drag your **MobileInputHandler** gameobject
4. In the function dropdown, select `MobileInputHandler.OnLeftPressed()`

### Step 3: Add Pointer Up Event
1. Click **Add Event Type** again
2. Select **Pointer Up**
3. In the callback, drag your **MobileInputHandler** gameobject
4. Select `MobileInputHandler.OnLeftReleased()`

### Repeat for Right and Jump Buttons

---

## EventTrigger Event Types Reference

```
Pointer Down    → Button pressed (OnPress)
Pointer Up      → Button released (OnRelease)
Pointer Enter   → Finger/mouse enters button area
Pointer Exit    → Finger/mouse leaves button area
Select          → Button selected (rarely used for mobile)
Submit          → Button submitted (rarely used for mobile)
```

For platformers, you typically only need **Pointer Down** and **Pointer Up**.

---

## Example Canvas Hierarchy

```
Canvas (Screen Space - Overlay)
│
├─ LeftButton (Button)
│  ├─ Image
│  └─ EventTrigger
│     ├─ Pointer Down → OnLeftPressed()
│     └─ Pointer Up → OnLeftReleased()
│
├─ RightButton (Button)
│  ├─ Image
│  └─ EventTrigger
│     ├─ Pointer Down → OnRightPressed()
│     └─ Pointer Up → OnRightReleased()
│
├─ JumpButton (Button)
│  ├─ Image
│  └─ EventTrigger
│     ├─ Pointer Down → OnJumpPressed()
│     └─ Pointer Up → OnJumpReleased()
│
└─ TextUI (optional)
   ├─ TimerText
   └─ CoinCounter
```

---

## Recommended Button Layout for Mobile

### Portrait Mode (1080x1920)
```
┌──────────────────────────┐
│                          │ ↑
│       GAME VIEW          │ |
│                          │ 1920px
│                          │ |
├──────────────────────────┤ ↓
│ [◀]    [Jump]  [▶]       │
│ 100px   100px  100px     │
└──────────────────────────┘
```

**Button Positioning:**
- **LeftButton**: X = 60, Y = -60 (bottom-left with margin)
- **JumpButton**: X = 0, Y = -60 (bottom-center)
- **RightButton**: X = 60, Y = -60 (bottom-right with margin)
- **Size**: 100x100 pixels each

### Script Attachment
1. Create empty GameObject: `InputManager`
2. Attach `MobileInputHandler` script
3. Assign button references in Inspector

---

## Advanced: Custom Input UI

If you want visual feedback (button pressed animations), use Button's built-in state system:

```csharp
// In MobileInputHandler
private void OnLeftPressed()
{
    leftButton.OnSelect(null); // Visual feedback
    playerController.SetMobileHorizontalInput(-1f);
}

private void OnLeftReleased()
{
    leftButton.OnDeselect(null);
    playerController.SetMobileHorizontalInput(0f);
}
```

---

## Testing in Editor

### Using Keyboard + UI Buttons
1. Run scene in Play mode
2. Use **A/D/Space** for keyboard input (PlayerController supports both)
3. **Buttons work too** for mobile simulation

### Testing Mobile on Device
1. Build to Android
2. Deploy to device
3. Test button responsiveness
4. Adjust button size/position if needed

---

## Common Issues & Fixes

### Issue: Jump button not responding
- ✅ Ensure EventTrigger component is present
- ✅ Ensure Pointer Down event is configured
- ✅ Ensure MobileInputHandler is assigned in callback
- ✅ Check that PlayerController.OnMobileJumpButtonPressed() exists

### Issue: Movement feels laggy
- ✅ Check `groundDrag` in PlayerController (8 is default)
- ✅ Ensure button presses are registered in PointerDown (not PointerUp)
- ✅ Verify `fixedDeltaTime` is 0.02 (50 FPS) in Time settings

### Issue: Button visual feedback not working
- ✅ Ensure button has Image component with sprite
- ✅ Ensure button's Navigation is set to "Automatic"
- ✅ Test with Button.OnSelect() after press

---

**This setup provides responsive, mobile-optimized input for precision platformers.**

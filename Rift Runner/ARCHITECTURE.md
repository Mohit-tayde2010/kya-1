# Rift Runner - Architecture Overview

## System Design Diagram

```
┌─────────────────────────────────────────────────────────┐
│                    GAME RUNTIME                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │  Player      │  │  Level       │  │  Game        │ │
│  │  Controller  │  │  Manager     │  │  Manager     │ │
│  ├──────────────┤  ├──────────────┤  ├──────────────┤ │
│  │ • Movement   │  │ • Timer      │  │ • Currency   │ │
│  │ • Jumping    │  │ • Stars      │  │ • Unlocks    │ │
│  │ • Input      │  │ • Coins      │  │ • Save/Load  │ │
│  │ • Physics    │  │ • Win Logic  │  │ • Progress   │ │
│  └──────────────┘  └──────────────┘  └──────────────┘ │
│       ↓                   ↓                   ↓        │
│  Rigidbody2D         Events & UI      PlayerPrefs      │
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │  Collectible │  │  Hazards     │  │  Goal Post   │ │
│  │  (Coins)     │  │  (Spikes)    │  │  (Finish)    │ │
│  ├──────────────┤  ├──────────────┤  ├──────────────┤ │
│  │ • Pickup     │  │ • Kill       │  │ • Trigger    │ │
│  │ • Sound      │  │ • Restart    │  │ • Complete   │ │
│  │ • Counter    │  │ • Reload     │  │ • Next Level │ │
│  └──────────────┘  └──────────────┘  └──────────────┘ │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │          UI & Mobile Input                       │  │
│  ├──────────────────────────────────────────────────┤  │
│  │ • Mobile Input Handler (EventTrigger buttons)    │  │
│  │ • Win Screen (Results display)                  │  │
│  │ • Timer/Coin HUD (Optional)                      │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## Data Flow

### Level Completion Flow
```
Player touches Goal → GoalPost.OnTriggerEnter()
                   ↓
         LevelManager.CompleteLevel()
                   ↓
         Calculate stars (1/2/3)
                   ↓
    GameManager.UpdateLevelProgress()
                   ↓
    • Award currency
    • Update PlayerPrefs
    • Unlock next level
                   ↓
         LevelManager.ShowWinScreen()
                   ↓
    Win Screen displays + game pauses
```

### Coin Collection Flow
```
Player touches Coin → CoinPickup.OnTriggerEnter()
                   ↓
         LevelManager.CollectCoin()
                   ↓
      CoinCollected event fired
                   ↓
     UI updates coin counter
```

### Save/Load Flow
```
Game Start → GameManager.Awake()
          ↓
   GameManager.LoadProgression()
          ↓
   Read from PlayerPrefs:
   • Total currency
   • Stars per level
   • Unlocked levels
          ↓
   Load into memory
```

---

## Input Flow (Desktop + Mobile)

### Desktop (Keyboard)
```
User presses A/D/Space
         ↓
PlayerController.Update() reads Input.GetAxis()
         ↓
PlayerController.ApplyHorizontalMovement()
PlayerController.HandleJumping()
         ↓
Rigidbody2D velocity updated
```

### Mobile (UI Buttons)
```
User taps Left/Right/Jump button
         ↓
EventTrigger fires Pointer Down/Up
         ↓
MobileInputHandler.OnLeftPressed() etc.
         ↓
PlayerController.SetMobileHorizontalInput()
PlayerController.OnMobileJumpButtonPressed()
         ↓
PlayerController applies input in FixedUpdate()
         ↓
Rigidbody2D velocity updated
```

---

## Physics System

### Ground Detection (Gizmo visualization)
```
Player Sprite (center)
         ↓
  groundCheckOffset
         ↓
  Physics2D.OverlapCircle(groundCheckRadius)
         ↓
  Hit Ground Layer? → isGrounded = true/false
```

### Movement System
```
Horizontal Input (-1, 0, +1)
         ↓
rb.velocity.x = input * moveSpeed
         ↓
rb.drag = (isGrounded) ? groundDrag : airDrag
         ↓
Smooth deceleration when input = 0
```

### Jump System
```
Jump Input (Space or Button)
         ↓
timeSinceJumpPressed <= jumpBufferTime?
         ↓
timeSinceGrounded <= coyoteTime?
         ↓
YES: Execute jump
rb.velocity.y = jumpForce
isJumping = true
         ↓
Hold Space: Continue upward
         ↓
Release Space: Cut off (multiply by jumpCutOffMultiplier)
         ↓
Land: isJumping = false, reset timers
```

---

## Event System

### LevelManager Events
```
LevelManager.CoinCollected(int totalCoins)
  → Subscribe in UI to update coin counter

LevelManager.LevelComplete(int stars, float time, int coins)
  → Subscribe in UI to show results
  → Subscribe in GameManager to save progress
```

### Scriptable Architecture (Optional Future)
```
Currently: Direct FindObjectOfType<>() calls

Future (if needed):
├─ Create ScriptableObject events
├─ Decouple systems with observer pattern
└─ More reusable, testable architecture
```

---

## Singleton Pattern (GameManager)

```csharp
// Single instance per game session
public static GameManager Instance { get; private set; }

// Usage anywhere:
GameManager.Instance.GetTotalCurrency();
GameManager.Instance.UpdateLevelProgress(1, 3, 10);
```

**Advantages:**
- Persistent across scene loads
- Global access without FindObjectOfType()
- Guaranteed single instance

**Disadvantages:**
- Can hide dependencies
- Hard to test in isolation

---

## Save System (PlayerPrefs)

### Structure
```
RiftRunner_Currency                 → int (total currency)
RiftRunner_Stars_Level_1            → int (0-3 stars)
RiftRunner_Stars_Level_2            → int (0-3 stars)
...
RiftRunner_Unlocked_Level_2         → int (0/1 boolean)
RiftRunner_Unlocked_Level_3         → int (0/1 boolean)
...
```

### Lifecycle
```
Game Start
    ↓
GameManager.LoadProgression() reads PlayerPrefs
    ↓
User plays level
    ↓
GameManager.UpdateLevelProgress() writes PlayerPrefs
    ↓
PlayerPrefs.Save() flushes to disk
```

---

## Mobile Optimization

### Performance Considerations
- Physics2D overhead (especially ground check)
- No complex state machines in hot loop
- Minimal allocations in FixedUpdate
- Object pooling for coins/hazards (if >20 per level)

### Mobile Input Considerations
- Touch is imprecise → buttons must be large (>80px)
- Coyote time helps forgive timing errors
- Jump buffering helps button presses feel responsive
- EventTrigger provides more responsive input than Button.onClick

---

## Extension Points (For Future Features)

### Add Wall Sliding
```
1. Add wall detection in PlayerController.Update()
2. Modify jump logic to allow wall jumps
3. Add wall slide gravity reduction
```

### Add Double Jump
```
1. Track jumpsRemaining (0-2)
2. Reset on landing
3. Increment in jump logic
```

### Add Dashing
```
1. New input state for dash
2. High velocity burst in direction
3. Brief cooldown to prevent spam
```

### Add Moving Platforms
```
1. Platform script with moving Rigidbody2D
2. Parent player to platform on contact
3. Unparent on jump/leaving
```

### Add Character Variants
```
1. Create character ScriptableObject (speed, jump, weight)
2. Load character at level start
3. Apply to PlayerController values
4. Visual model swap in character shop
```

---

## Development Workflow

### Level Design Iteration
```
1. Create level geometry
2. Place coins and hazards
3. Set targetTime and coinCount in LevelManager
4. Playtest with keyboard
5. Adjust difficulty/rewards
6. Repeat
```

### Physics Tuning
```
1. Play level with default settings
2. Adjust one variable at a time
3. Observe impact on feel
4. Document values in comments
5. Create preset profiles (Easy/Medium/Hard)
```

### Mobile Testing
```
1. Build to Android device
2. Test all buttons
3. Check framerate (target 60 FPS)
4. Verify touch responsiveness
5. Monitor battery/heat
6. Iterate on button layout
```

---

**This architecture is designed for clarity, performance, and ease of extension.**

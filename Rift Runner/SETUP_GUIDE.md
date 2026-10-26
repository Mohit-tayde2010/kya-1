# Rift Runner - MVP Core Architecture Guide

## Project Overview
This is the **core gameplay architecture** for Rift Runner, a precision parkour platformer. All scripts are production-ready, modular, and optimized for mobile 2D physics.

---

## Script Summary

### 1. **PlayerController.cs** (Player > Physics & Input)
The foundation of tight, responsive movement.

**Key Features:**
- Physics-based 2D movement with Rigidbody2D
- **Coyote Time**: Jump for 0.1s after leaving ground (prevents frustration)
- **Jump Buffering**: Press jump 0.1s before landing, executes on landing (rewards prediction)
- **Variable Jump Height**: Hold for higher jump, tap for short hop
- **Mobile + Keyboard Support**: Keyboard (A/D/Space) for testing, UI buttons for mobile

**Tunable Variables (in Inspector):**
- `moveSpeed` (default: 8) - Horizontal movement speed
- `jumpForce` (default: 12) - Initial upward velocity
- `gravityScale` (default: 2.5) - Gravity multiplier
- `coyoteTime` (default: 0.1) - Window to jump after leaving ground
- `jumpBufferTime` (default: 0.1) - Buffer before landing to register jump
- `groundCheckRadius` (default: 0.2) - Size of ground detection circle
- `maxFallSpeed` (default: 20) - Terminal velocity cap

**Usage:**
1. Add to your player GameObject
2. Assign Rigidbody2D (gravity enabled, body type: dynamic)
3. Set `groundLayer` to your platform layer mask
4. Adjust variables in Inspector until movement feels tight

---

### 2. **LevelManager.cs** (Level > Core Logic)
Manages timer, collectibles, stars, and win conditions per level.

**Key Features:**
- Tracks elapsed time and coins collected
- Calculates 3-star performance:
  - **Star 1**: Level completed
  - **Star 2**: Time < target time
  - **Star 3**: All coins collected
- Communicates with GameManager for persistent progression
- Event system for UI updates

**Tunable Variables:**
- `levelID` - Unique level identifier
- `targetTime` - Time limit for 2-star bonus
- `totalCoinsInLevel` - Expected coin count for 3-star

**Setup:**
1. Add to level root GameObject
2. Assign the Win Screen prefab
3. Set target time and coin counts

**Events (subscribe in UI):**
```csharp
LevelManager.CoinCollected += (totalCoins) => { /* Update UI */ };
LevelManager.LevelComplete += (stars, time, coins) => { /* Show result */ };
```

---

### 3. **GameManager.cs** (Global > Persistence)
Singleton that handles progression, currency, and save/load.

**Key Features:**
- Persistent currency tracking (survives scene loads)
- PlayerPrefs saving (unlocks, stars per level)
- Currency rewards: `10 per star + 5 per coin`
- Level unlock progression (completing level unlocks next)

**Singleton Usage:**
```csharp
GameManager.Instance.GetTotalCurrency();
GameManager.Instance.UpdateLevelProgress(levelID, starsEarned, coinsCollected);
GameManager.Instance.IsLevelUnlocked(levelID);
GameManager.Instance.TrySpendCurrency(50); // For shop
```

**Save Data Structure (PlayerPrefs):**
- `RiftRunner_Currency` - Total currency earned
- `RiftRunner_Stars_Level_X` - Stars earned on level X
- `RiftRunner_Unlocked_Level_X` - Whether level X is unlocked

---

### 4. **CoinPickup.cs** (Collectible > Pickup Logic)
Simple coin collection with audio and destruction.

**Setup:**
1. Create coin sprite GameObject
2. Add BoxCollider2D (is trigger)
3. Tag with "Player" (player detection)
4. Attach CoinPickup script
5. (Optional) Assign audio clip

---

### 5. **GoalPost.cs** (Goal Trigger)
Completes the level when player touches it.

**Setup:**
1. Create finish line GameObject
2. Add BoxCollider2D (is trigger)
3. Attach GoalPost script

---

### 6. **Hazard.cs** (Obstacles > Generic Hazard)
Kills player on contact and restarts level instantly.

**Setup:**
1. Create spike/lava GameObject
2. Add Collider2D (BoxCollider2D or CircleCollider2D, is trigger)
3. Attach Hazard script
4. Tag with "Player"

Handles both trigger and collision for flexibility.

---

### 7. **WinScreen.cs** (UI > Results Screen)
Displays stars earned, time, and coins collected at level end.

**Setup:**
1. Create Canvas
2. Create prefab with:
   - Text for stars/time/coins display
   - 3 Image components for star visuals
   - Button for "Next Level"
   - Button for "Restart Level"
3. Attach WinScreen script
4. Assign all references in Inspector

**Features:**
- Auto-pauses game (Time.timeScale = 0)
- Visual star display (yellow = earned, gray = missed)
- Responsive button handling

---

### 8. **MobileInputHandler.cs** (UI > Mobile Controls)
Converts on-screen button presses into player input.

**Setup:**
1. Create Canvas with three buttons: Left, Right, Jump
2. Create empty GameObject in scene
3. Attach MobileInputHandler script
4. Assign buttons to Left, Right, Jump fields

**How It Works:**
- Uses EventTrigger (PointerDown/PointerUp)
- Calls PlayerController methods on button events
- Left/Right set horizontal input (-1, 0, +1)
- Jump calls OnMobileJumpButtonPressed/Released

---

## Scene Setup Checklist

### Layer Setup
1. Create layers:
   - `Ground` - Platforms, terrain
   - `Player` - Player GameObject
   - `Hazard` - Spikes, obstacles
   - `Collectible` - Coins

### GameObject Tagging
```
Player GameObject    → Tag: "Player"
Coin GameObjects     → Tag: "Player" (for detection)
Hazard GameObjects   → Tag: "Player" (for detection)
```

### Player Setup
```
Player GameObject:
├─ Sprite Renderer (sprite assigned)
├─ Rigidbody2D
│  ├─ Body Type: Dynamic
│  ├─ Gravity Scale: (leave as 1, PlayerController overrides)
│  ├─ Constraints: Rotation Z = Freeze
│  └─ Collision: enabled
├─ BoxCollider2D (NOT trigger)
│  └─ Size: (adjust to sprite)
└─ PlayerController script
   ├─ moveSpeed: 8
   ├─ jumpForce: 12
   ├─ gravityScale: 2.5
   └─ groundLayer: Ground layer
```

### Level Structure
```
Level Scene:
├─ Ground (Sprite + BoxCollider2D, layer: Ground)
├─ Platforms (Sprite + BoxCollider2D each, layer: Ground)
├─ Player (Rigidbody2D + PlayerController)
├─ Coins (SpriteRenderer + CircleCollider2D trigger, CoinPickup script)
├─ Hazards (Sprite + Collider2D trigger, Hazard script)
├─ Goal (Sprite + BoxCollider2D trigger, GoalPost script)
├─ LevelManager (empty GO with LevelManager script + Canvas reference)
└─ UI Canvas
   ├─ Win Screen Prefab (disabled by default)
   ├─ Mobile Buttons (Left, Right, Jump)
   └─ Timer/Coin Display (optional, subscribe to LevelManager events)
```

---

## Mobile Input Setup (Detailed)

### Canvas Structure
```
Canvas (Screen Space - Overlay)
├─ LeftButton (Button component)
├─ RightButton (Button component)
├─ JumpButton (Button component)
├─ Timer Text (Text component)
└─ Coin Counter (Text component)
```

### Button Configuration
1. **LeftButton**
   - Image: Left arrow icon (or "◀")
   - Position: Bottom-left of screen
   - Size: ~100x100 px

2. **RightButton**
   - Image: Right arrow icon (or "▶")
   - Position: Bottom-center-right
   - Size: ~100x100 px

3. **JumpButton**
   - Image: Up arrow or "JUMP" text
   - Position: Bottom-right
   - Size: ~100x100 px

### Input Hierarchy
```
MobileInputHandler (script) 
  ↓ finds PlayerController
  ↓ calls SetMobileHorizontalInput(±1)
  ↓ PlayerController applies to Rigidbody2D velocity
```

---

## Physics Tuning Guide

### For Tight, Responsive Feel
- **Higher `moveSpeed`** (10-12): Feels snappier
- **Higher `jumpForce`** (14-16): Air control feels better
- **Lower `jumpCutOffMultiplier`** (0.3-0.4): Shorter hops feel punchier
- **Lower `coyoteTime`** (0.08): Encourages precise timing

### For Forgiving, Casual Feel
- **Lower `moveSpeed`** (6-7): More time to react
- **Lower `jumpForce`** (10): Slower, easier to control
- **Higher `jumpCutOffMultiplier`** (0.7): Easier short hops
- **Higher `coyoteTime`** (0.15): More forgiveness

### For Super Meat Boy / Celeste Style
- `moveSpeed`: 10
- `jumpForce`: 16
- `gravityScale`: 2.8
- `jumpCutOffMultiplier`: 0.4
- `coyoteTime`: 0.08
- `jumpBufferTime`: 0.08

---

## Debugging Tips

### Ground Detection Issues
- Use Scene view Gizmo (green circle shows ground check radius)
- Ensure player BoxCollider2D is NOT set as trigger
- Ensure ground platforms have colliders (NOT triggers)
- Ensure `groundLayer` is correctly set in PlayerController

### Coins Not Collecting
- Verify coin BoxCollider2D is set as **trigger**
- Ensure player is tagged "Player"
- Check LevelManager is in scene and coin can find it

### Hazard Not Killing Player
- Verify hazard collider is **trigger**
- Ensure player is tagged "Player"
- Check that PlayerController script is enabled

### Win Screen Not Showing
- Verify `winScreenPrefab` is assigned to LevelManager
- Ensure WinScreen script is on prefab
- Check Canvas reference is set

---

## Next Steps for Full Game

1. **Polish Movement** - Playtesting, tweaking physics
2. **Level Design** - Create diverse, challenging layouts
3. **Character System** - Model variants (visual/gameplay traits)
4. **Shop UI** - Currency spending for characters
5. **Progression UI** - Level select with stars display
6. **Sound & Juice** - Audio, particles, screen shake
7. **Android Build** - Configure for Play Store

---

## FAQ

**Q: Can I use this with 3D?**
A: No, all scripts use Rigidbody2D and Collider2D (2D physics). For 3D, rewrite for Rigidbody/Collider.

**Q: How do I add a double jump?**
A: Track `jumpsRemaining` and reset on landing. Modify `HandleJumping()` to allow 2 jumps.

**Q: Can I add wall slides?**
A: Yes, add wall detection in `Update()` and allow jumping from walls. Modify coyote time logic.

**Q: How do I save best times per level?**
A: Extend `GameManager` to track `Best_Time_Level_X` in PlayerPrefs.

---

**Created for Rift Runner MVP - All scripts are production-ready and tested.**

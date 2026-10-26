# Rift Runner - Quick Start Checklist

## Pre-Scene Setup (One-Time)

- [ ] Create Unity 2D project (2021.3 LTS or newer)
- [ ] Create layers: `Ground`, `Player`, `Hazard`, `Collectible`
- [ ] Import all C# scripts from `/Assets/Scripts/`
- [ ] Create GameManager prefab and assign to `DontDestroyOnLoad`

---

## Per-Level Scene Setup

### Player GameObject
- [ ] Create sprite, add SpriteRenderer
- [ ] Add Rigidbody2D (Dynamic, Gravity ON, Z rotation freeze)
- [ ] Add BoxCollider2D (NOT trigger, size matches sprite)
- [ ] Assign PlayerController script
- [ ] Set `groundLayer` to "Ground" layer mask
- [ ] Tag as "Player"

### Platforms/Terrain
- [ ] Create sprite for each platform
- [ ] Add BoxCollider2D (NOT trigger)
- [ ] Assign to "Ground" layer
- [ ] Position in level

### Coins (1-10 per level)
- [ ] Create small sprite
- [ ] Add CircleCollider2D (IS trigger)
- [ ] Attach CoinPickup script
- [ ] Tag as "Player"
- [ ] Duplicate for each coin placement

### Goal Post
- [ ] Create sprite (flag or pole)
- [ ] Add BoxCollider2D (IS trigger)
- [ ] Attach GoalPost script
- [ ] Place at end of level

### Hazards (Spikes, Obstacles)
- [ ] Create sprite
- [ ] Add BoxCollider2D (IS trigger)
- [ ] Attach Hazard script
- [ ] Tag as "Player"
- [ ] Duplicate for each hazard

### Level Manager
- [ ] Create empty GameObject named "LevelManager"
- [ ] Attach LevelManager script
- [ ] Set `levelID` (1, 2, 3, etc.)
- [ ] Set `targetTime` (e.g., 60 seconds)
- [ ] Count coins in level, set `totalCoinsInLevel`
- [ ] Create Canvas and assign to `uiCanvas`
- [ ] Create WinScreen prefab, assign to `winScreenPrefab`

### UI Canvas
- [ ] Create Canvas (Screen Space - Overlay)
- [ ] Add LeftButton, RightButton, JumpButton
- [ ] Add EventTrigger to each button
- [ ] Configure Pointer Down/Up callbacks
- [ ] Create empty GameObject, attach MobileInputHandler
- [ ] Assign button references to MobileInputHandler

### Optional UI Display
- [ ] Add Text for timer display
- [ ] Add Text for coin counter
- [ ] Subscribe to `LevelManager.CoinCollected` event
- [ ] Update UI on event trigger

---

## Build Settings
- [ ] Set active scene to first level
- [ ] Scenes: Level1, Level2, Level3, etc.
- [ ] Platform: Android
- [ ] Target API: 30+
- [ ] Orientation: Portrait

---

## Testing Checklist

### Keyboard Controls (Editor)
- [ ] A/D moves left/right
- [ ] Space jumps
- [ ] Jump feel is tight and responsive
- [ ] Coyote time works (jump after walking off ledge)
- [ ] Jump buffering works (press before landing, jumps on landing)

### Mobile Buttons (Editor + Device)
- [ ] Left button moves player left
- [ ] Right button moves player right
- [ ] Jump button makes player jump
- [ ] Hold jump = higher jump
- [ ] Tap jump = short hop
- [ ] Buttons feel responsive on device

### Gameplay
- [ ] Coins collect on touch
- [ ] Coin count updates in UI
- [ ] Hazards kill player instantly
- [ ] Goal post triggers level complete
- [ ] Win screen displays correctly
- [ ] Timer shows elapsed time
- [ ] Stars calculate correctly (1/2/3)

### Progression
- [ ] Currency awarded on level complete
- [ ] PlayerPrefs saves data
- [ ] Closing/reopening game loads progression
- [ ] Next level is unlocked on completion
- [ ] GameManager persists across scenes

---

## Physics Tuning
- [ ] Test with default values first
- [ ] Adjust `moveSpeed` if movement feels too fast/slow
- [ ] Adjust `jumpForce` if jumps feel too high/low
- [ ] Adjust `coyoteTime` to control difficulty
- [ ] Adjust `gravityScale` for game feel

---

## Performance Optimization
- [ ] Profile in Editor (Window > Analysis > Profiler)
- [ ] Check frame rate (target 60 FPS on mobile)
- [ ] Monitor memory usage
- [ ] Disable physics on off-screen objects (if needed)
- [ ] Use object pooling for coins/hazards (if >20 per level)

---

## Deployment to Android
- [ ] Install Android NDK/SDK
- [ ] Configure keystore for signing
- [ ] Build APK or AAB
- [ ] Test on real device
- [ ] Verify touch input, performance, battery usage

---

**After completing this checklist, your MVP is ready for level iteration and polish!**

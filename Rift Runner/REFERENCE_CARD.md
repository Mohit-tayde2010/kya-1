# Rift Runner - Developer Reference Card

## 🎮 Physics Presets (Copy & Paste Values)

### Celeste-Style (Default)
```
moveSpeed: 8
jumpForce: 12
gravityScale: 2.5
coyoteTime: 0.1
jumpBufferTime: 0.1
jumpCutOffMultiplier: 0.5
```

### Super Meat Boy (Tight & Snappy)
```
moveSpeed: 10
jumpForce: 16
gravityScale: 2.8
coyoteTime: 0.08
jumpBufferTime: 0.08
jumpCutOffMultiplier: 0.35
```

### Casual (Forgiving)
```
moveSpeed: 6
jumpForce: 10
gravityScale: 2.0
coyoteTime: 0.15
jumpBufferTime: 0.15
jumpCutOffMultiplier: 0.7
```

---

## 🏗️ Scene Setup (Copy This Structure)

```
Level1 Scene
├─ Ground (Sprite + BoxCollider2D, "Ground" layer)
├─ Platforms (Sprites + BoxCollider2D each, "Ground" layer)
├─ Player
│  ├─ Sprite
│  ├─ Rigidbody2D (Body Type: Dynamic, Gravity: ON, Z: Freeze)
│  ├─ BoxCollider2D (NOT trigger)
│  ├─ PlayerController (script)
│  └─ Tag: "Player"
├─ Coins (Sprites + CircleCollider2D trigger, CoinPickup script, Tag: "Player")
├─ Hazards (Sprites + BoxCollider2D trigger, Hazard script, Tag: "Player")
├─ GoalPost (Sprite + BoxCollider2D trigger, GoalPost script)
├─ LevelManager (empty GO with LevelManager script)
└─ Canvas
   ├─ LeftButton (Button + EventTrigger)
   ├─ RightButton (Button + EventTrigger)
   └─ JumpButton (Button + EventTrigger)
```

---

## 📱 EventTrigger Setup (Quick Reference)

1. Select button → Add Component → EventTrigger
2. Add Event Type → Pointer Down
   - Callback: MobileInputHandler → OnLeftPressed()
3. Add Event Type → Pointer Up
   - Callback: MobileInputHandler → OnLeftReleased()
4. Repeat for Right and Jump buttons

---

## 💾 PlayerPrefs Keys

```
RiftRunner_Currency              // Total earned currency (int)
RiftRunner_Stars_Level_1         // Stars for level 1 (0-3)
RiftRunner_Stars_Level_2         // Stars for level 2 (0-3)
RiftRunner_Unlocked_Level_2      // Is level 2 unlocked (0/1)
RiftRunner_Unlocked_Level_3      // Is level 3 unlocked (0/1)
```

---

## ⭐ 3-Star System

| Star | Earns | Condition |
|------|-------|-----------|
| ⭐ | 10 currency | Reach goal |
| ⭐⭐ | 10 currency | Time < targetTime |
| ⭐⭐⭐ | 10 currency | Collected all coins |
| Coins | 5 per coin | Each coin collected |

**Total Max:** 30 currency + (coinsInLevel × 5)

---

## 🔧 Variable Reference

### PlayerController Tuning
```
moveSpeed              - Horizontal speed (pixels/sec)
jumpForce              - Initial upward velocity
gravityScale           - Gravity multiplier
coyoteTime             - Jump window after leaving ground
jumpBufferTime         - Jump buffer before landing
groundCheckRadius      - Size of ground detection circle
maxFallSpeed           - Terminal velocity cap
```

### LevelManager Setup
```
levelID                - Unique identifier (1, 2, 3...)
targetTime             - Time limit for 2-star
totalCoinsInLevel      - Expected coins for 3-star
```

### GameManager Constants
```
currencyPerStar = 10   - Hardcoded in script
currencyPerCoin = 5    - Hardcoded in script
```

---

## 🎬 Method Quick Reference

### PlayerController
```
SetMobileHorizontalInput(float value)     // Called by mobile buttons
OnMobileJumpButtonPressed()                // Button down event
OnMobileJumpButtonReleased()               // Button up event
IsGrounded() → bool                        // Ground detection
```

### LevelManager
```
CollectCoin()                              // Register pickup
CompleteLevel()                            // End level logic
GetElapsedTime() → float                   // Current time
GetCoinsCollected() → int                  // Current coins
```

### GameManager (Singleton)
```
GameManager.Instance.UpdateLevelProgress() // Save progress
GameManager.Instance.GetTotalCurrency()    // Query currency
GameManager.Instance.IsLevelUnlocked()     // Check unlock
GameManager.Instance.TrySpendCurrency()    // Deduct currency
```

---

## 🐛 Debugging Checklist

### Movement Issues
- [ ] PlayerController script attached? (enabled?)
- [ ] Rigidbody2D present? (Dynamic, not Kinematic)
- [ ] Rigidbody2D not on UI? (Physics2D doesn't work on canvas)
- [ ] Ground layer correctly set? (LayerMask in inspector)
- [ ] Player tag correct? (exactly "Player")

### Coin Not Collecting
- [ ] CoinPickup script attached?
- [ ] Collider is trigger?
- [ ] Player tag = "Player"?
- [ ] LevelManager in scene?

### Hazard Not Killing
- [ ] Hazard script attached?
- [ ] Collider is trigger?
- [ ] Player tag = "Player"?
- [ ] PlayerController has [Serializable] for enabled/disabled?

### Win Screen Not Showing
- [ ] WinScreen prefab assigned to LevelManager?
- [ ] Canvas assigned to LevelManager?
- [ ] WinScreen script on prefab?
- [ ] All text/image references filled?

### Mobile Buttons Not Working
- [ ] EventTrigger on button? (not canvas)
- [ ] Pointer Down event configured?
- [ ] Pointer Up event configured?
- [ ] MobileInputHandler in scene?
- [ ] Button references assigned?

### Progression Not Saving
- [ ] GameManager persistent (DontDestroyOnLoad)?
- [ ] GameManager Singleton pattern correct?
- [ ] PlayerPrefs.Save() called?
- [ ] PlayerPrefs keys match (exactly)?

---

## 📊 Performance Targets

```
Frame Rate:              60 FPS (mobile)
Physics Queries/Frame:   ~5-10 (reasonable)
Memory Usage:            <100 MB (typical)
Battery Drain:           Minimal (no heavy physics)
GC Allocations:          <1 MB per frame
```

**Monitor with:** Window > Analysis > Profiler

---

## 🚀 Deployment Checklist

### Before Building APK
- [ ] All scenes added to Build Settings
- [ ] Platform set to Android
- [ ] Target API 30+
- [ ] Orientation: Portrait
- [ ] 60 FPS locked (Unity Settings)

### Before Play Store Submission
- [ ] Icon added (512x512)
- [ ] Screenshots taken (1080x1920)
- [ ] Tested on multiple devices
- [ ] No crashes/crashes after 5 min
- [ ] Touch controls responsive
- [ ] Save/load works reliably

---

## 🎯 Level Design Quick Tips

### Difficulty Progression
- Level 1-3: Introduction (simple platforms, no hazards)
- Level 4-7: Precision (tight jumps, spike placement)
- Level 8-10+: Mastery (complex routing, optimization)

### Coin Placement
- Place 1 coin on easy path (guaranteed)
- Place 2 coins on normal path (60-70% of players)
- Place 1 coin on hard path (30% of players)

### Time Targets
- 1-star: Finish level (no time limit)
- 2-star: Comfortable completion (no rushes)
- 3-star: Optimize routing (10-30% faster than casual)

### Hazard Placement
- First level: 0 hazards (learning movement)
- Tutorial: Introduce 1-2 hazards clearly
- Main levels: Use spikes strategically (not random)

---

## 💡 Common Tweaks

### "Jump feels floaty"
↓ jumpForce (11, 10, 9...)
OR ↑ gravityScale (2.8, 3.0, 3.2...)

### "Can't reach platforms"
↑ jumpForce (13, 14, 15...)

### "Feel like I'm slipping"
↑ groundDrag (default: 8)
OR ↑ moveSpeed slightly

### "Too hard to control"
↑ coyoteTime (0.12, 0.15...)
↑ jumpBufferTime (0.12, 0.15...)

### "Button presses feel laggy"
Ensure EventTrigger Pointer Down (not Up)
Check Time.timeScale (must = 1 in gameplay)
Profile with Profiler (check GC spikes)

---

## 🔗 File Quick Links

| Need | File | Section |
|------|------|---------|
| Setup level | QUICK_START.md | Per-Level Scene Setup |
| Physics tuning | SETUP_GUIDE.md | Physics Tuning Guide |
| Mobile input | MOBILE_INPUT_GUIDE.md | Manual Setup |
| System design | ARCHITECTURE.md | System Design Diagram |
| Code examples | PATTERNS.md | Any pattern |
| All files | FILE_SUMMARY.md | File Descriptions |

---

## 📞 Emergency Contacts (Debugging)

**Game won't start?**
→ Check Console for errors
→ Verify all script references assigned
→ Look for null reference exceptions

**Physics behaving weird?**
→ Check Rigidbody2D settings
→ Verify groundLayer LayerMask
→ Look at ground detection Gizmo (Scene view)

**Performance is bad?**
→ Open Profiler (Window > Analysis)
→ Look for GC.Alloc spikes
→ Check Physics2D queries per frame

**Can't build for Android?**
→ Install Android NDK/SDK
→ Set correct Target API
→ Check Java is installed
→ Verify keystore path

---

**Print this card and keep it near your desk. Update values as you iterate!**

*Last Updated: February 2026 | Rift Runner MVP*

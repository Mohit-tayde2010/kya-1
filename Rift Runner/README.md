# Rift Runner - MVP Core Architecture

**A production-ready, precision parkour platformer framework for Android.**

---

## 📦 What You Get

### Core Scripts (8 files)
1. **PlayerController.cs** - Physics-based movement with coyote time, jump buffering, variable jump height
2. **LevelManager.cs** - Timer, coin tracking, 3-star performance system
3. **GameManager.cs** - Global progression, persistent save/load, currency management
4. **CoinPickup.cs** - Collectible logic with audio and counters
5. **GoalPost.cs** - Level completion trigger
6. **Hazard.cs** - Generic obstacle that kills player and restarts level
7. **WinScreen.cs** - Results display with star visuals and navigation
8. **MobileInputHandler.cs** - Touch controls using EventTrigger (press/release detection)

### Documentation (5 guides)
- **SETUP_GUIDE.md** - Complete scene setup instructions (layer, tag, GameObject hierarchy)
- **QUICK_START.md** - 60-second checklist to get your first level running
- **MOBILE_INPUT_GUIDE.md** - EventTrigger vs Button.onClick, button layout, advanced input
- **ARCHITECTURE.md** - System design, data flows, extension points
- **PATTERNS.md** - 10 common implementation patterns (UI, audio, leaderboards, etc.)

---

## 🎮 Key Features

### Movement System (Tight & Responsive)
- ✅ Physics-based with Rigidbody2D
- ✅ Coyote time (jump for 0.1s after leaving ground)
- ✅ Jump buffering (press 0.1s before landing = instant jump)
- ✅ Variable jump height (hold vs tap)
- ✅ Smooth acceleration/deceleration
- ✅ Mobile + Keyboard support (simultaneous)

### Level Progression
- ✅ 3-star system: completion, time, collection
- ✅ Dynamic currency rewards (10 per star, 5 per coin)
- ✅ Auto-unlock next level on completion
- ✅ PlayerPrefs save/load (persistent across sessions)

### Mobile Optimized
- ✅ On-screen buttons (Left, Right, Jump)
- ✅ EventTrigger for responsive press/release detection
- ✅ Handles touch input without lag
- ✅ Adjustable button layout and positioning

### Game Feel
- ✅ Fast restart on death (0.2s delay)
- ✅ Pause on level complete (for results screen)
- ✅ Audio support for jumps, coins, collisions
- ✅ Particle effect hooks (ready for implementation)

---

## 🚀 Getting Started

### 1. Quick Setup (5 minutes)
```
1. Create new 2D project (Unity 2021.3+)
2. Copy /Assets/Scripts/ folder into project
3. Create layers: Ground, Player, Hazard, Collectible
4. Follow QUICK_START.md checklist
5. Hit Play!
```

### 2. Build Your First Level
```
1. Create ground sprite + BoxCollider2D
2. Add player with PlayerController
3. Add 5 coins (CoinPickup script)
4. Add hazards (Hazard script)
5. Add goal post (GoalPost script)
6. Create LevelManager + Canvas UI
7. Adjust moveSpeed/jumpForce to feel "right"
```

### 3. Test on Mobile
```
1. Set Platform to Android
2. Build APK/AAB
3. Deploy to device
4. Test button responsiveness + performance
5. Iterate on button size/layout
```

---

## 📐 Physics Tuning Guide

### Default (Balanced, like Celeste-lite)
```
moveSpeed: 8
jumpForce: 12
gravityScale: 2.5
coyoteTime: 0.1s
jumpBufferTime: 0.1s
```

### Snappy (Super Meat Boy)
```
moveSpeed: 10
jumpForce: 16
gravityScale: 2.8
coyoteTime: 0.08s
jumpCutOffMultiplier: 0.35
```

### Forgiving (Casual)
```
moveSpeed: 6
jumpForce: 10
gravityScale: 2.0
coyoteTime: 0.15s
jumpCutOffMultiplier: 0.75
```

**All values are tunable in the Inspector with [SerializeField].**

---

## 📱 Mobile Input Setup

### Canvas Structure
```
Canvas (Screen Space - Overlay)
├─ LeftButton (Button + EventTrigger)
├─ RightButton (Button + EventTrigger)
├─ JumpButton (Button + EventTrigger)
└─ UI Displays (Timer, Coins)
```

### Button Events
- **Pointer Down** → Movement/Jump starts
- **Pointer Up** → Movement/Jump ends
- This allows holding buttons for continuous movement

See **MOBILE_INPUT_GUIDE.md** for detailed EventTrigger setup.

---

## 🏗️ Scene Structure

```
Level Scene
├─ Ground (sprite + non-trigger collider, "Ground" layer)
├─ Platforms (multiple, "Ground" layer)
├─ Player
│  ├─ Sprite
│  ├─ Rigidbody2D (dynamic, not kinematic)
│  ├─ BoxCollider2D (non-trigger)
│  ├─ PlayerController
│  └─ Tag: "Player"
├─ Coins (sprites + trigger colliders, CoinPickup)
├─ Hazards (sprites + trigger colliders, Hazard)
├─ GoalPost (sprite + trigger collider)
├─ LevelManager (empty GO with script)
└─ Canvas (UI buttons + displays)
```

---

## 💾 Save System (PlayerPrefs)

### What Gets Saved
```
RiftRunner_Currency              → Total currency earned
RiftRunner_Stars_Level_X         → Stars (0-3) for level X
RiftRunner_Unlocked_Level_X      → Is level X unlocked (0/1)
```

### How It Works
```
Game Start → Load from PlayerPrefs
Level Complete → Calculate stars
Save via PlayerPrefs
Next time game starts → Progress persists
```

---

## 🎯 3-Star System

| Star | Condition |
|------|-----------|
| ⭐ | Level completed (reach goal) |
| ⭐⭐ | Finished within target time |
| ⭐⭐⭐ | Collected all coins |

**Currency Reward:** 10 per star + 5 per coin collected

---

## 🔌 Extension Points

### Add These Features
1. **Double Jump** - Track jumpsRemaining counter
2. **Wall Slide** - Add wall detection, modify jump logic
3. **Dash** - High velocity burst with cooldown
4. **Moving Platforms** - Parent player on contact
5. **Character Variants** - Load traits from ScriptableObject
6. **Screen Shake** - Particle/juice feedback on jump
7. **Leaderboard** - Local high score tracking (see PATTERNS.md)
8. **Difficulty Modes** - Preset physics multipliers

See **ARCHITECTURE.md** for data flow diagrams.

---

## 📊 Performance

### Target: 60 FPS on Mobile
- Physics checks optimized (OverlapCircle only on ground)
- No allocations in hot loops (FixedUpdate)
- Minimal FindObjectOfType (cached references preferred)
- Object pooling for coins/hazards if >20 per level

**Monitor with Profiler:** Window > Analysis > Profiler

---

## 🧪 Testing Checklist

### Before Deployment
- [ ] Keyboard controls (A/D/Space) work smoothly
- [ ] Mobile buttons respond without lag
- [ ] Coyote time feels forgiving but not broken
- [ ] Jump buffering catches inputs reliably
- [ ] Coins collect and counter updates
- [ ] Hazards kill player instantly
- [ ] Win screen displays correctly
- [ ] Currency saves between sessions
- [ ] 60 FPS on target device
- [ ] No crashes or null reference errors

---

## 📚 Documentation Index

| File | Purpose |
|------|---------|
| **SETUP_GUIDE.md** | Full scene setup with every detail |
| **QUICK_START.md** | Rapid checklist (one per level) |
| **MOBILE_INPUT_GUIDE.md** | EventTrigger configuration + button layout |
| **ARCHITECTURE.md** | System design, data flows, diagrams |
| **PATTERNS.md** | 10 production-ready code patterns |

---

## 💡 Common Questions

**Q: Can I use this for 3D?**  
A: No, these are Rigidbody2D/Collider2D scripts. For 3D, rewrite for Rigidbody/Collider.

**Q: How do I make movement feel tighter?**  
A: Lower `coyoteTime` (0.08), increase `moveSpeed` (10+), reduce `jumpCutOffMultiplier` (0.3).

**Q: Can I add a double jump?**  
A: Yes, track `int jumpsRemaining` and reset on landing. See PATTERNS.md.

**Q: How do I test on phone without building APK every time?**  
A: Use a device simulation, or build once and iterate with remote debugging.

**Q: What if buttons feel unresponsive?**  
A: Ensure EventTrigger is on button (not canvas), set button size >80px, check Time.timeScale.

---

## 🎬 Next Steps

1. **Import scripts** into your project
2. **Follow QUICK_START.md** (10 minutes)
3. **Design 5 test levels** and playtest
4. **Tune physics** until it feels right
5. **Polish UI & visuals**
6. **Build for Android**
7. **Launch on Play Store**

---

## 📝 License & Attribution

This is **original production code** designed for Rift Runner MVP. Use freely in your game projects.

---

**Built for precision. Optimized for mobile. Ready to ship.**

*Last Updated: February 2026*

# Rift Runner - Visual Setup Guide

## 📐 Rigidbody2D Configuration

```
┌─────────────────────────────────┐
│  Rigidbody2D Inspector           │
├─────────────────────────────────┤
│ ✓ Enabled                       │
│                                 │
│ Body Type:          Dynamic      │
│ Gravity Scale:      (leave 1.0) │  ← PlayerController overrides
│ Drag:               0            │
│ Angular Drag:       0            │
│                                 │
│ Constraints:                    │
│ ✓ Freeze Rotation Z             │  ← Important! Prevents spinning
│                                 │
│ Collision:                      │
│ ✓ Continuous (or Discrete)      │
│ ✓ Active by default             │
└─────────────────────────────────┘
```

---

## 🏃 Player GameObject Hierarchy

```
Player (Root)
│
├─ [Sprite Renderer]              ← Your character sprite here
│  └─ Sprite: (assign your sprite)
│  └─ Order in Layer: 0
│
├─ [Box Collider 2D]              ← NOT trigger! Solid collision
│  └─ Size: (match sprite size)
│  └─ isTrigger: ✗ UNCHECKED
│
├─ [Rigidbody 2D]                 ← See config above
│  └─ Body Type: Dynamic
│  └─ Gravity: enabled
│
└─ [PlayerController Script]      ← All movement happens here
   ├─ moveSpeed: 8
   ├─ jumpForce: 12
   ├─ groundLayer: Ground layer mask
   └─ ... (other variables)
```

---

## 🪙 Coin Setup

```
┌──────────────────────┐
│  Coin GameObject     │
├──────────────────────┤
│ Sprite Renderer      │
│ ├─ Sprite: Coin     │
│ └─ Order: 1         │  ← Above ground
│                      │
│ Circle Collider 2D   │
│ ├─ Radius: 0.3      │
│ └─ isTrigger: ✓     │  ← MUST be trigger!
│                      │
│ CoinPickup Script    │
└──────────────────────┘
```

**Multiplied for each coin** (e.g., 10 coins = 10x Coin GameObject)

---

## ⚔️ Hazard (Spike) Setup

```
┌──────────────────────┐
│  Spike GameObject    │
├──────────────────────┤
│ Sprite Renderer      │
│ ├─ Sprite: Spike    │
│ └─ Order: 0         │
│                      │
│ Box/Circle Collider2D│
│ ├─ Size: 1x1        │
│ └─ isTrigger: ✓     │  ← MUST be trigger!
│                      │
│ Hazard Script        │
├─ restartDelay: 0.2  │
└──────────────────────┘
```

---

## 🚩 Goal Post Setup

```
┌──────────────────────┐
│  Goal GameObject     │
├──────────────────────┤
│ Sprite Renderer      │
│ ├─ Sprite: Flag     │
│ ├─ Color: Gold      │  ← Visual distinction
│ └─ Order: 0         │
│                      │
│ Box Collider 2D      │
│ ├─ Size: 2x2        │
│ └─ isTrigger: ✓     │  ← MUST be trigger!
│                      │
│ GoalPost Script      │
└──────────────────────┘
```

---

## 🎮 Mobile Button Layout (Portrait 1080x1920)

```
┌────────────────────────────────┐
│                                │
│                                │
│         GAME VIEW              │  1080x1920 Canvas
│       (1080x1620)              │
│                                │
│                                │
├────────────────────────────────┤
│ ┌────┐     ┌────┐     ┌────┐  │ 300px
│ │  ◀ │     │ ⬆  │     │  ▶ │  │
│ └────┘     └────┘     └────┘  │
│  Left     Jump       Right     │
└────────────────────────────────┘

Button Specs:
├─ Size: 100x100 pixels each
├─ Left: X=-140, Y=-60
├─ Jump: X=0, Y=-60
└─ Right: X=140, Y=-60
```

### Canvas Structure

```
Canvas (Screen Space - Overlay, 1080x1920)
├─ LeftButton
│  ├─ Image (white with alpha)
│  ├─ Text: "◀" (or icon)
│  └─ EventTrigger (Pointer Down/Up)
├─ JumpButton
│  ├─ Image
│  ├─ Text: "JUMP" or "⬆"
│  └─ EventTrigger (Pointer Down/Up)
└─ RightButton
   ├─ Image
   ├─ Text: "▶"
   └─ EventTrigger (Pointer Down/Up)
```

---

## 🔷 Layer & Tag Configuration

```
┌──────────────────────────────┐
│  Layers (Create These)       │
├──────────────────────────────┤
│ Default                      │
│ TransparentFX                │
│ Ignore Raycast               │
│ Water                        │
│ UI                           │
│ ✓ Ground        ← New!       │  Used for platforms
│ ✓ Player        ← New!       │  (not strictly needed)
│ ✓ Hazard        ← New!       │  For organization
│ ✓ Collectible   ← New!       │  For organization
└──────────────────────────────┘

┌──────────────────────────────┐
│  Tags (Create These)         │
├──────────────────────────────┤
│ Default                      │
│ Untagged                     │
│ ✓ Player        ← New!       │  Player GameObject
│ ✓ Ground        ← New!       │  Platforms
│ ✓ Collectible   ← New!       │  Coins
│ ✓ Hazard        ← New!       │  Spikes
└──────────────────────────────┘

Layer Mask Setup (PlayerController Inspector):
groundLayer = "Ground" (select in dropdown)
```

---

## 📊 EventTrigger Setup (Step-by-Step)

### Step 1: Add EventTrigger Component
```
Button GameObject (e.g., "LeftButton")
├─ Inspector
│  └─ [Add Component]
│     └─ Search: "EventTrigger"
│        └─ Select EventTrigger
```

### Step 2: Add Pointer Down Event
```
EventTrigger Inspector:
├─ Triggers list
│  ├─ [Add Event Type] ← Click
│  │  └─ Select "Pointer Down"
│  │
│  ├─ New entry appears:
│  │  └─ PointerDown
│  │     └─ [+] Callback
│  │        ├─ Drag MobileInputHandler into field
│  │        ├─ Function dropdown
│  │        └─ Select OnLeftPressed()
```

### Step 3: Add Pointer Up Event
```
EventTrigger Inspector:
├─ Triggers list
│  ├─ [Add Event Type] ← Click again
│  │  └─ Select "Pointer Up"
│  │
│  ├─ New entry appears:
│  │  └─ PointerUp
│  │     └─ [+] Callback
│  │        ├─ Drag MobileInputHandler into field
│  │        ├─ Function dropdown
│  │        └─ Select OnLeftReleased()
```

**Repeat for RightButton and JumpButton**

---

## 🎨 Player Movement Feel Diagram

```
Floaty (High gravity)          Perfect (Goldilocks)        Heavy (Low gravity)
┌─────────────────┐            ┌─────────────────┐         ┌─────────────────┐
│                 │            │                 │         │                 │
│  ↑ ↑ ↑ ↑ ↑ ↑    │            │   ↑ ↑ ↑ ↑ ↑    │         │     ↑   ↑   ↑   │
│                 │            │                 │         │                 │
│  Falls slow     │            │  Falls fast     │         │  Falls very slow│
│  Feels slow     │            │  Feels snappy   │         │  Feels sluggy   │
│                 │            │                 │         │                 │
│ gravityScale:2.0│            │ gravityScale:2.5│         │ gravityScale:1.8│
└─────────────────┘            └─────────────────┘         └─────────────────┘

Solution: Tune gravityScale first, then jumpForce
```

---

## 📈 Ground Detection Visualization (Gizmo)

```
When you add PlayerController and select player in scene,
a green circle appears at player feet:

        Player Sprite
            [■]
            |  ← groundCheckOffset
            ○  ← Green circle = ground detection radius
            |
     ─ ─ ─ ─ ─ ─ ─  Ground platform

Gizmo only visible when:
✓ PlayerController selected in scene
✓ Game NOT running (edit mode)
✓ Script has [SerializeField] for position offset

If no circle: Check that groundCheckRadius > 0
             and groundCheckOffset is visible in inspector
```

---

## 🔄 Input Flow Diagram

```
DESKTOP (Keyboard)
├─ User presses A
├─ Input.GetAxis("Horizontal") = -1
├─ PlayerController.Update()
│  └─ horizontalInput = -1
├─ PlayerController.FixedUpdate()
│  └─ rb.velocity.x = -8 (moveSpeed)
└─ Player moves left

MOBILE (Touch)
├─ User touches LeftButton
├─ EventTrigger fires PointerDown
├─ MobileInputHandler.OnLeftPressed()
│  └─ playerController.SetMobileHorizontalInput(-1f)
├─ PlayerController.Update()
│  └─ horizontalInput = -1
├─ PlayerController.FixedUpdate()
│  └─ rb.velocity.x = -8 (moveSpeed)
└─ Player moves left
```

---

## 🎪 Win Screen Canvas Hierarchy

```
Canvas (Screen Space - Overlay)
├─ Background (Image, black semi-transparent)
│  └─ Color: (0, 0, 0, 0.8)
├─ Title (Text)
│  └─ "Level Complete!"
├─ StarsDisplay
│  ├─ Star1 (Image, yellow/gray based on earned)
│  ├─ Star2 (Image, yellow/gray)
│  └─ Star3 (Image, yellow/gray)
├─ TimeDisplay (Text)
│  └─ "Time: 45.2s"
├─ CoinDisplay (Text)
│  └─ "Coins: 9/10"
├─ NextButton (Button)
│  ├─ Image
│  ├─ Text: "Next Level"
│  └─ OnClick: GoToNextLevel()
└─ RestartButton (Button)
   ├─ Image
   ├─ Text: "Restart"
   └─ OnClick: RestartLevel()
```

---

## 🧪 Build Settings Checklist

```
File > Build Settings
├─ Scenes In Build:
│  ├─ 0: Level_1 (or MainMenu)
│  ├─ 1: Level_2
│  ├─ 2: Level_3
│  └─ ... Level_N
├─ Platform: Android
├─ Target API Level: 30+
├─ Orientation: Portrait
├─ Resolution:
│  ├─ Default Orientation: Portrait
│  └─ Portrait (Upside Down): ✗ Unchecked
└─ Quality Settings:
   └─ Target Frame Rate: 60
```

---

## 📱 Android Build Configuration

```
Edit > Project Settings > Player
├─ Android
│  ├─ Resolution
│  │  ├─ Default Orientation: Portrait
│  │  └─ Aspect Ratio: Portrait (9:16)
│  ├─ Debugging
│  │  └─ ✓ Development Build: (for testing)
│  └─ Publishing Settings
│     ├─ Package Name: com.yourcompany.riftrunner
│     └─ Keystore Settings: (for Play Store)
└─ Quality
   └─ Default Quality Level: High
      ├─ V Sync Count: Don't Sync
      ├─ Target Frame Rate: 60
      └─ Physics2D Gravity: (9.81, -9.81)
```

---

**All diagrams are ASCII art for easy copy/paste reference during development.**

*Print and post near your desk!*

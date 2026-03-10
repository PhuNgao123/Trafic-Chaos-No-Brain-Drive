# Shield Power-Up Setup Checklist ✅

## Checklist Tạo Shield Prefab

### 1. Tạo GameObject
- [ ] Tạo 3D Object (Sphere/Cube) tên "ShieldPowerUp"
- [ ] Scale: 0.5, 0.5, 0.5 (hoặc kích thước phù hợp)

### 2. Visual
- [ ] Tạo Material "ShieldMat" với màu sáng
- [ ] Gán Material vào Shield GameObject
- [ ] (Optional) Bật Emission để shield phát sáng

### 3. Collider - QUAN TRỌNG!
- [ ] Có Collider component (Sphere/Box Collider)
- [ ] ✅ CHECK "Is Trigger" = true
- [ ] Radius/Size phù hợp với visual

### 4. Script
- [ ] Add component "ShieldPowerUp"
- [ ] Set Invincibility Duration (mặc định 5 giây)
- [ ] (Optional) Gán Pickup Sound
- [ ] (Optional) Gán Pickup Effect

### 5. Animation (Optional)
- [ ] Add "RotateObject" script để quay
- [ ] Add "BobbingEffect" script để lên xuống
- [ ] (Optional) Add Particle System

### 6. Lưu Prefab
- [ ] Kéo GameObject vào folder Assets/Prefabs
- [ ] GameObject trong Hierarchy chuyển màu xanh
- [ ] Có thể xóa instance trong scene

## Checklist Setup Player

### Player GameObject
- [ ] Tag = "Player"
- [ ] Có component "PlayerHealth"
- [ ] Có component "PlayerDamageHandler"
- [ ] Có component "PlayerInvincibility" ⭐ MỚI
- [ ] (Optional) Tạo Shield Visual Effect child object

## Checklist Test

### Test Cơ Bản
- [ ] Kéo Shield prefab vào scene
- [ ] Click Play
- [ ] Player chạm vào shield
- [ ] Shield biến mất ✅
- [ ] Console log: "Invincibility activated" ✅
- [ ] Player không nhận damage trong 5 giây ✅
- [ ] Console log: "Invincibility deactivated" sau 5 giây ✅

### Test Nâng Cao
- [ ] Shield quay (nếu có RotateObject)
- [ ] Shield lên xuống (nếu có BobbingEffect)
- [ ] Particle effect hoạt động
- [ ] Sound effect phát khi nhặt
- [ ] Pickup effect spawn khi nhặt

## Checklist Spawn System (Optional)

### Spawn Tự Động
- [ ] Tạo empty GameObject "ShieldSpawner"
- [ ] Add component "ShieldSpawner"
- [ ] Gán Shield Prefab vào Spawner
- [ ] Set Spawn Interval (15 giây)
- [ ] Set Max Active Shields (3)

### Spawn Points
- [ ] Tạo empty GameObjects làm spawn points
- [ ] Đặt ở các vị trí trong game
- [ ] Gán vào Spawner → Spawn Points array
- [ ] (Optional) Tổ chức trong folder "SpawnPoints"

### Test Spawner
- [ ] Click Play
- [ ] Shield spawn sau Initial Delay
- [ ] Shield spawn mỗi X giây
- [ ] Không spawn quá Max Active Shields
- [ ] Shield spawn ở các spawn points khác nhau

## Common Issues & Solutions

### ❌ Shield không biến mất
- ✅ Check: Collider → Is Trigger = true
- ✅ Check: Player Tag = "Player"
- ✅ Check: Console có error không

### ❌ Player vẫn nhận damage
- ✅ Check: PlayerInvincibility đã add vào Player
- ✅ Check: Console có log "Invincibility activated"
- ✅ Check: PlayerDamageHandler là version mới (có check invincibility)

### ❌ Shield rơi xuống
- ✅ Xóa Rigidbody component
- ✅ Hoặc check "Is Kinematic" trong Rigidbody

### ❌ Spawner không spawn
- ✅ Check: Shield Prefab đã gán vào Spawner
- ✅ Check: Spawn Points đã gán và có vị trí hợp lệ
- ✅ Check: Console có error không

## Files Cần Thiết

### Scripts Bắt Buộc
- ✅ PlayerInvincibility.cs
- ✅ ShieldPowerUp.cs
- ✅ PlayerDamageHandler.cs (updated)

### Scripts Optional
- ⭕ RotateObject.cs (animation)
- ⭕ BobbingEffect.cs (animation)
- ⭕ ShieldSpawner.cs (auto spawn)
- ⭕ ShieldTimerUI.cs (UI display)

### Assets Cần Có
- ✅ Shield Prefab
- ✅ Shield Material
- ⭕ Pickup Sound (optional)
- ⭕ Pickup Effect Prefab (optional)

## Hoàn Thành! 🎉

Khi tất cả checklist đã ✅:
- Shield power-up hoạt động hoàn hảo
- Player có thể nhặt shield và bất tử
- System tự động spawn shields
- Game play thú vị hơn!

---

**Lưu ý**: Các mục có ⭕ là optional, không bắt buộc nhưng làm game đẹp hơn.

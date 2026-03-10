# Shield Power-Up System - Tóm Tắt

## Files Đã Tạo

### Core Scripts
1. **PlayerInvincibility.cs** - Quản lý trạng thái bất tử
2. **ShieldPowerUp.cs** - Collectible shield item
3. **PlayerDamageHandler.cs** - Đã cập nhật để hỗ trợ invincibility

### Optional Scripts
4. **ShieldTimerUI.cs** - Hiển thị shield timer trên UI
5. **ShieldSpawner.cs** - Tự động spawn shields trong game

## Setup Nhanh (3 Bước)

### 1. Setup Player
- Add component `PlayerInvincibility` vào Player GameObject
- Player phải có tag "Player"

### 2. Tạo Shield Prefab
- Tạo GameObject mới với visual (Sphere/Cube)
- Add Collider, check "Is Trigger"
- Add script `ShieldPowerUp`
- Lưu thành Prefab

### 3. Spawn Shield
- Kéo Shield Prefab vào scene, HOẶC
- Tạo empty GameObject, add `ShieldSpawner`, gán Shield Prefab

## Cách Hoạt Động

1. Player chạm vào Shield (trigger collision)
2. ShieldPowerUp kích hoạt PlayerInvincibility
3. Shield biến mất
4. Player bất tử trong X giây (mặc định 5s)
5. Trong thời gian bất tử: PlayerDamageHandler bỏ qua damage
6. Hết thời gian: Player nhận damage bình thường

## Xem Chi Tiết
Đọc file `SHIELD_POWERUP_SETUP.md` để biết hướng dẫn đầy đủ.

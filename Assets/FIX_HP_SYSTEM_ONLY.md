# FIX HỆ THỐNG HP - CHỈ HP THÔI

## ✅ ĐÃ LÀM
- [x] Di chuyển PlayerHealth từ Player root → CarPhysic
- [x] Di chuyển PlayerDamageHandler từ Player root → CarPhysic

## 🔧 CẦN LÀM (3 BƯỚC)

### BƯỚC 1: Xóa Scripts Cũ Ở Player Root
1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **Player** root GameObject
3. Xóa các component sau (nếu còn):
   - **PlayerHealth** → Remove Component
   - **PlayerDamageHandler** → Remove Component
4. **Lưu prefab** (Ctrl+S)

### BƯỚC 2: Cập Nhật HealthUI Reference
1. Mở Scene `Assets/Scenes/GameScene.unity`
2. Trong Hierarchy, tìm **HealthUI** (trong Canvas)
3. Chọn HealthUI
4. Trong Inspector → **HealthUI (Script)** component
5. Tìm field **Player Health**
6. Kéo **Player → CarPhysic** từ Hierarchy vào field **Player Health**
7. **Lưu Scene** (Ctrl+S)

### BƯỚC 3: Kiểm Tra CarPhysic
Chọn Player → CarPhysic, đảm bảo có:
- ✅ **PlayerHealth** component
- ✅ **PlayerDamageHandler** component
- ✅ **Rigidbody** (Use Gravity = TRUE, Is Kinematic = FALSE)
- ✅ **Box Collider** (IsTrigger = OFF, Enabled = TRUE)
- ✅ **Tag = "Player"**

## 🧪 TEST

1. Chạy game
2. Kiểm tra health bar hiển thị 100%
3. Đâm vào Enemy vehicle
4. Kiểm tra Console log:
```
[PlayerDamageHandler] Collision detected with: Hatchback(Clone), Tag: Vehicle
[PlayerDamageHandler] Player took 20 damage from Hatchback(Clone). Current health: 80
```
5. Health bar phải giảm xuống

## ⚠️ NẾU KHÔNG HOẠT ĐỘNG

### Health bar không hiển thị
→ Kiểm tra lại Bước 2 (HealthUI reference)

### Đâm xe không trừ HP
→ Kiểm tra Console log:
- Nếu KHÔNG có log gì → Collision không trigger
  - Kiểm tra CarPhysic có Box Collider (IsTrigger = OFF)
  - Kiểm tra Enemy có Tag = "Vehicle"
  - Kiểm tra Enemy có Rigidbody (Is Kinematic = FALSE)
- Nếu có log "Collision detected" → Kiểm tra Enemy có VehicleDamage component

### Enemy bay khỏi đường
→ Sửa Enemy prefabs:
1. Mở Enemy prefab (Hatchback, Van, Taxi, v.v.)
2. Chọn root GameObject
3. Sửa **Rigidbody**:
   - Use Gravity = **FALSE**
   - Is Kinematic = **FALSE**
   - Constraints → Freeze Position: **Y** (check)
   - Constraints → Freeze Rotation: **X, Y, Z** (check cả 3)
4. Lưu prefab
5. Làm tương tự cho TẤT CẢ Enemy prefabs

## 📋 DANH SÁCH ENEMY PREFABS CẦN SỬA
- Hatchback.prefab
- Hatchback 1.prefab
- Hatchback 2.prefab
- Pickup.prefab
- Pickup 1.prefab
- Pickup 2.prefab
- Van.prefab
- Van 1.prefab
- VanBig.prefab
- VanBig 1.prefab
- Taxi.prefab
- Police.prefab
- Towtruck.prefab
- Towtruck 1.prefab
- Truck.prefab

## ✨ XONG!
Sau khi làm 3 bước trên, HP system sẽ hoạt động:
- Đâm xe → Trừ HP
- Health bar giảm
- HP = 0 → Game Over

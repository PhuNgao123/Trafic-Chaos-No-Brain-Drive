# CHECKLIST - Sau Khi Di Chuyển Scripts

## ✅ ĐÃ LÀM
- [x] Di chuyển PlayerHealth từ Player root → CarPhysic child
- [x] Di chuyển PlayerDamageHandler từ Player root → CarPhysic child

## 🔧 CẦN LÀM NGAY

### BƯỚC 0: Di Chuyển PlayerInvincibility (QUAN TRỌNG!)
1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **Player** root GameObject
3. Tìm **PlayerInvincibility** component
4. Click chuột phải → **Copy Component**
5. Chọn **CarPhysic** child GameObject
6. Click chuột phải trong Inspector → **Paste Component As New**
7. Quay lại **Player** root, xóa PlayerInvincibility cũ (Remove Component)
8. **Lưu prefab** (Ctrl+S)

### BƯỚC 1: Xóa Scripts Cũ Ở Player Root
1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **Player** root GameObject
3. Trong Inspector, kiểm tra và xóa các component sau (nếu còn):
   - **PlayerHealth** → Remove Component
   - **PlayerDamageHandler** → Remove Component
   - **PlayerInvincibility** → Remove Component (nếu chưa xóa ở Bước 0)
4. **Lưu prefab** (Ctrl+S)

### BƯỚC 2: Cập Nhật HealthUI Reference
1. Mở Scene `Assets/Scenes/GameScene.unity`
2. Trong Hierarchy, tìm **HealthUI** GameObject (thường trong Canvas)
3. Chọn HealthUI
4. Trong Inspector, tìm **HealthUI (Script)** component
5. Tìm field **Player Health** (đang trống hoặc có reference cũ)
6. Kéo **Player → CarPhysic** từ Hierarchy vào field **Player Health**
   - HOẶC: Click vòng tròn bên phải → chọn CarPhysic (PlayerHealth)
7. **Lưu Scene** (Ctrl+S)

### BƯỚC 3: Xóa CollisionForwarder (Không Cần Nữa)
1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **CarPhysic** child GameObject
3. Nếu có **CollisionForwarder** component → Remove nó
4. Lưu prefab

### BƯỚC 4: Kiểm Tra CarPhysic Setup
Đảm bảo CarPhysic có đầy đủ:
- ✅ Rigidbody (Use Gravity = TRUE, Is Kinematic = FALSE)
- ✅ Box Collider (IsTrigger = OFF, Enabled = TRUE)
- ✅ Box Collider thứ 2 (IsTrigger = ON) - cho power-ups
- ✅ **PlayerHealth** component
- ✅ **PlayerDamageHandler** component
- ✅ **PlayerInvincibility** component (QUAN TRỌNG!)
- ✅ PlayerPhysics component
- ✅ PlayerController component
- ✅ Tag = "Player"

## 🧪 TEST

### Test 1: Health UI Hiển Thị
1. Chạy game
2. Kiểm tra health bar hiển thị đúng (100%)
3. Nếu KHÔNG hiển thị → Kiểm tra lại Bước 2

### Test 2: Collision Detection
1. Chạy game
2. Đâm vào Enemy vehicle
3. Kiểm tra Console log:
```
[PlayerDamageHandler] Collision detected with: Hatchback(Clone), Tag: Vehicle
[PlayerDamageHandler] Player took 20 damage from Hatchback(Clone). Current health: 80
```
4. Kiểm tra health bar giảm xuống

### Test 3: Game Over
1. Đâm nhiều lần cho đến khi health = 0
2. Game phải kết thúc (hiển thị Game Over screen)

## ⚠️ NẾU CÓ LỖI

### Lỗi: "PlayerHealth reference is not assigned!"
→ Quay lại Bước 2, cập nhật HealthUI reference

### Lỗi: "PlayerHealth component not found!"
→ Kiểm tra PlayerHealth đã ở CarPhysic chưa

### Lỗi: Collision không trigger
→ Kiểm tra CarPhysic có Box Collider với IsTrigger = OFF

### Lỗi: Health bar không giảm
→ Kiểm tra Console log xem có collision event không

## 📝 LƯU Ý

**TẠI SAO DI CHUYỂN SCRIPTS?**
- Collision events (OnCollisionEnter) chỉ được gọi trên GameObject có Collider
- CarPhysic có Collider → PlayerDamageHandler phải ở CarPhysic
- PlayerHealth phải cùng GameObject với PlayerDamageHandler (vì PlayerDamageHandler dùng GetComponent)

**CÓ ẢNH HƯỞNG GÌ KHÔNG?**
- Không ảnh hưởng gameplay
- Chỉ cần cập nhật UI references
- Các script khác vẫn hoạt động bình thường

# HƯỚNG DẪN FIX COLLISION - Đơn Giản Nhất

## VẤN ĐỀ HIỆN TẠI
- Xe đâm vào nhau KHÔNG trừ HP
- Enemy vehicle bay khỏi đường
- Console log chỉ thấy collision với "Road" và "Barrier", KHÔNG thấy "Vehicle"

## NGUYÊN NHÂN
Player prefab có **3 BoxColliders** gây xung đột:
1. BoxCollider trên Player root (IsTrigger = TRUE) 
2. BoxCollider trên CarPhysic child (IsTrigger = FALSE)
3. BoxCollider trên CarPhysic child (IsTrigger = TRUE)

## GIẢI PHÁP - THỰC HIỆN TRONG UNITY EDITOR

### BƯỚC 1: Sửa Player Prefab
1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **Player** root GameObject (không phải CarPhysic)
3. Trong Inspector, tìm **Box Collider** component
4. **XÓA** Box Collider này (click dấu ... → Remove Component)
5. Lưu prefab (Ctrl+S)

### BƯỚC 2: Kiểm Tra CarPhysic Child
1. Vẫn trong Player prefab, chọn **CarPhysic** child GameObject
2. Kiểm tra có **2 Box Colliders**:
   - Collider 1: IsTrigger = OFF (để phát hiện collision)
   - Collider 2: IsTrigger = ON (để phát hiện power-ups)
3. Đảm bảo **Rigidbody** có:
   - Use Gravity = FALSE
   - Is Kinematic = FALSE (QUAN TRỌNG!)
   - Collision Detection = Continuous Dynamic

### BƯỚC 3: Sửa PlayerDamageHandler Script
PlayerDamageHandler đang ở Player root, nhưng Collider ở CarPhysic child.

**CÁCH 1 (Đơn giản):** Di chuyển PlayerDamageHandler
1. Chọn Player root GameObject
2. Kéo **PlayerDamageHandler** component xuống **CarPhysic** child
3. Lưu prefab

**CÁCH 2 (Nếu cách 1 không được):** Sửa code
- Sẽ cần sửa PlayerDamageHandler.cs để lắng nghe collision từ child

### BƯỚC 4: Kiểm Tra Enemy Prefabs
1. Mở một Enemy prefab (ví dụ: Hatchback.prefab)
2. Kiểm tra **Rigidbody**:
   - Use Gravity = FALSE (QUAN TRỌNG! Nếu TRUE sẽ bay)
   - Is Kinematic = FALSE
   - Constraints: Freeze Position Y, Freeze Rotation X, Y, Z
3. Kiểm tra **Box Collider**:
   - IsTrigger = OFF
   - Enabled = TRUE
4. Kiểm tra **Tag = "Vehicle"**
5. Lưu prefab

### BƯỚC 5: Test
1. Chạy game
2. Mở Scene view (để thấy green wireframe của Colliders)
3. Cho Player đâm vào Enemy
4. Kiểm tra Console log - phải thấy:
   ```
   [PlayerDamageHandler] Collision detected with: Hatchback(Clone), Tag: Vehicle
   [PlayerDamageHandler] Player took 20 damage...
   ```

## TẠI SAO ENEMY BAY KHỎI ĐƯỜNG?

**Nguyên nhân:** VehicleMove.cs dùng `transform.position` để di chuyển, nhưng:
- Nếu Use Gravity = TRUE → Gravity kéo xe xuống
- Nếu Is Kinematic = TRUE → Physics bị tắt, xe không va chạm được

**Giải pháp:**
- Use Gravity = FALSE
- Is Kinematic = FALSE  
- Thêm Constraints để giữ xe trên đường (Freeze Position Y)

## NẾU VẪN KHÔNG HOẠT ĐỘNG

Kiểm tra trong Console log khi đâm xe:
- Nếu KHÔNG thấy log gì → Collision không trigger (kiểm tra lại Colliders)
- Nếu thấy "Collision detected with: Hatchback(Clone), Tag: Vehicle" nhưng không trừ HP → Kiểm tra VehicleDamage component trên Enemy
- Nếu thấy "Collision detected with: Hatchback(Clone), Tag: Untagged" → Enemy không có Tag "Vehicle"

## LƯU Ý QUAN TRỌNG

**Kinematic Rigidbody + OnCollisionEnter:**
- Kinematic object chỉ phát hiện collision với NON-kinematic object
- Player (Kinematic) + Enemy (Kinematic) = KHÔNG collision!
- Player (Kinematic) + Enemy (Non-kinematic) = CÓ collision ✓

**Vì vậy:**
- Player: Is Kinematic = TRUE (hoặc FALSE, tùy PlayerPhysics)
- Enemy: Is Kinematic = FALSE (BẮT BUỘC!)

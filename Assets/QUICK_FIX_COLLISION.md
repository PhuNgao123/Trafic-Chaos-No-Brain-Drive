# FIX NHANH - Collision Không Hoạt Động

## VẤN ĐỀ
Xe đâm vào nhau không trừ HP vì PlayerDamageHandler ở Player root nhưng Collider ở CarPhysic child.

## GIẢI PHÁP - 2 PHÚT

### Cách 1: Thêm CollisionForwarder (ĐỀ XUẤT)

1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **CarPhysic** child GameObject
3. Click **Add Component**
4. Tìm và thêm **CollisionForwarder** script
5. Lưu prefab (Ctrl+S)
6. Test game

### Cách 2: Di Chuyển PlayerDamageHandler

1. Mở `Assets/Prefabs/Cars/Player.prefab`
2. Chọn **Player** root GameObject
3. Click chuột phải vào **PlayerDamageHandler** component → Copy Component
4. Chọn **CarPhysic** child GameObject  
5. Click chuột phải trong Inspector → Paste Component As New
6. Quay lại **Player** root, xóa PlayerDamageHandler cũ
7. Lưu prefab (Ctrl+S)
8. Test game

## FIX ENEMY BAY KHỎI ĐƯỜNG

Mở từng Enemy prefab và sửa **Rigidbody**:
- Use Gravity = **FALSE** (quan trọng!)
- Is Kinematic = **FALSE**
- Constraints: 
  - Freeze Position: **Y** (check)
  - Freeze Rotation: **X, Y, Z** (check cả 3)

Áp dụng cho tất cả Enemy prefabs:
- Hatchback, Hatchback 1, Hatchback 2
- Pickup, Pickup 1, Pickup 2
- Van, Van 1, VanBig, VanBig 1
- Taxi, Police, Towtruck, Towtruck 1, Truck

## KIỂM TRA

Sau khi fix, test game và kiểm tra Console log:
```
[PlayerDamageHandler] Collision detected with: Hatchback(Clone), Tag: Vehicle
[PlayerDamageHandler] Player took 20 damage from Hatchback(Clone). Current health: 80
```

Nếu thấy log này → Thành công! ✓

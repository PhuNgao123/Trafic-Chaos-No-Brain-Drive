# Hướng Dẫn Setup Shield Với Ảnh/Sprite Có Sẵn

## Bước 1: Import Ảnh Shield Vào Unity

1. Kéo file ảnh shield vào Unity (folder **Assets/Sprites** hoặc **Assets/Images**)
2. Chọn ảnh trong Project window
3. Trong **Inspector**, cấu hình:
   ```
   Texture Type: Sprite (2D and UI)
   Sprite Mode: Single
   Pixels Per Unit: 100 (hoặc giữ mặc định)
   Filter Mode: Bilinear
   ```
4. Click **Apply**

## Bước 2: Tạo Shield GameObject Với Sprite

### Cách 1: Dùng Sprite Renderer (2D)

1. Trong **Hierarchy**, click chuột phải → **Create Empty**
2. Đổi tên: **"ShieldPowerUp"**
3. Với ShieldPowerUp được chọn:
   - Click **Add Component** → **Sprite Renderer**
   - Kéo ảnh shield vào field **Sprite**
   - Điều chỉnh **Sorting Layer** và **Order in Layer** nếu cần

4. Điều chỉnh **Transform**:
   ```
   Position: X=0, Y=1, Z=0 (vị trí test)
   Rotation: X=0, Y=0, Z=0
   Scale: X=0.5, Y=0.5, Z=0.5 (điều chỉnh cho phù hợp)
   ```

### Cách 2: Dùng Quad (3D với Texture)

1. Trong **Hierarchy**, click chuột phải → **3D Object → Quad**
2. Đổi tên: **"ShieldPowerUp"**
3. Tạo Material mới:
   - Trong Project, click chuột phải → **Create → Material**
   - Đặt tên: **"ShieldSpriteMat"**
   - Chọn Shader: **Unlit/Transparent** (hoặc **Sprites/Default**)
   - Kéo ảnh shield vào field **Texture** (hoặc **Sprite**)
4. Kéo Material vào Quad
5. Điều chỉnh Scale cho phù hợp

## Bước 3: Setup Collider (QUAN TRỌNG!)

1. Chọn ShieldPowerUp trong Hierarchy
2. Click **Add Component**
3. Chọn:
   - **Box Collider** (cho sprite vuông/chữ nhật)
   - **Sphere Collider** (cho sprite tròn)
   - **Capsule Collider** (cho sprite dài)

4. Trong Collider component:
   - ✅ **CHECK "Is Trigger"** - CỰC KỲ QUAN TRỌNG!
   - Điều chỉnh **Size/Radius** để khớp với hình ảnh
   - Điều chỉnh **Center** nếu cần

> 💡 **Tip**: Bật **Gizmos** trong Scene view để thấy collider (màu xanh lá)

## Bước 4: Add Scripts

### 4.1: Add ShieldPowerUp Script
1. Chọn ShieldPowerUp trong Hierarchy
2. Click **Add Component** → tìm **"ShieldPowerUp"**
3. Cấu hình:
   ```
   Invincibility Duration: 5 (giây)
   Pickup Effect: None (optional)
   Pickup Sound: None (optional)
   ```

### 4.2: Add Animation Scripts (Optional)
1. Click **Add Component** → tìm **"RotateObject"**
   - Rotation Speed: 50
   - Rotation Axis: (0, 1, 0) - quay quanh trục Y

2. Click **Add Component** → tìm **"BobbingEffect"**
   - Bobbing Speed: 2
   - Bobbing Amount: 0.3

## Bước 5: Tạo Prefab

1. Trong **Project** window, vào folder **Assets/Prefabs**
2. Kéo **ShieldPowerUp** từ **Hierarchy** vào folder **Prefabs**
3. GameObject trong Hierarchy chuyển màu xanh → thành công!
4. Có thể xóa instance trong scene

## Bước 6: Setup Player (Nếu Chưa)

1. Chọn **Player** GameObject
2. Đảm bảo **Tag = "Player"**
3. Click **Add Component** → tìm **"PlayerInvincibility"**
4. Cấu hình:
   ```
   Default Invincibility Duration: 5
   Shield Visual Effect: None (hoặc tạo effect - xem bên dưới)
   ```

## Bước 7: (Optional) Tạo Shield Visual Effect Cho Player

Khi player nhặt shield, có thể hiển thị visual effect quanh player:

### Cách 1: Dùng Cùng Sprite Shield
1. Chọn Player trong Hierarchy
2. Click chuột phải Player → **Create Empty**
3. Đổi tên child: **"ShieldEffect"**
4. Add component **Sprite Renderer** (hoặc Quad)
5. Gán ảnh shield vào
6. Scale lớn hơn player một chút (ví dụ: 1.5, 1.5, 1.5)
7. Điều chỉnh **Color** → Alpha = 0.5 (trong suốt)
8. Kéo **ShieldEffect** vào field **Shield Visual Effect** trong PlayerInvincibility
9. ShieldEffect sẽ tự động ẩn/hiện

### Cách 2: Dùng Particle System
1. Chọn Player → click chuột phải → **Effects → Particle System**
2. Đổi tên: **"ShieldParticles"**
3. Cấu hình particles với màu xanh/vàng
4. Kéo vào **Shield Visual Effect** trong PlayerInvincibility

## Bước 8: Test

1. Kéo **ShieldPowerUp prefab** vào scene
2. Click **Play**
3. Di chuyển player đến shield
4. Kiểm tra:
   - ✅ Shield biến mất
   - ✅ Console: "Invincibility activated"
   - ✅ Shield effect hiện quanh player (nếu có)
   - ✅ Player không nhận damage
   - ✅ Sau 5 giây: "Invincibility deactivated"

## Bước 9: Spawn Shields Trong Game

### Tạo Spawner
1. Hierarchy → click chuột phải → **Create Empty**
2. Đổi tên: **"ShieldSpawner"**
3. Add component **"ShieldSpawner"**
4. Cấu hình:
   ```
   Shield Prefab: Kéo ShieldPowerUp prefab vào
   Spawn Interval: 15 (giây)
   Max Active Shields: 3
   Initial Delay: 5
   Spawn On Start: false
   ```

### Tạo Spawn Points
1. Tạo empty GameObjects ở các vị trí muốn spawn:
   - **Create Empty** → đặt tên "SpawnPoint1"
   - Đặt ở vị trí trong game (trên đường, bên lề, etc.)
   - Lặp lại cho nhiều vị trí

2. Gán vào Spawner:
   - Chọn ShieldSpawner
   - Trong **Spawn Points**, set **Size** = số spawn points
   - Kéo từng spawn point vào các Element

## Tips & Tricks

### Làm Shield Nổi Bật Hơn
1. **Glow Effect**: 
   - Dùng shader **Sprites/Default** với **Color** sáng
   - Hoặc add **Light** component (Point Light) làm child

2. **Scale Animation**:
   - Add script để scale lớn nhỏ:
   ```csharp
   void Update() {
       float scale = 0.5f + Mathf.Sin(Time.time * 2f) * 0.1f;
       transform.localScale = Vector3.one * scale;
   }
   ```

3. **Trail Effect**:
   - Add **Trail Renderer** component
   - Set màu và width phù hợp

### Tối Ưu Performance
- Nếu có nhiều shields, dùng **Object Pooling** thay vì Instantiate/Destroy
- Giảm Particle count nếu có nhiều shields cùng lúc

## Cấu Trúc Cuối Cùng

```
ShieldPowerUp (Prefab)
├─ Transform
├─ Sprite Renderer (hoặc Quad + Material)
│  └─ Sprite: [Ảnh shield của bạn]
├─ Box/Sphere Collider (Is Trigger = ✅)
├─ ShieldPowerUp (Script)
├─ RotateObject (Script) - optional
└─ BobbingEffect (Script) - optional
```

```
Player
├─ ... (các components khác)
├─ PlayerInvincibility (Script) ⭐
└─ ShieldEffect (Child GameObject) - optional
   └─ Sprite Renderer (ảnh shield, alpha 0.5)
```

## Troubleshooting

### Sprite không hiển thị
- Check Sorting Layer và Order in Layer
- Check Camera là Orthographic (cho 2D) hoặc Perspective (cho 3D)
- Check sprite có trong Scene view không

### Shield không trigger
- ✅ Collider phải có "Is Trigger" = true
- ✅ Player phải có Tag = "Player"
- ✅ Player phải có Collider (không cần Is Trigger)

### Sprite bị mờ/pixelated
- Tăng resolution ảnh gốc
- Trong Import Settings: Filter Mode = Bilinear hoặc Trilinear
- Compression = None (cho chất lượng tốt nhất)

Xong! Shield với ảnh có sẵn đã setup thành công! 🛡️✨

# Hướng Dẫn Tạo Shield Prefab - Chi Tiết Từng Bước

## Phần 1: Tạo Shield GameObject Cơ Bản

### Bước 1: Tạo GameObject Mới
1. Trong Unity Editor, click chuột phải vào **Hierarchy** window
2. Chọn **3D Object → Sphere** (hoặc Cube, Cylinder tùy thích)
3. Đổi tên thành **"ShieldPowerUp"**

### Bước 2: Điều Chỉnh Transform
1. Chọn ShieldPowerUp trong Hierarchy
2. Trong **Inspector** window, tìm **Transform** component:
   ```
   Position: X=0, Y=1, Z=0 (hoặc vị trí bạn muốn test)
   Rotation: X=0, Y=0, Z=0
   Scale: X=0.5, Y=0.5, Z=0.5 (shield nhỏ hơn xe)
   ```

### Bước 3: Tạo Material Cho Shield
1. Trong **Project** window, vào folder **Assets/Materials**
2. Click chuột phải → **Create → Material**
3. Đặt tên: **"ShieldMat"**
4. Chọn ShieldMat, trong Inspector:
   - **Albedo Color**: Chọn màu sáng (vàng, xanh dương, hoặc cyan)
   - Ví dụ: RGB(0, 200, 255) - màu xanh dương sáng
   - **Metallic**: 0.5
   - **Smoothness**: 0.8
   - (Optional) **Emission**: Check và chọn cùng màu với Albedo để shield phát sáng

5. Kéo **ShieldMat** vào ShieldPowerUp GameObject trong Hierarchy

## Phần 2: Setup Collider (QUAN TRỌNG!)

### Bước 4: Cấu Hình Collider
1. Chọn ShieldPowerUp trong Hierarchy
2. Trong Inspector, tìm **Sphere Collider** component (đã có sẵn)
3. ✅ **CHECK** ô **"Is Trigger"** - CỰC KỲ QUAN TRỌNG!
4. Điều chỉnh **Radius** nếu cần (mặc định 0.5 là OK)

> ⚠️ **LƯU Ý**: Nếu không check "Is Trigger", shield sẽ va chạm vật lý thay vì trigger event!

## Phần 3: Thêm Script

### Bước 5: Add ShieldPowerUp Script
1. Chọn ShieldPowerUp trong Hierarchy
2. Trong Inspector, click **"Add Component"**
3. Gõ **"ShieldPowerUp"** và chọn script
4. Script sẽ xuất hiện với các settings:

   ```
   Shield Power Up (Script)
   ├─ Invincibility Duration: 5 (thời gian bất tử - giây)
   ├─ Pickup Effect: None (optional - kéo prefab effect vào)
   └─ Pickup Sound: None (optional - kéo audio clip vào)
   ```

5. Điều chỉnh **Invincibility Duration** nếu muốn (5 giây là mặc định)

## Phần 4: Thêm Hiệu Ứng (Optional Nhưng Đẹp)

### Bước 6A: Thêm Rotation Animation
1. Chọn ShieldPowerUp trong Hierarchy
2. Click **"Add Component"**
3. Gõ **"New Script"** và đặt tên: **"RotateObject"**
4. Mở script và paste code:

```csharp
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
```

5. Save và quay lại Unity

### Bước 6B: Thêm Bobbing Animation (Lên Xuống)
1. Thêm script **"BobbingEffect"**:

```csharp
using UnityEngine;

public class BobbingEffect : MonoBehaviour
{
    [SerializeField] private float bobbingSpeed = 2f;
    [SerializeField] private float bobbingAmount = 0.3f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
```

### Bước 6C: Thêm Particle Effect (Nâng Cao)
1. Click chuột phải vào ShieldPowerUp trong Hierarchy
2. Chọn **Effects → Particle System**
3. Đổi tên child object thành **"ShieldParticles"**
4. Trong Inspector của ShieldParticles:
   ```
   Start Lifetime: 1
   Start Speed: 0.5
   Start Size: 0.1
   Start Color: Cùng màu với shield
   Emission → Rate over Time: 10
   Shape → Shape: Sphere
   Shape → Radius: 0.5
   ```

## Phần 5: Tạo Prefab

### Bước 7: Lưu Thành Prefab
1. Trong **Project** window, mở folder **Assets/Prefabs**
2. Nếu chưa có folder, tạo mới: Click chuột phải → **Create → Folder** → đặt tên "Prefabs"
3. Kéo **ShieldPowerUp** từ **Hierarchy** vào folder **Prefabs**
4. Shield GameObject trong Hierarchy sẽ chuyển sang màu xanh (nghĩa là đã là prefab)
5. Có thể xóa instance trong scene (click chuột phải → Delete)

## Phần 6: Test Shield

### Bước 8: Setup Player (Nếu Chưa Có)
1. Chọn **Player** GameObject trong Hierarchy
2. Đảm bảo có **Tag = "Player"**:
   - Trong Inspector, phía trên cùng, dropdown **Tag** → chọn **"Player"**
3. Click **"Add Component"** → tìm **"PlayerInvincibility"**
4. Nếu chưa có PlayerHealth và PlayerDamageHandler, add cả 2

### Bước 9: Test Trong Scene
1. Kéo **ShieldPowerUp prefab** từ Project vào **Scene**
2. Đặt ở vị trí player có thể chạm vào
3. Click **Play** button
4. Di chuyển player đến shield
5. Kiểm tra:
   - ✅ Shield biến mất khi player chạm vào
   - ✅ Console log hiển thị: "[PlayerInvincibility] Invincibility activated for 5 seconds"
   - ✅ Player không nhận damage khi va chạm với vehicles
   - ✅ Sau 5 giây, console log: "[PlayerInvincibility] Invincibility deactivated"

## Phần 7: Spawn Shield Trong Game

### Cách 1: Đặt Sẵn Trong Scene
- Kéo ShieldPowerUp prefab vào scene nhiều lần ở các vị trí khác nhau
- Shield sẽ xuất hiện ngay khi game start

### Cách 2: Spawn Tự Động (Recommended)

#### Bước 10: Tạo Spawner
1. Trong Hierarchy, click chuột phải → **Create Empty**
2. Đổi tên: **"ShieldSpawner"**
3. Add component **"ShieldSpawner"** script
4. Trong Inspector:
   ```
   Shield Prefab: Kéo ShieldPowerUp prefab vào đây
   Spawn Interval: 15 (spawn mỗi 15 giây)
   Max Active Shields: 3 (tối đa 3 shields cùng lúc)
   ```

#### Bước 11: Tạo Spawn Points
1. Tạo empty GameObjects làm spawn points:
   - Click chuột phải Hierarchy → **Create Empty**
   - Đặt tên: **"ShieldSpawnPoint1"**
   - Đặt ở vị trí muốn spawn shield
   - Lặp lại để tạo nhiều spawn points (SpawnPoint2, SpawnPoint3...)

2. Gán spawn points vào Spawner:
   - Chọn ShieldSpawner
   - Trong Inspector, tìm **Spawn Points**
   - Set **Size** = số lượng spawn points (ví dụ: 5)
   - Kéo từng spawn point vào các Element

3. (Optional) Tổ chức hierarchy:
   - Tạo empty GameObject tên "SpawnPoints"
   - Kéo tất cả spawn points vào làm children

## Troubleshooting

### Shield không biến mất khi player chạm
- ✅ Kiểm tra Collider có **Is Trigger = true**
- ✅ Kiểm tra Player có **Tag = "Player"**
- ✅ Xem Console có error không

### Shield rơi xuống đất
- Nếu có Rigidbody, xóa nó đi (shield không cần physics)
- Hoặc check **Is Kinematic** trong Rigidbody

### Player vẫn nhận damage
- ✅ Kiểm tra PlayerInvincibility đã add vào Player
- ✅ Kiểm tra Console có log "Invincibility activated"
- ✅ Kiểm tra PlayerDamageHandler đã được cập nhật (file mới)

## Tùy Chỉnh Nâng Cao

### Thay Đổi Model
Thay vì Sphere, có thể dùng:
- **3D Icon**: Import model shield từ Asset Store
- **Sprite**: Dùng Quad + texture 2D
- **Custom Model**: Import file .fbx/.obj

### Thêm Sound Effect
1. Import audio file vào **Assets/Audio** (hoặc tạo folder mới)
2. Chọn ShieldPowerUp prefab
3. Trong ShieldPowerUp script, kéo audio clip vào **Pickup Sound**

### Thêm Pickup Effect
1. Tạo particle effect prefab (hoặc dùng có sẵn)
2. Kéo vào **Pickup Effect** trong ShieldPowerUp script
3. Effect sẽ spawn khi nhặt shield

## Kết Quả Cuối Cùng

Shield prefab hoàn chỉnh sẽ có:
```
ShieldPowerUp (Prefab)
├─ Transform (Position, Rotation, Scale)
├─ Mesh Filter (Sphere)
├─ Mesh Renderer (ShieldMat material)
├─ Sphere Collider (Is Trigger = ✅)
├─ ShieldPowerUp (Script)
├─ RotateObject (Script) - optional
├─ BobbingEffect (Script) - optional
└─ ShieldParticles (Particle System) - optional
```

Chúc bạn thành công! 🛡️

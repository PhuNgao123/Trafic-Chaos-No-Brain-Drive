# Hướng Dẫn Setup Shield Power-Up (Invincibility)

## Tổng Quan
Hệ thống shield power-up cho phép player nhặt shield để kích hoạt trạng thái bất tử (invincible) trong một khoảng thời gian. Khi bất tử, player không nhận damage từ va chạm với vehicles.

## Components Đã Tạo

### 1. PlayerInvincibility.cs
- Quản lý trạng thái bất tử của player
- Đếm ngược thời gian invincibility
- Hỗ trợ visual effect cho shield

### 2. ShieldPowerUp.cs
- Collectible shield item
- Trigger khi player va chạm
- Tự động kích hoạt invincibility và destroy

### 3. PlayerDamageHandler.cs (Đã cập nhật)
- Kiểm tra invincibility trước khi apply damage
- Bỏ qua damage nếu player đang bất tử

## Setup Trong Unity

### Bước 1: Setup Player GameObject

1. Chọn Player GameObject trong scene
2. Add component `PlayerInvincibility`:
   - Click "Add Component"
   - Tìm "PlayerInvincibility"
   - Điều chỉnh `Default Invincibility Duration` (mặc định: 5 giây)

3. (Optional) Tạo Shield Visual Effect:
   - Tạo child GameObject cho Player (ví dụ: "ShieldEffect")
   - Add visual (Sphere với transparent material, particle effect, etc.)
   - Kéo GameObject này vào field `Shield Visual Effect` trong PlayerInvincibility
   - Visual này sẽ tự động hiện/ẩn khi invincibility active/inactive

### Bước 2: Tạo Shield Power-Up Prefab

1. Tạo GameObject mới trong scene (tên: "ShieldPowerUp")

2. Add 3D model hoặc visual cho shield:
   - Có thể dùng Sphere, Cube, hoặc import model
   - Thêm material màu sáng (vàng, xanh dương) để dễ nhận biết
   - Scale phù hợp (ví dụ: 0.5, 0.5, 0.5)

3. Add Collider:
   - Add component "Box Collider" hoặc "Sphere Collider"
   - ✅ CHECK "Is Trigger"

4. Add Script:
   - Add component "ShieldPowerUp"
   - Set `Invincibility Duration` (thời gian bất tử khi nhặt)
   - (Optional) Add pickup effect và sound

5. Add Tag:
   - Đảm bảo Player GameObject có tag "Player"

6. (Optional) Add rotation animation:
   ```csharp
   // Thêm script đơn giản để quay shield
   void Update() {
       transform.Rotate(0, 50 * Time.deltaTime, 0);
   }
   ```

7. Tạo Prefab:
   - Kéo GameObject vào folder Assets/Prefabs/
   - Xóa instance trong scene

### Bước 3: Spawn Shield Trong Game

Có 2 cách spawn shield:

#### Cách 1: Đặt sẵn trong scene
- Kéo ShieldPowerUp prefab vào scene ở vị trí mong muốn
- Shield sẽ xuất hiện ngay khi game start

#### Cách 2: Spawn động (Recommended)
Tạo script spawner:

```csharp
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private Transform[] spawnPoints;
    
    private float timer;
    
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval)
        {
            SpawnShield();
            timer = 0f;
        }
    }
    
    void SpawnShield()
    {
        if (spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(shieldPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
        }
    }
}
```

### Bước 4: Kiểm Tra Setup

1. Player phải có:
   - ✅ PlayerHealth
   - ✅ PlayerDamageHandler
   - ✅ PlayerInvincibility
   - ✅ Tag "Player"

2. Shield Power-Up phải có:
   - ✅ ShieldPowerUp script
   - ✅ Collider với Is Trigger = true
   - ✅ Visual/Model

3. Test trong game:
   - Chạy game
   - Di chuyển player đến shield
   - Shield biến mất khi nhặt
   - Player không nhận damage trong thời gian invincible
   - Console log hiển thị "[PlayerInvincibility] Invincibility activated"

## Tùy Chỉnh

### Thay đổi thời gian bất tử
- Trong PlayerInvincibility: `Default Invincibility Duration`
- Trong ShieldPowerUp: `Invincibility Duration` (override cho từng shield)

### Thêm visual effect
- Gán GameObject vào `Shield Visual Effect` trong PlayerInvincibility
- Effect sẽ tự động hiện khi invincible

### Thêm âm thanh
- Gán AudioClip vào `Pickup Sound` trong ShieldPowerUp

### Thêm particle effect khi nhặt
- Tạo particle effect prefab
- Gán vào `Pickup Effect` trong ShieldPowerUp

## Events (Nâng Cao)

PlayerInvincibility cung cấp events để tích hợp với UI hoặc logic khác:

```csharp
// Subscribe to events
PlayerInvincibility invincibility = GetComponent<PlayerInvincibility>();

invincibility.OnInvincibilityStarted += () => {
    Debug.Log("Shield activated!");
    // Update UI, play sound, etc.
};

invincibility.OnInvincibilityEnded += () => {
    Debug.Log("Shield expired!");
    // Update UI
};

// Lấy thời gian còn lại
float remaining = invincibility.GetRemainingTime();
```

## Troubleshooting

### Shield không biến mất khi nhặt
- Kiểm tra Player có tag "Player"
- Kiểm tra Collider có Is Trigger = true
- Xem Console log có warning/error

### Player vẫn nhận damage khi invincible
- Kiểm tra PlayerInvincibility đã được add vào Player
- Kiểm tra PlayerDamageHandler đã được cập nhật
- Xem Console log có message "Player is invincible!"

### Shield visual không hiện
- Kiểm tra đã gán GameObject vào Shield Visual Effect
- Kiểm tra GameObject có active trong hierarchy

## Mở Rộng

Có thể thêm các tính năng:
- Shield với thời gian khác nhau (bronze/silver/gold)
- Shield stack (nhiều shield = thời gian dài hơn)
- Shield bar UI hiển thị thời gian còn lại
- Particle trail khi player invincible
- Sound effect khi shield sắp hết

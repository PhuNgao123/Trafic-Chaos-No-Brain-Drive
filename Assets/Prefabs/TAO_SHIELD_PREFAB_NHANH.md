# Tạo Shield Prefab Nhanh - Không Cần Ảnh

## Cách 1: Dùng 3D Object Có Sẵn (NHANH NHẤT - 2 PHÚT)

### Bước 1: Tạo Shield GameObject
1. **Hierarchy** → Click chuột phải → **3D Object → Sphere**
2. Đổi tên: **"ShieldPowerUp"**

### Bước 2: Điều Chỉnh
1. **Transform**:
   - Scale: **0.5, 0.5, 0.5**
   - Position: **0, 1, 0** (để test)

2. **Material** (làm đẹp):
   - Chọn ShieldPowerUp
   - Trong **Inspector** → **Materials** → Click vào **Default-Material**
   - Thay đổi **Albedo Color** → Chọn màu sáng (vàng, xanh dương, cyan)
   - Hoặc tạo Material mới trong **Assets/Materials** rồi kéo vào

### Bước 3: Setup Collider
1. ShieldPowerUp đã có **Sphere Collider** sẵn
2. Trong Inspector, tìm **Sphere Collider**:
   - ✅ **CHECK "Is Trigger"**
   - Radius: **0.5** (giữ mặc định)

### Bước 4: Add Script
1. Click **Add Component**
2. Gõ **"ShieldPowerUp"** → Enter
3. Để mặc định:
   - Invincibility Duration: **5**

### Bước 5: Add Animation (Optional - 30 giây)
1. Click **Add Component** → gõ **"RotateObject"** → Enter
2. Click **Add Component** → gõ **"BobbingEffect"** → Enter

### Bước 6: Lưu Prefab
1. Kéo **ShieldPowerUp** từ **Hierarchy** vào folder **Assets/Prefabs**
2. Xong! GameObject chuyển màu xanh

---

## Cách 2: Dùng Cube (Giống Minecraft)

Làm giống Cách 1 nhưng:
- Bước 1: Chọn **3D Object → Cube** thay vì Sphere
- Bước 3: Dùng **Box Collider** (đã có sẵn)
- Bước 5: Add **RotateObject** với Rotation Axis = **(1, 1, 0)** để quay chéo

---

## Cách 3: Dùng Capsule (Hình Viên Thuốc)

Làm giống Cách 1 nhưng:
- Bước 1: Chọn **3D Object → Capsule**
- Bước 2: Scale = **(0.3, 0.3, 0.3)**
- Bước 3: Dùng **Capsule Collider**

---

## Cách 4: Dùng Nhiều Objects (Shield Đẹp Hơn)

### Tạo Shield Phức Tạp
1. **Hierarchy** → Create Empty → Tên: **"ShieldPowerUp"**

2. Tạo phần giữa:
   - Click chuột phải ShieldPowerUp → **3D Object → Sphere**
   - Đổi tên: **"Core"**
   - Scale: **0.3, 0.3, 0.3**
   - Màu: Vàng sáng

3. Tạo vòng ngoài:
   - Click chuột phải ShieldPowerUp → **3D Object → Torus** (nếu có)
   - Hoặc dùng **Cylinder** với Scale: **(0.6, 0.05, 0.6)**
   - Đổi tên: **"Ring"**
   - Màu: Xanh dương

4. Add vào parent (ShieldPowerUp):
   - **Sphere Collider** → Is Trigger = ✅
   - **ShieldPowerUp** script
   - **RotateObject** script (quay cả group)

5. Lưu prefab

---

## Setup Nhanh Trong Scene

### Sau Khi Có Prefab:

**Test ngay:**
1. Kéo **ShieldPowerUp prefab** vào Scene
2. Đặt ở vị trí player đi qua
3. Click **Play** → Test

**Spawn tự động:**
1. **Hierarchy** → Create Empty → Tên: **"ShieldSpawner"**
2. Add component **"ShieldSpawner"**
3. Kéo **ShieldPowerUp prefab** vào field **Shield Prefab**
4. Tạo 3-5 empty GameObjects làm spawn points:
   - Create Empty → Tên: "SpawnPoint1", "SpawnPoint2"...
   - Đặt ở các vị trí khác nhau
5. Kéo các spawn points vào **Spawner → Spawn Points** array

---

## Màu Đề Xuất Cho Shield

### Màu Nổi Bật:
- **Vàng**: RGB(255, 220, 0) - Dễ thấy nhất
- **Cyan**: RGB(0, 255, 255) - Sci-fi
- **Xanh lá sáng**: RGB(0, 255, 100) - Health/Protection
- **Tím**: RGB(200, 0, 255) - Power-up
- **Cam**: RGB(255, 150, 0) - Warm

### Làm Phát Sáng:
1. Chọn Material của shield
2. Scroll xuống → Tìm **Emission**
3. ✅ Check **Emission**
4. Click màu → Chọn cùng màu với Albedo
5. Tăng **Intensity** lên 1-2

---

## Checklist Tạo Prefab (1 Phút)

- [ ] Tạo 3D Object (Sphere/Cube/Capsule)
- [ ] Scale = 0.5, 0.5, 0.5
- [ ] Collider → Is Trigger = ✅
- [ ] Add script "ShieldPowerUp"
- [ ] (Optional) Add "RotateObject"
- [ ] (Optional) Add "BobbingEffect"
- [ ] Kéo vào folder Prefabs
- [ ] Xong!

---

## Nếu Muốn Dùng Ảnh Sau

Khi có ảnh shield:
1. Kéo ảnh vào Unity
2. Tạo **Quad** (3D Object → Quad)
3. Tạo Material → Gán ảnh vào Texture
4. Kéo Material vào Quad
5. Setup như trên (Collider + Scripts)

---

## Kết Quả

Shield prefab đơn giản nhưng hoạt động tốt:
- ✅ Có visual (Sphere/Cube màu sáng)
- ✅ Có collider trigger
- ✅ Có script hoạt động
- ✅ Có animation quay/lên xuống
- ✅ Sẵn sàng spawn trong game

**Thời gian**: 2-5 phút
**Không cần**: Ảnh, model phức tạp, asset store

Đơn giản mà hiệu quả! 🛡️

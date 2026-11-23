# BÁO CÁO ĐẶC TẢ THIẾT KẾ SCHEMA HỆ THỐNG ĐỐI TƯỢNG (OBJECT SCHEMA SPECIFICATION)
## DỰ ÁN: THE CRADLE OF LIGHT (2D METROIDVANIA PUZZLE ADVENTURE)
**Đơn vị thực hiện:** Team 3009 – DUONG GIA HUY (23127052) & NGUYEN HIEU THUAN (23127125)  
**Tài liệu tham chiếu:** Game Design Document (GDD v0.2) + Cập nhật hệ thống Đại Bác Dịch Chuyển  
**Phạm vi trọng tâm:** Vertical Slice – Vùng Gió (P0) & Khung kiến trúc mở rộng Vùng Lửa/Rừng (P1)  
**Engine:** Unity 2D (URP) | **Ngôn ngữ:** C# (.NET Standard)  
**Phiên bản tài liệu Schema:** v2.0 — Đồng bộ hoàn toàn & Rà soát toàn vẹn

---

# MỤC LỤC
1. [TỔNG QUAN KIẾN TRÚC & PHÂN BỔ CẤU TRÚC FOLDER](#1-tổng-quan-kiến-trúc--phân-bố-cấu-trúc-folder)
   - 1.1 Nguyên lý thiết kế kiến trúc
   - 1.2 Bản đồ thư mục & phân bổ file C# chi tiết trong `GamePJ/Assets`
2. [BẢNG SCHEMA ĐẶC TẢ CHI TIẾT TỪNG COMPONENT](#2-bảng-schema-đặc-tả-chi-tiết-từng-component)
   - 2.1 Nhóm Quản trị Cốt lõi, Lưu trữ, Mạng Đại Bác & Phòng (Core Framework)
   - 2.2 Nhóm Tinh Linh Ánh Sáng (Player Architecture - SRP)
   - 2.3 Nhóm Đạn đạo, Quả cầu chờ & Vật phẩm rơi (Projectiles & Pickups)
   - 2.4 Nhóm Interfaces & Trừu tượng hóa (Interfaces & Abstractions)
   - 2.5 Nhóm Môi trường & Cơ quan Giải đố (Environment & Puzzles)
   - 2.6 Nhóm Tương tác Thế giới, Đại Bác Dịch Chuyển & Cạm bẫy (Interactables & Hazards)
   - 2.7 Nhóm Kẻ địch Bóng tối & Trùm (Enemy AI & Boss Hierarchy)
   - 2.8 Nhóm NPCs, Kinh tế Lò rèn & Thư viện Lore (NPCs, Economy & Lore)
   - 2.9 Nhóm Giao diện Người dùng (User Interface - UI Systems)
3. [CHIẾN LƯỢC MỞ RỘNG VÙNG LỬA & VÙNG RỪNG (SCALABILITY BLUEPRINT)](#3-chiến-lược-mở-rộng-vùng-lửa--vùng-rừng-scalability-blueprint)
4. [MÃ NGUỒN MERMAID CLASS DIAGRAM TOÀN DIỆN CHO DRAW.IO](#4-mã-nguồn-mermaid-class-diagram-toàn-diện-cho-drawio)
5. [LỘ TRÌNH TRIỂN KHAI & DANH MỤC KIỂM THỬ KỸ THUẬT](#5-lộ-trình-triển-khai--danh-mục-kiểm-thử-kỹ-thuật)

---

# 1. TỔNG QUAN KIẾN TRÚC & PHÂN BỔ CẤU TRÚC FOLDER

## 1.1 Nguyên lý thiết kế kiến trúc
1. **Kiến trúc phân rã trách nhiệm đơn lẻ (Single Responsibility Principle - SRP):**
   - Loại bỏ mô hình "God Class" trong Unity. Player được phân rã thành `PlayerController` (điều phối Input) cùng 6 module chuyên biệt: `PlayerMovement`, `PlayerCombat`, `PlayerAbilities`, `PlayerStats`, `PlayerInteraction`, và `PlayerLightAura`.
2. **Kiến trúc hướng dữ liệu & Mở rộng không giới hạn (Data-Driven & Open/Closed Principle):**
   - Các dữ liệu cấu hình đạn (`ProjectileDataSO`), nâng cấp lò rèn (`UpgradeItemSO`), mảnh ký ức (`MemoryFragmentSO`), trạm Đại Bác (`CannonStationSO`), và âm thanh (`SoundData`) được tách thành `ScriptableObject`.
   - Không gán cứng bất kỳ biến cụ thể nào theo từng NPC, kỹ năng hay vùng đất trong hệ thống lưu trữ (`GameSaveData` và `WorldState` dùng danh sách/tập hợp ID tổng quát: `unlockedAbilityIDs`, `rescuedNPCIDs`, `purifiedAreaIDs`, `unlockedCannonIDs`).
   - Tách rõ **Runtime State (`WorldState` dùng `HashSet<T>` tra cứu $O(1)$)** và **Serialization DTO (`GameSaveData` dùng `List<T>` tương thích 100% với `UnityEngine.JsonUtility`)**.
3. **Mô phỏng quang học an toàn (Safe Optical Raycasting with CircleCast):**
   - Đạn ánh sáng bay với vận tốc $16.0\text{ units/s}$ sử dụng quét `Physics2D.CircleCast` liên tục theo từng frame (bán kính $0.1\text{ units}$) để triệt tiêu lỗi xuyên vật cản (tunneling).
   - Giới hạn cứng tối đa 3 lần phản xạ qua gương ($\vec{r} = \vec{d} - 2(\vec{d}\cdot\vec{n})\vec{n}$); ở lần va chạm thứ 4 tự động kích hoạt nổ diện rộng $1.5\text{ units}$ và biến mất.
4. **Cơ chế liên thông Metroidvania & Đại Bác Dịch Chuyển (Diegetic Fast Travel):**
   - Toàn bộ Cửa Phong Ấn (`SealGate`) mở vĩnh viễn 2 chiều phục vụ Backtracking.
   - Hệ thống **Đại Bác Dịch Chuyển (`TravelCannon`)** thay thế cho Trục Gió Liên Thông, cung cấp cơ chế bay nhanh giữa các điểm trọng yếu (Base Hub $\leftrightarrow$ Tiền phòng Boss LVL-03, và mở rộng cho Vùng Lửa/Rừng) thông qua chuỗi hoạt ảnh cách điệu: xoay nòng $\rightarrow$ hút tinh linh $\rightarrow$ tụ lực $\rightarrow$ bắn khỏi màn hình $\rightarrow$ chuyển cảnh $\rightarrow$ đáp từ trên trời xuống.

---

## 1.2 Bản đồ thư mục & phân bổ file C# chi tiết trong `GamePJ/Assets`

> *Lưu ý:* Thư mục `Art/Enviroment/` giữ nguyên tên theo cấu trúc vật lý hiện có của project `GamePJ`. Mọi tệp `.cs` thuộc `MonoBehaviour` được đặt tên trùng khớp 100% với tên Class để tránh lỗi nhận diện của Unity.

```text
GamePJ/Assets/Project/
├── Animations/
│   ├── Enemies/                     # Animator Controllers & Clips cho Crawler, Slime, Weaver, Spitter, Bosses
│   ├── Environment/                 # Animator Controllers cho TravelCannon, SealGate, RadiantBloom
│   └── Player/                      # Animator Controllers & Clips (Idle, Move, Jump, Fall, Launch, Dash, Stun)
├── Art/
│   ├── Enviroment/                  # Spritesheets Tilemap, Gương, Hoa Soi Sáng, Bệ Thờ, Đại Bác, Cửa Phong Ấn
│   └── Player/                      # Player.aseprite & Sprite assets
├── Audio/                           # Audio Clips (BGM suy tàn/hồi sinh, SFX pha lê, đại bác, hoa nở...)
├── Prefabs/
│   ├── Characters/                  # Player, BoranNPC, SacredTreeNPC, EventNPC
│   ├── Enemies/                     # ShadowCrawler, ShadowSlime, ShadowWeaver, ShadowSpitter, TutorialDarkCore, WindCoreBoss
│   ├── Environment/                 # ReflectionMirror, RemotePedestal, RadiantBloom, EnergyPylon, SealGate,
│   │                                # LightPressurePad, WindZone, CheckpointAltar, TravelCannon
│   ├── Hazards/                     # DarkSpikes, DarkAbyss, ShadowCage, LoreStele
│   ├── Projectiles/                 # LightProjectile, FloatingLightOrb, EnemyProjectile, LightShardPickup
│   └── UI/                          # HUDCanvas, MapCanvas, ForgeShopCanvas, CannonSelectCanvas, DialogueCanvas, PauseMenuCanvas
├── Scenes/
│   ├── Prototypes/                  # Cảnh thử nghiệm cơ học
│   ├── LVL01_AwakeningHollow.unity  # Tutorial 5 phòng tuyến tính
│   ├── AREA02_TheCradle_Hub.unity   # Hub Cổ Thụ Ánh Sáng (đặt Khẩu Đại Bác Base)
│   ├── AREA03_WindsThreshold.unity  # Trạm Nghỉ Đầu Gió (mở khóa Launch & Lò rèn Boran)
│   ├── LVL02_WhisperingCaverns.unity# Hẻm Núi Thì Thầm (giải cứu Boran, Mảnh Ký Ức #01)
│   └── LVL03_EyeOfTheTempest.unity  # Đỉnh Gió Gầm (Khẩu Đại Bác tiền phòng Boss, Mảnh #02, Boss Lõi Gió)
├── ScriptableObjects/
│   ├── Upgrades/                    # UpgradeItemSO: ManaTank_I/II/III, FastRecovery, QuickCharge, LightAura, EmpoweredLight, RadiantBurst
│   ├── Lore/                        # MemoryFragmentSO: Fragment_01, Fragment_02... (12 mảnh tổng thể)
│   ├── Projectiles/                 # ProjectileDataSO: BaseLightBullet, FlameBullet, FloraBullet
│   ├── Cannons/                     # CannonStationSO: Cannon_Hub_Base, Cannon_LVL03_BossPrep (mở rộng Fire/Forest)
│   └── Audio/                       # SoundData: BGM_Ruined, BGM_Purified, SFX_CrystalPing, SFX_CannonFire...
└── Scripts/
    ├── Core/
    │   ├── GameManager.cs           # State machine vòng đời game, Bullet Time, chuyển cảnh
    │   ├── GameState.cs             # Enum: Boot, MainMenu, Gameplay, BulletTime, Paused, Respawning, Cutscene, GameOver, Victory
    │   ├── CannonNetworkManager.cs  # Quản lý mạng lưới Đại Bác xuyên Scene & chuỗi dịch chuyển
    │   ├── RoomController.cs        # Quản lý từng phòng: hint 3 cấp (30/60/90s), clear VFX, Event NPC exit
    │   ├── SaveSystem/
    │   │   ├── SaveManager.cs       # Đọc/ghi JSON cục bộ mã hóa cho 3 Save Slots & Autosave
    │   │   ├── GameSaveData.cs      # DTO thuần List/Primitive tương thích JsonUtility
    │   │   └── WorldState.cs        # Bộ quản lý trạng thái thế giới runtime (HashSet O(1))
    │   ├── Audio/
    │   │   ├── AudioManager.cs      # Điều phối BGM chuyển trạng thái, SFX 2D spatial, Low-pass filter
    │   │   └── SoundData.cs         # ScriptableObject chứa clip, volume, looping
    │   └── Camera/
    │       ├── CameraFollow2D.cs    # Camera bám theo Tinh linh, Look-ahead theo chuột/hướng chạy
    │       └── CameraZoomController.cs # Dynamic Zoom khi Bullet Time (+10%) hoặc đấu Boss (-15%)
    ├── Player/
    │   ├── PlayerController.cs      # Bộ điều phối trung tâm kết nối PlayerInput với 6 module
    │   ├── PlayerMovement.cs        # Di chuyển A/D (4.5 u/s), Nhảy Space (12 u/s), Rơi sàn S+Space (0.3s)
    │   ├── PlayerAbilities.cs       # Ground/Air Dash (Ctrl), Launch (Chuột phải), Ability ID System
    │   ├── PlayerCombat.cs          # Ngắm bắn Chuột trái (Tap/Hold), Bullet Time (0.1x), Hủy [X], hồi Mana
    │   ├── PlayerStats.cs           # 3 HP / 3 Lives, Mana (100-200), Light Shards (0-999), i-frames 1.5s
    │   ├── PlayerInteraction.cs     # Quét bán kính 1.8 units, hỗ trợ Tap [F] & Hold [F] (2.0s), White Outline
    │   └── PlayerLightAura.cs       # Quản lý URP 2D Light (4.0 - 6.0 units), phản hồi mất máu diegetic
    ├── Projectiles/
    │   ├── ProjectileDataSO.cs      # ScriptableObject cấu hình đạn ánh sáng (Base/Flame/Flora)
    │   ├── LightProjectile.cs       # Đạn ánh sáng: CircleCast, nảy max 3 lần, nổ AOE lần 4, chịu lực gió
    │   ├── FloatingLightOrb.cs      # Quả cầu chờ khi Tap bắn (<0.25s), tồn tại 8.0s, click vào để ngắm bắn
    │   ├── EnemyProjectile.cs       # Đạn gai đen của Slime/Spitter (triệt tiêu khi đụng LightProjectile)
    │   └── LightShardPickup.cs      # Mảnh Sáng rơi ra trên map, tự động hút vào Player trong bán kính 2.5 units
    ├── Environment/
    │   ├── Interfaces/
    │   │   ├── IInteractable.cs     # Interface cho mọi vật thể bấm/giữ [F] (Prompt, HoldDuration, Highlight)
    │   │   ├── IDamageable.cs       # Interface cho mọi vật thể nhận sát thương đạn ánh sáng
    │   │   └── IActivatable.cs      # Interface cho cơ quan kích hoạt (SealGate, TravelCannon...)
    │   ├── Puzzles/
    │   │   ├── ReflectionMirror.cs  # Gương: Cố định 45°, Xoay bằng bệ [F], hoặc Tự động xoay liên tục
    │   │   ├── MirrorType.cs        # Enum: Fixed_45Deg, Steerable_Pedestal, AutoRotating_Clockwise
    │   │   ├── RemotePedestal.cs    # Bệ cần xoay trên sàn an toàn, bấm [F] xoay gương 45° qua cáp sáng
    │   │   ├── RadiantBloom.cs      # Hoa Soi Sáng: sáng 8.0s (5s cực đại), héo úa và hồi lại sau 60.0s
    │   │   ├── BloomState.cs        # Enum: Ready, FlareActive, Withered
    │   │   ├── EnergyPylon.cs       # Lõi năng lượng nhận đạn (vĩnh viễn/hẹn giờ 5s) kích hoạt IActivatable
    │   │   ├── SealGate.cs          # Cửa đá phong ấn 2 chiều mở vĩnh viễn, kích hoạt Autosave
    │   │   ├── LightPressurePad.cs  # Bệ Đá Ánh Sáng (LVL-01 Phòng 1): chạm thân Tinh linh mở cửa
    │   │   ├── WindZone.cs          # Luồng gió đa năng: gió ngang (+30%/-50%), gió nâng, gió giật chu kỳ
    │   │   └── WindZoneType.cs      # Enum: Gust_Horizontal, Updraft_Vertical
    │   ├── Interactables/
    │   │   ├── CheckpointAltar.cs   # Bệ thờ: lướt qua hồi 100% HP/Mana + Save; kẹt >90s hiện [F] Gợi ý
    │   │   ├── TravelCannon.cs      # Khẩu Đại Bác Dịch Chuyển: [F] Use Cannon / "Tạm chưa mở khóa", hoạt ảnh bay
    │   │   ├── CannonStationSO.cs   # ScriptableObject định danh trạm Đại Bác xuyên Scene
    │   │   ├── CannonState.cs       # Enum: Locked, Unlocked_Active, Launching, Receiving
    │   │   ├── ShadowCage.cs        # Lồng giam đá kẹt NPCBase: bắn đạn hoặc giữ [F] 2.0s để phá vỡ
    │   │   └── LoreStele.cs         # Bia Ký Ức: bấm [F] đọc lore, nạp vào CodexManager
    │   └── Hazards/
    │       ├── DarkSpikes.cs        # Bãi gai đen: trừ 1 HP, knockback 0.2s, tái sinh mỏm an toàn + 1.5s i-frame
    │       └── DarkAbyss.cs         # Hố sâu: tái sinh mỏm an toàn, 0 mất HP (Campaign) / -1 Mạng (Survival)
    ├── Enemies/
    │   ├── Base/
    │   │   ├── EnemyBase.cs         # Lớp cơ sở: Máu, Khiên sương đen, Stun 1.5s, Rơi 1-3 LightShardPickup
    │   │   └── BossBase.cs          # Lớp cơ sở Boss: kế thừa máu từ EnemyBase, quản lý BossPhase, Cutscene
    │   ├── AI/
    │   │   ├── ShadowCrawler.cs     # Tuần tra mặt đất (2.0 u/s), phát hiện 4.0 u, húc cận chiến, có khiên
    │   │   ├── ShadowSlime.cs       # Tuần tra đánh xa (1.2 u/s), nén 0.5s telegraph, bắn gai 6.0 u/s, có khiên
    │   │   ├── ShadowWeaver.cs      # Nhện trần nhà phục kích thả rơi theo trục Y, bắn tơ, không khiên
    │   │   └── ShadowSpitter.cs     # Pháo đài vách đá cố định, bắn gai 8.0 u/s chu kỳ 2.5s, không khiên
    │   └── Bosses/
    │       ├── BossPhase.cs         # Enum: Phase1_Shockwaves, Phase2_VortexStorm, Vulnerable_Exposed
    │       ├── TutorialDarkCore.cs  # Lõi Bóng Tối sơ khai (LVL-01 Phòng 5): 3 HP, sóng xung kích 3s
    │       └── WindCoreBoss.cs      # Lõi Gió Cổ Đại (LVL-03 Boss): 6 HP, bão xoáy, rơi Tinh Hoa Gió (Air Dash)
    ├── NPC/
    │   ├── Base/
    │   │   ├── NPCBase.cs           # Lớp cơ sở: npcID, state, isCoreFunctionalNPC, virtual OnRescued()
    │   │   └── NPCState.cs          # Enum: Trapped, Rescued, Missed
    │   ├── BoranNPC.cs              # NPC Chức năng: cứu tại LVL-02 -> về AREA-03 mở Lò rèn vĩnh viễn
    │   ├── SacredTreeNPC.cs         # Cổ Thụ Ánh Sáng tại Hub: dẫn dắt cốt truyện & đánh giá Phồn Vinh
    │   └── EventNPC.cs              # NPC Sự kiện (Phòng 4 LVL-01 & cư dân thường): rời phòng sẽ chuyển Missed
    ├── Upgrades/
    │   ├── UpgradeItemSO.cs         # ScriptableObject dữ liệu gói nâng cấp Lò rèn
    │   ├── UpgradeType.cs           # Enum: MaxManaTank, FastRecovery, QuickChargeDelay, LightAuraRadius, EmpoweredPierce, RadiantBurstAOE
    │   ├── UpgradeManager.cs        # Xử lý giao dịch LS & áp dụng hiệu ứng lên PlayerStats/Combat/Aura
    │   ├── MemoryFragmentSO.cs      # ScriptableObject dữ liệu Mảnh Ký Ức thế giới (12 mảnh)
    │   └── CodexManager.cs          # Quản lý danh sách Mảnh Ký Ức đã mở khóa
    └── UI/
        ├── HUDController.cs         # 3 HP/Lives Orbs, Mana Bar vạch 20, LS Counter, Icon gió, Autosave icon
        ├── MapUI.cs                 # Room Grid Map (Tab/M): phòng đã khám phá, Checkpoint, Cannon, NPC
        ├── ForgeShopUI.cs           # Giao diện mua nâng cấp tại Lò rèn Boran
        ├── CannonSelectUI.cs        # Bảng chọn điểm bay Đại Bác (danh sách khẩu đã mở + nút Exit)
        ├── DialogueUI.cs            # Khung thoại NPC, Bia Ký Ức, Gợi ý Checkpoint & thông báo khóa Đại Bác
        ├── PauseMenuUI.cs           # Menu tạm dừng (Esc): Resume, Controls & Hints, Settings, Return to Title
        └── MainMenuUI.cs            # Continue, New Game (Slot 1-3, Mode), Settings (Assist Mode), Codex, Quit
```

---

# 2. BẢNG SCHEMA ĐẶC TẢ CHI TIẾT TỪNG COMPONENT

Mỗi component dưới đây được đặc tả đầy đủ 3 phần: **Đặc tính (Characteristics / Fields)**, **Hành vi (Actions / Methods)** và **Chú thích liên kết (Relationships & GDD Rules)**, đồng nhất 100% với sơ đồ Mermaid ở Phần 4.

---

## 2.1 Nhóm Quản trị Cốt lõi, Lưu trữ, Mạng Đại Bác & Phòng (Core Framework)

### 2.1.1 `GameManager` & `GameState`
* **Vị trí file:** `Assets/Project/Scripts/Core/GameManager.cs` & `GameState.cs`
* **Loại:** `MonoBehaviour` (Singleton, `DontDestroyOnLoad`) & `Enum`
* **Ý nghĩa:** Điều phối vòng đời game, máy trạng thái toàn cục, Bullet Time, chuyển cảnh và sinh tử.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `GameState` | `enum` | `Boot` | `Boot`, `MainMenu`, `Gameplay`, `BulletTime`, `Paused`, `Respawning`, `Cutscene`, `GameOver`, `Victory`. |
| **Characteristic** | `currentState` | `GameState` | `GameState.Boot` | Trạng thái hiện hành của toàn bộ game. |
| **Characteristic** | `saveManager` | `SaveManager` | Serialized | Tham chiếu hệ thống lưu trữ JSON. |
| **Characteristic** | `audioManager` | `AudioManager` | Serialized | Tham chiếu hệ thống âm thanh. |
| **Characteristic** | `cannonNetwork` | `CannonNetworkManager` | Serialized | Tham chiếu mạng lưới Đại Bác dịch chuyển xuyên Scene. |
| **Characteristic** | `zoomController` | `CameraZoomController` | Serialized | Tham chiếu điều khiển Dynamic Zoom camera. |
| **Action** | `ChangeState` | `void (GameState newState)` | N/A | Chuyển trạng thái, đóng băng vật lý (`timeScale = 0`) khi `Paused`, ẩn HUD khi `Cutscene`. |
| **Action** | `SetBulletTime` | `void (bool active)` | N/A | Đặt `Time.timeScale` = `0.1f` (hoặc `1.0f`), kích hoạt `AudioManager.SetLowPassFilter` và `CameraZoomController.ZoomBulletTime`. |
| **Action** | `LoadSceneAsync` | `IEnumerator (string sceneName, Action onLoaded)` | N/A | Thực hiện Fade chuyển cảnh và nạp Scene mới (dùng cho Cửa chuyển màn, Đại Bác và Respawn). |
| **Action** | `TriggerGameOver` | `void ()` | N/A | (Survival Mode) Ghi đè tọa độ hồi sinh về Hub, gọi Autosave đồng bộ khẩn cấp chống Alt+F4 trước khi hiện Game Over. |
| **Action** | `TriggerVictory` | `void ()` | N/A | Khởi chạy Ending Cutscene và bảng tổng kết chỉ số Phồn Vinh. |

* **Chú thích liên kết:**
  - Cấu thành (`*--`) `SaveManager`, `AudioManager`, `CannonNetworkManager`, `CameraZoomController`.
  - Lắng nghe `PlayerCombat` (bật/tắt Bullet Time), `PlayerStats.OnPlayerDied` (xử lý Respawn Campaign hoặc Game Over Survival), và `PauseMenuUI` (phím `Esc`).

---

### 2.1.2 `SaveManager`, `GameSaveData` & `WorldState`
* **Vị trí file:** `Assets/Project/Scripts/Core/SaveSystem/SaveManager.cs`, `GameSaveData.cs`, `WorldState.cs`
* **Loại:** `MonoBehaviour` (`SaveManager`), `[System.Serializable] class` (`GameSaveData`), `class` (`WorldState`)
* **Ý nghĩa:** Quản lý 3 Save Slots độc lập (JSON mã hóa cục bộ). Tách biệt **`WorldState`** (dùng `HashSet` trong RAM để tra cứu nhanh $O(1)$) và **`GameSaveData`** (dùng `List` để `UnityEngine.JsonUtility` tuần tự hóa an toàn).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `SaveManager.currentData` | `GameSaveData` | New instance | Bộ nhớ đệm dữ liệu lưu trữ hiện tại. |
| **Characteristic** | `SaveManager.worldState` | `WorldState` | New instance | Trạng thái thế giới đang vận hành trong RAM. |
| **Characteristic** | `SaveManager.savePath` | `string` | `Application.persistentDataPath` | Đường dẫn thư mục lưu `save_slot_{i}.json`. |
| **Characteristic** | `GameSaveData.saveSlotIndex` | `int` | `1` | Chỉ số Save Slot (1, 2, hoặc 3). |
| **Characteristic** | `GameSaveData.gameMode` | `string` | `"Campaign"` | Chế độ chơi: `"Campaign"` (3 HP vô hạn lần thử) hoặc `"Survival"` (3 Mạng sống). |
| **Characteristic** | `GameSaveData.usedAssistMode`| `bool` | `false` | Đánh dấu đã bật Assist Mode (khóa vĩnh viễn Achievements cho slot này theo GDD 4.12). |
| **Characteristic** | `GameSaveData.currentSceneName`| `string` | `"LVL01_AwakeningHollow"` | Tên Scene lưu lần cuối. |
| **Characteristic** | `GameSaveData.lastCheckpointPosition`| `Vector3` | `Vector3.zero` | Tọa độ Bệ thờ Checkpoint gần nhất. |
| **Characteristic** | `GameSaveData.currentHealth` | `int` | `3` | Sinh mệnh hiện tại. |
| **Characteristic** | `GameSaveData.currentMaxMana` | `float` | `100.0f` | Dung lượng Mana tối đa hiện tại. |
| **Characteristic** | `GameSaveData.lightShardsCount`| `int` | `0` | Số Mảnh Sáng (LS) đang sở hữu (0–999). |
| **Characteristic** | `GameSaveData.purchasedUpgradeIDs`| `List<string>` | Empty List | Danh sách mã nâng cấp đã mua tại Lò rèn Boran. |
| **Characteristic** | `GameSaveData.unlockedAbilityIDs`| `List<string>` | Empty List | Danh sách mã kỹ năng mở khóa (`"Ability_Launch"`, `"Ability_AirDash"`, `"Ability_ZephyrResistance"`...). |
| **Characteristic** | `GameSaveData.openedGateIDs` | `List<string>` | Empty List | Danh sách mã Cửa Phong Ấn 2 chiều đã mở vĩnh viễn. |
| **Characteristic** | `GameSaveData.unlockedCannonIDs`| `List<string>` | Empty List | Danh sách mã các Khẩu Đại Bác đã mở khóa (`"CANNON_Hub_Base"`, `"CANNON_LVL03_BossPrep"`...). |
| **Characteristic** | `GameSaveData.purifiedAreaIDs`| `List<string>` | Empty List | Danh sách mã phân khu đã thanh tẩy (`"AREA_Wind"`, tương lai: `"AREA_Fire"`, `"AREA_Forest"`). |
| **Characteristic** | `GameSaveData.rescuedNPCIDs` | `List<string>` | Empty List | Danh sách mã NPC đã giải cứu (`"NPC_Boran"`, `"NPC_Tut_Room4"`, tương lai: `"NPC_Ignis"`, `"NPC_Lyra"`). |
| **Characteristic** | `GameSaveData.missedNPCIDs` | `List<string>` | Empty List | Danh sách mã NPC Sự kiện đã bỏ lỡ trong lượt chơi hiện tại. |
| **Characteristic** | `GameSaveData.collectedMemoryFragmentIDs`| `List<int>` | Empty List | Danh sách ID Mảnh Ký Ức đã nhặt (1..12). |
| **Characteristic** | `WorldState.openedGates` | `HashSet<string>` | Empty Set | Bản sao runtime tra cứu $O(1)$ các cửa đã mở. |
| **Characteristic** | `WorldState.unlockedCannons` | `HashSet<string>` | Empty Set | Bản sao runtime các khẩu Đại Bác đã mở khóa. |
| **Characteristic** | `WorldState.purifiedAreas` | `HashSet<string>` | Empty Set | Bản sao runtime các vùng đất đã thanh tẩy. |
| **Characteristic** | `WorldState.exploredRoomIDs` | `HashSet<string>` | Empty Set | Danh sách các phòng đã đặt chân vào (phục vụ `MapUI` xóa Fog of War). |
| **Action** | `SaveManager.SaveGame` | `void (int slotIndex)` | N/A | Đồng bộ `WorldState` $\rightarrow$ `GameSaveData`, mã hóa JSON và ghi ra đĩa. |
| **Action** | `SaveManager.LoadGame` | `GameSaveData (int slotIndex)` | N/A | Đọc JSON, giải mã và đổ ngược dữ liệu vào `WorldState` và `PlayerStats`. |
| **Action** | `SaveManager.TriggerAutosave` | `void ()` | N/A | Lưu tự động và gọi `HUDController.ShowAutosaveIndicator()` (1.5s). |
| **Action** | `WorldState.RegisterGateOpened`| `void (string gateID)` | N/A | Ghi nhận cửa đã mở vào `openedGates`. |
| **Action** | `WorldState.RegisterCannonUnlocked`| `void (string cannonID)` | N/A | Ghi nhận khẩu Đại Bác đã mở vào `unlockedCannons`. |
| **Action** | `WorldState.RegisterAreaPurified`| `void (string areaID)` | N/A | Đánh dấu vùng nguyên tố đã thanh tẩy, kích hoạt đổi BGM & thời tiết. |

* **Chú thích liên kết:**
  - Được kích hoạt Autosave bởi: `CheckpointAltar`, `SealGate`, `TravelCannon` (khi mở khóa/sử dụng), `UpgradeManager` (khi mua đồ), `NPCBase.OnRescued()`, `BossBase.OnBossDefeated()`, và `GameManager.TriggerGameOver()`.

---

### 2.1.3 `CannonNetworkManager`
* **Vị trí file:** `Assets/Project/Scripts/Core/CannonNetworkManager.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Quản lý danh mục toàn bộ các Khẩu Đại Bác trên toàn bản đồ Metroidvania, tự động xử lý luật mở khóa khẩu Base và điều phối chuỗi hoạt ảnh chuyển cảnh bay giữa các Scene.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `allStations` | `List<CannonStationSO>` | Inspector List | Toàn bộ danh sách trạm Đại Bác của game (`Cannon_Hub_Base`, `Cannon_LVL03_BossPrep`...). |
| **Characteristic** | `pendingArrivalCannonID`| `string` | `""` | ID trạm đích đang chờ đón Tinh linh rơi xuống sau khi load xong Scene mới. |
| **Action** | `UnlockCannon` | `void (string cannonID)` | N/A | Đăng ký `cannonID` vào `WorldState.unlockedCannons`. **Luật Base:** Ngay khi có ít nhất 1 khẩu ngoài được mở, tự động mở khóa luôn khẩu có `isHubBaseCannon == true`. Gọi `SaveManager.TriggerAutosave()`. |
| **Action** | `IsCannonUnlocked` | `bool (string cannonID)` | N/A | Kiểm tra khẩu có nằm trong `WorldState.unlockedCannons` hay không. |
| **Action** | `GetAvailableDestinations`| `List<CannonStationSO> (string excludeID)` | N/A | Trả về các trạm `CannonStationSO` đã mở khóa ngoại trừ khẩu đang đứng (`excludeID`). |
| **Action** | `StartCannonTravel` | `void (TravelCannon origin, CannonStationSO dest)` | N/A | Đưa game sang `GameState.Cutscene`, lưu `pendingArrivalCannonID = dest.cannonID`, chạy `origin.PlayLaunchSequence(dest)` $\rightarrow$ gọi `GameManager.LoadSceneAsync(dest.sceneName)` (hoặc dịch chuyển camera nếu cùng Scene) $\rightarrow$ tìm `TravelCannon` đích gọi `PlayLandingSequence()`. |

* **Chú thích liên kết:**
  - Tương tác trực tiếp với `TravelCannon`, `CannonSelectUI`, `WorldState`, `SaveManager`, và `GameManager`.

---

### 2.1.4 `RoomController`
* **Vị trí file:** `Assets/Project/Scripts/Core/RoomController.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Quản lý vòng đời của từng căn phòng/khu vực nhỏ: ghi nhận khám phá bản đồ, chuyển hóa môi trường khi giải đố xong, điều phối hệ thống gợi ý 3 cấp độ (GDD 5.6) và xử lý NPC Sự kiện khi người chơi rời phòng.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `roomID` | `string` | `""` | Mã định danh phòng (đồng bộ với `MapUI` và `WorldState.exploredRoomIDs`). |
| **Characteristic** | `isCleared` | `bool` | `false` | Cờ phòng đã được thanh tẩy / giải đố xong. |
| **Characteristic** | `stuckTimer` | `float` | `0.0f` | Bộ đếm thời gian người chơi ở trong phòng mà chưa giải xong câu đố. |
| **Characteristic** | `missedShotCount` | `int` | `0` | Đếm số lần bắn trượt (đạt 3 lần hoặc 60s sẽ bật Gợi ý Cấp 2). |
| **Characteristic** | `roomCheckpoints` | `CheckpointAltar[]` | Array | Bệ thờ liên kết để bật Gợi ý Cấp 3 khi `stuckTimer >= 90.0f`. |
| **Characteristic** | `roomEventNPCs` | `List<EventNPC>` | List | Các NPC Sự kiện trong phòng (sẽ bị đánh dấu `Missed` nếu người chơi rời phòng mà chưa cứu). |
| **Action** | `OnPlayerEnterRoom` | `void ()` | N/A | Đăng ký `roomID` vào `WorldState.exploredRoomIDs`, bắt đầu đếm `stuckTimer`. |
| **Action** | `OnPlayerExitRoom` | `void ()` | N/A | Nếu `EventNPC` trong phòng vẫn đang `Trapped` $\rightarrow$ chuyển trạng thái thành `Missed` (GDD 3.1). |
| **Action** | `RegisterMissedShot` | `void ()` | N/A | Tăng `missedShotCount`, kiểm tra điều kiện kích hoạt Gợi ý Cấp 2 (tia laser xanh ngọc). |
| **Action** | `EvaluateHintTier` | `void ()` | N/A | 30s: nhấp nháy `EnergyPylon`; 60s hoặc trượt 3 lần: bật định hướng gương & laser xanh ngọc; 90s: gọi `CheckpointAltar.ActivateHintMode()`. |
| **Action** | `ClearRoom` | `void ()` | N/A | Đặt `isCleared = true`, xua tan sương đen phòng, bật thực vật phát quang, thưởng 5 LS (GDD 4.8). |

---

### 2.1.5 `AudioManager` & `SoundData`
* **Vị trí file:** `Assets/Project/Scripts/Core/Audio/AudioManager.cs` & `SoundData.cs`
* **Loại:** `MonoBehaviour` (`AudioManager`) & `ScriptableObject` (`SoundData`)
* **Ý nghĩa:** Quản lý âm thanh không gian 2D (Spatial Panning), chuyển lớp nhạc nền Suy tàn $\rightarrow$ Hồi sinh và Low-pass Filter khi Bullet Time.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `bgmSource` | `AudioSource` | `null` | Kênh phát nhạc nền chính (BGM). |
| **Characteristic** | `sfxSource` | `AudioSource` | `null` | Kênh phát hiệu ứng âm thanh (SFX). |
| **Characteristic** | `lowPassFilter` | `AudioLowPassFilter` | `null` | Bộ lọc làm trầm dải tần cao khi vào Bullet Time. |
| **Characteristic** | `SoundData.soundID` | `string` | `""` | Mã âm thanh (VD: `"SFX_CrystalPing"`, `"SFX_CannonFire"`). |
| **Characteristic** | `SoundData.clip` | `AudioClip` | `null` | Tệp AudioClip gốc. |
| **Characteristic** | `SoundData.volume` | `float` | `1.0f` | Âm lượng chuẩn. |
| **Characteristic** | `SoundData.isLooping` | `bool` | `false` | Cờ lặp liên tục (BGM / Ambient gió). |
| **Action** | `PlayBGM` | `void (SoundData data)` | N/A | Chuyển nhạc nền mượt mà với hiệu ứng Fade-in 2.0s. |
| **Action** | `PlaySFX` | `void (SoundData data)` | N/A | Phát SFX hỗ trợ 2D Spatial Panning theo khoảng cách tới Tinh linh. |
| **Action** | `SetLowPassFilter` | `void (bool active)` | N/A | Giảm 50% âm lượng BGM và lọc dải tần cao trong 0.2s khi bật Bullet Time. |

---

### 2.1.6 `CameraFollow2D` & `CameraZoomController`
* **Vị trí file:** `Assets/Project/Scripts/Core/Camera/CameraFollow2D.cs` & `CameraZoomController.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Camera 2D Orthographic bám theo Tinh linh với Look-ahead, Dynamic Zoom và tùy chọn tắt Screen Shake (Accessibility GDD 9.4).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `targetPlayer` | `Transform` | `null` | Transform của Tinh linh. |
| **Characteristic** | `smoothSpeed` | `float` | `0.125f` | Tốc độ làm mượt Smooth Damping. |
| **Characteristic** | `lookAheadOffset` | `Vector2` | `(1.5f, 0.5f)` | Độ lệch nhìn trước theo hướng chạy hoặc con trỏ chuột. |
| **Characteristic** | `enableScreenShake`| `bool` | `true` | Tùy chọn chống say chuyển động (bật/tắt trong Settings). |
| **Characteristic** | `cam` | `Camera` | `Camera.main` | Camera 2D chính của màn chơi. |
| **Characteristic** | `baseOrthoSize` | `float` | `5.0f` | Kích thước bao quát chuẩn. |
| **Action** | `FollowUpdate` | `void ()` | N/A | Cập nhật tọa độ camera mỗi khung hình `LateUpdate()`. |
| **Action** | `ZoomBulletTime` | `void (bool inBulletTime)`| N/A | Phóng to ~10% (`baseOrthoSize * 0.9f`) khi ngắm bắn hoặc thoại NPC. |
| **Action** | `ZoomBossArena` | `void (bool inArena)` | N/A | Thu phóng lùi ~15% (`baseOrthoSize * 1.15f`) khi đấu Boss. |

---

## 2.2 Nhóm Tinh Linh Ánh Sáng (Player Architecture - SRP)

### 2.2.1 `PlayerController`
* **Vị trí file:** `Assets/Project/Scripts/Player/PlayerController.cs`
* **Loại:** `MonoBehaviour` (`[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]`)
* **Ý nghĩa:** Bộ điều phối trung tâm của Tinh linh (kích thước 0.8 × 0.8 units, CircleCollider2D đường kính 0.8 units), tiếp nhận sự kiện từ `PlayerInput.inputactions` và phân phát tới 6 module con.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `movement` | `PlayerMovement` | Component Ref | Module di chuyển cơ bản & nhảy. |
| **Characteristic** | `combat` | `PlayerCombat` | Component Ref | Module ngắm bắn, Bullet Time & Mana. |
| **Characteristic** | `abilities` | `PlayerAbilities` | Component Ref | Module Dash & Launch. |
| **Characteristic** | `stats` | `PlayerStats` | Component Ref | Module HP/Lives, Mana, Light Shards & i-frames. |
| **Characteristic** | `interaction` | `PlayerInteraction` | Component Ref | Module tương tác ngữ cảnh `[F]`. |
| **Characteristic** | `aura` | `PlayerLightAura` | Component Ref | Module quầng sáng URP 2D Light. |
| **Action** | `Awake` | `void ()` | N/A | Khởi tạo và liên kết sự kiện nội bộ (VD: `stats.OnDamaged += combat.ForceInterruptAim`). |
| **Action** | `SetControlEnabled`| `void (bool enabled)` | N/A | Khóa/mở toàn bộ Input & Physics (dùng khi vào `Cutscene`, `TravelCannon`, `DialogueUI`, `CannonSelectUI`). |

---

### 2.2.2 `PlayerStats`
* **Vị trí file:** `Assets/Project/Scripts/Player/PlayerStats.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Quản lý 3 HP (Campaign) / 3 Lives (Survival), thanh Mana, tiền tệ Light Shards (0–999), khung bất tử 1.5s i-frames và vị trí mặt đất an toàn cuối cùng.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `currentHealth` | `int` | `3` | Sinh mệnh hiện tại (3 HP Campaign / 3 Lives Survival). |
| **Characteristic** | `maxHealth` | `int` | `3` | Sinh mệnh tối đa. |
| **Characteristic** | `currentMana` | `float` | `100.0f` | Năng lượng hiện tại (0 đến `maxMana`). |
| **Characteristic** | `maxMana` | `float` | `100.0f` | Mana tối đa (nâng cấp lên 125 / 150 / 200 tại Lò rèn Boran). |
| **Characteristic** | `lightShards` | `int` | `0` | Tiền tệ Mảnh Sáng đang giữ (giới hạn tối đa 999 LS). |
| **Characteristic** | `isInvulnerable`| `bool` | `false` | Cờ miễn nhiễm sát thương tạm thời. |
| **Characteristic** | `iFrameTimer` | `float` | `1.5f` | Thời lượng miễn nhiễm sát thương kèm chớp trắng (1.5s). |
| **Characteristic** | `lastSafeGroundedPos`| `Vector2` | `Vector2.zero` | Tọa độ đứng đất an toàn gần nhất để tái tạo khi rơi hố hoặc chạm gai. |
| **Action** | `TakeDamage` | `void (int dmg, Vector2 sourcePos)`| N/A | Trừ 1 HP/Life, đẩy lùi (knockback 0.2s), kích hoạt 1.5s i-frames, bắn `OnDamaged` ngắt Bullet Time; nếu về 0 gọi `OnPlayerDied`. |
| **Action** | `FallIntoAbyss` | `void ()` | N/A | Campaign: tái tạo tức thì tại `lastSafeGroundedPos` + 1.5s i-frames (**0 mất HP**). Survival: trừ 1 Mạng rồi tái tạo. |
| **Action** | `Heal` | `void (int amount)` | N/A | Hồi lượng HP chỉ định (không vượt `maxHealth`). |
| **Action** | `ConsumeMana` | `bool (float amount)` | N/A | Trừ Mana nếu đủ ($\ge 20$), trả về `true`; nếu thiếu nháy đỏ HUD & phát tiếng tít cảnh báo. |
| **Action** | `AddLightShards`| `void (int amount)` | N/A | Cộng LS (kẹp trần `Mathf.Min(999, ...)`), phát SFX lấp lánh. |
| **Action** | `RestoreFullStats`| `void ()` | N/A | Hồi đầy 100% HP/Lives và 100% Mana (khi chạm `CheckpointAltar`). |
| **Action** | `SetSafeGroundedPos`| `void (Vector2 pos)` | N/A | Cập nhật vị trí mặt đất an toàn mỗi khi `isGrounded == true` trên nền vững. |

* **Chú thích liên kết:** Bắn sự kiện `OnHealthChanged`, `OnManaChanged`, `OnShardsChanged`, `OnDamaged`, `OnPlayerDied` cho `HUDController`, `PlayerCombat`, `PlayerLightAura`, và `GameManager`.

---

### 2.2.3 `PlayerMovement`
* **Vị trí file:** `Assets/Project/Scripts/Player/PlayerMovement.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Điều khiển di chuyển ngang (A/D), nhảy (Space), rơi sàn một chiều (S+Space), Coyote Time và tương tác hệ số tốc độ gió.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `moveSpeed` | `float` | `4.5f` | Tốc độ di chuyển ngang cơ sở (4.5 units/s). |
| **Characteristic** | `accelTime` | `float` | `0.15f` | Gia tốc đạt vận tốc cực đại sau 0.15s. |
| **Characteristic** | `decelTime` | `float` | `0.10f` | Ma sát dừng trượt hoàn toàn sau 0.10s. |
| **Characteristic** | `jumpForce` | `float` | `12.0f` | Xung lực nhảy thẳng đứng (độ cao max 2.88 units, bay 0.96s). |
| **Characteristic** | `gravity` | `float` | `25.0f` | Trọng lực chuẩn thế giới ($g = 25.0\text{ units/s}^2$). |
| **Characteristic** | `isGrounded` | `bool` | `true` | Trạng thái tiếp đất hiện tại. |
| **Characteristic** | `coyoteTimer` | `float` | `0.1f` | Thời gian đệm nhảy/Launch sát mép vực (0.1s). |
| **Characteristic** | `isOneWayDropping`| `bool` | `false` | Cờ đang bỏ qua va chạm nền một chiều (0.3s). |
| **Characteristic** | `windSpeedMultiplier`| `float` | `1.0f` | Hệ số ảnh hưởng của `WindZone` (xuôi gió `1.3f`, ngược gió `0.5f`). |
| **Action** | `Move` | `void (Vector2 input)` | N/A | Di chuyển ngang theo `moveSpeed * windSpeedMultiplier`. |
| **Action** | `Jump` | `void ()` | N/A | Nhảy khi `isGrounded == true` hoặc `coyoteTimer > 0`. |
| **Action** | `DropThroughPlatform`| `void ()` | N/A | Bỏ qua va chạm One-way Platform trong 0.3s khi nhấn S + Space. |
| **Action** | `SetMovementLock`| `void (bool isLocked)` | N/A | Khóa phím A/D (khi vào Bullet Time hoặc bị Knockback). |
| **Action** | `SetWindSpeedMultiplier`| `void (float mult)` | N/A | Được `WindZone` gọi khi Tinh linh vào/ra vùng lộng gió. |

---

### 2.2.4 `PlayerAbilities`
* **Vị trí file:** `Assets/Project/Scripts/Player/PlayerAbilities.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Quản lý năng lực cơ động nâng cao (Ground/Air Dash — phím Ctrl, Launch — Chuột phải) theo hệ thống **Ability ID** mở rộng vô hạn.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `unlockedAbilities` | `HashSet<string>` | Empty Set | Tập hợp mã kỹ năng mở khóa (`"Ability_Launch"`, `"Ability_AirDash"`, `"Ability_ZephyrResistance"`, tương lai: `"Ability_FlameShot"`, `"Ability_TripleShot"`). |
| **Characteristic** | `canAirDash` | `bool` | `true` | Lượt Air Dash trên không (tối đa 1 lần/lần nhảy, chạm tường KHÔNG reset). |
| **Characteristic** | `dashSpeed` | `float` | `13.0f` | Vận tốc lướt ngang phím Ctrl (13.0 units/s). |
| **Characteristic** | `dashDuration` | `float` | `0.2f` | Thời lượng lướt (~2.6 units; khi Air Dash tạm ngắt trọng lực $g = 0$ trong 0.2s). |
| **Characteristic** | `groundDashCooldown`| `float` | `0.6f` | Thời gian hồi chiêu giữa 2 lần lướt trên mặt đất (0.6s). |
| **Characteristic** | `launchSpeed` | `float` | `20.0f` | Vận tốc phóng thân thể Chuột phải (20.0 units/s, 0 Mana, 0 Cooldown). |
| **Characteristic** | `launchStraightDuration`| `float` | `0.2f` | Bỏ qua trọng lực trong 0.2s đầu để bay thẳng tắp theo chuột. |
| **Characteristic** | `launchGravityBlendDuration`| `float` | `0.15f` | Kéo dần trọng lực thành quỹ đạo cong parabol trong 0.15s kế tiếp. |
| **Action** | `HasAbility` | `bool (string abilityID)`| N/A | Kiểm tra người chơi đã mở khóa kỹ năng hay chưa. |
| **Action** | `UnlockAbility` | `void (string abilityID)`| N/A | Mở khóa kỹ năng mới, đồng bộ lưu vào `GameSaveData.unlockedAbilityIDs`. |
| **Action** | `TryExecuteDash` | `void ()` | N/A | Trên đất: Ground Dash (cooldown 0.6s). Trên không: Air Dash nếu `HasAbility("Ability_AirDash") && canAirDash` (hỗ trợ combo sau Launch). |
| **Action** | `TryExecuteLaunch`| `void (Vector2 mousePos)`| N/A | Yêu cầu bắt buộc: `HasAbility("Ability_Launch")`, `isGrounded == true` (hoặc Coyote 0.1s) và không ngắm bắn; sai điều kiện phát tiếng *buzz*. |
| **Action** | `ResetAirDash` | `void ()` | N/A | Hồi lại `canAirDash = true` duy nhất khi `isGrounded == true`. |

---

### 2.2.5 `PlayerCombat`
* **Vị trí file:** `Assets/Project/Scripts/Player/PlayerCombat.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Điều khiển 2 phương thức bắn Chuột trái (Tap $< 0.25\text{s}$ tạo `FloatingLightOrb` / Hold $\ge 0.25\text{s}$), Bullet Time (0.1x), tia laser dự báo, hủy ngắm `[X]` và hồi Mana.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `manaCostPerShot` | `float` | `20.0f` | Chi phí 20 điểm Mana/viên đạn. |
| **Characteristic** | `bulletTimeScale` | `float` | `0.1f` | Làm chậm thời gian 90%; trọng lực trên không giảm còn 10% ($2.5\text{ u/s}^2$). |
| **Characteristic** | `normalRegenRate` | `float` | `2.0f` | Tốc độ hồi tự nhiên (2.0 điểm/s; nâng cấp Lò rèn lên 3.0 điểm/s). |
| **Characteristic** | `fastRegenRate` | `float` | `25.0f` | Tốc độ hồi tăng tốc (25.0 điểm/s). |
| **Characteristic** | `fastRegenDelay` | `float` | `3.0f` | Thời gian chờ kích hoạt hồi nhanh (3.0s; nâng cấp Lò rèn giảm còn 2.0s). |
| **Characteristic** | `aimHoldThreshold` | `float` | `0.25f` | Ngưỡng phân biệt Tap ($<0.25\text{s}$) và Hold ($\ge 0.25\text{s}$). |
| **Characteristic** | `projectilePrefab` | `GameObject` | Prefab Ref | Prefab của `LightProjectile`. |
| **Characteristic** | `floatingOrbPrefab`| `GameObject` | Prefab Ref | Prefab của `FloatingLightOrb` cho chế độ Tap bắn. |
| **Characteristic** | `aimLaser` | `LineRenderer` | Component Ref | Tia laser dự báo đường bay & điểm nảy (đổi xanh ngọc khi Hint Cấp 2 trúng đích). |
| **Action** | `StartAimHold` | `void ()` | N/A | Hold Chuột trái (yêu cầu Mana $\ge 20$): gọi `EnterBulletTime()`, khóa A/D, bật tia laser. |
| **Action** | `ReleaseAimFire` | `void ()` | N/A | Nhả Chuột trái: trừ 20 Mana, sinh `LightProjectile`, gọi `ExitBulletTime()`. |
| **Action** | `ExecuteTapAim` | `void ()` | N/A | Tap Chuột trái: trừ 20 Mana ngay, sinh `FloatingLightOrb` lơ lửng trên đầu (tồn tại 8.0s). |
| **Action** | `CancelAim` | `void ()` | N/A | Phím `[X]`: ngắt Bullet Time. Hold $\rightarrow$ không trừ Mana; Tap $\rightarrow$ không hoàn Mana (cầu tiếp tục lơ lửng). |
| **Action** | `ForceInterruptAim`| `void ()` | N/A | Gọi khi trúng đòn lúc đang Hold ngắm: ngắt Bullet Time tức thì, không bắn, không trừ 20 Mana (GDD 4.5). |
| **Action** | `EnterBulletTime` | `void ()` | N/A | Gọi `GameManager.SetBulletTime(true)` & khóa di chuyển ngang. |
| **Action** | `ExitBulletTime` | `void ()` | N/A | Gọi `GameManager.SetBulletTime(false)` & mở lại di chuyển ngang. |
| **Action** | `UpdateManaRegen` | `void ()` | N/A | Hồi Mana theo `Time.deltaTime` (Game Time — tự chậm 90% trong Bullet Time). |

---

### 2.2.6 `PlayerInteraction` & `PlayerLightAura`
* **Vị trí file:** `Assets/Project/Scripts/Player/PlayerInteraction.cs` & `PlayerLightAura.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Quét đối tượng `IInteractable` gần nhất trong bán kính $1.8\text{ units}$ (hỗ trợ cả bấm `[F]` và giữ `[F]` theo `HoldDuration`) và điều khiển URP 2D Light.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `interactRadius` | `float` | `1.8f` | Bán kính quét tương tác (1.8 units). |
| **Characteristic** | `currentTarget` | `IInteractable`| `null` | Duy nhất 1 vật thể tương tác gần nhất được hiển thị White Outline + nhãn `[F]`. |
| **Characteristic** | `holdProgressTimer`| `float` | `0.0f` | Bộ đếm tiến trình khi giữ phím `[F]` (dùng cho `ShadowCage` cần 2.0s). |
| **Characteristic** | `pointLight` | `Light2D` | Component Ref | Đèn điểm 2D URP gắn trên Tinh linh. |
| **Characteristic** | `baseRadius` | `float` | `4.0f` | Bán kính chiếu sáng gốc (4.0 units). |
| **Characteristic** | `currentRadius` | `float` | `4.0f` | Bán kính hiện tại (6.0 units khi mua nâng cấp *Light Aura* 1.5×). |
| **Action** | `ScanForInteractables`| `void ()` | N/A | Tìm `IInteractable` gần nhất trong 1.8 units, bật `Highlight(true)` kèm prompt động. |
| **Action** | `HandleInteractionInput`| `void (bool isHeld)` | N/A | Nếu `HoldDuration == 0`: bấm `[F]` gọi ngay `Interact()`. Nếu `HoldDuration > 0`: nạp `holdProgressTimer` đủ mới gọi `Interact()`. |
| **Action** | `SetAuraScale` | `void (float mult)` | N/A | Cập nhật `currentRadius = baseRadius * mult` (được `UpgradeManager` gọi). |
| **Action** | `TriggerDamageFlicker`| `void ()` | N/A | Thu hẹp nhẹ và chớp nháy vầng sáng (phản hồi diegetic khi mất máu). |

---

## 2.3 Nhóm Đạn đạo, Quả cầu chờ & Vật phẩm rơi (Projectiles & Pickups)

### 2.3.1 `ProjectileDataSO` & `LightProjectile`
* **Vị trí file:** `Assets/Project/Scripts/Projectiles/ProjectileDataSO.cs` & `LightProjectile.cs`
* **Loại:** `ScriptableObject` (`ProjectileDataSO`) & `MonoBehaviour` (`LightProjectile`)
* **Ý nghĩa:** Cấu hình hướng dữ liệu và thực thể Đạn Ánh Sáng thực thi phản xạ quang học CircleCast.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `ProjectileDataSO.projectileName`| `string` | `"BaseLightBullet"`| Tên loại đạn (`Base`, `Flame`, `Flora`). |
| **Characteristic** | `ProjectileDataSO.baseSpeed` | `float` | `16.0f` | Vận tốc bay chuẩn (16.0 units/s). |
| **Characteristic** | `ProjectileDataSO.maxBounceLimit`| `int` | `3` | Giới hạn phản xạ tối đa qua gương (3 lần). |
| **Characteristic** | `ProjectileDataSO.lifeTime` | `float` | `5.0f` | Thời gian tồn tại tối đa nếu không trúng gì (5.0s). |
| **Characteristic** | `ProjectileDataSO.circleCastRadius`| `float` | `0.1f` | Bán kính quét CircleCast chống xuyên tường/gương. |
| **Characteristic** | `ProjectileDataSO.baseExplosionRadius`| `float` | `1.5f` | Bán kính nổ AOE mặc định ở lần va chạm thứ 4 (hoặc khi có *Radiant Burst*). |
| **Characteristic** | `LightProjectile.config` | `ProjectileDataSO`| Asset Ref | Cấu hình gốc được nạp vào viên đạn. |
| **Characteristic** | `LightProjectile.moveDirection` | `Vector2` | `Vector2.zero` | Vector hướng bay hiện thời (có thể bị uốn cong bởi `WindZone`). |
| **Characteristic** | `LightProjectile.bounceCount` | `int` | `0` | Bộ đếm số lần đã nảy qua mặt gương. |
| **Characteristic** | `LightProjectile.isEmpowered` | `bool` | `false` | Cờ nâng cấp *Empowered Light* (gây 2 Hit, xuyên thủng khiên sương đen). |
| **Characteristic** | `LightProjectile.hasRadiantBurst`| `bool` | `false` | Cờ nâng cấp *Radiant Burst* (nổ AOE 1.5 units ngay cả ở lần chạm đầu tiên). |
| **Action** | `Initialize` | `void (Vector2 dir, ProjectileDataSO cfg, bool empowered, bool burst)` | N/A | Khởi tạo hướng bay và các cờ nâng cấp từ Lò rèn. |
| **Action** | `FixedUpdateSweep`| `void ()` | N/A | Quét `Physics2D.CircleCast` dọc quãng đường mỗi bước vật lý để xử lý va chạm chính xác. |
| **Action** | `Reflect` | `void (Vector2 hitNormal)`| N/A | Phản xạ quang học; nếu `bounceCount >= config.maxBounceLimit` (lần chạm thứ 4) $\rightarrow$ gọi `Explode()`. |
| **Action** | `Explode` | `void ()` | N/A | Nổ AOE gây sát thương trong bán kính `baseExplosionRadius` (1.5 units) và hủy đạn. |
| **Action** | `ApplyWindForce` | `void (Vector2 windDelta)`| N/A | Cộng lệch hướng từ `WindZone` (bị bỏ qua nếu gặp khung gương thì nổ biến mất). |

* **Chú thích liên kết:** Tương tác với `ReflectionMirror`, `RadiantBloom`, `IDamageable` (`EnemyBase`, `EnergyPylon`, `ShadowCage`), `WindZone`, và triệt tiêu `EnemyProjectile` khi va chạm trên không trung.

---

### 2.3.2 `FloatingLightOrb` (Quả cầu chờ — Tap Aim)
* **Vị trí file:** `Assets/Project/Scripts/Projectiles/FloatingLightOrb.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Hiện thực hóa Phương thức A (Tap nhanh $< 0.25\text{s}$ trong GDD 4.4): quả cầu lơ lửng trên đầu Tinh linh chờ người chơi click vào để bật Bullet Time và bắn.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `remainingLifeTime`| `float` | `8.0f` | Thời gian tồn tại tối đa (8.0s); hết giờ tự tan và KHÔNG hoàn lại 20 Mana. |
| **Characteristic** | `isBeingAimed` | `bool` | `false` | Cờ người chơi đã click vào quả cầu để mở Bullet Time ngắm bắn. |
| **Action** | `OnOrbClicked` | `void ()` | N/A | Click lần 1: bật Bullet Time & tia laser từ tâm quả cầu. Click lần 2: phóng `LightProjectile` và hủy quả cầu. |
| **Action** | `CancelOrbAim` | `void ()` | N/A | Khi bấm `[X]`: tắt Bullet Time, trả quyền di chuyển, quả cầu tiếp tục lơ lửng cho tới hết 8.0s. |

---

### 2.3.3 `EnemyProjectile` & `LightShardPickup`
* **Vị trí file:** `Assets/Project/Scripts/Projectiles/EnemyProjectile.cs` & `LightShardPickup.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Đạn gai đen của kẻ địch (Slime/Spitter/Weaver) và Mảnh Sáng tiền tệ tự động hút về Tinh linh.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `EnemyProjectile.speed` | `float` | `6.0f` | Vận tốc bay (Slime: `6.0 u/s`; Spitter: `8.0 u/s`). |
| **Characteristic** | `EnemyProjectile.direction`| `Vector2` | `Vector2.zero` | Hướng bay của tia gai/tơ bóng tối. |
| **Characteristic** | `LightShardPickup.shardValue`| `int` | `1` | Giá trị LS của mảnh nhặt được. |
| **Characteristic** | `LightShardPickup.magnetRadius`| `float` | `2.5f` | Bán kính tự động hút về phía Tinh linh (GDD 4.3). |
| **Action** | `EnemyProjectile.Initialize`| `void (Vector2 dir, float spd)`| N/A | Khởi tạo vận tốc và hướng bắn. |
| **Action** | `EnemyProjectile.CancelByLight`| `void ()` | N/A | Bị triệt tiêu tức thì thành hạt sáng khi va chạm với `LightProjectile`. |
| **Action** | `LightShardPickup.AttractToPlayer`| `void (Transform player)` | N/A | Bay hút nhanh về tâm Tinh linh khi nằm trong 2.5 units và gọi `PlayerStats.AddLightShards(shardValue)`. |

---

## 2.4 Nhóm Interfaces & Trừu tượng hóa (Interfaces & Abstractions)

### 2.4.1 `IInteractable`, `IDamageable` & `IActivatable`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Interfaces/`
* **Loại:** `C# Interface`
* **Ý nghĩa:** 3 hợp đồng đa hình cốt lõi giúp tách rời hoàn toàn đối tượng kích hoạt và đối tượng thực thi.

```csharp
public interface IInteractable
{
    string InteractionPrompt { get; } // Nhãn động: "[F] Vặn cần xoay", "[F] Use Cannon", "Giữ [F] Giải cứu"...
    float HoldDuration { get; }       // 0f = bấm ngay; 2.0f = nhấn giữ (ShadowCage)
    void Interact();                  // Thực thi hành vi tương tác
    void Highlight(bool state);       // Bật/tắt đường viền sáng trắng (White Glowing Outline)
}

public interface IDamageable
{
    void TakeDamage(int amount);      // Tiếp nhận sát thương từ LightProjectile
}

public interface IActivatable
{
    void Activate();                  // Kích hoạt cơ quan (mở SealGate, mở khóa TravelCannon...)
    void Deactivate();                // Hủy kích hoạt (dùng cho công tắc hẹn giờ Timed Switch)
}
```

---

## 2.5 Nhóm Môi trường & Cơ quan Giải đố (Environment & Puzzles)

### 2.5.1 `ReflectionMirror` & `MirrorType`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Puzzles/ReflectionMirror.cs` & `MirrorType.cs`
* **Loại:** `MonoBehaviour` & `Enum`
* **Ý nghĩa:** Bề mặt quang học phản xạ đạn ánh sáng (3 biến thể: Cố định, Xoay bằng bệ `[F]`, Tự động xoay liên tục).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `MirrorType` | `enum` | `Fixed_45Deg` | `Fixed_45Deg`, `Steerable_Pedestal`, `AutoRotating_Clockwise`. |
| **Characteristic** | `type` | `MirrorType` | `Fixed_45Deg` | Biến thể của tấm gương. |
| **Characteristic** | `currentAngle` | `float` | `45.0f` | Góc nghiêng hiện tại của mặt phản xạ. |
| **Characteristic** | `autoRotationSpeed`| `float` | `30.0f` | Vận tốc góc tự xoay liên tục (độ/s, dành cho `AutoRotating_Clockwise`). |
| **Action** | `RotateStep` | `void (float angle = 45f)`| N/A | Xoay gương thêm $45^\circ$ cùng chiều kim đồng hồ, khóa khấc ("khục") và lóe ánh kim. |
| **Action** | `GetNormalVector` | `Vector2 ()` | N/A | Trả về vector pháp tuyến 2D để `LightProjectile` tính góc nảy $i = r$. |
| **Action** | `OnProjectileHit` | `void (LightProjectile p)`| N/A | Phát SFX *Crystal Ping* và Hit Flash hình sao; khung viền gương hấp thụ & hủy đạn. |

---

### 2.5.2 `RemotePedestal`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Puzzles/RemotePedestal.cs`
* **Loại:** `MonoBehaviour`, hiện thực hóa `IInteractable`
* **Ý nghĩa:** Bệ cần xoay đặt trên sàn an toàn, nối lên gương trên cao bằng đường rãnh dây cáp phát quang.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `linkedMirror` | `ReflectionMirror` | `null` | Gương treo trên cao kết nối với bệ này. |
| **Characteristic** | `energyCable` | `LineRenderer` | `null` | Rãnh vân năng lượng chạy dọc vách đá. |
| **Characteristic** | `InteractionPrompt`| `string` | `"[F] Vặn cần xoay"` | Nhãn hiển thị khi vào tầm 1.8 units (`HoldDuration = 0f`). |
| **Action** | `Interact` | `void ()` | N/A | Phát tiếng đá cọ xát, truyền xung sáng dọc `energyCable` và gọi `linkedMirror.RotateStep(45f)`. |
| **Action** | `Highlight` | `void (bool state)` | N/A | Bật/tắt White Glowing Outline. |

---

### 2.5.3 `RadiantBloom` & `BloomState` (Hoa Soi Sáng)
* **Vị trí file:** `Assets/Project/Scripts/Environment/Puzzles/RadiantBloom.cs` & `BloomState.cs`
* **Loại:** `MonoBehaviour` & `Enum`
* **Ý nghĩa:** Đạo cụ thực vật phát quang môi trường. **KHÔNG tương tác bằng phím `[F]`**, chỉ kích hoạt khi trúng `LightProjectile`.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `BloomState` | `enum` | `Ready` | `Ready` (nụ chuông xanh ngọc), `FlareActive` (đang chiếu rọi), `Withered` (héo xám chờ hồi). |
| **Characteristic** | `state` | `BloomState` | `BloomState.Ready` | Trạng thái hiện tại của đóa hoa. |
| **Characteristic** | `flareDuration` | `float` | `8.0f` | Tổng thời lượng "Ánh Sáng Chiếu Rọi": **1.0s bùng nổ + 5.0s duy trì sáng cực đại + 2.0s mờ dần**. |
| **Characteristic** | `rechargeDuration`| `float` | `60.0f` | Thời gian chờ hồi phục từ lúc héo úa (`Withered`) về lại `Ready` để bắn kích hoạt lại (**60.0s**). |
| **Characteristic** | `ambientRoomLight`| `Light2D` | `null` | Đèn toàn cảnh căn phòng (nâng phơi sáng 0.2 $\rightarrow$ 1.0 đẩy lùi sương mù). |
| **Action** | `OnHitByLightProjectile`| `void ()` | N/A | Nếu `state == Ready`: kích hoạt `RadiantFlareRoutine()`. |
| **Action** | `RadiantFlareRoutine`| `IEnumerator ()` | N/A | 0–1s sóng xung kích & sáng cực đại; 1–6s duy trì sáng toàn phòng 5.0s; 6–8s Vignette tối dần & hóa xám `Withered`; chờ **60.0s** rồi gọi `ResetToReady()`. |

---

### 2.5.4 `EnergyPylon`, `SealGate` & `LightPressurePad`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Puzzles/`
* **Loại:** `MonoBehaviour` (`EnergyPylon` hiện thực `IDamageable`; `SealGate` hiện thực `IActivatable`)
* **Ý nghĩa:** Lõi tiếp nhận đạn ánh sáng (vĩnh viễn hoặc hẹn giờ 5.0s), Bệ Đá Ánh Sáng LVL-01 Phòng 1 và Cửa Phong Ấn 2 chiều.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `EnergyPylon.isActive` | `bool` | `false` | Trạng thái đã kích hoạt. |
| **Characteristic** | `EnergyPylon.isTimed` | `bool` | `false` | Công tắc hẹn giờ: đếm ngược `timerDuration` theo Game Time (5.0s $\rightarrow$ 50s thực trong Bullet Time). |
| **Characteristic** | `EnergyPylon.timerDuration`| `float` | `5.0f` | Thời gian duy trì nếu là Timed Switch. |
| **Characteristic** | `EnergyPylon.targets` | `List<IActivatable>` | List / UnityEvent | Danh sách cơ quan được kích hoạt (mở `SealGate`, mở khóa `TravelCannon`...). |
| **Characteristic** | `SealGate.gateID` | `string` | `""` | Mã định danh lưu mở vĩnh viễn vào `WorldState.openedGates`. |
| **Characteristic** | `SealGate.isOpen` | `bool` | `false` | Trạng thái cửa đá 2 chiều. |
| **Characteristic** | `LightPressurePad.targets`| `List<IActivatable>`| List | Bệ đá LVL-01 Phòng 1: kích hoạt khi Tinh linh lướt chạm trực tiếp vào bệ. |
| **Action** | `EnergyPylon.TakeDamage` | `void (int amount)` | N/A | Gọi `TriggerActivation()`: bốc cháy vàng kim và gọi `Activate()` lên mọi mục tiêu trong `targets`. |
| **Action** | `EnergyPylon.Deactivate` | `void ()` | N/A | Tắt kích hoạt khi hết `timerDuration` (chỉ áp dụng cho Timed Switch). |
| **Action** | `SealGate.Activate` | `void ()` | N/A | Trượt cửa lên vĩnh viễn, ghi nhận vào `WorldState.RegisterGateOpened(gateID)`. |
| **Action** | `SealGate.OnPlayerPassThrough`| `void ()` | N/A | Kích hoạt `SaveManager.TriggerAutosave()` và thông báo chuyển phòng cho `RoomController`. |

---

### 2.5.5 `WindZone` & `WindZoneType`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Puzzles/WindZone.cs` & `WindZoneType.cs`
* **Loại:** `MonoBehaviour` & `Enum`
* **Ý nghĩa:** Mô phỏng mọi dạng khí động học Vùng Gió (có thể ghép nhiều `WindZone` nhỏ hoặc tinh chỉnh phạm vi, cường độ, chu kỳ thổi để tạo địa hình gió đa dạng thay cho Trục Gió cứng).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `WindZoneType` | `enum` | `Gust_Horizontal` | `Gust_Horizontal` (vùng lộng gió ngang), `Updraft_Vertical` (luồng gió nâng/lốc xoáy). |
| **Characteristic** | `zoneType` | `WindZoneType` | `Gust_Horizontal` | Phân loại luồng gió. |
| **Characteristic** | `windVector` | `Vector2` | `(1.0f, 0.0f)` | Vector hướng thổi. |
| **Characteristic** | `windForce` | `float` | `8.0f` | Cường độ đẩy nâng (Updraft: `8.0 u/s`; LVL-02 gió nhẹ: `1.5–2.0 u/s`). |
| **Characteristic** | `tailwindMultiplier`| `float` | `1.3f` | Tăng $+30\%$ tốc độ Tinh linh khi đi xuôi chiều gió (GDD 4.6). |
| **Characteristic** | `headwindMultiplier`| `float` | `0.5f` | Giảm $-50\%$ tốc độ Tinh linh khi đi ngược chiều gió (GDD 4.6). |
| **Characteristic** | `isPeriodic` | `bool` | `false` | Gió giật chu kỳ (LVL-01 Phòng 3: 3s thổi / 2s ngừng; LVL-03: đổi chiều 4s). |
| **Characteristic** | `blowTime` / `pauseTime`| `float` | `3.0f` / `2.0f` | Thời gian thổi và thời gian lặng gió. |
| **Action** | `ApplyToPlayer` | `void (PlayerMovement pm, PlayerAbilities pa)`| N/A | Áp hệ số tốc độ/lực nâng; giảm bẻ hướng nếu `pa.HasAbility("Ability_ZephyrResistance")` (Tinh Hoa Gió). |
| **Action** | `DeflectProjectile` | `void (LightProjectile p)`| N/A | Uốn cong quỹ đạo đạn ánh sáng theo `windVector * windForce`. |

---

## 2.6 Nhóm Tương tác Thế giới, Đại Bác Dịch Chuyển & Cạm bẫy (Interactables & Hazards)

### 2.6.1 `TravelCannon`, `CannonStationSO` & `CannonState` (HỆ THỐNG ĐẠI BÁC DỊCH CHUYỂN)
* **Vị trí file:** `Assets/Project/Scripts/Environment/Interactables/TravelCannon.cs`, `CannonStationSO.cs`, `CannonState.cs`
* **Loại:** `MonoBehaviour` (`TravelCannon` hiện thực `IInteractable` & `IActivatable`), `ScriptableObject` (`CannonStationSO`), `Enum` (`CannonState`)
* **Ý nghĩa:** Công cụ dịch chuyển nhanh cách điệu dạng Khẩu Đại Bác liên thông bản đồ Metroidvania. Trong bản đồ án bố trí 2 khẩu: **Base (Hub)** (tạm khóa cho tới khi mở được khẩu đầu tiên) và **Tiền phòng Boss LVL-03**; sẵn sàng mở rộng thêm vô số khẩu ở Vùng Lửa / Vùng Rừng.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `CannonState` | `enum` | `Locked` | `Locked`, `Unlocked_Active`, `Launching`, `Receiving`. |
| **Characteristic** | `CannonStationSO.cannonID` | `string` | `""` | Mã định danh toàn cục (VD: `"CANNON_Hub_Base"`, `"CANNON_LVL03_BossPrep"`). |
| **Characteristic** | `CannonStationSO.displayName`| `string` | `""` | Tên địa điểm hiện trên bảng chọn (`"Cổ Thụ Ánh Sáng"`, `"Đỉnh Gió Gầm - Tiền Phòng Boss"`). |
| **Characteristic** | `CannonStationSO.sceneName` | `string` | `""` | Tên Scene Unity chứa trạm Đại Bác này. |
| **Characteristic** | `CannonStationSO.isHubBaseCannon`| `bool` | `false` | Cờ khẩu tại Base: tự động mở khóa khi mạng lưới có $\ge 1$ khẩu ngoài được mở. |
| **Characteristic** | `TravelCannon.stationData` | `CannonStationSO` | Asset Ref | Dữ liệu trạm gắn với khẩu đại bác này. |
| **Characteristic** | `TravelCannon.currentState` | `CannonState` | `Locked` | Trạng thái hiện tại (đồng bộ với `CannonNetworkManager` lúc `Start()`). |
| **Characteristic** | `TravelCannon.barrelPivot` | `Transform` | `null` | Trục xoay nòng đại bác. |
| **Characteristic** | `TravelCannon.muzzlePoint` | `Transform` | `null` | Đầu nòng nơi hút Tinh linh vào và khai hỏa. |
| **Characteristic** | `TravelCannon.landingPoint` | `Transform` | `null` | Vị trí tiếp đất khi Tinh linh đáp từ trên trời xuống trạm này. |
| **Characteristic** | `TravelCannon.launchAngle` | `float` | `75.0f` | Góc xoay nòng hướng lên trời (hoặc tính theo hướng điểm đến). |
| **Characteristic** | `TravelCannon.chargeDuration`| `float` | `1.0f` | Thời gian tụ năng lượng sáng và rung nòng trước khi bắn. |
| **Characteristic** | `TravelCannon.launchFlySpeed`| `float` | `35.0f` | Vận tốc bắn Tinh linh vút khỏi khung hình camera. |
| **Characteristic** | `TravelCannon.InteractionPrompt`| `string` | Động theo state | Nếu `Unlocked_Active`: `"[F] Use Cannon"`. Nếu `Locked`: `"[F] Kiểm tra"` (bấm `[F]` hiện `"Tạm chưa mở khóa"`). |
| **Action** | `Interact` | `void ()` | N/A | **Nếu `Locked`:** Gọi `DialogueUI.ShowNotification("Tạm chưa mở khóa")`.<br>**Nếu `Unlocked_Active`:** Lấy danh sách các khẩu đã mở khác từ `CannonNetworkManager` và mở `CannonSelectUI` (kèm nút `"Exit"`). |
| **Action** | `Highlight` | `void (bool state)` | N/A | Bật/tắt White Glowing Outline và hiển thị prompt `[F]` tương ứng trạng thái. |
| **Action** | `Activate` | `void ()` | N/A | Hiện thực `IActivatable` (mở khóa khi giải xong câu đố/điều kiện): chuyển sang `Unlocked_Active`, gọi `CannonNetworkManager.UnlockCannon(stationData.cannonID)`. |
| **Action** | `PlayLaunchSequence`| `IEnumerator (CannonStationSO target)` | N/A | Khóa input Player $\rightarrow$ Xoay nòng lên `launchAngle` $\rightarrow$ Hút Tinh linh vào nòng $\rightarrow$ Tụ năng lượng (`chargeDuration`) $\rightarrow$ Bắn Tinh linh bay vút khỏi màn hình (`launchFlySpeed`) $\rightarrow$ Chuyển cảnh / Load map. |
| **Action** | `PlayLandingSequence`| `IEnumerator (PlayerController player)` | N/A | Đặt Tinh linh trên bầu trời phía trên `landingPoint` $\rightarrow$ Bay lao xuống tiếp đất an toàn (VFX sóng sáng) $\rightarrow$ Mở lại Physics & Input (`GameState.Gameplay`). |

---

### 2.6.2 `CheckpointAltar`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Interactables/CheckpointAltar.cs`
* **Loại:** `MonoBehaviour`, hiện thực hóa `IInteractable`
* **Ý nghĩa:** Trạm lưu thụ động (lướt qua hồi 100% HP/Mana + Autosave) kiêm trạm Gợi ý Cấp 3 khi bế tắc $> 90\text{s}$.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `checkpointID` | `string` | `""` | Mã định danh Checkpoint. |
| **Characteristic** | `respawnPoint` | `Transform` | `this.transform` | Tọa độ hồi sinh an toàn. |
| **Characteristic** | `isHintActive` | `bool` | `false` | Bật lên `true` khi `RoomController` báo bế tắc $> 90\text{s}$. |
| **Characteristic** | `hintText` | `string` | `""` | Câu cổ ngữ ẩn dụ hướng dẫn logic giải đố. |
| **Characteristic** | `InteractionPrompt`| `string` | `"[F] Đọc gợi ý"` | Chỉ hiển thị viền trắng & prompt khi `isHintActive == true`. |
| **Action** | `OnTriggerEnter2D` | `void (Collider2D col)`| N/A | Bùng ngọn lửa xanh ngọc, hồi 100% HP/Mana, lưu tọa độ và gọi `SaveManager.TriggerAutosave()`. |
| **Action** | `ActivateHintMode` | `void ()` | N/A | Đổi lửa sang vàng kim nhấp nháy, đặt `isHintActive = true`. |
| **Action** | `Interact` | `void ()` | N/A | Nếu `isHintActive`: mở `DialogueUI` hiển thị `hintText`. |

---

### 2.6.3 `ShadowCage` & `LoreStele`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Interactables/ShadowCage.cs` & `LoreStele.cs`
* **Loại:** `MonoBehaviour`, hiện thực hóa `IInteractable` (`ShadowCage` hiện thực thêm `IDamageable`)
* **Ý nghĩa:** Lồng hắc thạch giam giữ bất kỳ NPC nào (đa hình qua `NPCBase`) và Bia Ký Ức cổ đại.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `ShadowCage.durability` | `int` | `2` | Độ bền lồng (chịu 2 viên đạn ánh sáng hoặc giữ phím `[F]` 2.0s). |
| **Characteristic** | `ShadowCage.jailedNPC` | `NPCBase` | `null` | **Đa hình:** Nhận mọi lớp kế thừa `NPCBase` (`BoranNPC`, `EventNPC`, tương lai `IgnisNPC`, `LyraNPC`). |
| **Characteristic** | `ShadowCage.HoldDuration`| `float` | `2.0f` | Thời gian nhấn giữ `[F]` để phá vỡ lồng bằng tay (`InteractionPrompt = "Giữ [F] Giải cứu"`). |
| **Characteristic** | `LoreStele.fragmentID` | `int` | `1` | ID Mảnh Ký Ức (Vùng Gió: #01, #02). |
| **Characteristic** | `LoreStele.InteractionPrompt`| `string` | `"[F] Đọc Bia Ký Ức"`| Nhãn tương tác (`HoldDuration = 0f`). |
| **Action** | `ShadowCage.TakeDamage` | `void (int amount)`| N/A | Giảm độ bền khi trúng `LightProjectile`; về 0 gọi `BreakCage()`. |
| **Action** | `ShadowCage.Interact` | `void ()` | N/A | Khi giữ `[F]` đủ 2.0s $\rightarrow$ gọi `BreakCage()`. |
| **Action** | `ShadowCage.BreakCage` | `void ()` | N/A | Vỡ vụn lồng, gọi hàm ảo `jailedNPC.OnRescued()`. |
| **Action** | `LoreStele.Interact` | `void ()` | N/A | Mở `DialogueUI` đọc cốt truyện, gọi `CodexManager.UnlockFragment(fragmentID)` và Autosave. |

---

### 2.6.4 `DarkSpikes` & `DarkAbyss`
* **Vị trí file:** `Assets/Project/Scripts/Environment/Hazards/DarkSpikes.cs` & `DarkAbyss.cs`
* **Loại:** `MonoBehaviour`
* **Ý nghĩa:** Cạm bẫy bãi gai đen và hố sâu vực thẳm.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `DarkSpikes.damageAmount`| `int` | `1` | Trừ 1 HP (Campaign) hoặc 1 Mạng (Survival). |
| **Characteristic** | `DarkSpikes.respawnToSafeGround`| `bool` | `true` | Nếu `true`: đẩy lùi 0.2s, tan biến và tái tạo tại `lastSafeGroundedPos` + 1.5s i-frames. |
| **Action** | `DarkSpikes.OnCollisionEnter2D`| `void (Collision2D col)`| N/A | Gọi `PlayerStats.TakeDamage(damageAmount, transform.position)`. |
| **Action** | `DarkAbyss.OnTriggerEnter2D`| `void (Collider2D other)`| N/A | Gọi `PlayerStats.FallIntoAbyss()` (Campaign: **0 trừ HP**, tái tạo mỏm an toàn; Survival: trừ 1 Mạng). |

---

## 2.7 Nhóm Kẻ địch Bóng tối & Trùm (Enemy AI & Boss Hierarchy)

### 2.7.1 `EnemyBase`
* **Vị trí file:** `Assets/Project/Scripts/Enemies/Base/EnemyBase.cs`
* **Loại:** `abstract class MonoBehaviour`, hiện thực hóa `IDamageable`
* **Ý nghĩa:** Lớp cơ sở cho mọi kẻ địch bóng tối & Boss: quản lý máu, Khiên Sương Đen, Choáng 1.5s và rơi Mảnh Sáng (`LightShardPickup`).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `maxHealth` | `int` (`protected`) | `2` | Máu tối đa (Boss kế thừa và ghi đè giá trị trên Inspector). |
| **Characteristic** | `currentHealth` | `int` (`protected`) | `2` | Máu hiện tại. |
| **Characteristic** | `hasShadowShield`| `bool` | `true` | Khiên bảo vệ: viên 1 vỡ khiên + Stun 1.5s; viên 2 hạ gục (*Empowered Light* gây 2 damage xuyên thẳng). |
| **Characteristic** | `isStunned` | `bool` | `false` | Trạng thái choáng ngắt đòn tấn công. |
| **Characteristic** | `stunDuration` | `float` | `1.5f` | Thời gian choáng (1.5s). |
| **Characteristic** | `minShards` / `maxShards`| `int` | `1` / `3` | Số lượng `LightShardPickup` rơi ra khi bị tiêu diệt. |
| **Action** | `TakeDamage` | `virtual void (int amount)`| N/A | Hiện thực `IDamageable`: xử lý phá khiên/choáng hoặc trừ `currentHealth` $\rightarrow$ gọi `Die()`. |
| **Action** | `BreakShield` | `void ()` | N/A | Tắt `hasShadowShield`, VFX vỡ giáp, gọi `ApplyStun(stunDuration)`. |
| **Action** | `ApplyStun` | `void (float dur)` | N/A | Tạm ngưng AI và ngắt đòn tụ lực trong `dur` giây. |
| **Action** | `Die` | `virtual void ()` | N/A | Tan rã hạt sáng, gọi `DropLoot()`. |
| **Action** | `DropLoot` | `void ()` | N/A | Sinh ngẫu nhiên `minShards..maxShards` Prefab `LightShardPickup`. |

---

### 2.7.2 Các biến thể Quái bóng tối: `ShadowCrawler`, `ShadowSlime`, `ShadowWeaver`, `ShadowSpitter`
* **Vị trí file:** `Assets/Project/Scripts/Enemies/AI/` (`ShadowCrawler.cs`, `ShadowSlime.cs`, `ShadowWeaver.cs`, `ShadowSpitter.cs`)
* **Loại:** `MonoBehaviour`, kế thừa `EnemyBase`

| Tên Lớp (Class) | Đặc tính đặc thù (Characteristics) | Hành vi thực thi (Actions) | Quy tắc chiến đấu GDD (MEC-05) |
| :--- | :--- | :--- | :--- |
| **`ShadowCrawler`** | `- patrolSpeed: 2.0f`<br>`- detectRange: 4.0f` | `+ PatrolUpdate()`<br>`+ ChargePlayer()` | Tuần tra mặt đất 2.0 u/s. Phát hiện thẳng 4.0 u không bị che khuất sẽ lao húc (1 HP). Có 1 khiên. |
| **`ShadowSlime`** | `- moveSpeed: 1.2f`<br>`- attackRange: 6.0f`<br>`- shootInterval: 3.0f`<br>`- telegraphDuration: 0.5f` | `+ TelegraphSquash()`<br>`+ ShootThorn()` | Di chuyển 1.2 u/s. Trong 6.0 u dừng nén thân 0.5s rồi bắn `EnemyProjectile` (6.0 u/s, chu kỳ 3.0s). Có 1 khiên. |
| **`ShadowWeaver`** | `- dropSpeed: 15.0f`<br>`- ceilingSensor: BoxCollider2D` | `+ DetectPlayerBelow()`<br>`+ DropAttack()`<br>`+ HoverAndShootWeb()` | Ngủ trên trần hang. Tinh linh vào cột cảm biến bên dưới $\rightarrow$ thả dây lao thẳng ngang cao độ Y; nếu lùi xa thì lơ lửng bắn tơ. Không khiên (1 đạn tan rã). |
| **`ShadowSpitter`** | `- shootInterval: 2.5f`<br>`- projectileSpeed: 8.0f` | `+ FireThornCycle()` | Pháo đài cố định trên vách/trần. Mỗi 2.5s bắn `EnemyProjectile` (8.0 u/s). Đạn có thể bị `LightProjectile` triệt tiêu. Không khiên. |

---

### 2.7.3 `BossBase`, `TutorialDarkCore`, `WindCoreBoss` & `BossPhase`
* **Vị trí file:** `Assets/Project/Scripts/Enemies/Base/BossBase.cs`, `Assets/Project/Scripts/Enemies/Bosses/`
* **Loại:** `abstract class` (`BossBase` kế thừa `EnemyBase`), `MonoBehaviour`, `Enum`
* **Ý nghĩa:** Quản lý Boss theo Phase (kế thừa trực tiếp `maxHealth`/`currentHealth` từ `EnemyBase`, không khai báo trùng biến máu).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `BossPhase` | `enum` | `Phase1_Shockwaves` | `Phase1_Shockwaves`, `Phase2_VortexStorm`, `Vulnerable_Exposed`. |
| **Characteristic** | `BossBase.currentPhase` | `BossPhase` | `Phase1_Shockwaves` | Giai đoạn hành vi hiện tại của Boss. |
| **Characteristic** | `BossBase.areaID` | `string` | `""` | Mã vùng đất sẽ được thanh tẩy khi Boss bị tiêu diệt (VD: `"AREA_Wind"`). |
| **Characteristic** | `BossBase.rewardAbilityID`| `string` | `""` | Mã kỹ năng mở khóa sau khi hạ Boss (VD: `"Ability_AirDash"`). |
| **Characteristic** | `TutorialDarkCore.shockwaveInterval`| `float` | `3.0f` | Lõi sơ khai LVL-01 Phòng 5 (`maxHealth = 3`): phát sóng xung kích chu kỳ 3.0s. |
| **Characteristic** | `WindCoreBoss.bossVortex` | `WindZone` | Scene Ref | Luồng bão xoáy đổi chiều trong đấu trường LVL-03 (`maxHealth = 6`). |
| **Characteristic** | `WindCoreBoss.arenaMirrors`| `ReflectionMirror[]`| Array | Cụm gương quanh đấu trường để bắn dội đạn vào tâm lõi khi mở giáp. |
| **Action** | `BossBase.SetPhase` | `void (BossPhase next)` | N/A | Chuyển pha tấn công. |
| **Action** | `BossBase.Die` | `override void ()` | N/A | Ghi nhận `WorldState.RegisterAreaPurified(areaID)`, mở khóa `rewardAbilityID`, chạy Cutscene thanh tẩy và Autosave. |
| **Action** | `WindCoreBoss.ExposeCoreWeakpoint`| `void ()` | N/A | Mở giáp bão tạm thời (4.0s) để người chơi bật Bullet Time bắn nảy gương trúng lõi. |

---

## 2.8 Nhóm NPCs, Kinh tế Lò rèn & Thư viện Lore (NPCs, Economy & Lore)

### 2.8.1 `NPCBase`, `NPCState`, `BoranNPC`, `SacredTreeNPC` & `EventNPC`
* **Vị trí file:** `Assets/Project/Scripts/NPC/`
* **Loại:** `abstract class` (`NPCBase` hiện thực `IInteractable`), `MonoBehaviour`, `Enum`
* **Ý nghĩa:** Phân định rõ NPC Chức năng cốt lõi (không bao giờ mất) và NPC Sự kiện (rời phòng chưa cứu sẽ biến mất `Missed`).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `NPCState` | `enum` | `Trapped` | `Trapped`, `Rescued`, `Missed`. |
| **Characteristic** | `NPCBase.npcID` | `string` | `""` | Mã định danh NPC (`"NPC_Boran"`, `"NPC_Tut_Room4"`...). |
| **Characteristic** | `NPCBase.state` | `NPCState` | `NPCState.Trapped`| Trạng thái hiện tại. |
| **Characteristic** | `NPCBase.isCoreFunctionalNPC`| `bool` | `false` | `true` cho Boran (bất tử, không bao giờ chuyển `Missed`); `false` cho `EventNPC`. |
| **Characteristic** | `BoranNPC.forgeLocation` | `Transform` | `AREA-03` | Vị trí Lò rèn tại Trạm Nghỉ Đầu Gió. |
| **Action** | `NPCBase.OnRescued` | `virtual void ()` | N/A | **Hàm ảo đa hình (được `ShadowCage.BreakCage()` gọi):** Đổi `state = Rescued`, ghi vào `GameSaveData.rescuedNPCIDs`, VFX bay về Hub và Autosave. |
| **Action** | `NPCBase.Interact` | `virtual void ()` | N/A | Mở `DialogueUI` trò chuyện (phím `[F]`). |
| **Action** | `BoranNPC.OnRescued` | `override void ()` | N/A | Gọi `base.OnRescued()` + dịch chuyển về `forgeLocation` (AREA-03), thắp lửa Lò rèn vĩnh viễn. |
| **Action** | `BoranNPC.OpenForgeMenu` | `void ()` | N/A | Mở `ForgeShopUI` khi người chơi bấm `[F]` tại Lò rèn. |
| **Action** | `EventNPC.MarkAsMissed` | `void ()` | N/A | Được `RoomController.OnPlayerExitRoom()` gọi nếu chưa cứu: đặt `state = Missed`, ẩn khỏi màn chơi. |
| **Action** | `SacredTreeNPC.EvaluateProsperity`| `void ()` | N/A | Đánh giá số NPC đã cứu & vùng đã thanh tẩy để xác định kết cục (*Desolate / Restored / Radiant Dawn*). |

---

### 2.8.2 `UpgradeManager`, `UpgradeItemSO` & `UpgradeType`
* **Vị trí file:** `Assets/Project/Scripts/Upgrades/`
* **Loại:** `MonoBehaviour` (`UpgradeManager`), `ScriptableObject` (`UpgradeItemSO`), `Enum` (`UpgradeType`)
* **Ý nghĩa:** Quản lý 8 gói nâng cấp tại Lò rèn Boran (MEC-07) và mở rộng sẵn cho Ignis/Lyra.

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Enum** | `UpgradeType` | `enum` | `MaxManaTank` | `MaxManaTank`, `FastRecovery`, `QuickChargeDelay`, `LightAuraRadius`, `EmpoweredPierce`, `RadiantBurstAOE`. |
| **Characteristic** | `UpgradeManager.catalog` | `List<UpgradeItemSO>`| Inspector List | Danh mục toàn bộ nâng cấp bày bán tại Lò rèn. |
| **Characteristic** | `UpgradeManager.purchasedIDs`| `HashSet<string>` | Empty Set | Tập hợp mã nâng cấp đã mua (đồng bộ với `GameSaveData.purchasedUpgradeIDs`). |
| **Characteristic** | `UpgradeItemSO.upgradeID` | `string` | `""` | Mã định danh nâng cấp. |
| **Characteristic** | `UpgradeItemSO.upgradeName`| `string` | `""` | Tên hiển thị (VD: `"Bình Mana Cấp I"`). |
| **Characteristic** | `UpgradeItemSO.costInShards`| `int` | `45` | Giá bằng Light Shards (LS). |
| **Characteristic** | `UpgradeItemSO.type` | `UpgradeType` | `MaxManaTank` | Loại chỉ số tác động. |
| **Characteristic** | `UpgradeItemSO.valueModifier`| `float` | `125.0f` | Giá trị mới áp dụng (125/150/200 Mana, 3%/s, 2.0s, 1.5x Aura, 2 Hit, 1.5u AOE). |
| **Action** | `TryPurchaseUpgrade` | `bool (UpgradeItemSO item)`| N/A | Kiểm tra đủ LS & chưa mua $\rightarrow$ trừ LS, lưu `purchasedIDs`, gọi `ApplyUpgradeEffects()`, Autosave. |
| **Action** | `ApplyUpgradeEffects` | `void (UpgradeItemSO item)`| N/A | Áp chỉ số lên `PlayerStats` (Mana max), `PlayerCombat` (Regen/Delay/Đạn), hoặc `PlayerLightAura` (Bán kính sáng 1.5×). |
| **Action** | `IsPurchased` | `bool (string id)` | N/A | Kiểm tra gói nâng cấp đã sở hữu hay chưa. |

---

### 2.8.3 `CodexManager` & `MemoryFragmentSO`
* **Vị trí file:** `Assets/Project/Scripts/Upgrades/CodexManager.cs` & `MemoryFragmentSO.cs`
* **Loại:** `MonoBehaviour` (`CodexManager`) & `ScriptableObject` (`MemoryFragmentSO`)
* **Ý nghĩa:** Lưu trữ và hiển thị Thư viện Ký Ức (12 mảnh toàn game; Vùng Gió có Mảnh #01 tại nhánh Boran LVL-02 và Mảnh #02 tại nhánh phụ LVL-03).

| Phân loại | Tên Định danh | Kiểu / Chữ ký | Giá trị mặc định | Mô tả chi tiết & Chức năng |
| :--- | :--- | :--- | :--- | :--- |
| **Characteristic** | `MemoryFragmentSO.fragmentID`| `int` | `1` | Số thứ tự mảnh ký ức (1–12). |
| **Characteristic** | `MemoryFragmentSO.title` | `string` | `""` | Tiêu đề mẩu ký ức. |
| **Characteristic** | `MemoryFragmentSO.loreContent`| `string` | `""` | Nội dung cốt truyện cổ đại. |
| **Characteristic** | `CodexManager.allFragments`| `List<MemoryFragmentSO>`| Inspector List | Toàn bộ danh sách 12 asset mảnh ký ức. |
| **Characteristic** | `CodexManager.unlockedIDs` | `HashSet<int>` | Empty Set | Các ID đã thu thập (đồng bộ với `GameSaveData.collectedMemoryFragmentIDs`). |
| **Action** | `UnlockFragment` | `void (int id)` | N/A | Mở khóa mảnh ký ức, đồng bộ Save và thông báo trên HUD. |
| **Action** | `GetUnlockedList` | `List<MemoryFragmentSO> ()` | N/A | Trả về danh sách mảnh đã mở để hiển thị trong `MainMenuUI` (Codex). |

---

## 2.9 Nhóm Giao diện Người dùng (User Interface - UI Systems)

### 2.9.1 Các màn hình UI: `HUDController`, `MapUI`, `ForgeShopUI`, `CannonSelectUI`, `DialogueUI`, `PauseMenuUI`, `MainMenuUI`
* **Vị trí file:** `Assets/Project/Scripts/UI/`
* **Loại:** `MonoBehaviour`

| Tên Lớp (Class) | Thành phần hiển thị (Characteristics) | Phương thức cập nhật (Actions) | Liên kết & Quy tắc GDD |
| :--- | :--- | :--- | :--- |
| **`HUDController`** | - `GameObject[] healthOrbs` (3 đốm sáng: 3 HP Campaign / 3 Lives Survival)<br>- `Slider manaBar` (vạch chia mỗi 20 điểm)<br>- `TextMeshProUGUI lightShardCounter` (max 999)<br>- `GameObject windDirectionIcon`<br>- `CanvasGroup bulletTimeVignette`<br>- `GameObject autosaveIcon` | `+ UpdateHealth(int hp)`<br>`+ UpdateMana(float mana)`<br>`+ UpdateShards(int count)`<br>`+ ShowAutosaveIndicator()`<br>`+ SetHUDVisible(bool visible)` | Luôn hiển thị 3 đốm sáng ở cả 2 chế độ. Tự động Fade-out trong `Cutscene` (gồm cả lúc bắn Đại Bác). |
| **`MapUI`** | - `GameObject mapPanel` (Room Grid Map)<br>- Marker: Checkpoint (lửa xanh), Hub (Cổ Thụ), Đại Bác (icon pháo), NPC gặp nạn (dấu hỏi vàng) | `+ ToggleMap(bool show)`<br>`+ RefreshExploredRooms(WorldState s)` | Bật/tắt bằng phím `Tab` hoặc `M`. Phòng chưa đi phủ Fog of War. |
| **`ForgeShopUI`** | - `GameObject shopPanel`<br>- `Transform itemSlotContainer`<br>- `TextMeshProUGUI lockedTooltip` | `+ OpenShop(UpgradeManager mgr, bool isBoranRescued)`<br>`+ CloseShop()` | Nếu chưa cứu Boran, hiển thị trạng thái khóa + tooltip gợi ý cứu Boran tại LVL-02. |
| **`CannonSelectUI`** | - `GameObject panel`<br>- `Transform optionContainer`<br>- `GameObject destinationButtonPrefab`<br>- `Button exitButton` | `+ OpenMenu(TravelCannon origin, List<CannonStationSO> dests)`<br>`+ OnSelectDestination(CannonStationSO dest)`<br>`+ CloseMenu()` | Hiện danh sách các Khẩu Đại Bác đã mở khóa khác + nút **`Exit`** tắt bảng chọn. |
| **`DialogueUI`** | - `GameObject dialogueBox`<br>- `TextMeshProUGUI speakerName`<br>- `TextMeshProUGUI dialogueText` | `+ ShowDialogue(string name, string text)`<br>`+ ShowNotification(string msg)`<br>`+ CloseDialogue()` | Dùng cho hội thoại NPC, Bia Ký Ức, Gợi ý Checkpoint và thông báo `"Tạm chưa mở khóa"` của Đại Bác. |
| **`PauseMenuUI`** | - `GameObject pausePanel`<br>- Các nút: Resume, Controls & Hints, Settings, Return to Main Menu (kèm hộp thoại xác nhận) | `+ TogglePause()`<br>`+ OnResumeClicked()`<br>`+ OnReturnToTitleClicked()` | Phím `Esc`: gọi `GameManager.ChangeState(Paused)`, đóng băng vật lý, giảm 50% nhạc nền. |
| **`MainMenuUI`** | - Nút Continue (mờ nếu trống), New Game (chọn Slot 1–3 & Mode), Settings (Audio, Screen Shake, Assist Mode), Codex, Credits, Quit | `+ OnContinueClicked()`<br>`+ OnNewGameSelected(int slot, string mode)`<br>`+ OnSettingsClicked()` | Giao tiếp trực tiếp với `SaveManager` và `CodexManager`. |

---

# 3. CHIẾN LƯỢC MỞ RỘNG VÙNG LỬA & VÙNG RỪNG (SCALABILITY BLUEPRINT)

Mọi thành phần cốt lõi đều được thiết kế theo nguyên lý **Open/Closed** để bổ sung Vùng Lửa (LVL-04/05) và Vùng Rừng Ánh Sáng (LVL-06/07) bằng cách **thêm Asset & Subclass mới mà không sửa code cốt lõi**:

1. **Mở rộng Đạn Nguyên Tố:** Tạo thêm asset `FlameBullet.asset` (nổ nhiệt phá khối đá nứt) và `FloraBullet.asset` (bắn 3 tia `Triple Shot`) dựa trên `ProjectileDataSO`, gán mã kỹ năng `"Ability_FlameShot"` / `"Ability_TripleShot"` vào `PlayerAbilities`.
2. **Mở rộng Mạng lưới Đại Bác:** Khi dựng xong Vùng Lửa / Vùng Rừng, chỉ cần tạo thêm `Cannon_LVL05_BossPrep.asset`, `Cannon_LVL07_BossPrep.asset` (`CannonStationSO`) và thêm vào `CannonNetworkManager.allStations` $\rightarrow$ `CannonSelectUI` và `SaveManager` tự động hỗ trợ mà không cần viết thêm dòng code nào.
3. **Mở rộng Kẻ địch & Bẫy mới:** Quái Vùng Lửa/Rừng kế thừa `EnemyBase`; Dung nham (`LavaHazard`) kế thừa `DarkSpikes`; Boss Lửa/Rừng kế thừa `BossBase`.
4. **Mở rộng NPC & Lò rèn:** Thợ rèn Ignis và Dược sư Lyra kế thừa `NPCBase`, dùng lại Prefab `ShadowCage` (nhờ đa hình `NPCBase jailedNPC`), tự động lưu trạng thái vào `GameSaveData.rescuedNPCIDs` và bổ sung `UpgradeItemSO` vào `UpgradeManager`.

---

# 4. MÃ NGUỒN MERMAID CLASS DIAGRAM TOÀN DIỆN CHO DRAW.IO

Copy toàn bộ đoạn code bên dưới $\rightarrow$ mở **draw.io** $\rightarrow$ **Arrange (Sắp xếp) > Insert (Chèn) > Advanced (Nâng cao) > Mermaid** $\rightarrow$ Dán và nhấn **Insert** (sau đó chọn **Bố cục > Hữu cơ...** để căn chỉnh thoáng đẹp):

```text
classDiagram
    direction TB

    %% ==========================================
    %% 1. ENUMS (8 TRẠNG THÁI & PHÂN LOẠI)
    %% ==========================================
    class GameState {
        <<enumeration>>
        Boot
        MainMenu
        Gameplay
        BulletTime
        Paused
        Respawning
        Cutscene
        GameOver
        Victory
    }

    class MirrorType {
        <<enumeration>>
        Fixed_45Deg
        Steerable_Pedestal
        AutoRotating_Clockwise
    }

    class BloomState {
        <<enumeration>>
        Ready
        FlareActive
        Withered
    }

    class WindZoneType {
        <<enumeration>>
        Gust_Horizontal
        Updraft_Vertical
    }

    class CannonState {
        <<enumeration>>
        Locked
        Unlocked_Active
        Launching
        Receiving
    }

    class BossPhase {
        <<enumeration>>
        Phase1_Shockwaves
        Phase2_VortexStorm
        Vulnerable_Exposed
    }

    class NPCState {
        <<enumeration>>
        Trapped
        Rescued
        Missed
    }

    class UpgradeType {
        <<enumeration>>
        MaxManaTank
        FastRecovery
        QuickChargeDelay
        LightAuraRadius
        EmpoweredPierce
        RadiantBurstAOE
    }

    %% ==========================================
    %% 2. INTERFACES (3 HỢP ĐỒNG ĐA HÌNH)
    %% ==========================================
    class IInteractable {
        <<interface>>
        +string InteractionPrompt
        +float HoldDuration
        +Interact()
        +Highlight(bool state)
    }

    class IDamageable {
        <<interface>>
        +TakeDamage(int amount)
    }

    class IActivatable {
        <<interface>>
        +Activate()
        +Deactivate()
    }

    %% ==========================================
    %% 3. CORE FRAMEWORK, SAVE, CANNON NETWORK & ROOM
    %% ==========================================
    class GameManager {
        -GameState currentState
        -SaveManager saveManager
        -AudioManager audioManager
        -CannonNetworkManager cannonNetwork
        -CameraZoomController zoomController
        +ChangeState(GameState newState)
        +SetBulletTime(bool active)
        +LoadSceneAsync(string sceneName)
        +TriggerGameOver()
        +TriggerVictory()
    }

    class SaveManager {
        -GameSaveData currentData
        -WorldState worldState
        -string savePath
        +SaveGame(int slotIndex)
        +LoadGame(int slotIndex) GameSaveData
        +TriggerAutosave()
    }

    class GameSaveData {
        +int saveSlotIndex
        +string gameMode
        +bool usedAssistMode
        +string currentSceneName
        +Vector3 lastCheckpointPosition
        +int currentHealth
        +float currentMaxMana
        +int lightShardsCount
        +List~string~ purchasedUpgradeIDs
        +List~string~ unlockedAbilityIDs
        +List~string~ openedGateIDs
        +List~string~ unlockedCannonIDs
        +List~string~ purifiedAreaIDs
        +List~string~ rescuedNPCIDs
        +List~string~ missedNPCIDs
        +List~int~ collectedMemoryFragmentIDs
    }

    class WorldState {
        -HashSet~string~ openedGates
        -HashSet~string~ unlockedCannons
        -HashSet~string~ purifiedAreas
        -HashSet~string~ exploredRoomIDs
        +RegisterGateOpened(string gateID)
        +RegisterCannonUnlocked(string cannonID)
        +RegisterAreaPurified(string areaID)
    }

    class CannonNetworkManager {
        -List~CannonStationSO~ allStations
        -string pendingArrivalCannonID
        +UnlockCannon(string cannonID)
        +IsCannonUnlocked(string cannonID) bool
        +GetAvailableDestinations(string excludeID) List~CannonStationSO~
        +StartCannonTravel(TravelCannon origin, CannonStationSO dest)
    }

    class RoomController {
        -string roomID
        -bool isCleared
        -float stuckTimer
        -int missedShotCount
        -CheckpointAltar[] roomCheckpoints
        -List~EventNPC~ roomEventNPCs
        +OnPlayerEnterRoom()
        +OnPlayerExitRoom()
        +RegisterMissedShot()
        +EvaluateHintTier()
        +ClearRoom()
    }

    class AudioManager {
        -AudioSource bgmSource
        -AudioSource sfxSource
        -AudioLowPassFilter lowPassFilter
        +PlayBGM(SoundData data)
        +PlaySFX(SoundData data)
        +SetLowPassFilter(bool active)
    }

    class SoundData {
        <<ScriptableObject>>
        +string soundID
        +AudioClip clip
        +float volume
        +bool isLooping
    }

    class CameraFollow2D {
        -Transform targetPlayer
        -float smoothSpeed
        -Vector2 lookAheadOffset
        -bool enableScreenShake
        +FollowUpdate()
    }

    class CameraZoomController {
        -Camera cam
        -float baseOrthoSize
        +ZoomBulletTime(bool inBulletTime)
        +ZoomBossArena(bool inArena)
    }

    GameManager --> GameState
    GameManager *-- SaveManager
    GameManager *-- AudioManager
    GameManager *-- CannonNetworkManager
    GameManager *-- CameraZoomController
    SaveManager *-- GameSaveData
    SaveManager *-- WorldState
    AudioManager o-- SoundData
    CannonNetworkManager ..> WorldState : Syncs Unlocked Cannons
    CannonNetworkManager ..> SaveManager : Triggers Autosave
    RoomController ..> WorldState : Registers Explored Room

    %% ==========================================
    %% 4. PLAYER ARCHITECTURE (SRP MODULES)
    %% ==========================================
    class PlayerController {
        -PlayerMovement movement
        -PlayerCombat combat
        -PlayerAbilities abilities
        -PlayerStats stats
        -PlayerInteraction interaction
        -PlayerLightAura aura
        +Awake()
        +SetControlEnabled(bool enabled)
    }

    class PlayerStats {
        -int currentHealth
        -int maxHealth
        -float currentMana
        -float maxMana
        -int lightShards
        -bool isInvulnerable
        -float iFrameTimer
        -Vector2 lastSafeGroundedPos
        +TakeDamage(int dmg, Vector2 sourcePos)
        +FallIntoAbyss()
        +Heal(int amount)
        +ConsumeMana(float amount) bool
        +AddLightShards(int amount)
        +RestoreFullStats()
        +SetSafeGroundedPos(Vector2 pos)
    }

    class PlayerMovement {
        -float moveSpeed
        -float accelTime
        -float decelTime
        -float jumpForce
        -float gravity
        -bool isGrounded
        -float coyoteTimer
        -bool isOneWayDropping
        -float windSpeedMultiplier
        +Move(Vector2 input)
        +Jump()
        +DropThroughPlatform()
        +SetMovementLock(bool isLocked)
        +SetWindSpeedMultiplier(float mult)
    }

    class PlayerAbilities {
        -HashSet~string~ unlockedAbilities
        -bool canAirDash
        -float dashSpeed
        -float dashDuration
        -float groundDashCooldown
        -float launchSpeed
        -float launchStraightDuration
        -float launchGravityBlendDuration
        +HasAbility(string abilityID) bool
        +UnlockAbility(string abilityID)
        +TryExecuteDash()
        +TryExecuteLaunch(Vector2 mousePos)
        +ResetAirDash()
    }

    class PlayerCombat {
        -float manaCostPerShot
        -float bulletTimeScale
        -float normalRegenRate
        -float fastRegenRate
        -float fastRegenDelay
        -float aimHoldThreshold
        -GameObject projectilePrefab
        -GameObject floatingOrbPrefab
        -LineRenderer aimLaser
        +StartAimHold()
        +ReleaseAimFire()
        +ExecuteTapAim()
        +CancelAim()
        +ForceInterruptAim()
        +EnterBulletTime()
        +ExitBulletTime()
        +UpdateManaRegen()
    }

    class PlayerInteraction {
        -float interactRadius
        -IInteractable currentTarget
        -float holdProgressTimer
        +ScanForInteractables()
        +HandleInteractionInput(bool isHeld)
    }

    class PlayerLightAura {
        -Light2D pointLight
        -float baseRadius
        -float currentRadius
        +SetAuraScale(float mult)
        +TriggerDamageFlicker()
    }

    PlayerController *-- PlayerStats
    PlayerController *-- PlayerMovement
    PlayerController *-- PlayerAbilities
    PlayerController *-- PlayerCombat
    PlayerController *-- PlayerInteraction
    PlayerController *-- PlayerLightAura
    CameraFollow2D ..> PlayerController : Tracks Target
    PlayerCombat ..> GameManager : Toggles BulletTime
    PlayerStats ..> PlayerCombat : Interrupts Aim on Damage
    PlayerStats ..> PlayerLightAura : Triggers Flicker
    PlayerInteraction ..> IInteractable : Scans & Interacts

    %% ==========================================
    %% 5. PROJECTILES, ORB & PICKUPS
    %% ==========================================
    class ProjectileDataSO {
        <<ScriptableObject>>
        +string projectileName
        +float baseSpeed
        +int maxBounceLimit
        +float lifeTime
        +float circleCastRadius
        +float baseExplosionRadius
    }

    class LightProjectile {
        -ProjectileDataSO config
        -Vector2 moveDirection
        -int bounceCount
        -bool isEmpowered
        -bool hasRadiantBurst
        +Initialize(Vector2 dir, ProjectileDataSO cfg, bool empowered, bool burst)
        +FixedUpdateSweep()
        +Reflect(Vector2 hitNormal)
        +Explode()
        +ApplyWindForce(Vector2 windDelta)
    }

    class FloatingLightOrb {
        -float remainingLifeTime
        -bool isBeingAimed
        +OnOrbClicked()
        +CancelOrbAim()
    }

    class EnemyProjectile {
        -float speed
        -Vector2 direction
        +Initialize(Vector2 dir, float spd)
        +CancelByLight()
    }

    class LightShardPickup {
        -int shardValue
        -float magnetRadius
        +AttractToPlayer(Transform player)
    }

    PlayerCombat ..> LightProjectile : Spawns (Hold)
    PlayerCombat ..> FloatingLightOrb : Spawns (Tap)
    FloatingLightOrb ..> LightProjectile : Fires on 2nd Click
    LightProjectile o-- ProjectileDataSO
    LightProjectile ..> EnemyProjectile : Cancels on Clash
    LightShardPickup ..> PlayerStats : Adds LS (2.5u Magnet)

    %% ==========================================
    %% 6. ENVIRONMENT, PUZZLES, CANNON & HAZARDS
    %% ==========================================
    class ReflectionMirror {
        -MirrorType type
        -float currentAngle
        -float autoRotationSpeed
        +RotateStep(float angle)
        +GetNormalVector() Vector2
        +OnProjectileHit(LightProjectile p)
    }

    class RemotePedestal {
        -ReflectionMirror linkedMirror
        -LineRenderer energyCable
        +Interact()
        +Highlight(bool state)
    }

    class RadiantBloom {
        -BloomState state
        -float flareDuration
        -float rechargeDuration
        -Light2D ambientRoomLight
        +OnHitByLightProjectile()
        +RadiantFlareRoutine()
    }

    class EnergyPylon {
        -bool isActive
        -bool isTimed
        -float timerDuration
        -List~IActivatable~ targets
        +TakeDamage(int amount)
        +TriggerActivation()
        +Deactivate()
    }

    class SealGate {
        -string gateID
        -bool isOpen
        +Activate()
        +Deactivate()
        +OnPlayerPassThrough()
    }

    class LightPressurePad {
        -List~IActivatable~ targets
        +OnTriggerEnter2D(Collider2D col)
    }

    class WindZone {
        -WindZoneType zoneType
        -Vector2 windVector
        -float windForce
        -float tailwindMultiplier
        -float headwindMultiplier
        -bool isPeriodic
        -float blowTime
        -float pauseTime
        +ApplyToPlayer(PlayerMovement pm, PlayerAbilities pa)
        +DeflectProjectile(LightProjectile p)
    }

    class CannonStationSO {
        <<ScriptableObject>>
        +string cannonID
        +string displayName
        +string sceneName
        +bool isHubBaseCannon
    }

    class TravelCannon {
        -CannonStationSO stationData
        -CannonState currentState
        -Transform barrelPivot
        -Transform muzzlePoint
        -Transform landingPoint
        -float launchAngle
        -float chargeDuration
        -float launchFlySpeed
        +Interact()
        +Highlight(bool state)
        +Activate()
        +Deactivate()
        +PlayLaunchSequence(CannonStationSO target)
        +PlayLandingSequence(PlayerController player)
    }

    class CheckpointAltar {
        -string checkpointID
        -Transform respawnPoint
        -bool isHintActive
        -string hintText
        +OnTriggerEnter2D(Collider2D col)
        +ActivateHintMode()
        +Interact()
        +Highlight(bool state)
    }

    class ShadowCage {
        -int durability
        -NPCBase jailedNPC
        +TakeDamage(int amount)
        +Interact()
        +Highlight(bool state)
        +BreakCage()
    }

    class LoreStele {
        -int fragmentID
        +Interact()
        +Highlight(bool state)
    }

    class DarkSpikes {
        -int damageAmount
        -bool respawnToSafeGround
        +OnCollisionEnter2D(Collision2D col)
    }

    class DarkAbyss {
        +OnTriggerEnter2D(Collider2D other)
    }

    IInteractable <|.. RemotePedestal
    IInteractable <|.. TravelCannon
    IInteractable <|.. CheckpointAltar
    IInteractable <|.. ShadowCage
    IInteractable <|.. LoreStele
    IDamageable <|.. EnergyPylon
    IDamageable <|.. ShadowCage
    IActivatable <|.. SealGate
    IActivatable <|.. TravelCannon

    ReflectionMirror --> MirrorType
    RadiantBloom --> BloomState
    WindZone --> WindZoneType
    TravelCannon --> CannonState
    TravelCannon o-- CannonStationSO
    CannonNetworkManager o-- CannonStationSO
    TravelCannon ..> CannonNetworkManager : Requests Travel / Unlock
    RemotePedestal --> ReflectionMirror : Controls 45-deg via Cable
    EnergyPylon --> IActivatable : Activates Gate / Cannon
    LightPressurePad --> IActivatable : Activates Gate
    LightProjectile ..> ReflectionMirror : Bounces (max 3)
    LightProjectile ..> RadiantBloom : Hits -> 8s Flare / 60s Recharge
    LightProjectile ..> IDamageable : Deals Damage
    ShadowCage --> NPCBase : Jails / Calls OnRescued
    WindZone ..> LightProjectile : Deflects
    WindZone ..> PlayerMovement : Modifies Speed / Lift
    DarkSpikes ..> PlayerStats : Deals 1 Damage
    DarkAbyss ..> PlayerStats : Pitfall Respawn
    CheckpointAltar ..> SaveManager : Triggers Autosave
    SealGate ..> SaveManager : Triggers Autosave
    RoomController ..> CheckpointAltar : Activates Tier-3 Hint (90s)

    %% ==========================================
    %% 7. ENEMIES & BOSS HIERARCHY
    %% ==========================================
    class EnemyBase {
        <<abstract>>
        #int maxHealth
        #int currentHealth
        -bool hasShadowShield
        -bool isStunned
        -float stunDuration
        -int minShards
        -int maxShards
        +TakeDamage(int amount)
        +BreakShield()
        +ApplyStun(float dur)
        +Die()
        +DropLoot()
    }

    class ShadowCrawler {
        -float patrolSpeed
        -float detectRange
        +PatrolUpdate()
        +ChargePlayer()
    }

    class ShadowSlime {
        -float moveSpeed
        -float attackRange
        -float shootInterval
        -float telegraphDuration
        +TelegraphSquash()
        +ShootThorn()
    }

    class ShadowWeaver {
        -float dropSpeed
        -BoxCollider2D ceilingSensor
        +DetectPlayerBelow()
        +DropAttack()
        +HoverAndShootWeb()
    }

    class ShadowSpitter {
        -float shootInterval
        -float projectileSpeed
        +FireThornCycle()
    }

    class BossBase {
        <<abstract>>
        #BossPhase currentPhase
        #string areaID
        #string rewardAbilityID
        +SetPhase(BossPhase nextPhase)
        +Die()
    }

    class TutorialDarkCore {
        -float shockwaveInterval
        +EmitShockwave()
    }

    class WindCoreBoss {
        -WindZone bossVortex
        -ReflectionMirror[] arenaMirrors
        +Phase1_Shockwaves()
        +Phase2_VortexStorm()
        +ExposeCoreWeakpoint()
    }

    IDamageable <|.. EnemyBase
    EnemyBase <|-- ShadowCrawler
    EnemyBase <|-- ShadowSlime
    EnemyBase <|-- ShadowWeaver
    EnemyBase <|-- ShadowSpitter
    EnemyBase <|-- BossBase
    BossBase <|-- TutorialDarkCore
    BossBase <|-- WindCoreBoss
    BossBase --> BossPhase

    ShadowSlime ..> EnemyProjectile : Fires (6 u/s)
    ShadowSpitter ..> EnemyProjectile : Fires (8 u/s)
    ShadowWeaver ..> EnemyProjectile : Fires Web
    EnemyBase ..> LightShardPickup : Drops 1-3 LS
    EnemyBase ..> PlayerStats : Contact Damage
    BossBase ..> PlayerAbilities : Unlocks Reward Ability
    BossBase ..> WorldState : Purifies Area

    %% ==========================================
    %% 8. NPCS, LORE & UPGRADES (DATA-DRIVEN)
    %% ==========================================
    class NPCBase {
        <<abstract>>
        #string npcID
        #NPCState state
        #bool isCoreFunctionalNPC
        +OnRescued()
        +Interact()
        +Highlight(bool state)
    }

    class BoranNPC {
        -Transform forgeLocation
        +OnRescued()
        +OpenForgeMenu()
    }

    class SacredTreeNPC {
        +GiveCampaignGuidance()
        +EvaluateProsperity()
    }

    class EventNPC {
        +MarkAsMissed()
    }

    class UpgradeItemSO {
        <<ScriptableObject>>
        +string upgradeID
        +string upgradeName
        +int costInShards
        +UpgradeType type
        +float valueModifier
        +Sprite icon
    }

    class UpgradeManager {
        -List~UpgradeItemSO~ catalog
        -HashSet~string~ purchasedIDs
        +TryPurchaseUpgrade(UpgradeItemSO item) bool
        +ApplyUpgradeEffects(UpgradeItemSO item)
        +IsPurchased(string id) bool
    }

    class MemoryFragmentSO {
        <<ScriptableObject>>
        +int fragmentID
        +string title
        +string loreContent
    }

    class CodexManager {
        -List~MemoryFragmentSO~ allFragments
        -HashSet~int~ unlockedIDs
        +UnlockFragment(int id)
        +GetUnlockedList() List~MemoryFragmentSO~
    }

    IInteractable <|.. NPCBase
    NPCBase <|-- BoranNPC
    NPCBase <|-- SacredTreeNPC
    NPCBase <|-- EventNPC
    NPCBase --> NPCState
    RoomController ..> EventNPC : Marks Missed on Exit
    UpgradeItemSO --> UpgradeType
    BoranNPC --> UpgradeManager : Hosts Forge Shop
    UpgradeManager o-- UpgradeItemSO
    UpgradeManager ..> PlayerStats : Upgrades Max Mana
    UpgradeManager ..> PlayerCombat : Upgrades Regen / Bullet
    UpgradeManager ..> PlayerLightAura : Scales Light Radius 1.5x
    CodexManager o-- MemoryFragmentSO
    LoreStele ..> CodexManager : Unlocks Fragment

    %% ==========================================
    %% 9. USER INTERFACE (UI SYSTEMS)
    %% ==========================================
    class HUDController {
        -GameObject[] healthOrbs
        -Slider manaBar
        -TextMeshProUGUI lightShardCounter
        -GameObject windDirectionIcon
        -CanvasGroup bulletTimeVignette
        -GameObject autosaveIcon
        +UpdateHealth(int hp)
        +UpdateMana(float mana)
        +UpdateShards(int count)
        +ShowAutosaveIndicator()
        +SetHUDVisible(bool visible)
    }

    class MapUI {
        -GameObject mapPanel
        +ToggleMap(bool show)
        +RefreshExploredRooms(WorldState state)
    }

    class ForgeShopUI {
        -GameObject shopPanel
        -Transform itemSlotContainer
        -TextMeshProUGUI lockedTooltip
        +OpenShop(UpgradeManager mgr, bool isBoranRescued)
        +CloseShop()
    }

    class CannonSelectUI {
        -GameObject panel
        -Transform optionContainer
        -GameObject destinationButtonPrefab
        -Button exitButton
        +OpenMenu(TravelCannon origin, List~CannonStationSO~ dests)
        +OnSelectDestination(CannonStationSO dest)
        +CloseMenu()
    }

    class DialogueUI {
        -GameObject dialogueBox
        -TextMeshProUGUI speakerName
        -TextMeshProUGUI dialogueText
        +ShowDialogue(string name, string text)
        +ShowNotification(string msg)
        +CloseDialogue()
    }

    class PauseMenuUI {
        -GameObject pausePanel
        +TogglePause()
        +OnResumeClicked()
        +OnReturnToTitleClicked()
    }

    class MainMenuUI {
        +OnContinueClicked()
        +OnNewGameSelected(int slotIndex, string mode)
        +OnSettingsClicked()
    }

    HUDController ..> PlayerStats : Listens to Events
    HUDController ..> GameManager : Listens to State / BulletTime
    MapUI ..> WorldState : Reads Explored / Cannon / NPC
    ForgeShopUI ..> UpgradeManager : Binds Shop Catalog
    CannonSelectUI ..> CannonNetworkManager : Triggers Cannon Travel
    TravelCannon ..> CannonSelectUI : Opens Destination Menu
    TravelCannon ..> DialogueUI : Shows Locked Notice
    DialogueUI ..> NPCBase : Renders Dialogue Text
    PauseMenuUI ..> GameManager : Pauses / Resumes
    MainMenuUI ..> SaveManager : Loads / Creates Slot
```

---

# 5. LỘ TRÌNH TRIỂN KHAI & DANH MỤC KIỂM THỬ KỸ THUẬT

### 5.1 Kế hoạch phân kỳ thực hiện (Sprints):
* **Sprint 1 — Khung điều khiển Tinh Linh (Player SRP & Core):**
  - Tách `PlayerController.cs` thành 6 module độc lập và cập nhật `PlayerInput.inputactions` đầy đủ (A/D, Space, S+Space, Left Ctrl, Chuột trái Tap/Hold, Chuột phải Launch, Tap/Hold `[F]`, `[X]`, `Tab/M`, `Esc`).
  - Dựng `GameManager`, `SaveManager`, `WorldState` và `HUDController`.
* **Sprint 2 — Quang học, Đạo cụ & Hoa Soi Sáng:**
  - Hiện thực `LightProjectile` (CircleCast, max 3 lần nảy, lần 4 nổ AOE 1.5u) và `FloatingLightOrb` (Tap Aim 8.0s).
  - Xây dựng `ReflectionMirror`, `RemotePedestal`, `EnergyPylon` (dùng `IActivatable`), `SealGate`, `LightPressurePad`, và `RadiantBloom` (sáng 8.0s trong đó 5.0s cực đại, hồi lại sau 60.0s).
* **Sprint 3 — LVL-01 Tutorial, Hub & Hệ thống Đại Bác Dịch Chuyển:**
  - Dựng 5 phòng tuyến tính LVL-01 (gồm `EventNPC` Phòng 4, `TutorialDarkCore` Phòng 5) và `RoomController` (hệ thống gợi ý 3 cấp độ).
  - Hiện thực `CannonStationSO`, `CannonNetworkManager`, `TravelCannon` và `CannonSelectUI` (đặt 1 khẩu Base trạng thái `Locked` hiện `"Tạm chưa mở khóa"` khi bấm `[F]`).
* **Sprint 4 — Vùng Gió, Nhiệm vụ Boran & Boss (LVL-02 & LVL-03):**
  - Tích hợp `WindZone` (gió ngang/gió nâng/gió giật), nhiệm vụ cứu `BoranNPC` mở `ForgeShopUI` tại AREA-03.
  - Bố trí Khẩu Đại Bác thứ 2 tại tiền phòng Boss LVL-03 (mở khóa sẽ tự động mở thông với khẩu Base).
  - Hoàn thiện 4 loại quái bóng tối (`ShadowCrawler`, `ShadowSlime`, `ShadowWeaver`, `ShadowSpitter`) và trận đánh trùm `WindCoreBoss`.

### 5.2 Danh mục kiểm thử chất lượng (QA Verification Checklist):
1. **QA-PHY-01 (Quang học):** Bắn đạn vào 2 gương song song $\rightarrow$ Đạn nảy đúng 3 lần, ở lần thứ 4 phát nổ AOE $1.5\text{ units}$ và biến mất.
2. **QA-MEC-01 (Launch & Tap Aim):** Nhấn Chuột phải trên không $\rightarrow$ bị chặn + tiếng buzz; Tap Chuột trái $<0.25\text{s}$ $\rightarrow$ trừ 20 Mana tạo `FloatingLightOrb` tồn tại 8.0s; trúng đòn khi đang Hold ngắm $\rightarrow$ ngắt Bullet Time không mất Mana.
3. **QA-ENV-01 (Hoa Soi Sáng):** Bắn đạn vào `RadiantBloom` $\rightarrow$ Sáng toàn phòng 8.0s (1s bùng nổ, 5s sáng cực đại, 2s mờ dần héo xám) $\rightarrow$ chờ đúng **60.0s** hồi về trạng thái `Ready`.
4. **QA-CAN-01 (Đại Bác Dịch Chuyển):**
   - Trước khi mở khẩu LVL-03: lại gần khẩu Base bấm `[F]` $\rightarrow$ hiện thông báo `"Tạm chưa mở khóa"`.
   - Sau khi mở khóa khẩu tiền phòng Boss LVL-03: cả 2 khẩu chuyển `Unlocked_Active`, lại gần hiện `[F] Use Cannon`. Bấm `[F]` hiện bảng `CannonSelectUI` có tên trạm kia và nút `"Exit"`. Chọn bay $\rightarrow$ xoay nòng, hút Tinh linh, tụ lực, bắn bay khỏi màn hình, chuyển cảnh và Tinh linh đáp từ trên trời xuống đúng `landingPoint`.
5. **QA-QST-01 (NPC Chức năng vs NPC Sự kiện):** Bỏ qua Boran ở LVL-02 sang LVL-03 $\rightarrow$ Boran vẫn ở trong lồng chờ cứu. Bỏ qua `EventNPC` Phòng 4 LVL-01 rời phòng $\rightarrow$ NPC chuyển `Missed`.

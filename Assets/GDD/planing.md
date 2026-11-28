# KẾ HOẠCH SẢN XUẤT & PHÂN CÔNG CHI TIẾT (PRODUCTION PLAN)
## DỰ ÁN: THE CRADLE OF LIGHT — VERTICAL SLICE (VÙNG GIÓ)
**Nhóm thực hiện:** Team 3009 — **Dương Gia Huy (Huy)** & **Nguyễn Hiếu Thuận (Thuận)**  
**Tài liệu cơ sở:** [schema_design.md](schema_design.md) (v2.0) & [Team 3009 - GDD - 23127052_23127125.docx.md](Team%203009%20-%20GDD%20-%2023127052_23127125.docx.md) (v0.2)  
**Ngày lập kế hoạch:** 28/11/2026  
**Mốc 1 — Báo cáo tiến độ:** **14/12/2026** *(dự phòng 16/12)* — Mục tiêu: Chơi thử hoàn chỉnh **LVL-01 (`The Awakening Hollow` — 5 phòng Tutorial)**  
**Mốc 2 — Vấn đáp chính thức:** **25/12/2026** — Mục tiêu: Hoàn thiện **100% Vertical Slice Vùng Gió** (LVL-01 $\rightarrow$ Hub $\rightarrow$ AREA-03 $\rightarrow$ LVL-02 $\rightarrow$ LVL-03 & Boss Lõi Gió + Hệ thống Đại Bác Dịch Chuyển)

---

# MỤC LỤC
1. [ĐÁNH GIÁ HIỆN TRẠNG MÃ NGUỒN & PHÂN ĐỊNH VAI TRÒ](#1-đánh-giá-hiện-trạng-mã-nguồn--phân-định-vai-trò)
2. [QUY TẮC GIT & CHIẾN LƯỢC CHỐNG XUNG ĐỘT (ZERO-CONFLICT PROTOCOL)](#2-quy-tắc-git--chiến-lược-chống-xung-đột-zero-conflict-protocol)
3. [DANH MỤC HẠNG MỤC CÔNG VIỆC CHI TIẾT (MASTER WBS — 10 MODULE)](#3-danh-mục-hạng-mục-công-việc-chi-tiết-master-wbs--10-module)
4. [PHÂN CHIA KHỐI LƯỢNG CÔNG VIỆC GIỮA HUY & THUẬN](#4-phân-chia-khối-lượng-công-việc-giữa-huy--thuận)
5. [LỘ TRÌNH THỰC HIỆN THEO KHUNG 5 NGÀY (28/11 – 25/12/2026)](#5-lộ-trình-thực-hiện-theo-khung-5-ngày-2811--25122026)
6. [DANH SÁCH KIỂM DUYỆT ĐẦU RA (MILESTONE ACCEPTANCE & RISK MITIGATION)](#6-danh-sách-kiểm-duyệt-đầu-ra-milestone-acceptance--risk-mitigation)

---

# 1. ĐÁNH GIÁ HIỆN TRẠNG MÃ NGUỒN & PHÂN ĐỊNH VAI TRÒ

## 1.1 Kiểm kê mã nguồn hiện có trong `GamePJ` (tính đến 28/11/2026)
Qua kiểm tra trực tiếp thư mục `Assets/Project/` và lịch sử commit (`f69e9d7`, `59bb344`):
* **Engine & Packages sẵn có:** Unity 6 (URP 2D `17.6.0`, New Input System `1.20.0`, 2D Aseprite Importer `6.0.0`, 2D Tilemap + Extras, 2D Animation, UGUI/TMP).
* **Code hiện có (do Huy dựng khung ban đầu):**
  * `Assets/Project/Scripts/Player/PlayerController.cs` (119 dòng, namespace cũ `WispShooting.Player`): Đã có di chuyển ngang có gia tốc (`smoothInputX`), nhảy cơ bản (`jumpable`), máu đơn giản (`maxHealth = 100` kèm event `OnHealthChanged`, `OnDeath`) và bắt va chạm Tag `"Ground"`.
  * `Assets/Project/Scripts/Player/PlayerInput.inputactions`: Mới có 2 action (`Movement` A/D/Mũi tên, `Jump` Space).
* **Art hiện có:** `Assets/Project/Art/Player/Player.aseprite`, `Assets/Project/Art/Enviroment/Ground.png`.
* **Cấu hình Unity hiện còn thiếu (`ProjectSettings/TagManager.asset`):** Mới chỉ có 1 Tag (`Ground`), 1 Layer tùy chỉnh (`Ground` ở Layer 6), và 1 Sorting Layer (`Default`). Chưa có các Layer cho đạn, gương, quái, sàn một chiều, hay Sorting Layers cho URP 2D.

## 1.2 Nguyên tắc phân công giữa 2 thành viên
* **Dương Gia Huy (Huy) — Lead Gameplay Feel, Combat Physics, Bosses & 2D Art/Lighting/VFX:**
  * *Sở trường:* Vẽ đồ họa 2D (Aseprite/Sprite/Tilemap), thiết kế nhân vật, thành thạo Unity Editor (Animator, URP 2D Lighting, Shader Graph) và đã nắm sẵn khung `PlayerController.cs` + `PlayerInput.inputactions`.
  * *Phụ trách chính:* Refactor toàn bộ cụm `Player/` (6 module cảm giác tay & Bullet Time), vật lý quang học `Projectiles/` (`CircleCast`, nảy gương, uốn gió), `Camera/`, 2 trận Boss (`TutorialDarkCore`, `WindCoreBoss`), **toàn bộ đồ họa 2D/Animation/VFX/Lighting URP**, và làm Art Pass các màn chơi.
* **Nguyễn Hiếu Thuận (Thuận) — Lead Systems Architecture, Puzzles, Enemy AI, World Persistence, UI & QA:**
  * *Sở trường:* Chịu trách nhiệm toàn bộ các hệ thống xương sống, kiến trúc dữ liệu và cơ chế tương tác.
  * *Phụ trách chính:* Toàn bộ `Interfaces/` & `Enums`, `Core/` (`GameManager`, `SaveManager`/`WorldState`, `CannonNetworkManager`, `RoomController`, `AudioManager`), toàn bộ `Environment/` (Gương, Bệ cần xoay, Hoa Soi Sáng, Lõi năng lượng, Cửa Phong Ấn, Gió, Checkpoint, Đại Bác Dịch Chuyển, Lồng giam, Bia Ký Ức, Bẫy gai/Hố sâu), `Enemies/Base/` & 4 Quái `AI/`, toàn bộ `NPC/` & `Upgrades/` (Boran, Lò rèn, Codex), toàn bộ `UI/` (7 màn hình), Grey-box màn LVL-02/LVL-03, Audio & QA Build.

---

# 2. QUY TẮC GIT & CHIẾN LƯỢC CHỐNG XUNG ĐỘT (ZERO-CONFLICT PROTOCOL)

Để 2 thành viên làm việc song song liên tục trong 28 ngày mà **không gặp Merge Conflict** (đặc biệt với các file YAML của Unity như `.unity`, `.prefab`, `.asset`):

1. **Tách biệt quyền sở hữu thư mục 100% (Strict Folder Ownership):**
   * **Huy độc quyền sửa:** `Scripts/Player/`, `Scripts/Projectiles/`, `Scripts/Core/Camera/`, `Scripts/Enemies/Bosses/`, `Art/`, `Animations/`, `Settings/` (URP Volume/Renderer).
   * **Thuận độc quyền sửa:** `Scripts/Core/` *(trừ `Camera/`)*, `Scripts/Environment/`, `Scripts/Enemies/Base/`, `Scripts/Enemies/AI/`, `Scripts/NPC/`, `Scripts/Upgrades/`, `Scripts/UI/`, `Audio/`.
2. **Đóng băng Hợp đồng Giao tiếp (Contract Freeze — Hoàn tất trước tối 29/11):**
   * Thuận commit trước 3 Interface (`IInteractable`, `IDamageable`, `IActivatable`), 8 Enum và chữ ký Events/Public Methods của `GameManager`/`SaveManager` lên nhánh `dev`. Từ ngày 30/11 trở đi, hai người chỉ gọi nhau qua Interface/Event — **không bao giờ phải sửa file của nhau**.
3. **Quy tắc Prefab 2 tầng (Logic Root vs Visual Child):**
   * Mọi Prefab đều có cấu trúc: `PrefabRoot` (chứa Script logic + Collider do Thuận/chủ sở hữu cấu hình) $\rightarrow$ Con `Visuals` (chứa `SpriteRenderer`, `Animator`, `Light2D`, `ParticleSystem` do Huy vẽ & gắn). Hai người không sửa cùng 1 Prefab trong cùng một ngày; khi bàn giao Prefab sẽ nhắn chốt trên nhóm.
4. **Khóa quyền sửa Scene (One Owner Per Scene Per Phase):**
   * Tại một thời điểm, **chỉ duy nhất 1 người được mở và lưu một file `.unity` chính**.
   * Mọi thử nghiệm cơ chế cá nhân phải làm trong Scene riêng: `Scenes/Prototypes/Test_Huy.unity` và `Scenes/Prototypes/Test_Thuan.unity`.
5. **Quy ước Nhánh Git (Branching Workflow):**
   * `main`: Bản ổn định nhất dùng để báo cáo (chỉ merge ở cuối Khung 3 và Khung 6).
   * `dev`: Nhánh tích hợp chung (pull & merge ít nhất 1 lần mỗi cuối ngày).
   * Nhánh cá nhân: `feat/huy-<tinh-nang>` và `feat/thuan-<tinh-nang>`. **Bắt buộc `git pull origin dev` trước khi tạo PR/merge.**

---

# 3. DANH MỤC HẠNG MỤC CÔNG VIỆC CHI TIẾT (MASTER WBS — 10 MODULE)

**Quy ước Mức độ ưu tiên:**
* **`P0-M1` (Bắt buộc Mốc 1 — trước 14/12):** Cần thiết để chơi thử trọn vẹn LVL-01 Tutorial (5 phòng).
* **`P0-M2` (Bắt buộc Mốc 2 — trước 25/12):** Cần thiết để hoàn thiện 100% Vùng Gió (Hub, AREA-03, LVL-02, LVL-03, Boran, Đại Bác, Boss Lõi Gió, Full UI/Save).
* **`P1-Bonus` (Tùy chọn sau P0):** Chỉ làm khi P0 đã hoàn tất và sạch lỗi (Survival Mode, Assist Mode, Extra Polish).

---

### MODULE 00 — Cấu hình Dự án Unity Editor (`M00`)
| Mã | Hạng mục & File / Setting cụ thể | Chi tiết kỹ thuật cần thực hiện | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M00.01` | `ProjectSettings/TagManager.asset` | Tạo đủ Layers: `Player`, `Ground`, `OneWayPlatform`, `Enemy`, `LightProjectile`, `EnemyProjectile`, `MirrorSurface`, `MirrorFrame`, `Interactable`, `Hazard`, `WindZone`. Tạo 6 Sorting Layers: `Background_Parallax`, `Environment_Tilemap`, `Props_Interactive`, `Characters`, `Projectiles_Laser`, `VFX_Foreground`. | `P0-M1` | **Huy** |
| `M00.02` | `ProjectSettings/Physics2DSettings.asset` | Cấu hình Gravity $Y = -25.0$ và Layer Collision Matrix (VD: `LightProjectile` bỏ qua `Player`, va chạm `MirrorSurface`/`MirrorFrame`/`Enemy`/`EnemyProjectile`/`Ground`; `Enemy` bỏ qua `Enemy`; `LightShardPickup` chỉ trigger `Player`). | `P0-M1` | **Huy** |
| `M00.03` | `Assets/Settings/DefaultVolumeProfile.asset` & Shader | Bật URP 2D Global Light + Bloom Post-Processing (cho đạn, tia laser, Hoa Soi Sáng). Tạo Shader Graph 2D `SG_WhiteOutline` để bật/tắt **White Glowing Outline** cho mọi `IInteractable`. | `P0-M1` | **Huy** |
| `M00.04` | `Assets/Project/Scripts/Player/PlayerInput.inputactions` | Bổ sung đủ 10 Action Maps: `Move` (A/D, $\leftarrow$/$\rightarrow$), `Jump` (Space), `DropDown` (S/$\downarrow$ + Space), `Dash` (Left Ctrl), `AimFire` (Left Mouse), `Launch` (Right Mouse), `Interact` (F — hỗ trợ Tap & Hold 2.0s), `CancelAim` (X), `ToggleMap` (Tab/M), `Pause` (Esc). | `P0-M1` | **Huy** |
| `M00.05` | `ProjectSettings/EditorBuildSettings.asset` | Đăng ký thứ tự Scene chuẩn: `MainMenu`, `LVL01_AwakeningHollow`, `AREA02_TheCradle_Hub`, `AREA03_WindsThreshold`, `LVL02_WhisperingCaverns`, `LVL03_EyeOfTheTempest`. | `P0-M2` | **Thuận** |

---

### MODULE 01 — Hợp đồng Interfaces & Enums (`M01`) *(Đóng băng ở Khung 1A)*
| Mã | File C# (`Assets/Project/Scripts/...`) | Các thành phần / chữ ký bắt buộc | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M01.01` | `Environment/Interfaces/IInteractable.cs`<br>`IDamageable.cs`, `IActivatable.cs` | `IInteractable` (`InteractionPrompt`, `HoldDuration`, `Interact()`, `Highlight(bool)`), `IDamageable` (`TakeDamage(int)`), `IActivatable` (`Activate()`, `Deactivate()`). | `P0-M1` | **Thuận** |
| `M01.02` | 8 file Enum theo đúng đường dẫn `schema_design.md` | `GameState`, `MirrorType`, `BloomState`, `WindZoneType`, `CannonState`, `BossPhase`, `NPCState`, `UpgradeType`. | `P0-M1` | **Thuận** |

---

### MODULE 02 — Hệ thống Cốt lõi, Lưu trữ, Phòng, Mạng Đại Bác, Audio & Camera (`M02`)
| Mã | File C# | Hàm & Tính năng cốt lõi | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M02.01` | `Core/GameManager.cs` | Singleton `DontDestroyOnLoad`; `ChangeState()`, `SetBulletTime(bool)` (0.1× + Low-pass + Zoom), `LoadSceneAsync()`, `TriggerGameOver()` (ghi đè Hub chống Alt+F4), `TriggerVictory()`. | `P0-M1` | **Thuận** |
| `M02.02` | `Core/SaveSystem/GameSaveData.cs`<br>`WorldState.cs`, `SaveManager.cs` | `WorldState` (`HashSet<string>` runtime: `openedGates`, `unlockedCannons`, `purifiedAreas`, `exploredRoomIDs`); `GameSaveData` (DTO `List<T>` tương thích `JsonUtility`, có `unlockedAbilityIDs`, `rescuedNPCIDs`, `missedNPCIDs`, `usedAssistMode`); `SaveManager` (`SaveGame`, `LoadGame`, `TriggerAutosave`, mã hóa JSON 3 slots). | `P0-M1` *(cơ bản)*<br>`P0-M2` *(3 slots)* | **Thuận** |
| `M02.03` | `Core/RoomController.cs` | `OnPlayerEnterRoom()`, `OnPlayerExitRoom()` (đánh dấu `EventNPC` thành `Missed`), `RegisterMissedShot()`, `EvaluateHintTier()` (30s nhấp nháy Pylon, 60s/trượt 3 lần bật laser xanh ngọc, 90s bật lửa vàng Checkpoint), `ClearRoom()` (đổi ánh sáng phòng + thưởng 5 LS). | `P0-M1` | **Thuận** |
| `M02.04` | `Core/Audio/SoundData.cs`<br>`Core/Audio/AudioManager.cs` | `SoundData` (SO); `AudioManager`: `PlayBGM()` (fade 2.0s chuyển Suy tàn $\rightarrow$ Hồi sinh), `PlaySFX()` (2D spatial panning), `SetLowPassFilter(bool)` (giảm 50% âm lượng & lọc cao tần khi Bullet Time). | `P0-M1` *(SFX cơ bản)*<br>`P0-M2` *(Full)* | **Thuận** |
| `M02.05` | `Core/CannonNetworkManager.cs` | `allStations`, `pendingArrivalCannonID`, `UnlockCannon()` (mở khẩu ngoài đầu tiên $\rightarrow$ tự mở khẩu Base + Autosave), `GetAvailableDestinations(excludeID)`, `StartCannonTravel()` (điều phối chuỗi bắn $\rightarrow$ chuyển Scene $\rightarrow$ đáp xuống). | `P0-M2` | **Thuận** |
| `M02.06` | `Core/Camera/CameraFollow2D.cs`<br>`CameraZoomController.cs` | Smooth damping follow, `lookAheadOffset` theo hướng đi/chuột, `enableScreenShake` toggle; `ZoomBulletTime` (+10%), `ZoomBossArena` (-15%). | `P0-M1` | **Huy** |

---

### MODULE 03 — Kiến trúc Nhân vật Tinh Linh (`M03`) *(Refactor từ `PlayerController.cs` hiện có)*
| Mã | File C# (`Scripts/Player/`) | Hàm & Tính năng cốt lõi | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M03.01` | `PlayerController.cs` | Chuyển sang namespace `CradleOfLight.Player`. Kết nối `PlayerInput` tới 6 module con, `Awake()` nối event (`stats.OnDamaged += combat.ForceInterruptAim`), `SetControlEnabled(bool)` cho Cutscene/UI/Đại Bác. Cấu hình `CircleCollider2D` đường kính `0.8 units`. | `P0-M1` | **Huy** |
| `M03.02` | `PlayerStats.cs` | 3 HP (Campaign) / 3 Lives (Survival), `maxMana = 100` (nâng lên 125/150/200), `lightShards` (max 999), `iFrameTimer = 1.5s`, `lastSafeGroundedPos`. Các hàm `TakeDamage()` (knockback 0.2s), `FallIntoAbyss()` (0 mất HP trong Campaign), `Heal()`, `ConsumeMana()`, `AddLightShards()`, `RestoreFullStats()`. | `P0-M1` | **Huy** |
| `M03.03` | `PlayerMovement.cs` | `moveSpeed = 4.5f` (accel 0.15s, decel 0.1s), `jumpForce = 12.0f` ($g = 25$), `coyoteTimer = 0.1s`, `DropThroughPlatform()` (S+Space bỏ qua One-way Platform 0.3s), `SetMovementLock(bool)`, `SetWindSpeedMultiplier(float)`. | `P0-M1` | **Huy** |
| `M03.04` | `PlayerAbilities.cs` | `HashSet<string> unlockedAbilities`, `HasAbility()`, `UnlockAbility()`. **Ground Dash** (`dashSpeed = 13`, `0.2s`, cooldown `0.6s`) ở `P0-M1`; **Launch** (`launchSpeed = 20`, `0.2s` bay thẳng + `0.15s` hòa trọng lực, yêu cầu `isGrounded`, 0 Mana) và **Air Dash** (1 lần/nhảy, $g=0$ trong `0.2s`, reset khi chạm đất) hoàn thiện ở `P0-M2`. | `P0-M1` *(Ground Dash)*<br>`P0-M2` *(Launch & Air Dash)* | **Huy** |
| `M03.05` | `PlayerCombat.cs` | Chuột trái Hold ($\ge 0.25\text{s}$): `EnterBulletTime()` (0.1×, khóa A/D, trọng lực rơi 10%, `LineRenderer` dự báo đường nảy) $\rightarrow$ `ReleaseAimFire()` (-20 Mana). Tap ($< 0.25\text{s}$): `ExecuteTapAim()` (-20 Mana ngay, sinh `FloatingLightOrb`). Phím `[X]` `CancelAim()`, `ForceInterruptAim()` (bị đánh không mất Mana), `UpdateManaRegen()` (2/s tự nhiên, sau 3s chờ $\rightarrow$ 25/s theo Game Time). | `P0-M1` | **Huy** |
| `M03.06` | `PlayerInteraction.cs`<br>`PlayerLightAura.cs` | `PlayerInteraction`: quét OverlapCircle `1.8u`, chọn 1 `IInteractable` gần nhất, bật Outline + Prompt, hỗ trợ cả Tap `[F]` và Hold `[F]` (`holdProgressTimer`). `PlayerLightAura`: URP `Light2D` bán kính `4.0u` (mở rộng `1.5×` = `6.0u`), `TriggerDamageFlicker()`. | `P0-M1` | **Huy** |

---

### MODULE 04 — Đạn đạo Quang học, Quả cầu chờ & Mảnh Sáng (`M04`)
| Mã | File C# (`Scripts/Projectiles/`) | Hàm & Tính năng cốt lõi | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M04.01` | `ProjectileDataSO.cs` | ScriptableObject cấu hình: `baseSpeed = 16`, `maxBounceLimit = 3`, `lifeTime = 5.0`, `circleCastRadius = 0.1`, `baseExplosionRadius = 1.5`. Tạo asset `BaseLightBullet.asset`. | `P0-M1` | **Huy** |
| `M04.02` | `LightProjectile.cs` | `FixedUpdateSweep()` dùng `CircleCast2D`, `Reflect(normal)` (nảy tối đa 3 lần, lần 4 tự `Explode()` AOE 1.5u gây sát thương), `ApplyWindForce()`, triệt tiêu `EnemyProjectile`, hỗ trợ cờ nâng cấp `isEmpowered` (2 Hit xuyên khiên) & `hasRadiantBurst`. | `P0-M1` | **Huy** |
| `M04.03` | `FloatingLightOrb.cs` | Quả cầu chờ khi Tap bắn: lơ lửng trên đầu, đếm `remainingLifeTime = 8.0s`, bắt click lần 1 bật Bullet Time, click lần 2 phóng `LightProjectile`, hủy ngắm `[X]` không hoàn Mana. | `P0-M1` | **Huy** |
| `M04.04` | `EnemyProjectile.cs`<br>`LightShardPickup.cs` | `EnemyProjectile`: tia gai/tơ bóng tối (Slime 6 u/s, Spitter 8 u/s), `CancelByLight()`. `LightShardPickup`: tự động hút vào Player trong bán kính `2.5 units`, cộng LS. | `P0-M1` | **Huy** |

---

### MODULE 05 — Môi trường, Giải đố, Đại Bác Dịch Chuyển & Cạm bẫy (`M05`)
| Mã | File C# (`Scripts/Environment/`) | Hàm & Tính năng cốt lõi | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M05.01` | `Puzzles/ReflectionMirror.cs`<br>`Puzzles/RemotePedestal.cs` | `ReflectionMirror`: 3 chế độ (`Fixed_45Deg`, `Steerable_Pedestal`, `AutoRotating_Clockwise`), `RotateStep(45f)`, `GetNormalVector()`, tách Collider mặt kính vs khung viền (khung hấp thụ đạn). `RemotePedestal` (`IInteractable`): bấm `[F]` xoay gương + xung sáng `energyCable`. | `P0-M1` *(Fixed & Pedestal)*<br>`P0-M2` *(Auto-Rotating)* | **Thuận** |
| `M05.02` | `Puzzles/RadiantBloom.cs` | Hoa Soi Sáng (KHÔNG dùng `[F]`, kích hoạt khi trúng `LightProjectile`): chuỗi `8.0s` (1s bùng nổ phơi sáng 0.2 $\rightarrow$ 1.0, **5.0s sáng cực đại**, 2s mờ dần héo xám `Withered`) $\rightarrow$ hồi phục về `Ready` sau **`60.0s`**. | `P0-M1` | **Thuận** |
| `M05.03` | `Puzzles/EnergyPylon.cs`<br>`SealGate.cs`, `LightPressurePad.cs` | `EnergyPylon` (`IDamageable`): vĩnh viễn hoặc hẹn giờ `5.0s` Game Time, kích hoạt `List<IActivatable>`. `SealGate` (`IActivatable`): mở vĩnh viễn 2 chiều, lưu `WorldState`, Autosave khi qua cửa. `LightPressurePad`: chạm thân Tinh linh mở cửa Phòng 1 LVL-01. | `P0-M1` | **Thuận** |
| `M05.04` | `Puzzles/WindZone.cs` | 2 chế độ: `Gust_Horizontal` (tăng $+30\%$ xuôi gió, giảm $-50\%$ ngược gió, gió giật chu kỳ 3s/2s) và `Updraft_Vertical` (lực nâng `8.0 u/s`). Uốn cong `LightProjectile`; kiểm tra `HasAbility("Ability_ZephyrResistance")`. | `P0-M1` *(Gió chu kỳ)*<br>`P0-M2` *(Full khí động học)* | **Thuận** |
| `M05.05` | `Interactables/CheckpointAltar.cs`<br>`ShadowCage.cs`, `LoreStele.cs` | `CheckpointAltar`: lướt qua hồi 100% HP/Mana + Autosave; bế tắc $>90\text{s}$ bật lửa vàng + `[F] Đọc gợi ý`. `ShadowCage`: nhận 2 đạn HOẶC giữ `[F]` `2.0s` để vỡ, gọi `jailedNPC.OnRescued()` (`NPCBase`). `LoreStele`: bấm `[F]` mở khóa `CodexManager`. | `P0-M1` *(Checkpoint & Cage)*<br>`P0-M2` *(LoreStele)* | **Thuận** |
| `M05.06` | `Interactables/CannonStationSO.cs`<br>`Interactables/TravelCannon.cs` | **Hệ thống Đại Bác Dịch Chuyển:** Tạo 2 asset SO (`Cannon_Hub_Base`, `Cannon_LVL03_BossPrep`). Trạng thái `Locked`: bấm `[F]` hiện `"Tạm chưa mở khóa"`. Trạng thái `Unlocked_Active`: hiện `[F] Use Cannon` $\rightarrow$ mở `CannonSelectUI` $\rightarrow$ `PlayLaunchSequence()` (xoay nòng lên trời, hút Tinh linh, tụ lực, bắn khỏi màn hình) & `PlayLandingSequence()` (đáp từ trên trời xuống `landingPoint`). | `P0-M2` | **Thuận** *(Logic)* + **Huy** *(Anim/VFX)* |
| `M05.07` | `Hazards/DarkSpikes.cs`<br>`Hazards/DarkAbyss.cs` | `DarkSpikes`: trừ 1 HP/Life, knockback 0.2s, tái tạo mỏm an toàn + 1.5s i-frames. `DarkAbyss`: gọi `FallIntoAbyss()` (0 mất HP trong Campaign). | `P0-M1` | **Thuận** |

---

### MODULE 06 — Kẻ địch Bóng tối & Trùm (`M06`)
| Mã | File C# (`Scripts/Enemies/`) | Hàm & Tính năng cốt lõi | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M06.01` | `Base/EnemyBase.cs`<br>`Base/BossBase.cs` | `EnemyBase` (`IDamageable`): `protected maxHealth/currentHealth`, `hasShadowShield`, `BreakShield()`, `ApplyStun(1.5f)`, `Die()`, `DropLoot()` (1–3 `LightShardPickup`). `BossBase` kế thừa `EnemyBase`: quản lý `BossPhase`, `areaID`, `rewardAbilityID`, thanh tẩy vùng & Autosave khi chết. | `P0-M1` | **Thuận** |
| `M06.02` | `AI/ShadowSpitter.cs` | Pháo đài cố định trên vách/trần (dùng ngay ở Phòng 4 LVL-01), bắn `EnemyProjectile` `8.0 u/s` mỗi `2.5s`, không khiên (1 đạn chết). | `P0-M1` | **Thuận** |
| `M06.03` | `AI/ShadowCrawler.cs`<br>`AI/ShadowSlime.cs`<br>`AI/ShadowWeaver.cs` | `ShadowCrawler`: tuần tra 2.0 u/s, tầm nhìn 4.0 u, lao húc, có khiên. `ShadowSlime`: đi 1.2 u/s, tầm 6.0 u, telegraph nén thân 0.5s rồi bắn gai 6.0 u/s mỗi 3s, có khiên. `ShadowWeaver`: ngủ trần hang, thả rơi theo cột cảm biến Y, lơ lửng bắn tơ, không khiên. | `P0-M2` | **Thuận** |
| `M06.04` | `Bosses/TutorialDarkCore.cs` | Lõi Bóng Tối sơ khai Phòng 5 LVL-01 (`maxHealth = 3`): phát sóng xung kích chu kỳ `3.0s`, vỡ lõi mở Cánh Cổng Cổ Đại & sáng rực căn phòng. | `P0-M1` | **Huy** |
| `M06.05` | `Bosses/WindCoreBoss.cs` | Trùm cuối LVL-03 (`maxHealth = 6`): Phase 1 sóng xung kích, Phase 2 bão xoáy đổi chiều (`bossVortex`), mở giáp 4.0s (`ExposeCoreWeakpoint`) buộc dùng gương + Bullet Time. Chết $\rightarrow$ rơi Tinh Hoa Gió mở `Ability_AirDash` + `Ability_ZephyrResistance`, thanh tẩy Vùng Gió. | `P0-M2` | **Huy** |

---

### MODULE 07 — NPCs, Kinh tế Lò rèn Boran & Thư viện Codex (`M07`)
| Mã | File C# (`Scripts/NPC/` & `Upgrades/`) | Hàm & Tính năng cốt lõi | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M07.01` | `NPC/Base/NPCBase.cs`<br>`NPC/EventNPC.cs` | `NPCBase` (`IInteractable`): `npcID`, `state`, `isCoreFunctionalNPC`, `virtual OnRescued()`. `EventNPC`: dùng cho NPC thử nghiệm Phòng 4 LVL-01 (rời phòng chưa cứu $\rightarrow$ `MarkAsMissed()`). | `P0-M1` | **Thuận** |
| `M07.02` | `NPC/BoranNPC.cs`<br>`NPC/SacredTreeNPC.cs` | `BoranNPC` (`isCoreFunctionalNPC = true`): bị nhốt trong `ShadowCage` LVL-02, không bao giờ biến mất; cứu xong về `AREA-03` mở `ForgeShopUI`. `SacredTreeNPC` tại Hub: dẫn chuyện, tổng hợp Phồn Vinh. | `P0-M2` | **Thuận** |
| `M07.03` | `Upgrades/UpgradeItemSO.cs`<br>`Upgrades/UpgradeManager.cs` | Tạo đủ 8 asset `UpgradeItemSO` (Mana Tank I/II/III, Fast Recovery, Quick Charge, Light Aura 1.5×, Empowered Light, Radiant Burst). `UpgradeManager`: kiểm tra LS, trừ tiền, lưu `purchasedIDs`, áp chỉ số lên `PlayerStats`/`PlayerCombat`/`PlayerLightAura`, Autosave. | `P0-M2` | **Thuận** |
| `M07.04` | `Upgrades/MemoryFragmentSO.cs`<br>`Upgrades/CodexManager.cs` | Tạo 2 asset Lore Vùng Gió (`Fragment_01` nhánh Boran LVL-02, `Fragment_02` nhánh phụ LVL-03) + bộ khung 12 mảnh cho Codex. | `P0-M2` | **Thuận** |

---

### MODULE 08 — Hệ thống Giao diện Người dùng (`M08`)
| Mã | File C# (`Scripts/UI/`) | Thành phần & Chức năng | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M08.01` | `HUDController.cs` | 3 đốm sáng HP/Lives (nhấp nháy đỏ khi còn 1 HP), thanh Mana có vạch chia mỗi 20 điểm, bộ đếm LS (0–999), icon hướng gió, Vignette tối góc Bullet Time, icon Autosave xoay 1.5s, tự ẩn khi Cutscene. | `P0-M1` | **Thuận** |
| `M08.02` | `DialogueUI.cs`<br>`PauseMenuUI.cs` | `DialogueUI`: khung thoại NPC, Bia Ký Ức, Gợi ý Checkpoint và thông báo `"Tạm chưa mở khóa"` của Đại Bác. `PauseMenuUI` (phím `Esc`): Resume, Controls & Hints, Settings (Audio/Screen Shake), Return to Main Menu. | `P0-M1` | **Thuận** |
| `M08.03` | `ForgeShopUI.cs`<br>`CannonSelectUI.cs` | `ForgeShopUI`: danh sách 8 nâng cấp Boran, làm mờ/đỏ giá khi thiếu LS, hiện tooltip khóa nếu chưa cứu Boran. `CannonSelectUI`: danh sách động các khẩu Đại Bác đã mở khóa + nút `"Exit"`. | `P0-M2` | **Thuận** |
| `M08.04` | `MapUI.cs`<br>`MainMenuUI.cs` | `MapUI` (`Tab`/`M`): Room Grid Map + Fog of War, icon Checkpoint, Hub, Đại Bác, dấu `?` vàng NPC. `MainMenuUI`: Continue, New Game (3 Save Slots + chọn Mode), Settings, Codex, Credits, Quit. | `P0-M2` | **Thuận** |

---

### MODULE 09 — Đồ họa 2D, Hoạt ảnh, VFX & Âm thanh (`M09`)
| Mã | Nhóm Tài nguyên (`Assets/Project/...`) | Chi tiết Asset cần sản xuất | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M09.01` | `Art/Player/` & `Animations/Player/` | Hoàn thiện `Player.aseprite` + Animator Controller: Idle, Move/Lean 15°, Jump, Fall, Aim/Bullet Time, Launch (kéo dẹt giọt nước), Dash, Hit Flash/Recoil, Respawn. | `P0-M1` *(cơ bản)*<br>`P0-M2` *(Launch/AirDash)* | **Huy** |
| `M09.02` | `Art/Enviroment/` — Tilemap & Props LVL-01 | Tilemap Hang đá rêu phong (`LVL-01`) + Props: Bệ Đá Ánh Sáng, Bệ Cần Xoay, Gương Cố Định/Xoay, Hoa Soi Sáng (3 trạng thái: Ready/Bloom/Withered), Cửa Phong Ấn, Bệ Thờ Checkpoint (lửa xanh/vàng), Lồng giam, Bãi gai, Hố tối, hướng dẫn phím vẽ lên vách đá. | `P0-M1` | **Huy** |
| `M09.03` | `Art/Enviroment/` — Tilemap & Props Hub + Vùng Gió | Tilemap & Parallax 2 trạng thái (Suy tàn / Hồi sinh) cho Hub Cổ Thụ (`AREA-02`), Trạm Nghỉ Đầu Gió (`AREA-03` — lạnh lẽo vs lò rèn đỏ lửa), Hẻm Núi Thì Thầm (`LVL-02`), Đỉnh Gió Gầm (`LVL-03`). Thêm Gương Tự Động Xoay (bánh răng), Bia Ký Ức, **Khẩu Đại Bác (`TravelCannon` — trụ + nòng xoay + anim tụ lực/bắn)**. | `P0-M2` | **Huy** |
| `M09.04` | `Art/Enemies/`, `Art/NPCs/` & `Animations/` | Sprites & Animator: `ShadowSpitter`, `TutorialDarkCore`, `EventNPC` (`P0-M1`); `ShadowCrawler`, `ShadowSlime`, `ShadowWeaver`, `WindCoreBoss`, `BoranNPC`, `SacredTreeNPC` (`P0-M2`). Đảm bảo mắt/lõi quái phát sáng rõ trên nền tối & có động tác báo hiệu (telegraph). | `P0-M1` / `P0-M2` | **Huy** |
| `M09.05` | VFX Particles & UI Sprites | VFX: Đuôi sáng Tinh linh, Laser Bullet Time, Crystal Hit Flash (gương), Vụ nổ AOE đạn, **Radiant Bloom Shockwave 8s**, Hạt gió, Hút/Tụ lực/Bắn Đại Bác, Vòng sóng thanh tẩy phòng/vùng. Bộ icon UI tối giản (Đốm HP, Thanh Mana, Mảnh Sáng, Icon gió, Icon Đại Bác). | `P0-M1` / `P0-M2` | **Huy** |
| `M09.06` | `Audio/` (SFX & BGM miễn phí bản quyền) | Tuyển chọn, cắt ghép & gắn `SoundData`: BGM Menu, BGM Suy tàn (gió + piano trầm), BGM Hồi sinh (dây + sáo), BGM Boss; SFX-001..006 (Jump, Dash, Launch Swoosh, Buzz chặn, Bullet Time Hum, Crystal Ping, Bệ xoay cọ xát, Hoa Soi Sáng 3 pha, Đại Bác tụ lực/bắn, Vỡ giáp, Búa lò rèn). | `P0-M1` *(cơ bản)*<br>`P0-M2` *(Full)* | **Thuận** |

---

### MODULE 10 — Dựng Màn chơi, Tích hợp & Kiểm thử QA (`M10`)
| Mã | Hạng mục & Scene | Chi tiết thực hiện | Ưu tiên | Người làm |
| :-: | :--- | :--- | :-: | :-: |
| `M10.01` | `Scenes/LVL01_AwakeningHollow.unity` | Dựng trọn vẹn 5 phòng Tutorial chuẩn GDD 5.2 (Phòng 1: A/D + Bệ đá; Phòng 2: Space/S+Space vượt hố; Phòng 3: Ground Dash vượt gió chu kỳ; Phòng 4: Hoa Soi Sáng + Bệ xoay gương + Spitter + cứu EventNPC; Phòng 5: TutorialDarkCore mở Cổng Cổ Đại). | `P0-M1` | **Thuận** *(Grey-box + Logic)* $\rightarrow$ **Huy** *(Art + Light)* |
| `M10.02` | `Scenes/AREA02_TheCradle_Hub.unity`<br>`AREA03_WindsThreshold.unity` | Hub Cổ Thụ (2 trạng thái héo/sáng + **Khẩu Đại Bác Base `Locked`**), lối dẫn sang AREA-03 (mở khóa vĩnh viễn `Ability_Launch` khi bước vào + khu Lò rèn Boran). | `P0-M2` | **Huy** *(Art/Scene)* + **Thuận** *(Logic/NPC/Cannon)* |
| `M10.03` | `Scenes/LVL02_WhisperingCaverns.unity` | Hẻm núi khí động học: gió ngang lệch đạn, vận dụng Launch, bãi gai, Crawler/Slime, nhánh phụ giải cứu Boran (SQ-01) & Bia Ký Ức #01. | `P0-M2` | **Thuận** *(Grey-box + Puzzle)* $\rightarrow$ **Huy** *(Art + Light)* |
| `M10.04` | `Scenes/LVL03_EyeOfTheTempest.unity` | Phong huyệt thẳng đứng: gió nâng/đổi chiều, câu đố Gương Tự Động Xoay, Weaver/Spitter, Bia Ký Ức #02, **Khẩu Đại Bác Tiền Phòng Boss** (mở khóa liên thông với Base), đấu trường `WindCoreBoss` + lối thông vòng về Hub. | `P0-M2` | **Thuận** *(Grey-box + Cannon/Puzzles)* $\rightarrow$ **Huy** *(Boss Arena + Art)* |
| `M10.05` | `Scenes/MainMenu.unity`, QA & Build `.exe` | Dựng Scene MainMenu; chạy đủ 5 bộ Test Case (`QA-PHY-01`, `QA-MEC-01`, `QA-ENV-01`, `QA-CAN-01`, `QA-QST-01`); kiểm tra độ phân giải `1280×720` & `1920×1080`, Mute Master Audio; tắt phím Debug F1–F3 và đóng gói bản Build `.exe` Windows 60FPS. | `P0-M2` | **Thuận** *(chủ trì QA/Build)* + **Huy** *(Fix Visual/Player Bugs)* |

---

# 4. PHÂN CHIA KHỐI LƯỢNG CÔNG VIỆC GIỮA HUY & THUẬN

Bảng đối chiếu cân bằng tải tổng thể để đảm bảo **không ai bị quá tải và không giẫm chân nhau trên Git**:

| Tiêu chí so sánh | **Dương Gia Huy (Huy)** | **Nguyễn Hiếu Thuận (Thuận)** |
| :--- | :--- | :--- |
| **Số lượng Script C# sở hữu** | **16 scripts** *(độ khó vật lý & feel cao)*:<br>• 7 script `Player/` (`PlayerController`, `Stats`, `Movement`, `Abilities`, `Combat`, `Interaction`, `LightAura`)<br>• 5 script `Projectiles/` (`ProjectileDataSO`, `LightProjectile`, `FloatingLightOrb`, `EnemyProjectile`, `LightShardPickup`)<br>• 2 script `Camera/` (`CameraFollow2D`, `CameraZoomController`)<br>• 2 script `Bosses/` (`TutorialDarkCore`, `WindCoreBoss`) | **36 scripts** *(logic hướng dữ liệu, cấu trúc rõ ràng)*:<br>• 3 `Interfaces/` + 8 `Enums`<br>• 7 script `Core/` (`GameManager`, `SaveManager`, `GameSaveData`, `WorldState`, `CannonNetworkManager`, `RoomController`, `AudioManager` + `SoundData`)<br>• 14 script `Environment/` (Mirrors, Pedestal, RadiantBloom, Pylon, Gate, Pad, WindZone, Checkpoint, TravelCannon + SO, Cage, Stele, Spikes, Abyss)<br>• 6 script `Enemies/` (`EnemyBase`, `BossBase`, 4 Quái `AI/`)<br>• 8 script `NPC/` & `Upgrades/` (4 NPC, Upgrade/Codex + SO)<br>• 7 script `UI/` (`HUD`, `Dialogue`, `Pause`, `ForgeShop`, `CannonSelect`, `Map`, `MainMenu`) |
| **Khối lượng Art / Editor / Non-Code** | **Rất lớn (~55% thời gian của Huy):**<br>• Cấu hình Layers, Physics2D, URP 2D Lighting, Bloom, Shader Outline, `PlayerInput`<br>• 100% Art 2D (Player, Tilemap 5 vùng 2 trạng thái, 12+ Props, 4 Quái, 2 Boss, 3 NPC, Đại Bác, Icon UI)<br>• Toàn bộ Animator & Particle VFX<br>• Art Pass & Lighting cho 5 màn chơi | **Trung bình (~30% thời gian của Thuận):**<br>• Cấu hình 15+ ScriptableObject Assets (8 Upgrades, 2 Lore, 2 Cannons, Audio)<br>• Tuyển chọn & tích hợp toàn bộ SFX/BGM<br>• Thiết kế Grey-box bố cục câu đố cho LVL-01, LVL-02, LVL-03<br>• Dựng Scene MainMenu, chạy Full QA Checklist & Build `.exe` |
| **Ước lượng tổng tỷ trọng công sức** | **~50%** | **~50%** |

---

# 5. LỘ TRÌNH THỰC HIỆN THEO KHUNG 5 NGÀY (28/11 – 25/12/2026)

Tổng quỹ thời gian: **28 ngày** — chia thành **5 Khung chuẩn (mỗi khung 5 ngày)** + **Khung 6 nước rút (3 ngày cuối chốt bản Build & Vấn đáp)**.

```text
[Khung 1: 28/11-02/12] -> [Khung 2: 03/12-07/12] -> [Khung 3: 08/12-12/12] -> ★ MỐC 1: BÁO CÁO TIẾN ĐỘ (14/12 hoặc 16/12)
                                                                                       │
★ MỐC 2: VẤN ĐÁP (25/12) <- [Khung 6: 23/12-25/12] <- [Khung 5: 18/12-22/12] <- [Khung 4: 13/12-17/12]
```

---

## KHUNG 1 (5 ngày: 28/11 – 02/12/2026) — Nền móng Kiến trúc, Player Core & Quang học Sơ khai
**Mục tiêu chung:** Khóa toàn bộ Interface/Enum, cấu hình xong Unity Editor, Tinh linh chạy/nhảy/lướt/bắn đạn nảy gương mượt mà trên cảnh thử nghiệm.

### Phase 1A (28/11 – 29/11 — 2 ngày): *Khóa hợp đồng & Dựng móng*
* **Huy:**
  - [ ] `M00.01`, `M00.02`: Thiết lập đầy đủ Layers, Sorting Layers, Gravity `-25` và Physics2D Collision Matrix.
  - [ ] `M00.03`: Cấu hình URP 2D Bloom Volume và tạo Shader `SG_WhiteOutline` (viền phát sáng trắng).
  - [ ] `M00.04`: Nâng cấp `PlayerInput.inputactions` đủ 10 thao tác điều khiển.
  - [ ] `M03.01`, `M03.03`: Refactor `PlayerController.cs` cũ sang mô hình SRP; hoàn thiện `PlayerMovement.cs` (A/D `4.5 u/s`, Space `12 u/s`, Coyote Time `0.1s`, S+Space rơi sàn mềm `0.3s`).
* **Thuận:**
  - [ ] `M01.01`, `M01.02`: Tạo và **commit lên `dev` ngay tối 28/11** 3 Interface (`IInteractable`, `IDamageable`, `IActivatable`) và toàn bộ 8 file Enum *(Contract Freeze)*.
  - [ ] `M02.01`: Code `GameManager.cs` (State Machine, `SetBulletTime(0.1f)`, `ChangeState`).
  - [ ] `M02.02`: Code `WorldState.cs`, `GameSaveData.cs` và khung `SaveManager.cs` (đọc/ghi JSON 1 slot cơ bản).
  - [ ] `M05.03`: Code `EnergyPylon.cs`, `SealGate.cs`, `LightPressurePad.cs` (test kích hoạt cửa mở 2 chiều trên `Test_Thuan.unity`).

### Phase 1B (30/11 – 02/12 — 3 ngày): *Bắn đạn Bullet Time, Gương phản xạ & HUD sơ khai*
* **Huy:**
  - [ ] `M03.02`, `M03.04`: Code `PlayerStats.cs` (3 HP, 100 Mana, 1.5s i-frames, knockback 0.2s, `lastSafeGroundedPos`) và `PlayerAbilities.cs` (khung Ability ID + **Ground Dash** phím Ctrl).
  - [ ] `M03.05`, `M04.01`, `M04.02`: Code `PlayerCombat.cs` (Hold Bullet Time 0.1×, trọng lực 10%, khóa A/D, tia laser, hủy `[X]`, hồi Mana 2/s $\rightarrow$ 25/s), `ProjectileDataSO` và `LightProjectile.cs` (`CircleCast2D`, nảy tối đa 3 lần, lần 4 nổ AOE `1.5u`).
  - [ ] `M03.06`, `M02.06`: Code `PlayerInteraction.cs` (quét `1.8u`, hỗ trợ Tap & Hold `[F]`), `PlayerLightAura.cs` (`Light2D` bán kính `4.0u`), `CameraFollow2D.cs` & `CameraZoomController.cs`.
* **Thuận:**
  - [ ] `M05.01`: Code `ReflectionMirror.cs` (`Fixed_45Deg` & `Steerable_Pedestal`, tách mặt kính & khung viền) và `RemotePedestal.cs` (bấm `[F]` xoay $45^\circ$ + `LineRenderer` cáp năng lượng).
  - [ ] `M05.07`: Code `DarkSpikes.cs` (trừ 1 HP + knockback/tái tạo mỏm an toàn) và `DarkAbyss.cs` (rơi hố 0 mất HP trong Campaign).
  - [ ] `M08.01`: Dựng Prefab `HUDCanvas` & code `HUDController.cs` (3 đốm sáng HP, thanh Mana vạch chia 20, Vignette Bullet Time, icon Autosave).
* **Đồng bộ cuối Khung 1 (tối 02/12):** Merge vào `dev`, kiểm chứng bắn đạn Bullet Time nảy qua gương xoay kích hoạt `EnergyPylon` mở `SealGate`.

---

## KHUNG 2 (5 ngày: 03/12 – 07/12/2026) — Cơ chế Màn Tutorial, Hoa Soi Sáng, Quái & Art LVL-01
**Mục tiêu chung:** Hoàn thiện toàn bộ cơ chế, AI, UI và Art cần thiết cho 5 phòng của LVL-01.

### Phase 2A (03/12 – 05/12 — 3 ngày): *Hoa Soi Sáng, Gió chu kỳ, Spitter, Lồng giam & Tap Aim*
* **Huy:**
  - [ ] `M04.03`, `M04.04`: Code `FloatingLightOrb.cs` (Tap bắn $<0.25\text{s}$ tạo cầu lơ lửng `8.0s`), `EnemyProjectile.cs` (bị `LightProjectile` triệt tiêu) và `LightShardPickup.cs` (hút `2.5u`).
  - [ ] `M09.01`: Vẽ hoàn chỉnh hoạt ảnh cơ bản của Tinh linh (`Player.aseprite`: Idle, Move/Lean, Jump, Fall, Dash, Aim, Hit) và dựng Animator Controller.
  - [ ] `M09.02`: Vẽ Tilemap Hang đá rêu phong (`LVL-01`) và bộ Sprite Props LVL-01 (Bệ đá, Bệ cần xoay, Gương, Hoa Soi Sáng 3 trạng thái, Cửa Phong Ấn, Bệ thờ, Lồng giam, Bãi gai, icon phím hướng dẫn trên vách đá).
* **Thuận:**
  - [ ] `M05.02`: Code `RadiantBloom.cs` (trúng đạn $\rightarrow$ sáng toàn phòng `8.0s` [1s tăng, **5s cực đại**, 2s mờ héo] $\rightarrow$ chờ **`60.0s`** hồi `Ready`).
  - [ ] `M05.04`: Code `WindZone.cs` (hỗ trợ gió cản chu kỳ 3s thổi / 2s ngừng cho Phòng 3 LVL-01 và uốn cong đạn).
  - [ ] `M05.05`, `M07.01`: Code `CheckpointAltar.cs` (hồi 100% HP/Mana + Autosave + chế độ gợi ý $>90\text{s}$), `ShadowCage.cs` (2 đạn hoặc giữ `[F]` `2.0s`), `NPCBase.cs` (có `virtual OnRescued()`) & `EventNPC.cs`.
  - [ ] `M06.01`, `M06.02`: Code `EnemyBase.cs`, `BossBase.cs` (commit lên `dev` tối 04/12 để Huy kế thừa làm Boss) và `ShadowSpitter.cs` (bắn gai `8.0 u/s` mỗi `2.5s`).

### Phase 2B (06/12 – 07/12 — 2 ngày): *Boss Tutorial, Quản lý Phòng, Dialogue/Pause UI & Audio LVL-01*
* **Huy:**
  - [ ] `M06.04`: Code `TutorialDarkCore.cs` (kế thừa `BossBase`, `maxHealth = 3`, sóng xung kích chu kỳ `3.0s`, phá hủy mở Cánh Cổng Cổ Đại).
  - [ ] `M09.04`, `M09.05`: Vẽ Sprite/Animator cho `ShadowSpitter`, `TutorialDarkCore`, `EventNPC`; tạo VFX cơ bản (Laser Bullet Time, Crystal Ping Hit Flash, Radiant Bloom Shockwave, nổ hạt sáng, thanh tẩy phòng).
* **Thuận:**
  - [ ] `M02.03`: Code `RoomController.cs` (gợi ý 3 cấp 30s/60s/90s, đếm trượt 3 lần, xử lý `EventNPC` thành `Missed` khi rời phòng, thưởng 5 LS khi clear phòng).
  - [ ] `M08.02`: Dựng & code `DialogueUI.cs` (hội thoại NPC, gợi ý Checkpoint, thông báo) và `PauseMenuUI.cs` (Esc: Resume, Controls & Hints, Settings Audio/Screen Shake).
  - [ ] `M02.04`, `M09.06`: Code `AudioManager.cs` + `SoundData.cs`; tìm và gắn bộ SFX/BGM cơ bản cho toàn bộ thao tác ở LVL-01.

---

## KHUNG 3 (5 ngày: 08/12 – 12/12/2026) — Lắp ráp, Polish & Chốt Bản Test MỐC 1 (LVL-01)
**Mục tiêu chung:** Hoàn thiện **100% màn chơi `LVL01_AwakeningHollow.unity` (5 phòng Tutorial)** đẹp, mượt, có âm thanh/ánh sáng URP và sẵn sàng cho ngày **Báo cáo tiến độ 14/12 (hoặc 16/12)**.

### Phase 3A (08/12 – 10/12 — 3 ngày): *Dựng màn LVL-01 (Thuận Grey-box $\rightarrow$ Huy Art Pass)*
* **Ngày 08/12 — Thuận (Độc quyền giữ Scene `LVL01_AwakeningHollow.unity`):**
  - [ ] `M10.01`: Dựng Grey-box đủ 5 phòng liên tục đúng tỷ lệ GDD 6.3 (lối đi tối thiểu `2 units`, hố nhảy vừa tầm `Space`, hốc đá Phòng 3 cách `2.5u`, góc gương Phòng 4 chuẩn $45^\circ$ bắn trúng `ShadowSpitter`, gắn `RoomController`, `CheckpointAltar`, `ShadowCage` + `EventNPC`, `RadiantBloom`, `TutorialDarkCore`).
  - [ ] Commit & bàn giao quyền sửa Scene `LVL01` cho Huy vào tối 08/12.
* **Ngày 09/12 – 10/12 — Huy (Độc quyền giữ Scene `LVL01_AwakeningHollow.unity`):**
  - [ ] `M10.01`: Phủ toàn bộ Tilemap, Parallax nền hang đá, ánh sáng URP 2D, Bloom, VFX bụi sáng dẫn hướng, hình hướng dẫn phím bấm trên vách đá và gán Sprite/Animator cho toàn bộ Prefab của LVL-01.
* **Ngày 09/12 – 10/12 — Thuận (làm song song trên `Test_Thuan.unity`, KHÔNG đụng `LVL01`):**
  - [ ] Chuẩn bị sẵn trước khung code cho Khung 4: `CannonStationSO`, `CannonNetworkManager`, `TravelCannon`, `UpgradeItemSO` (tạo sẵn 8 asset) để sau khi báo cáo tiến độ là ghép chạy ngay!

### Phase 3B (11/12 – 12/12 — 2 ngày): *Playtest nội bộ & Chốt bản Báo cáo Mốc 1*
* **Cả hai cùng test trên `dev`:**
  - [ ] Chạy thử nghiệm trọn vẹn LVL-01 từ Phòng 1 đến Phòng 5: kiểm tra thời lượng mục tiêu 5–7 phút, kiểm tra `QA-PHY-01` (nảy 3 lần nổ lần 4), `QA-ENV-01` (Hoa Soi Sáng 8s / 5s cực đại / hồi 60s), kiểm tra không bị kẹt địa hình (snagging), kiểm tra Save/Load tại Checkpoint & mở lại game.
  - [ ] Fix toàn bộ bug phát sinh; tạo bản Build thử nghiệm LVL-01 và merge nhánh ổn định **`milestone-1-report`** lên `main` vào tối **12/12** (dư 2 ngày đệm 13–14/12 trước buổi báo cáo).

---

## KHUNG 4 (5 ngày: 13/12 – 17/12/2026) — ★ BÁO CÁO TIẾN ĐỘ (14–16/12) + Hub, Boran Lò Rèn, Đại Bác & LVL-02
**Mục tiêu chung:** Trình diễn xuất sắc Mốc 1 (14/12 hoặc 16/12), đồng thời xây xong Hub Cổ Thụ (`AREA-02`), Trạm Nghỉ (`AREA-03`), Lò rèn Boran, Hệ thống Đại Bác, 3 Quái mới và toàn bộ màn `LVL-02`.

### Phase 4A (13/12 – 14/12 — 2 ngày): *★ Báo cáo Mốc 1 + Kỹ năng Launch & Hub / Trạm Nghỉ*
* **Huy:**
  - [ ] **Chuẩn bị & Báo cáo tiến độ Mốc 1** (14/12 hoặc 16/12).
  - [ ] `M03.04`: Hoàn thiện kỹ năng **Launch (Chuột phải)** trên `PlayerAbilities.cs` (`20 u/s`, `0.2s` bay thẳng + `0.15s` hòa trọng lực, `isGrounded == true`, 0 Mana) + Animation giọt nước & VFX vệt sao băng.
  - [ ] `M09.03`, `M09.04`: Vẽ Tilemap/Props cho **Hub Cổ Thụ (`AREA-02`)** (2 trạng thái héo/sáng), **Trạm Nghỉ Đầu Gió (`AREA-03`)** (lạnh lẽo vs lò rèn đỏ lửa), Sprite `BoranNPC`, `SacredTreeNPC` và **Khẩu Đại Bác (`TravelCannon` — trụ + nòng xoay + VFX hút/tụ lực/bắn)**.
* **Thuận:**
  - [ ] **Chuẩn bị & Báo cáo tiến độ Mốc 1** (14/12 hoặc 16/12).
  - [ ] `M07.02`, `M07.03`, `M08.03`: Hoàn thiện `BoranNPC.cs`, `SacredTreeNPC.cs`, `UpgradeManager.cs` (áp đủ 8 nâng cấp lên Player) và `ForgeShopUI.cs` (kèm trạng thái khóa + tooltip gợi ý nếu chưa cứu Boran).
  - [ ] `M02.05`, `M05.06`, `M08.03`: Hoàn thiện trọn bộ **Hệ thống Đại Bác Dịch Chuyển** (`CannonNetworkManager`, `TravelCannon`, `CannonSelectUI` + 2 asset `Cannon_Hub_Base` & `Cannon_LVL03_BossPrep`). Kiểm thử luồng: Khẩu Base bị `Locked` bấm `[F]` hiện `"Tạm chưa mở khóa"` $\rightarrow$ Mở khẩu LVL-03 $\rightarrow$ Cả 2 khẩu `Unlocked_Active` hiện `[F] Use Cannon` và bay qua lại giữa 2 Scene kèm hoạt ảnh đáp từ trên trời.

### Phase 4B (15/12 – 17/12 — 3 ngày): *3 Quái mới, Codex & Dựng màn `LVL02_WhisperingCaverns`*
* **Huy:**
  - [ ] `M09.03`, `M09.04`: Vẽ Tilemap Hẻm núi đá vôi (`LVL-02`) & Sprite/Animator cho 3 quái mới (`ShadowCrawler`, `ShadowSlime`, `ShadowWeaver` — đủ telegraph nén thân & vỡ khiên) + `LoreStele`.
  - [ ] `M10.02`: Dựng hoàn chỉnh Art & Lighting cho `AREA02_TheCradle_Hub.unity` và `AREA03_WindsThreshold.unity` (đặt Khẩu Đại Bác Base, vùng trigger mở khóa `Ability_Launch`, Lò rèn Boran).
* **Thuận:**
  - [ ] `M06.03`: Code đủ 3 loại quái `ShadowCrawler.cs`, `ShadowSlime.cs`, `ShadowWeaver.cs` (test hành vi phá khiên, choáng 1.5s, rơi 1–3 LS trên `Test_Thuan.unity`).
  - [ ] `M05.04`, `M05.05`, `M07.04`: Nâng cấp `WindZone.cs` đầy đủ lực gió ngang ($+30\% / -50\%$ tốc độ + lệch đạn parabol); code `LoreStele.cs`, `MemoryFragmentSO`, `CodexManager.cs` (tạo Mảnh #01, #02).
  - [ ] `M10.03`: Dựng Grey-box & lắp toàn bộ câu đố cho `LVL02_WhisperingCaverns.unity` (tuyến chính vượt hẻm gió bằng Launch, nhánh phụ cứu Boran SQ-01 trong `ShadowCage`, Bia Ký Ức #01, Cửa Phong Ấn 2 chiều). Bàn giao Scene `LVL02` cho Huy tối 17/12.

---

## KHUNG 5 (5 ngày: 18/12 – 22/12/2026) — Đỉnh Gió Gầm (LVL-03), Boss Lõi Gió, Air Dash & Toàn bộ UI
**Mục tiêu chung:** Hoàn thiện màn cao trào `LVL-03`, trận đấu trùm `WindCoreBoss`, mở khóa `Air Dash`, hoàn chỉnh MapUI, MainMenuUI và hệ thống Save 3 Slots.

### Phase 5A (18/12 – 20/12 — 3 ngày): *`LVL-03`, Gương Tự Động Xoay, Boss Lõi Gió & Air Dash*
* **Huy:**
  - [ ] `M10.03`: Art Pass + Lighting URP hoàn thiện `LVL02_WhisperingCaverns.unity` (ngày 18/12).
  - [ ] `M03.04`: Hoàn thiện nâng cấp **Air Dash** trong `PlayerAbilities.cs` (lướt ngang trên không 1 lần/nhảy, $g=0$ trong `0.2s`, nối combo sau `Launch`, reset khi chạm đất) + mở khóa `Ability_ZephyrResistance`.
  - [ ] `M06.05`, `M09.03`, `M09.04`: Vẽ Tilemap Phong huyệt dựng đứng (`LVL-03`), Gương Tự Động Xoay (bánh răng), Sprite/VFX của **`WindCoreBoss`**; lập trình hoàn chỉnh `WindCoreBoss.cs` (6 HP, Phase 1 sóng xung kích, Phase 2 bão xoáy, mở giáp 4s bắn nảy gương, rơi Tinh Hoa Gió + Cutscene thanh tẩy Vùng Gió).
* **Thuận:**
  - [ ] `M05.01`: Hoàn thiện chế độ `AutoRotating_Clockwise` trên `ReflectionMirror.cs` (xoay đều liên tục, không có bệ `[F]`, không hiện viền trắng).
  - [ ] `M10.04`: Dựng Grey-box & câu đố toàn bộ `LVL03_EyeOfTheTempest.unity` (cột gió nâng ghép từ nhiều `WindZone`, gió đổi chiều 4s, câu đố Gương Tự Động Xoay mở khóa **Khẩu Đại Bác Tiền Phòng Boss**, Bia Ký Ức #02, cổng lối vòng sau phòng Boss nối ngược về Hub). Bàn giao Scene `LVL03` cho Huy tối 20/12.
  - [ ] `M02.02`, `M00.05`: Hoàn thiện `SaveManager.cs` đầy đủ 3 Save Slots độc lập + mã hóa JSON + kiểm tra lỗi file Save hỏng; cấu hình `EditorBuildSettings` đủ 6 Scene.

### Phase 5B (21/12 – 22/12 — 2 ngày): *Art Pass LVL-03, MapUI, MainMenuUI & Full Audio*
* **Huy:**
  - [ ] `M10.04`: Art Pass + Lighting URP hoàn thiện `LVL03_EyeOfTheTempest.unity` & đấu trường `WindCoreBoss` (kèm hiệu ứng chuyển đổi trời bão $\rightarrow$ bầu trời quang đãng khi hạ Boss).
  - [ ] `M09.05`: Hoàn thiện toàn bộ VFX còn thiếu (VFX Đại Bác bắn/đáp đất, VFX thanh tẩy Vùng Gió, hoạt cảnh Ending Vertical Slice bay lên bầu trời bằng Air Dash).
* **Thuận:**
  - [ ] `M08.04`, `M10.05`: Hoàn thiện `MapUI.cs` (`Tab`/`M` Room Grid Map + Fog of War + icon Checkpoint/Hub/Đại Bác/NPC `?`) và `MainMenuUI.cs` + Scene `MainMenu.unity` (Continue, New Game chọn 3 Save Slots, Settings, Codex đọc Mảnh #01/#02, Credits, Quit).
  - [ ] `M09.06`: Hoàn thiện 100% âm thanh SFX & BGM (chuyển nhạc Suy tàn $\rightarrow$ Hồi sinh, tiếng búa lò rèn Boran 2D spatial, âm thanh 3 pha Hoa Soi Sáng, tiếng tụ lực/bắn Đại Bác).

---

## KHUNG 6 (3 ngày nước rút: 23/12 – 25/12/2026) — Tổng duyệt QA, Sửa lỗi & ★ VẤN ĐÁP CHÍNH THỨC
**Mục tiêu chung:** 0 lỗi Blocker, 0 lỗi Critical, bản Build `.exe` ổn định 60FPS sẵn sàng bảo vệ đồ án.

### Phase 6A (23/12 – 24/12 — 2 ngày): *Playtest toàn tuyến, QA Checklist & Build `.exe`*
* **Cả hai cùng chạy thử nghiệm toàn tuyến (`MainMenu` $\rightarrow$ `LVL-01` $\rightarrow$ `Hub` $\rightarrow$ `AREA-03` $\rightarrow$ `LVL-02` $\rightarrow$ `LVL-03` $\rightarrow$ `Boss` $\rightarrow$ `Ending`):**
  - [ ] **Thuận (Chủ trì QA & Build):** Chạy và ký xác nhận đủ 7 test case:
    1. `QA-PHY-01`: Đạn nảy tối đa 3 lần giữa 2 gương song song, lần 4 nổ AOE 1.5u biến mất.
    2. `QA-MEC-01`: Launch chỉ dùng khi chạm đất; Tap bắn tạo cầu 8s; bị đánh khi Hold không trừ Mana; Air Dash 1 lần/nhảy.
    3. `QA-ENV-01`: Hoa Soi Sáng chiếu sáng 8.0s (5.0s sáng cực đại), héo xám và hồi phục sau đúng **60.0s**.
    4. `QA-CAN-01`: Khẩu Base khi `Locked` bấm `[F]` hiện `"Tạm chưa mở khóa"`; sau khi mở khẩu LVL-03, cả 2 hiện `[F] Use Cannon`, bảng chọn có nút `"Exit"`, chọn bay chạy đủ hoạt ảnh bắn $\rightarrow$ chuyển cảnh $\rightarrow$ đáp từ trên trời xuống `landingPoint`.
    5. `QA-QST-01`: Bỏ qua Boran sang LVL-03 quay lại vẫn cứu được (Lò rèn hiện tooltip khóa trước khi cứu); bỏ qua `EventNPC` Phòng 4 LVL-01 rời phòng sẽ `Missed`.
    6. `QA-UX-01`: Kiểm tra hiển thị UI, chữ, viền trắng và gai đen ở cả `1280×720` và `1920×1080`.
    7. `QA-UX-02`: Tắt 100% âm lượng (Mute Master) vẫn chơi và nhận biết rõ mọi trạng thái qua hình ảnh.
  - [ ] **Huy (Chủ trì Polish & Fix Visual/Feel):** Cân chỉnh lần cuối độ sáng vùng tối, lực gió LVL-02/LVL-03 (tránh gây ức chế người mới), tốc độ đạn/nhảy và sửa các lỗi hiển thị Animation/VFX/Camera.
  - [ ] **Đóng gói Release Build (`24/12`):** Loại bỏ phím Debug (F1/F2/F3), build `.exe` Windows, test chạy độc lập ngoài Unity Editor đạt 60FPS ổn định, merge bản chốt lên nhánh `main` và gắn tag `v1.0-defense`.
  - *(Nếu dư thời gian ngày 24/12 — `P1-Bonus`: bật thêm Survival Mode 3 Lives & Assist Mode trong Settings).*

### Phase 6B (25/12/2026): *★ VẤN ĐÁP CHÍNH THỨC*
* **Chuẩn bị kịch bản Demo trực tiếp (~7–10 phút):**
  1. Khởi động từ `MainMenu` $\rightarrow$ Chơi nhanh vài phòng `LVL-01` (trình diễn Movement, Bullet Time Tap/Hold, Hoa Soi Sáng, Bệ xoay gương, Spitter, Boss sơ khai).
  2. Nạp Save Slot chuẩn bị sẵn tại `Hub` $\rightarrow$ Bước vào `AREA-03` mở khóa `Launch` $\rightarrow$ Bấm `[F]` vào Khẩu Đại Bác Base (chứng minh thông báo `"Tạm chưa mở khóa"`).
  3. Vào `LVL-02` trình diễn lực gió uốn cong đạn, giải cứu Boran (SQ-01) $\rightarrow$ Quay về `AREA-03` nâng cấp tại Lò rèn Boran.
  4. Tiến lên `LVL-03` giải đố Gương Tự Động Xoay mở khóa **Khẩu Đại Bác Tiền Phòng Boss** $\rightarrow$ Trình diễn bay Đại Bác về Base nâng cấp rồi bay ngược lên lại LVL-03!
  5. Đánh bại **`WindCoreBoss`**, hấp thụ Tinh Hoa Gió mở khóa **Air Dash**, xem Cutscene bầu trời quang đãng & bảng tổng kết Phồn Vinh.

---

# 6. DANH SÁCH KIỂM DUYỆT ĐẦU RA (MILESTONE ACCEPTANCE & RISK MITIGATION)

### 6.1 Tiêu chí nghiệm thu theo từng Mốc
| Mốc | Ngày chốt | Tiêu chí BẮT BUỘC đạt được (Go / No-Go) |
| :--- | :---: | :--- |
| **Mốc 1 (Báo cáo tiến độ)** | **14/12** *(hoặc 16/12)* | Chơi mượt từ đầu đến cuối **LVL-01 (5 phòng)**: A/D, Space, S+Space, Ground Dash, Bullet Time Hold/Tap, Bệ xoay gương, Hoa Soi Sáng (8s / 5s cực đại / 60s hồi), ShadowSpitter, ShadowCage + EventNPC, TutorialDarkCore, Checkpoint + Autosave, HUD 3 HP/Mana. **0 lỗi crash/soft-lock.** |
| **Mốc 2 (Vấn đáp chính thức)** | **25/12** | Hoàn thiện trọn vẹn **Vùng Gió**: LVL-01 $\rightarrow$ Hub $\rightarrow$ AREA-03 (Launch & Boran Forge 8 nâng cấp) $\rightarrow$ LVL-02 (Boran, Mảnh #01, gió ngang, 4 quái) $\rightarrow$ LVL-03 (Gương tự động xoay, **Đại Bác Dịch Chuyển Base $\leftrightarrow$ LVL-03**, Mảnh #02, Boss Lõi Gió mở Air Dash), MapUI, MainMenuUI (3 Save Slots), Audio/Lighting hoàn chỉnh, Build `.exe` 60FPS. **0 Blocker, 0 Critical.** |

### 6.2 Kế hoạch cắt giảm an toàn nếu trễ tiến độ (Contingency Plan)
Nếu gặp sự cố kỹ thuật hoặc áp lực thi cuối kỳ làm trễ lịch từ 2 ngày trở lên, nhóm áp dụng thứ tự ưu tiên cắt giảm (từ trên xuống) để **bảo vệ tuyệt đối trải nghiệm cốt lõi**:
1. **Cắt đầu tiên (`P1-Bonus`):** Tạm hoãn Survival Mode và Assist Mode (giữ mặc định Campaign Mode 3 HP).
2. **Tinh giản thứ hai:** Giảm độ dài hình học của `LVL-02` và `LVL-03` (mỗi màn 3–4 phòng tinh gọn nhưng đủ toàn bộ cơ chế: gió, cứu Boran, Gương tự động xoay, Đại Bác, Boss Lõi Gió) thay vì làm bản đồ quá rộng.
3. **Tinh giản thứ ba:** Dùng hoạt ảnh chuyển cảnh Fade đơn giản (1.5s) cho `TravelCannon` giữa lúc bắn khỏi màn hình và lúc đáp xuống nếu thiếu thời gian làm hoạt ảnh phức tạp.
4. **TUYỆT ĐỐI KHÔNG CẮT:** Bộ điều khiển Tinh linh (Bullet Time, Launch, Dash), Hệ thống Gương & Hoa Soi Sáng, Boran & Lò rèn, Hệ thống Đại Bác (Base $\leftrightarrow$ LVL-03), 4 loại quái và Boss Lõi Gió.

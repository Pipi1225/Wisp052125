# PHÂN CÔNG CÔNG VIỆC — TEAM 3009
## Dự án: *The Cradle of Light* (2D Metroidvania Puzzle Adventure)

- **Thành viên:** Dương Gia Huy (`23127052`) & Nguyễn Hiếu Thuận (`23127125`)
- **Mục tiêu trọng tâm:** Hoàn thiện bản **Vertical Slice (P0)** gồm `LVL-01` (Tutorial 5 phòng) → `AREA-02` (Hub Cổ Thụ) → `AREA-03` (Trạm Nghỉ Đầu Gió) → `LVL-02` & `LVL-03` (Vùng Gió & Boss Lõi Gió).
- **Tài liệu tham chiếu:**
  - GDD chi tiết: `Assets/GDD/Team 3009 - GDD - 23127052_23127125.docx.md`
  - Đặc tả kiến trúc & Class: `Assets/GDD/schema_design.md`

---

## 1. Tóm tắt nhanh: Ai làm gì?

| Mảng công việc | Huy phụ trách | Thuận phụ trách |
| :--- | :--- | :--- |
| **Kiến trúc nền** | 3 Interfaces (`IInteractable`, `IDamageable`, `IActivatable`), Camera | `GameManager`, `GameState`, `SaveSystem`, `CannonNetworkManager`, `RoomController`, `AudioManager` |
| **Nhân vật (Player)** | Input + tách `PlayerController` thành 6 module SRP | Lắng nghe event từ `PlayerStats`/`PlayerCombat` để cập nhật HUD & Save |
| **Đạn & Vật phẩm** | `LightProjectile`, `FloatingLightOrb`, `ProjectileDataSO`, `LightShardPickup` | `EnemyProjectile` (đạn gai/tơ quái, bị `LightProjectile` triệt tiêu) |
| **Môi trường & Puzzle** | Gương, Bệ xoay, Hoa Soi Sáng, `EnergyPylon`, `SealGate`, `LightPressurePad`, `WindZone`, `TravelCannon`, `CheckpointAltar`, Bẫy (`DarkSpikes`, `DarkAbyss`) | `ShadowCage` (lồng giam NPC), `LoreStele` (Bia Ký Ức) |
| **Quái, Boss, NPC & Kinh tế** | *(Hỗ trợ test va chạm với đạn & Player)* | `EnemyBase`, 4 loại quái, `BossBase`, `TutorialDarkCore`, `WindCoreBoss`, `NPCBase` (Boran, Cổ Thụ, EventNPC), `UpgradeManager` (Lò rèn), `CodexManager` |
| **Giao diện (UI)** | *(Gắn trigger mở UI từ Player/Cannon)* | Toàn bộ 7 UI (`HUDController`, `DialogueUI`, `ForgeShopUI`, `CannonSelectUI`, `MapUI`, `PauseMenuUI`, `MainMenuUI`) |
| **Scene phụ trách** | `LVL01_AwakeningHollow.unity`, `AREA03_WindsThreshold.unity` | `AREA02_TheCradle_Hub.unity`, `LVL02_WhisperingCaverns.unity`, `LVL03_EyeOfTheTempest.unity` |

---

## 2. Checklist chi tiết phần việc của Thuận

> Mục này liệt kê đầy đủ từng file, thông số cốt lõi cần bám theo `schema_design.md` và `GDD v0.2` để Thuận dễ theo dõi và đánh dấu tiến độ.

### A. Core Framework, Save System & Audio (`Assets/Project/Scripts/Core/`)
- [ ] **`GameState.cs` & `GameManager.cs`** *(làm sớm ở Sprint 1)*:
  - Enum `GameState`: `Boot`, `MainMenu`, `Gameplay`, `BulletTime`, `Paused`, `Respawning`, `Cutscene`, `GameOver`, `Victory`.
  - `SetBulletTime(bool active)`: đặt `Time.timeScale = 0.1f` (hoặc `1.0f`), gọi `AudioManager.SetLowPassFilter` và `CameraZoomController.ZoomBulletTime`.
  - `LoadSceneAsync(string sceneName, Action onLoaded)`: Fade chuyển cảnh cho Cửa chuyển màn, Đại Bác, Respawn.
  - `TriggerGameOver()` (Survival Mode): ghi đè tọa độ hồi sinh về Hub + Autosave khẩn cấp trước khi hiện Game Over.
  - `TriggerVictory()`: chạy Ending Cutscene + bảng tổng kết Phồn Vinh.
- [ ] **`SaveSystem/` (`GameSaveData.cs`, `WorldState.cs`, `SaveManager.cs`)**:
  - `GameSaveData`: dùng `List<T>` thuần tương thích `JsonUtility` cho 3 Save Slots (`save_slot_{i}.json`).
  - `WorldState`: dùng `HashSet<string>` (`openedGates`, `unlockedCannons`, `purifiedAreas`, `exploredRoomIDs`) tra cứu $O(1)$ lúc chạy game.
  - `SaveManager`: `SaveGame`, `LoadGame`, `TriggerAutosave()` (gọi `HUDController.ShowAutosaveIndicator()` 1.5s).
- [ ] **`CannonNetworkManager.cs`**:
  - Quản lý `List<CannonStationSO> allStations` và `pendingArrivalCannonID`.
  - **Luật Base:** khi có $\ge 1$ khẩu ngoài được mở (`UnlockCannon`), tự động mở luôn khẩu có `isHubBaseCannon == true`.
  - `StartCannonTravel(TravelCannon origin, CannonStationSO dest)`: đưa về `GameState.Cutscene`, chạy `origin.PlayLaunchSequence(dest)` → chuyển Scene → tìm `TravelCannon` đích gọi `PlayLandingSequence()`.
- [ ] **`RoomController.cs`**:
  - Khi Player vào phòng: đăng ký `roomID` vào `WorldState.exploredRoomIDs`, bắt đầu đếm `stuckTimer`.
  - Hint 3 cấp: **30s** nhấp nháy `EnergyPylon` → **60s hoặc bắn trượt 3 lần** bật laser xanh ngọc & định hướng gương → **90s** gọi `CheckpointAltar.ActivateHintMode()`.
  - `ClearRoom()`: xua sương đen, thưởng **5 LS**.
  - `OnPlayerExitRoom()`: nếu `EventNPC` trong phòng vẫn `Trapped` → chuyển thành `Missed`.
- [ ] **`Audio/SoundData.cs` & `AudioManager.cs`**:
  - `SoundData` (ScriptableObject): `soundID`, `clip`, `volume`, `isLooping`.
  - `PlayBGM` (Fade-in 2.0s khi thanh tẩy vùng), `PlaySFX` (2D Spatial Panning), `SetLowPassFilter` (giảm 50% âm lượng BGM + lọc tần cao trong 0.2s khi Bullet Time).

### B. Kẻ địch, Đạn quái & Boss (`Assets/Project/Scripts/Enemies/` & `Projectiles/`)
- [ ] **`EnemyProjectile.cs`** (`Scripts/Projectiles/`):
  - Slime bắn tốc độ `6.0 u/s`, Spitter bắn `8.0 u/s`; bị triệt tiêu tức thì nếu va chạm `LightProjectile`.
- [ ] **`Base/EnemyBase.cs`** (kế thừa `MonoBehaviour`, implement `IDamageable`):
  - `maxHealth`, `hasShadowShield`, `stunDuration = 1.5f`, rơi `1–3` `LightShardPickup`.
  - Có khiên: đạn 1 phá khiên + Stun `1.5s`; đạn 2 tiêu diệt (trừ khi đạn có cờ *Empowered Light* gây 2 Hit xuyên thẳng).
- [ ] **4 loại quái thường (`AI/`)**:
  - `ShadowCrawler.cs`: tuần tra `2.0 u/s`, phát hiện thẳng `4.0 u` lao húc (1 HP), **có khiên**.
  - `ShadowSlime.cs`: di chuyển `1.2 u/s`, tầm `6.0 u` nén thân `0.5s` (telegraph) rồi bắn `EnemyProjectile` chu kỳ `3.0s`, **có khiên**.
  - `ShadowWeaver.cs`: ngủ trần hang, `BoxCollider2D` cảm biến dọc bên dưới → thả dây lao thẳng theo trục Y (`15.0 u/s`), lùi xa thì treo bắn tơ, **không khiên**.
  - `ShadowSpitter.cs`: cố định trên vách/trần, mỗi `2.5s` bắn `EnemyProjectile` (`8.0 u/s`), **không khiên**.
- [ ] **Boss (`BossPhase.cs`, `Base/BossBase.cs`, `Bosses/`)**:
  - `BossBase`: kế thừa `EnemyBase` (dùng chung biến máu), có `areaID`, `rewardAbilityID`. Khi `Die()`: gọi `WorldState.RegisterAreaPurified(areaID)`, mở khóa `rewardAbilityID`, chạy Cutscene thanh tẩy và Autosave.
  - `TutorialDarkCore.cs` (`LVL-01` Phòng 5): `maxHealth = 3`, phát sóng xung kích chu kỳ `3.0s`.
  - `WindCoreBoss.cs` (`LVL-03` Boss): `maxHealth = 6`, kết hợp `WindZone` bão xoáy + `ReflectionMirror[]` quanh đấu trường; mở giáp `4.0s` (`ExposeCoreWeakpoint`) để Player bắn nảy gương trúng lõi; hạ gục rơi Tinh Hoa Gió mở `"Ability_AirDash"` + `"Ability_ZephyrResistance"`.

### C. NPC, Lồng giam, Lò rèn Nâng cấp & Lore (`NPC/`, `Upgrades/`, `Interactables/`)
- [ ] **`ShadowCage.cs`** (`Environment/Interactables/` — implement `IInteractable`, `IDamageable`):
  - Độ bền `2` (trúng 2 `LightProjectile` hoặc giữ `[F]` `2.0s`), vỡ ra gọi `jailedNPC.OnRescued()`.
- [ ] **`LoreStele.cs`** (`Environment/Interactables/` — implement `IInteractable`):
  - Bấm `[F]` mở `DialogueUI`, gọi `CodexManager.UnlockFragment(fragmentID)` và Autosave.
- [ ] **`NPC/` (`NPCState.cs`, `NPCBase.cs`, `BoranNPC.cs`, `SacredTreeNPC.cs`, `EventNPC.cs`)**:
  - `NPCBase`: implement `IInteractable`, có `npcID`, `state`, `isCoreFunctionalNPC`, `virtual OnRescued()`.
  - `BoranNPC` (`isCoreFunctionalNPC = true`): không bao giờ bị `Missed`; cứu ở `LVL-02` → dịch chuyển về `AREA-03`, thắp lửa Lò rèn vĩnh viễn, bấm `[F]` mở `ForgeShopUI`.
  - `EventNPC` (`isCoreFunctionalNPC = false`, gồm NPC Phòng 4 `LVL-01`): rời phòng mà chưa cứu → `MarkAsMissed()`.
  - `SacredTreeNPC` (`AREA-02` Hub): dẫn chuyện + đánh giá Phồn Vinh (*Desolate / Restored / Radiant Dawn*).
- [ ] **`Upgrades/` (`UpgradeType.cs`, `UpgradeItemSO.cs`, `UpgradeManager.cs`, `MemoryFragmentSO.cs`, `CodexManager.cs`)**:
  - Tạo 8 `UpgradeItemSO` trong `ScriptableObjects/Upgrades/` theo đúng bảng giá GDD 4.8 (`45, 120, 200, 70, 85, 25, 160, 135` LS).
  - `CodexManager` + tạo `MemoryFragmentSO` (tối thiểu Mảnh `#01` ở nhánh Boran `LVL-02` và Mảnh `#02` ở nhánh phụ `LVL-03`).

### D. Hệ thống Giao diện (`Assets/Project/Scripts/UI/` & `Prefabs/UI/`)
- [ ] **`HUDController.cs`**: 3 đốm sáng HP/Lives, Mana bar (vạch chia mỗi 20 điểm), bộ đếm Light Shards (max 999), icon hướng gió, Bullet Time vignette, icon Autosave góc dưới phải (1.5s), tự fade-out khi `Cutscene`.
- [ ] **`DialogueUI.cs`**: hộp thoại NPC, nội dung Bia Ký Ức, Gợi ý Checkpoint 90s, thông báo `"Tạm chưa mở khóa"` của Đại Bác.
- [ ] **`ForgeShopUI.cs`**: danh sách nâng cấp từ `UpgradeManager`, khóa + tooltip gợi ý nếu chưa cứu Boran.
- [ ] **`CannonSelectUI.cs`**: hiện danh sách trạm từ `CannonNetworkManager.GetAvailableDestinations()` + nút `Exit`.
- [ ] **`MapUI.cs`**: bật/tắt bằng `Tab` hoặc `M`, Room Grid Map, phủ Fog of War phòng chưa đi, icon Checkpoint / Hub / Đại Bác / NPC (dấu hỏi vàng).
- [ ] **`PauseMenuUI.cs`**: phím `Esc`, đóng băng vật lý, giảm 50% nhạc, gồm Resume / Controls & Hints / Settings / Return to Title (có hộp thoại xác nhận).
- [ ] **`MainMenuUI.cs`**: Continue, New Game (chọn 3 Slot & Campaign/Survival), Settings (Âm lượng, Screen Shake, Assist Mode), Codex, Credits, Quit.

### E. Thiết kế Màn chơi (Scenes) Thuận phụ trách (`Assets/Project/Scenes/`)
- [ ] **`AREA02_TheCradle_Hub.unity`**: Hub Cổ Thụ Ánh Sáng (`SacredTreeNPC`), đặt Khẩu Đại Bác Base (`CANNON_Hub_Base`, mặc định `Locked`).
- [ ] **`LVL02_WhisperingCaverns.unity`**: Hẻm Núi Thì Thầm (giải đố gió nhẹ + gương, nhánh phụ `ShadowCage` nhốt `BoranNPC` + `LoreStele` Mảnh `#01`).
- [ ] **`LVL03_EyeOfTheTempest.unity`**: Đỉnh Gió Gầm (gió giật & bão xoáy, Mảnh `#02`, Khẩu Đại Bác tiền phòng Boss `CANNON_LVL03_BossPrep`, đấu trùm `WindCoreBoss`).

---

## 3. Tóm tắt phần việc của Huy (để Thuận biết & phối hợp)

1. **Interfaces (`Scripts/Environment/Interfaces/`)**: `IInteractable`, `IDamageable`, `IActivatable` *(đẩy lên Git trước để Thuận dùng)*.
2. **Player SRP (`Scripts/Player/`)**: `PlayerInput.inputactions`, `PlayerController`, `PlayerMovement`, `PlayerAbilities`, `PlayerCombat`, `PlayerStats`, `PlayerInteraction`, `PlayerLightAura`.
3. **Camera (`Scripts/Core/Camera/`)**: `CameraFollow2D`, `CameraZoomController`.
4. **Đạn & Pickup (`Scripts/Projectiles/`)**: `ProjectileDataSO`, `LightProjectile` (`CircleCast`, max 3 bounce, lần 4 nổ AOE 1.5u), `FloatingLightOrb` (8s), `LightShardPickup` (hút 2.5u).
5. **Môi trường & Puzzle (`Scripts/Environment/`)**: `ReflectionMirror`, `RemotePedestal`, `RadiantBloom` (8s flare / 60s hồi), `EnergyPylon`, `SealGate`, `LightPressurePad`, `WindZone`, `TravelCannon` + `CannonStationSO`, `CheckpointAltar`, `DarkSpikes`, `DarkAbyss`.
6. **Scenes**: `LVL01_AwakeningHollow.unity` (5 phòng Tutorial) & `AREA03_WindsThreshold.unity` (Trạm Nghỉ Đầu Gió mở Launch + đặt Lò rèn Boran).

---

## 4. Lộ trình 4 Sprint & Điểm đồng bộ (Sync Points)

| Sprint | Huy làm | Thuận làm | Cần chốt / ghép chung |
| :--- | :--- | :--- | :--- |
| **Sprint 1**<br>*Core & Player* | 3 Interfaces (`IInteractable`, `IDamageable`, `IActivatable`) → 6 module Player + `PlayerInput` + `CameraFollow2D`. | `GameManager`, `GameState`, `SaveManager`, `GameSaveData`, `WorldState`, `AudioManager`, `HUDController` cơ bản. | Chốt đúng chữ ký 3 Interface & các C# `event` của `PlayerStats` / `PlayerCombat` để HUD & Save bắt được. |
| **Sprint 2**<br>*Quang học, Quái & UI* | `LightProjectile`, `FloatingLightOrb`, `LightShardPickup`, Gương, Bệ xoay, `EnergyPylon`, `SealGate`, `RadiantBloom`. | `EnemyBase`, `EnemyProjectile`, 4 quái (`Crawler`, `Slime`, `Weaver`, `Spitter`), `DialogueUI`, `PauseMenuUI`, `MainMenuUI`. | Test chung tại scene prototype: đạn Huy phá khiên/giết quái của Thuận, triệt tiêu `EnemyProjectile`, rơi `LightShardPickup`. |
| **Sprint 3**<br>*Tutorial, Hub & Đại Bác* | `TravelCannon`, `CannonStationSO`, `CheckpointAltar`, `LightPressurePad`, `DarkSpikes`, `DarkAbyss`, dựng `LVL01` & `AREA03`. | `CannonNetworkManager`, `CannonSelectUI`, `RoomController`, `TutorialDarkCore`, `ShadowCage`, `EventNPC`, `SacredTreeNPC`, dựng `AREA02`. | Ghép `EventNPC` (Phòng 4) & `TutorialDarkCore` (Phòng 5) vào `LVL01`; test khẩu Base `Locked` ở Hub. |
| **Sprint 4**<br>*Vùng Gió, Boran, Boss & QA* | `WindZone`, cân chỉnh Launch + Air Dash, lighting URP, hoàn thiện `LVL01` & `AREA03`. | `BoranNPC`, `UpgradeManager` + `ForgeShopUI`, `LoreStele` + `CodexManager`, `MapUI`, `WindCoreBoss`, dựng `LVL02` & `LVL03`. | Chạy đủ 5 bài test QA bên dưới + Playtest 20–30 phút toàn bộ Vertical Slice. |

---

## 5. Quy tắc Git & Phối hợp tránh xung đột

1. **Không sửa trực tiếp Scene của người kia:** Mỗi đối tượng (`ShadowCrawler`, `WindCoreBoss`, `TravelCannon`, `ReflectionMirror`, `HUDCanvas`...) phải tạo thành **Prefab** trong `Assets/Project/Prefabs/`. Ai phụ trách scene nào thì tự kéo Prefab vào scene đó.
2. **Push nền tảng trước ở Sprint 1:** Huy push `Interfaces/` trước; Thuận push `GameState.cs` và stub `GameManager.cs` trước để code hai bên luôn compile sạch.
3. **Dùng Placeholder trước, Art/Audio sau:** Sprint 1–3 dùng sprite/hình khối đơn giản và âm thanh mẫu để kiểm tra logic; ghép sprite chi tiết (`Player.aseprite`, Tilemap, VFX) ở Sprint 3–4.

---

## 6. Checklist Nghiệm thu Kỹ thuật (QA Verification — Bắt buộc đạt trước khi nộp)

- [ ] **QA-PHY-01 (Quang học):** Bắn đạn vào 2 gương song song → nảy đúng **3 lần**, lần chạm thứ **4** nổ AOE `1.5 units` và biến mất.
- [ ] **QA-MEC-01 (Launch & Tap Aim):** Chuột phải trên không → bị chặn + tiếng *buzz*; Tap Chuột trái `< 0.25s` → trừ 20 Mana tạo `FloatingLightOrb` (tồn tại `8.0s`); trúng đòn lúc đang Hold ngắm → ngắt Bullet Time không mất Mana.
- [ ] **QA-ENV-01 (Hoa Soi Sáng):** Bắn `RadiantBloom` → sáng toàn phòng `8.0s` (1s bùng nổ, 5s cực đại, 2s mờ dần héo xám) → chờ đúng **`60.0s`** hồi về `Ready`.
- [ ] **QA-CAN-01 (Đại Bác Dịch Chuyển):**
  - Trước khi mở khẩu `LVL-03`: bấm `[F]` tại khẩu Base ở Hub → hiện `"Tạm chưa mở khóa"`.
  - Sau khi mở khẩu tiền phòng Boss `LVL-03`: cả 2 khẩu chuyển `Unlocked_Active`, bấm `[F]` hiện `CannonSelectUI` có điểm đến + nút `Exit`; chọn bay → xoay nòng, hút Tinh linh, tụ lực, bắn bay khỏi màn hình, chuyển cảnh và đáp xuống đúng `landingPoint`.
- [ ] **QA-QST-01 (NPC Chức năng vs NPC Sự kiện):** Bỏ qua Boran ở `LVL-02` sang `LVL-03` → Boran vẫn ở trong lồng chờ cứu (quay lại cứu được). Bỏ qua `EventNPC` Phòng 4 `LVL-01` rời phòng → chuyển `Missed`.

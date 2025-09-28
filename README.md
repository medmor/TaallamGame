# Taallam — Arabic Learning RPG (2D)

A mission-based RPG for kids (ages 6–12). The player starts in a city, receives missions, and learns through quests, dialogue, and mini-games. Primary language: Arabic (RTL).

---

## 🎯 MVP game loop
1) Explore the city → 2) Receive a mission → 3) Complete simple learning goals → 4) Return to turn in → 5) Earn badges/rewards.

## ✅ TODO roadmap

### 0) Project baseline
- [*] Scene: `City` (Tilemap world boundaries + Confiner2D collider)
- [*] Player: top-down movement (Grid- or physics-based)
- [*] Camera: Cinemachine (Framing Transposer) following Player; add Cinemachine Confiner 2D
- [*] Input: Unity Input System
- [*] Save/Load: JSON file (profile per child)
	- Files added:
		- `Assets/Scripts/Save/SaveFile.cs`
		- `Assets/Scripts/Save/SaveSystem.cs`
		- `Assets/Scripts/Save/ProfileManager.cs`
		- `Assets/Scripts/Save/PlayerPositionSaver.cs`
		- `Assets/Plugins/WebGL/WebGLFileSync.jslib` (WebGL persistence)
	- Storage location: `Application.persistentDataPath/saves/<profileId>.json` (Desktop) / IndexedDB (WebGL)
	- How to wire:
		1. Create an empty GameObject named `Systems` in your start scene → Add `ProfileManager` component.
		2. On your Player prefab, add `PlayerPositionSaver`.
		3. When you complete a mission or open a pause/quit menu, call `ProfileManager.Instance.Save()` (via UI button or code).
		4. Optional: add `SaveHotkey` to the `Systems` object to save with F5 (hold Ctrl as well if you enable `requireCtrl`).
	- WebGL notes:
		- The `.jslib` plugin automatically syncs files to IndexedDB after saves and on load.
		- Data persists per origin; private browsing or cleared site data will remove saves.
	- Test: Enter Play Mode, move the player, press F5 to save (or use your pause menu), stop, Play again → the player should spawn at the saved position.

### 1) Mission system
Files added:
- `Assets/Scripts/Missions/MissionDefinition.cs`
- `Assets/Scripts/Missions/MissionManager.cs`
- `Assets/Scripts/Missions/MissionHud.cs`
- `Assets/Scripts/Missions/ReachAreaTrigger.cs`
- `Assets/Scripts/Missions/MissionInteractable.cs`
- `Assets/Scripts/Missions/CollectReporter.cs`

How to use:
1. Create mission assets
	- Project → Create → Taallam → Mission Definition
	- Fill `id`, Arabic title/summary, and add `goals` in order (types: ReachArea, Interact, Collect, Quiz, MiniGame)
2. Add MissionManager to the scene
	- On `Systems` GameObject add `MissionManager`
	- Drag your Mission Definition assets into the `missions` list
3. Show a simple HUD (optional)
	- Canvas → TextMeshProUGUI → add `MissionHud` and assign the label field
4. Place triggers/reporters
	- Reach area: add `ReachAreaTrigger` to a trigger collider; set `areaId`
	- Interact: on an NPC, add `MissionInteractable` and call `Interact()` when the player talks
	- Collect: call `CollectReporter.Report()` after picking up an item
5. Start and complete missions in code/UI
	- Accept: `MissionManager.Instance.AcceptMission("your_mission_id")`
	- After last step, talk to the giver and call: `MissionManager.Instance.TurnInActive()`

Persistence:
- The mission states are saved/loaded via the existing save system. `MissionSaveData` now includes `subProgress` for counted goals.

States supported:
- `Locked → Available → Active → TurnIn → Completed`

### 2) How to present missions (pick at least one for MVP)
- [ ] NPC Giver: talk to a Teacher/Policeman/Librarian to accept/turn in
- [ ] Quest Board: bulletin board in the plaza listing available missions
- [ ] Smartphone/Notebook UI: pop-up notifications and journal of tasks

MVP choice: NPC Giver (simple, story-friendly). Others can be added later.

### 3) Dialogue system (Arabic-first)
- [ ] Use TextMeshPro UGUI with an Arabic-supporting font (e.g., Noto Naskh Arabic)
- [ ] Enable RTL rendering (TMP right-to-left or RTL TextMeshPro package if needed)
- [ ] Data format for lines/choices (Ink/Yarn Spinner OR lightweight JSON/ScriptableObject)
- [ ] UI: bottom panel with speaker name, text, and optional choices
- [ ] Typing effect + input to advance; ability to trigger mission offers/turn-ins

Minimal line shape (example):

```json
{
  "id": "intro_001",
  "speaker_ar": "الشرطي",
  "text_ar": "مرحباً! هل تريد مساعدتي اليوم؟",
  "choices": [
	 { "text_ar": "نعم بالتأكيد!", "next": "mission_offer" },
	 { "text_ar": "لاحقاً.", "next": "goodbye" }
  ]
}
```

### 4) Arabic and localization
- [ ] All UI and content authored in Arabic (primary). Optional English fallback later
- [ ] Fonts: embed Arabic font with good readability for kids (large size, high contrast)
- [ ] Numerals: decide on Arabic-Indic vs. Western digits, keep consistent
- [ ] Text direction: ensure RTL in dialogue, HUD, and quest log
- [ ] Proofread for diacritics where it improves clarity (grades 1–3)

### 5) Learning design (ages 6–12)
- [ ] Align each mission with a skill (reading, basic math, science, safety, civics)
- [ ] Provide scaffolding: hints, retries, and positive feedback
- [ ] Keep sessions short (3–6 minutes/mission) with 2–4 steps each
- [ ] Progression: unlock harder variants; badge per skill area

### 6) UI/UX
- [ ] HUD: active mission title and the next goal in one line
- [ ] Quest Journal: list of available/active/completed missions
- [ ] Map/Minimap: optional markers for goals; colorblind-friendly palette
- [ ] Accessibility: readable fonts, large hit targets, audio cues when possible

### 7) Tech glue
- [ ] Event bus (C# events) for: dialogue events, mission state change, goal completed
- [ ] Simple achievement/badge system per skill
- [ ] Autosave on mission complete

---

## 🧭 City start setup (checklist)
- [ ] Create `City` scene with Tilemaps and colliders
- [ ] Place `Player` prefab at spawn point
- [ ] Add `CinemachineCamera` → Position Control: Framing Transposer → Follow: Player
- [ ] Add `Cinemachine Confiner 2D` bound to a polygon collider around the city
- [ ] Place 1–2 NPCs (Teacher/Librarian) with interaction prompts

---

## 📚 Mission ideas (learning-focused)

1) شرطي المرور — عبور الشارع بأمان (سلامة مرورية)
	- تحديات: اختيار مكان العبور الصحيح، انتظار الإشارة الخضراء، النظر يميناً/يساراً
	- نشاط: أسئلة اختيار من متعدد على لافتات الطريق

2) متجر الخضار — الحساب بالمبالغ الصغيرة (رياضيات)
	- تحديات: جمع الأسعار، اختيار التوليفة الصحيحة من العملات لإعطاء البائع
	- نشاط: لعبة سحب وإفلات للعملات لإعطاء الباقي الصحيح

3) المكتبة — ترتيب الحروف والبحث عن كتاب (لغة عربية)
	- تحديات: ترتيب كتب حسب الحرف الأول، العثور على كلمة تطابق بطاقة الفهرس
	- نشاط: تكوين كلمة صحيحة من حروف مبعثرة (مثال: "مدرسة")

4) الحديقة — فرز النفايات (علوم وبيئة)
	- تحديات: فرز ورق/بلاستيك/معدن في الحاوية المناسبة
	- نشاط: تفسير لماذا هذا الاختيار صحيح بلغة مبسطة

5) مركز الأنشطة — قراءة الوقت (رياضيات/حياة يومية)
	- تحديات: ضبط ساعة إلى ٣:٣٠، مطابقة الوقت الرقمي مع التناظري

6) المركز الصحي — وجبة صحية (تربية صحية)
	- تحديات: تكوين وجبة متوازنة من بطاقات طعام، تجنب السكر الزائد

7) المتحف — الأشكال والألوان (رياضيات مبكرة)
	- تحديات: تحديد شكل الإشارة (مثلث/مربع/دائرة) وألوان التنبيه

8) الساحة — كلمات مفقودة وحروف مشكّلة (لغة)
	- تحديات: اختيار الحرف الصحيح لإكمال الكلمة، وضع التشكيل البسيط حيث يفيد المعنى

9) محطة الحافلات — قراءة الخرائط (جغرافيا محلية)
	- تحديات: إيجاد المسار إلى المدرسة على خريطة صغيرة للمدينة

10) الاستعداد لرمضان/العيد — تخطيط مهام (تربية اجتماعية)
	- تحديات: ترتيب خطوات التحضير، مشاركة الأسرة، أدب الزيارة والتهنئة

---

## 🧱 Suggested data shapes (sketch)

MissionDefinition (ScriptableObject):
- `string id`
- `string title_ar`
- `string summary_ar`
- `string giverNpcId`
- `List<Goal>` goals
- `Reward reward`

Goal:
- `GoalType type` (ReachArea, Interact, Collect, Quiz, MiniGame)
- `string targetId`
- `int amount` (optional)
- `string payload` (quiz id, dialogue node id, etc.)

State persisted in save file per mission: `available`, `active`, `progress`, `completed`.

---

## 🔎 Acceptance criteria (MVP)
- The player spawns in the city and the camera follows correctly
- At least 1 mission giver NPC; 3 short missions from the list above
- Dialogue panel displays Arabic RTL text with a readable font
- Mission tracker shows the current step and updates as goals complete
- Progress is saved and restored between sessions

---

## 🚀 Next steps
1) Implement MissionDefinition ScriptableObject + MissionManager
2) Build one NPC dialogue flow that offers the traffic-safety mission
3) Create the City scene boundaries and hook up Cinemachine follow + Confiner
4) Add a simple HUD mission tracker and a basic journal screen
5) Playtest with kids in the target age range and iterate on clarity and difficulty

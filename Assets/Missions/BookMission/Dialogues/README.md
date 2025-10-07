# Ink Mission System Migration Guide

## Overview
The mission system has been refactored to be driven entirely by Ink stories. All mission logic, progression, and state is now handled in Ink files, with Unity acting as a bridge for world effects.

## Quick Setup

### 1. Add InkMissionBridge to Scene
- Create an empty GameObject named "InkMissionBridge" 
- Add the `InkMissionBridge` component
- Enable "Debug Logging" for testing

### 2. Mission Files Structure
```
Assets/Missions/BookMission/
├── globals.ink          # Mission variables and functions
├── 01_Start.ink         # Teacher (mission giver)
├── 02_Leila.ink         # Letter K collector
├── 02_Vendor.ink        # Letter T trader
├── 02_Child.ink         # Letter A mini-game
├── 02_FruitSeller.ink   # Letter B + Apple giver
└── 03_TurnIn.ink        # Librarian (mission completer)
```

### 3. Hook Up NPCs
For each NPC, add a `DialogueTrigger` component:
- Assign the appropriate .ink.json file (e.g., `01_Start.ink.json` for teacher)
- DialogueManager will automatically bind external functions

### 4. Listen to Mission Events
```csharp
// In your UI/game systems
void Start()
{
    var bridge = InkMissionBridge.Instance;
    if (bridge != null)
    {
        bridge.OnMissionStarted += HandleMissionStarted;
        bridge.OnMissionCompleted += HandleMissionCompleted;
        bridge.OnItemGiven += HandleItemGiven;
        bridge.OnSoundRequested += HandleSoundRequested;
    }
}

void HandleMissionStarted(string missionId)
{
    Debug.Log($"Mission started: {missionId}");
    // Update UI, play sound, etc.
}
```

## Mission Flow Example

### 1. Start Mission (Teacher)
```ink
+ [نعم، أقبل المهمة]
    ~ start_book_mission()  // Sets variables + calls external_StartMission
    رائع! اجمع الحروف الأربعة...
    -> END
```

### 2. Collect Items (NPCs)
```ink
+ [نعم، قلت "كَـ"]
    ~ collect_letter("k")   // Sets has_letter_k = true + calls external_GiveItem
    ممتاز! تفضل هذا الحرف: «ك».
    -> END
```

### 3. Complete Mission (Librarian)
```ink
{ complete_book_mission():  // Checks completion + calls external_CompleteMission
    أحسنت! لقد جمعت كل الحروف وكونت الكتاب.
    -> END
}
```

## External Functions Available

All these functions can be called from Ink:

- `external_StartMission(missionId)` - Fires OnMissionStarted event
- `external_CompleteMission(missionId)` - Fires OnMissionCompleted event  
- `external_GiveItem(itemId)` - Fires OnItemGiven event (for inventory)
- `external_PlaySound(soundId)` - Fires OnSoundRequested event
- `external_ShowEffect(effectId)` - Fires OnEffectRequested event
- `external_SaveProgress()` - Saves current story state

## Migration from Old MissionManager

### What Changed
- Mission logic moved from C# → Ink files
- MissionManager replaced with InkMissionBridge
- State tracking via Ink variables instead of C# enums
- External function calls for Unity side effects

### Compatibility
- Old MissionManager marked as obsolete but still functional
- MissionInteractable routes through InkMissionBridge first
- Legacy ReportInteract/ReportCollect calls are logged

### Full Migration Steps
1. ✅ Add InkMissionBridge to scene
2. ✅ Create mission .ink files with external function calls
3. ✅ Hook up NPCs with DialogueTrigger components
4. ✅ Test mission flow end-to-end
5. 🔄 Remove MissionManager references from UI/systems
6. 🔄 Delete old MissionDefinition ScriptableObjects
7. 🔄 Delete MissionManager.cs when no references remain

## Testing Checklist

- [ ] Teacher starts mission correctly
- [ ] Each NPC gives their letter/item
- [ ] Apple trading works (vendor)
- [ ] Librarian completes mission
- [ ] Save/load preserves progress
- [ ] UI updates on mission events
- [ ] No console errors or warnings

## Troubleshooting

**External functions not working?**
- Check InkMissionBridge is in scene and referenced
- Verify .ink files are compiled to .json
- Enable debug logging to see function calls

**Progress not saving?**
- Ensure `external_SaveProgress()` is called after state changes
- Check PlayerPrefs for `ink_story_state_*` keys

**Mission flow stuck?**
- Use Ink's web player to test .ink files directly
- Check mission status with `get_mission_status()` function
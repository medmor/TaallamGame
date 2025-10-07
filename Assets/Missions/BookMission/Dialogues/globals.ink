// Book Mission - Global Variables and Functions
// All mission logic is handled in Ink

// Mission state variables
VAR book_mission_started = false
VAR book_mission_completed = false

// Collection progress
VAR has_letter_k = false
VAR has_letter_t = false
VAR has_letter_a = false
VAR has_letter_b = false

// Helper items
VAR has_apple = false
VAR has_book = false

// NPC interaction flags
VAR talked_to_teacher = false
VAR talked_to_leila = false
VAR talked_to_vendor = false
VAR talked_to_librarian = false

// External functions - Unity will bind these
EXTERNAL external_StartMission(missionId)
EXTERNAL external_CompleteMission(missionId)
EXTERNAL external_GiveItem(itemId)
EXTERNAL external_PlaySound(soundId)
EXTERNAL external_ShowEffect(effectId)
EXTERNAL external_SaveProgress()
EXTERNAL debug_log(message)

=== function is_mission_complete ===
{
    - has_letter_k and has_letter_t and has_letter_a and has_letter_b:
        ~ return true
    - else:
        ~ return false
}

=== function can_make_book ===
{
    - is_mission_complete():
        ~ return true
    - else:
        ~ return false
}

=== function start_book_mission ===
~ book_mission_started = true
~ talked_to_teacher = true
~ external_StartMission("book_mission")
~ external_SaveProgress()

=== function complete_book_mission ===
{
    - can_make_book():
        ~ book_mission_completed = true
        ~ has_book = true
        ~ external_GiveItem("complete_book")
        ~ external_CompleteMission("book_mission")
        ~ external_PlaySound("mission_complete")
        ~ external_SaveProgress()
        ~ return true
    - else:
        ~ return false
}

=== function collect_letter(letter) ===
{
    - letter == "k":
        ~ has_letter_k = true
        ~ external_GiveItem("letter_k")
    - letter == "t":
        ~ has_letter_t = true
        ~ external_GiveItem("letter_t")
    - letter == "a":
        ~ has_letter_a = true
        ~ external_GiveItem("letter_a")
    - letter == "b":
        ~ has_letter_b = true
        ~ external_GiveItem("letter_b")
}
~ external_PlaySound("collect_item")
~ external_SaveProgress()

=== function get_apple ===
~ has_apple = true
~ external_GiveItem("apple")
~ external_SaveProgress()

=== function use_apple ===
{
    - has_apple:
        ~ has_apple = false
        ~ return true
    - else:
        ~ return false
}

=== function get_mission_status ===
{
    - book_mission_completed:
        ~ return "completed"
    - book_mission_started and is_mission_complete():
        ~ return "ready_to_complete"
    - book_mission_started:
        ~ return "in_progress"
    - else:
        ~ return "not_started"
}
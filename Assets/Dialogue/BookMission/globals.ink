// متغيرات المهمة العامة
VAR has_k = false  // حرف ك
VAR has_t = false  // حرف ت
VAR has_a = false  // حرف ا
VAR has_b = false  // حرف ب
VAR has_apple = false // تفاحة (للتبادل عند البائع)
VAR mission_accepted = false
VAR mission_completed = false

=== function is_mission_complete ===
{ has_k and has_t and has_a and has_b:
    ~ mission_completed = true
    ~ return true
- else:
    ~ return false
}
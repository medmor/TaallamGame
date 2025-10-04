# Use quoted include to avoid path parsing issues
INCLUDE globals.ink

== start ==
#speaker:أمينة #portrait:amina_neutral #layout:left #audio:animal_crossing_mid
أهلًا بك! هل جمعت الحروف لتكوين كلمة "كتاب"؟
{ is_mission_complete():
    #speaker:أمينة #portrait:amina_happy
    أحسنت! هذه وسام صغير ومكافأة لجهدك.
    ~ mission_accepted = false
    ~ mission_completed = true
    -> END
- else:
    #speaker:أمينة #portrait:amina_neutral
    يبدو أنك لم تجمع كل الحروف بعد. استمر في البحث في ساحة المدرسة.
    -> END
}
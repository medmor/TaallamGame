INCLUDE globals.ink

-> door_start

=== door_start ===
أين تريد الذهاب؟ #speaker:الباب #portrait:door_neutral

+ [غرفة المعلمة]
    -> tp_teacher_room
+ [الممر الرئيسي]
    -> tp_hall
+ [غرفة الموسيقى]
    -> tp_music
+ {got_roof_key} [السطح]
    -> tp_roof
+ [غرفة الورشة]
    -> tp_workshop
+ [غرفة التنظيف]
    -> tp_cleaning
+ [لاحقاً]
    -> END

=== tp_teacher_room ===
~ fade_to_black(5)
~ teleport_to("teacher_room")
-> END

=== tp_hall ===
~ fade_to_black(5)
~ teleport_to("hall")
-> END

=== tp_music ===
~ fade_to_black(5)
~ teleport_to("music")
-> END

=== tp_roof ===
~ fade_to_black(5)
~ teleport_to("roof")
-> END

=== tp_workshop ===
~ fade_to_black(5)
~ teleport_to("workshop")
-> END

=== tp_cleaning ===
~ fade_to_black(5)
~ teleport_to("cleaning")
-> END

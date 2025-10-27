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
~ teleport_to("teacher_room")
-> END

=== tp_hall ===
~ teleport_to("hall")
-> END

=== tp_music ===
~ teleport_to("music")
-> END

=== tp_roof ===
~ teleport_to("roof")
-> END

=== tp_workshop ===
~ teleport_to("workshop")
-> END

=== tp_cleaning ===
~ teleport_to("cleaning")
-> END

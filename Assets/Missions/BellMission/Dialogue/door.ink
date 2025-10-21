INCLUDE globals.ink

-> door_start

=== door_start ===
أين تريد الذهاب؟ #speaker:الباب #portrait:door_neutral

+ [الممر الرئيسي]
    -> tp_hall
+ [غرفة الورشة]
    -> tp_workshop
+ [غرفة الموسيقى]
    -> tp_music
+ {got_roof_key} [السطح]
    -> tp_roof
+ [لاحقاً]
    -> END

=== tp_hall ===
~ teleport_to("hall")
-> END

=== tp_workshop ===
~ teleport_to("workshop")
-> END

=== tp_music ===
~ teleport_to("music")
-> END

=== tp_roof ===
~ teleport_to("roof")
-> END

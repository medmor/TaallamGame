INCLUDE globals.ink

== start ==
#speaker:ليلى #portrait:leila_neutral #layout:right #audio:animal_crossing_mid
أهلاً! أريد أن ألعب. هل تساعدينني على نطق هذا الحرف؟
+ [نعم، ساعدني]
    #speaker:ليلى #portrait:leila_happy
    رائع! أنطق الحرف "ك" معي: كَـ — هل قلتها؟
    -> leila_pronounce
+ [لا الآن]
    #speaker:ليلى #portrait:leila_neutral
    حسناً. أراك لاحقًا!
    -> END

== leila_pronounce ==
# pronunciation choices moved here so the previous text diverts cleanly
+ [نعم]
    ~ has_k = true
    #speaker:ليلى #portrait:leila_happy #audio:animal_crossing_low
    ممتاز! تفضل هذا الحرف: «ك».
    -> END
+ [لا]
    #speaker:ليلى #portrait:leila_sad
    لا مشكلة، تعال لاحقًا عندما تكون مستعدًا.
    -> END
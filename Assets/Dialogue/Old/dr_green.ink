INCLUDE globals.ink

مرحبًا! #speaker:Dr. Green #portrait:dr_green_neutral #layout:left #audio:animal_crossing_mid
-> main

=== main ===
كيف تشعر اليوم؟
+ [سعيد]
    ~ playEmote("exclamation")
    هذا يجعلني أشعر <color=\#F8FF30>سعيدًا</color> أيضًا! #portrait:dr_green_happy
+ [حزين]
    حسنًا، هذا يجعلني <color=\#5B81FF>حزينًا</color> أيضًا. #portrait:dr_green_sad
    
- لا تثق به، إنه <b><color=\#FF1E35>ليس</color></b> طبيبًا حقيقيًا! #speaker:Ms. Yellow #portrait:ms_yellow_neutral #layout:right #audio:animal_crossing_high

~ playEmote("question")
حسنًا، هل لديك أي أسئلة أخرى؟ #speaker:Dr. Green #portrait:dr_green_neutral #layout:left #audio:animal_crossing_mid
+ [نعم]
    -> main
+ [لا]
    إلى اللقاء إذًا!
    ~ playEmote("exclamation")
    -> END
INCLUDE globals.ink

== start ==
#speaker:ليلى #portrait:leila_neutral #layout:right #audio:animal_crossing_mid
{
    - has_letter_k:
        أهلاً مرة أخرى! أتمنى أن تستفيد من الحرف "ك" الذي أعطيتك إياه.
        -> END
    - book_mission_started:
        أهلاً! أريد أن ألعب. هل تساعدينني على نطق هذا الحرف؟
        -> help_with_letter
    - else:
        أهلاً! أنا ليلى. أحب تعلم الحروف!
        -> casual_chat
}

== help_with_letter ==
+ [نعم، ساعدني]
    #speaker:ليلى #portrait:leila_happy
    رائع! أنطق الحرف "ك" معي: كَـ — هل قلتها؟
    -> pronunciation_check
+ [لا الآن]
    #speaker:ليلى #portrait:leila_neutral
    حسناً. أراك لاحقاً!
    -> END

== pronunciation_check ==
+ [نعم، قلت "كَـ"]
    ~ collect_letter("k")
    ~ talked_to_leila = true
    #speaker:ليلى #portrait:leila_happy #audio:animal_crossing_low
    ممتاز! تفضل هذا الحرف: «ك».
    أنت تتعلم بسرعة! استخدم هذا الحرف في مهمة الكتاب.
    -> END
+ [لا، لم أستطع]
    #speaker:ليلى #portrait:leila_sad
    لا مشكلة، تعال لاحقاً عندما تكون مستعداً لتجربة نطق الحرف.
    -> END

== casual_chat ==
+ [أريد تعلم الحروف أيضاً]
    #speaker:ليلى #portrait:leila_happy
    هذا رائع! اذهب إلى المعلم سامي، لديه مهمة ممتعة عن الحروف!
    -> END
+ [مع السلامة]
    #speaker:ليلى #portrait:leila_neutral
    مع السلامة! أتمنى لك يوماً سعيداً!
    -> END
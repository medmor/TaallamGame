INCLUDE globals.ink

== start ==
#speaker:ليلى الطالبة #portrait:leila_neutral #layout:right #audio:animal_crossing_mid
{
    - has_letter_k:
        أهلاً مرة أخرى! أتمنى أن تستفيد من الحرف "ك" الذي أعطيتك إياه.
        -> END
    - book_mission_started:
        أهلاً! أدرس في هذه الطاولة. هل تساعدني على نطق هذا الحرف؟
        -> help_with_letter
    - else:
        أهلاً! أنا ليلى. أحب القراءة والتعلم في المكتبة!
        -> casual_chat
}

== help_with_letter ==
+ [نعم، ساعدني]
    #speaker:ليلى الطالبة #portrait:leila_happy
    رائع! أنطق الحرف "ك" معي: كَـ — هل قلتها؟
    -> pronunciation_check
+ [لا الآن]
    #speaker:ليلى الطالبة #portrait:leila_neutral
    حسناً. سأكون هنا أدرس إذا احتجتني!
    -> END

== pronunciation_check ==
+ [نعم، قلت "كَـ"]
    ~ collect_letter("k")
    ~ talked_to_leila = true
    #speaker:ليلى الطالبة #portrait:leila_happy #audio:animal_crossing_low
    ممتاز! تفضل هذا الحرف: «ك».
    أنت تتعلم بسرعة! استخدم هذا الحرف في مهمة الكتاب.
    -> END
+ [لا، لم أستطع]
    #speaker:ليلى الطالبة #portrait:leila_sad
    لا مشكلة، تعال لاحقاً عندما تكون مستعداً لتجربة نطق الحرف.
    -> END

== casual_chat ==
+ [أريد تعلم الحروف أيضاً]
    #speaker:ليلى الطالبة #portrait:leila_happy
    هذا رائع! اذهب إلى أمينة المكتبة، لديها مهمة ممتعة عن الحروف!
    -> END
+ [مع السلامة]
    #speaker:ليلى الطالبة #portrait:leila_neutral
    مع السلامة! أتمنى لك دراسة مفيدة!
    -> END
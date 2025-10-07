INCLUDE globals.ink

== start ==
#speaker:المعلم سامي #portrait:teacher_neutral #layout:left #audio:animal_crossing_mid
{
    - get_mission_status() == "completed":
        أهلاً! أرى أنك أنجزت مهمة الحروف بنجاح. أحسنت!
        -> END
    - get_mission_status() == "in_progress":
        كيف تسير مهمة جمع الحروف؟
        -> check_progress
    - book_mission_started:
        أهلاً مرة أخرى! هل تحتاج تذكيراً بالمهمة؟
        -> remind_mission
    - else:
        مرحباً! اليوم لدينا لعبة تعليمية ممتعة. هل تستطيع مساعدتي في جمع حروف كلمة "كتاب"؟
        -> offer_mission
}

== offer_mission ==
+ [نعم، أقبل المهمة]
    ~ start_book_mission()
    #speaker:المعلم سامي #portrait:teacher_happy
    رائع! مهمتك هي جمع الحروف الأربعة: "ك"، "ت"، "ا"، "ب".
    تعال إلى ساحة المدرسة وابحث عن:
    - ليلى الطفلة الصغيرة - لديها حرف "ك"
    - عادل البائع - لديه حرف "ت" (ستحتاج تفاحة للمقايضة)
    - ابحث عن الحروف الأخرى في الساحة
    عندما تجمع كل الحروف، اذهب إلى أمينة المكتبة لتكوين الكتاب!
    -> END
+ [لا الآن]
    #speaker:المعلم سامي #portrait:teacher_neutral
    فهمت. عد إليّ عندما تكون مستعداً للمغامرة التعليمية!
    -> END

== remind_mission ==
+ [نعم، ذكرني بالمهمة]
    #speaker:المعلم سامي #portrait:teacher_neutral
    مهمتك جمع الحروف الأربعة لكلمة "كتاب": "ك"، "ت"، "ا"، "ب".
    ابحث في ساحة المدرسة وتحدث مع الأطفال والبائعين.
    عندما تجمع كل الحروف، اذهب إلى المكتبة لتكوين الكتاب!
    -> END
+ [لا، أتذكر]
    #speaker:المعلم سامي #portrait:teacher_happy
    ممتاز! حظاً موفقاً في جمع الحروف.
    -> END

== check_progress ==
{
    - is_mission_complete():
        #speaker:المعلم سامي #portrait:teacher_proud
        رائع! لقد جمعت كل الحروف. الآن اذهب إلى أمينة المكتبة لتكوين الكتاب!
        -> END
    - else:
        #speaker:المعلم سامي #portrait:teacher_neutral
        دعني أرى... ما زلت بحاجة لـ:
        { not has_letter_k: - الحرف "ك" من ليلى }
        { not has_letter_t: - الحرف "ت" من عادل البائع }
        { not has_letter_a: - الحرف "ا" }
        { not has_letter_b: - الحرف "ب" }
        استمر في البحث في ساحة المدرسة!
        -> END
}
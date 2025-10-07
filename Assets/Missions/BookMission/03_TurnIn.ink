INCLUDE globals.ink

== start ==
#speaker:أمينة المكتبة #portrait:librarian_neutral #layout:right #audio:animal_crossing_low
{
    - get_mission_status() == "completed":
        أهلاً! أرى أنك أنجزت مهمة الكتاب بالفعل. أحسنت!
        -> END
    - get_mission_status() == "ready_to_complete":
        هل أحضرت كتاب "ألِف باء" مكتملاً؟
        -> ready_to_complete
    - get_mission_status() == "in_progress":
        هل أحضرت كتاب "ألِف باء"؟
        -> check_progress
    - else:
        أهلاً بك في المكتبة! هل تحتاج المساعدة في العثور على شيء؟
        -> no_mission
}

== ready_to_complete ==
+ [نعم، هذا هو الكتاب مكتملاً]
    { complete_book_mission():
        #speaker:أمينة المكتبة #portrait:librarian_happy
        أحسنت! لقد جمعت كل الحروف وكونت الكتاب. شكراً لك!
        ~ talked_to_librarian = true
        -> END
    - else:
        #speaker:أمينة المكتبة #portrait:librarian_confused
        هناك خطأ ما... دعني أتحقق مرة أخرى.
        -> END
    }
+ [ليس بعد]
    #speaker:أمينة المكتبة #portrait:librarian_neutral
    لا مشكلة. عد عندما تجمع كل الحروف وتكون الكتاب.
    -> END

== check_progress ==
{
    - has_letter_k and has_letter_t and has_letter_a and has_letter_b:
        يبدو أن لديك كل الحروف! الآن يمكنك تكوين الكتاب.
        -> ready_to_complete
    - else:
        ما زلت بحاجة لجمع بعض الحروف:
        { not has_letter_k: - الحرف "ك" }
        { not has_letter_t: - الحرف "ت" }
        { not has_letter_a: - الحرف "ا" }
        { not has_letter_b: - الحرف "ب" }
        ابحث في ساحة المدرسة وتحدث مع الأطفال والبائعين.
        -> END
}

== no_mission ==
+ [أبحث عن كتاب للأطفال]
    #speaker:أمينة المكتبة #portrait:librarian_helpful
    ممتاز! لدينا قسم جميل للأطفال. ابحث عن كتاب "ألِف باء" - إنه مفيد جداً لتعلم الحروف.
    -> END
+ [لا شكراً]
    #speaker:أمينة المكتبة #portrait:librarian_neutral
    حسناً، أتمنى لك يوماً سعيداً!
    -> END
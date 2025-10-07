INCLUDE globals.ink

-> start

=== start ===
#speaker:أمينة المكتبة #portrait:librarian_neutral #layout:right #audio:animal_crossing_mid
{get_mission_status():
- "completed":
    أهلاً! أرى أنك أنجزت مهمة الحروف بنجاح. أحسنت!
    -> END
- "in_progress":
    كيف تسير مهمة جمع الحروف؟
    -> check_progress
- "ready_to_complete":
    رائع! لقد جمعت كل الحروف. الآن يمكنك تكوين الكتاب!
    -> END
- else:
    مرحباً! مرحباً بك في المكتبة. هل تريد أن نلعب لعبة الحروف؟ سنجمع حروف كلمة "كتاب"!
    -> offer_mission
}

=== offer_mission ===
+ [نعم، أقبل المهمة]
    ~ start_book_mission()
    #speaker:أمينة المكتبة #portrait:librarian_happy
    رائع! مهمتك هي جمع الحروف الأربعة: "ك"، "ت"، "ا"، "ب".
    ابحث في أرجاء المكتبة وتحدث مع:
    - ليلى الطالبة عند الطاولة الشرقية - لديها حرف "ك"
    - أحمد الطالب في قسم القراءة - لديه حرف "ت" 
    - فاطمة تقرأ قصة - لديها حرف "ا"
    - ابحث عن حرف "ب" في الكتب المبعثرة
    عندما تجمع كل الحروف، عد إليّ لتكوين الكتاب!
    -> END
+ [لا الآن]
    #speaker:أمينة المكتبة #portrait:librarian_neutral
    فهمت. عد إليّ عندما تكون مستعداً للعبة التعليمية!
    -> END

=== remind_mission ===
+ [نعم، ذكرني بالمهمة]
    #speaker:أمينة المكتبة #portrait:librarian_neutral
    مهمتك جمع الحروف الأربعة لكلمة "كتاب": "ك"، "ت"، "ا"، "ب".
    ابحث في المكتبة وتحدث مع الطلاب والطالبات في أقسام المكتبة المختلفة.
    عندما تجمع كل الحروف، عد إليّ لتكوين الكتاب!
    -> END
+ [لا، أتذكر]
    #speaker:أمينة المكتبة #portrait:librarian_happy
    ممتاز! حظاً موفقاً في جمع الحروف.
    -> END

=== check_progress ===
{
    - is_mission_complete():
        #speaker:أمينة المكتبة #portrait:librarian_proud
        رائع! لقد جمعت كل الحروف. الآن يمكنك تكوين الكتاب معي!
        -> END
    - else:
        #speaker:أمينة المكتبة #portrait:librarian_neutral
        دعني أرى... ما زلت بحاجة لـ:
        { not has_letter_k: - الحرف "ك" من ليلى عند الطاولة الشرقية }
        { not has_letter_t: - الحرف "ت" من أحمد في قسم القراءة }
        { not has_letter_a: - الحرف "ا" من فاطمة التي تقرأ }
        { not has_letter_b: - الحرف "ب" من الكتب المبعثرة }
        استمر في البحث في المكتبة!
        -> END
}
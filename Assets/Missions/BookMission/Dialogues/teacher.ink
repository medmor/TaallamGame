INCLUDE globals.ink

// Teacher - Mission Giver and Completer
// Starts the mission and completes it when all letters are collected

-> teacher_start

=== teacher_start ===
{
    - book_mission_completed:
        -> teacher_mission_completed
    - book_mission_started and can_make_book():
        -> teacher_ready_to_complete
    - book_mission_started:
        -> teacher_mission_in_progress
    - else:
        -> teacher_first_meeting
}

=== teacher_first_meeting ===
أهلاً وسهلاً يا طالبي العزيز! #speaker:المعلمة #portrait:teacher_happy #layout:left #audio:teacher

اليوم سنتعلم شيئاً جديداً ومثيراً. هل أنت مستعد للمغامرة؟

+ [نعم، أنا مستعد!]
    -> explain_mission
+ [ما نوع المغامرة؟]
    -> explain_mission
+ [لست متأكداً...]
    -> encourage_student

=== encourage_student ===
لا تقلق! ستكون مغامرة ممتعة وسهلة. #speaker:المعلمة #portrait:teacher_happy

ستتعلم حروفاً جديدة وتكوّن كلمة جميلة!

+ [حسناً، سأجرب]
    -> explain_mission

=== explain_mission ===
ممتاز! مهمتك اليوم هي جمع أربعة حروف خاصة. #speaker:المعلمة #portrait:teacher_thinking

الحروف هي: ك، ت، ا، ب

+ [ما هذه الحروف؟]
    -> explain_letters
+ [أين أجد هذه الحروف؟]
    -> explain_where
+ [ماذا أفعل بها؟]
    -> hint_about_word

=== explain_letters ===
هذه حروف عربية جميلة! #speaker:المعلمة #portrait:teacher_happy

ك - كاف، ت - تاء، ا - ألف، ب - باء

عندما تجمعها كلها، ستكوّن كلمة رائعة!

+ [أين أجد هذه الحروف؟]
    -> explain_where
+ [ما الكلمة التي ستتكون؟]
    -> hint_about_word

=== hint_about_word ===
هذا سر! ستعرف عندما تجمع كل الحروف. #speaker:المعلمة #portrait:teacher_thinking

لكن أستطيع أن أقول لك أنها شيء نقرأ منه ونتعلم!

+ [أين أجد الحروف؟]
    -> explain_where

=== explain_where ===
الحروف مع أصدقائك في المدرسة! #speaker:المعلمة #portrait:teacher_thinking

تحدث مع ليلى، فاطمة، أحمد، وأمينة المكتبة. كل واحد منهم لديه حرف مختلف.

+ [كيف سأحصل على الحروف؟]
    -> explain_challenges
+ [حسناً، سأذهب الآن!]
    -> start_mission

=== explain_challenges ===
كل شخص قد يعطيك تحدياً صغيراً أو سؤالاً. #speaker:المعلمة #portrait:teacher_thinking

أجب عليهم بصدق وساعدهم، وسيعطونك الحرف!

+ [أفهم! سأبدأ الآن]
    -> start_mission
    -> start_mission

=== start_mission ===
رائع! اذهب واجمع الحروف الأربعة. #speaker:المعلمة #portrait:teacher_happy

عندما تجمعها كلها، عد إليّ وسنكوّن الكلمة معاً!

~ start_book_mission()

+ [سأعود قريباً!]
    -> teacher_goodbye

=== teacher_mission_in_progress ===
أهلاً مرة أخرى! كيف تسير مهمة جمع الحروف؟ #speaker:المعلمة #portrait:teacher_thinking

{
    - has_letter_k and has_letter_t and has_letter_a:
        ممتاز! لديك ثلاثة حروف. ينقصك حرف الـ"ب" فقط!
    - has_letter_k and has_letter_t:
        جيد جداً! لديك حرفان. استمر في البحث!
    - has_letter_k or has_letter_t or has_letter_a or has_letter_b:
        بداية رائعة! لديك حرف واحد. ابحث عن الباقي!
    - else:
        لم تجد أي حروف بعد؟ تحدث مع الأصدقاء في المدرسة!
}

+ [أذكرني، من أتحدث معه؟]
    -> remind_npcs
+ [سأواصل البحث]
    -> teacher_goodbye

=== remind_npcs ===
الأصدقاء هم: ليلى، فاطمة، أحمد، وأمينة المكتبة. #speaker:المعلمة #portrait:teacher_thinking

كل واحد منهم لديه حرف مختلف ينتظرك!

+ [شكراً للتذكير]
    -> teacher_goodbye

=== teacher_ready_to_complete ===
مذهل! لديك كل الحروف الأربعة! #speaker:المعلمة #portrait:teacher_happy

الآن دعنا نكوّن الكلمة معاً. هل أنت مستعد؟

+ [نعم! ما هي الكلمة؟]
    -> reveal_word
+ [كيف نكوّن الكلمة؟]
    -> explain_formation

=== explain_formation ===
سنضع الحروف بالترتيب الصحيح. #speaker:المعلمة #portrait:teacher_thinking

ك + ت + ا + ب = ؟

+ [كتاب!]
    -> correct_answer
+ [لست متأكداً...]
    -> give_hint

=== give_hint ===
فكر... شيء نقرأ منه، له صفحات، مليء بالكلمات... #speaker:المعلمة #portrait:teacher_thinking

+ [كتاب!]
    -> correct_answer

=== reveal_word ===
ممتاز! الكلمة هي... #speaker:المعلمة #portrait:teacher_surprised

+ [أخبريني!]
    -> correct_answer

=== correct_answer ===
كتاب! #speaker:المعلمة #portrait:teacher_happy

أحسنت! لقد تعلمت كلمة جديدة وجمعت كل الحروف!

~ complete_book_mission()

هذا كتابك الخاص! احتفظ به واقرأ منه دائماً.

+ [شكراً معلمتي!]
    -> mission_complete_celebration

=== mission_complete_celebration ===
أنا فخورة بك جداً! #speaker:المعلمة #portrait:teacher_happy

لقد أكملت مهمتك الأولى بنجاح. أتطلع لمغامرات أخرى معك!

+ [أنا متحمس للمزيد!]
    -> teacher_goodbye

=== teacher_mission_completed ===
أهلاً بطلي الصغير! #speaker:المعلمة #portrait:teacher_happy

أرى أنك ما زلت تحمل كتابك. هذا رائع! اقرأ منه كل يوم.

+ [سأفعل ذلك!]
    -> teacher_goodbye
+ [هل لديك مهام جديدة؟]
    -> future_missions

=== future_missions ===
في المستقبل، نعم! #speaker:المعلمة #portrait:teacher_thinking

لكن الآن، استمتع بكتابك الجديد وتدرّب على القراءة.

+ [حسناً!]
    -> teacher_goodbye

=== teacher_goodbye ===
مع السلامة! أراك قريباً. #speaker:المعلمة #portrait:teacher_neutral

-> END
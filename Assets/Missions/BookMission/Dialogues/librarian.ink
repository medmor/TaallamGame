INCLUDE globals.ink

// Librarian - Adult who gives the letter ب (Ba)
// Challenge: Organizing books or answering questions about reading

-> librarian_start

=== librarian_start ===
{
    - book_mission_completed:
        -> librarian_after_mission
    - talked_to_librarian and has_letter_b:
        -> librarian_already_helped
    - book_mission_started:
        -> librarian_help_with_mission
    - else:
        -> librarian_casual_visit
}

=== librarian_casual_visit ===
أهلاً وسهلاً بك في المكتبة! #speaker:أمينة المكتبة #portrait:librarian_happy #layout:left #audio:librarian

أنا أمينة المكتبة. هل تبحث عن كتاب معين؟

+ [أحب استكشاف الكتب]
    -> explore_library
+ [هل يمكنك اقتراح كتاب؟]
    -> suggest_book
+ [أبحث عن شيء خاص]
    ما هو هذا الشيء الخاص؟ ربما أستطيع المساعدة. #speaker:أمينة المكتبة #portrait:librarian_thinking
    -> librarian_goodbye_casual
+ [أستطلع المكان فقط]
    -> just_browsing

=== explore_library ===
ممتاز! المكتبة مليئة بالكنوز. #speaker:أمينة المكتبة #portrait:librarian_happy

لدينا كتب قصص، وعلوم، وتاريخ، وشعر!

+ [هذا رائع!]
    -> librarian_goodbye_casual

=== suggest_book ===
لديّ كتب جميلة للأطفال! #speaker:أمينة المكتبة #portrait:librarian_happy

ما رأيك في قصص الحيوانات أو المغامرات؟

+ [أحب قصص المغامرات]
    -> adventure_books
+ [قصص الحيوانات أفضل]
    -> animal_books

=== adventure_books ===
إذن ستحب كتاب "رحلة سندباد"! #speaker:أمينة المكتبة #portrait:librarian_thinking
-> librarian_goodbye_casual

=== animal_books ===
لدي كتاب رائع عن "أصدقاء الغابة"! #speaker:أمينة المكتبة #portrait:librarian_thinking
-> librarian_goodbye_casual

=== just_browsing ===
تفضل! استكشف بحرية. #speaker:أمينة المكتبة #portrait:librarian_neutral

إذا احتجت مساعدة، أنا هنا.

+ [شكراً]
    -> librarian_goodbye_casual

=== librarian_help_with_mission ===
أهلاً مرة أخرى! #speaker:أمينة المكتبة #portrait:librarian_neutral

سمعت من المعلمة أنك تعمل على مهمة خاصة. أليس كذلك؟

+ [نعم! أجمع حروفاً]
    -> confirm_mission
+ [كيف عرفت؟]
    -> explain_knowledge
+ [هل يمكنك المساعدة؟]
    -> offer_help

=== explain_knowledge ===
المعلمة أخبرتني! #speaker:أمينة المكتبة #portrait:librarian_thinking

قالت أن طالباً ذكياً يجمع حروفاً لتكوين كلمة مهمة.

+ [نعم، هذا أنا!]
    -> confirm_mission

=== confirm_mission ===
رائع! أنا أحب هذه المهام التعليمية. #speaker:أمينة المكتبة #portrait:librarian_happy

لدي حرف خاص يمكنني إعطاؤك إياه!

+ [أي حرف؟]
    -> reveal_letter
+ [ما الذي أحتاج فعله؟]
    -> explain_task
+ [هذا رائع!]
    -> offer_help

=== reveal_letter ===
حرف الـ"ب"! #speaker:أمينة المكتبة #portrait:librarian_surprised

إنه حرف مهم جداً، أول حرف في كلمة "بيت" و "باب"!

+ [كيف أحصل عليه؟]
    -> explain_task
+ [رائع! ماذا أفعل؟]
    -> explain_task

=== offer_help ===
بالطبع يمكنني المساعدة! #speaker:أمينة المكتبة #portrait:librarian_happy

لكن أولاً، هل يمكنك مساعدتي في المكتبة؟

+ [نعم! ماذا تريدين؟]
    -> explain_task
+ [أنا مشغول...]
    -> librarian_understanding

=== librarian_understanding ===
أفهم أن لديك مهام كثيرة. #speaker:أمينة المكتبة #portrait:librarian_sad

لكن مساعدتي بسيطة جداً ولن تأخذ وقتاً طويلاً!

+ [حسناً، سأساعدك]
    -> explain_task
+ [آسف، لا أستطيع الآن]
    -> librarian_goodbye_disappointed

=== explain_task ===
ممتاز! المهمة بسيطة جداً. #speaker:أمينة المكتبة #portrait:librarian_thinking

أريدك أن تساعدني في ترتيب هذه الكتب. ضع كتب القصص على الرف الأزرق، وكتب العلوم على الرف الأحمر.

*تُظهر لك أمينة المكتبة مجموعة من الكتب المبعثرة*

+ [هذا سهل! سأبدأ]
    -> start_organizing
+ [كيف أعرف أي كتاب للعلوم؟]
    -> explain_categories

=== explain_categories ===
نظرة جيدة! #speaker:أمينة المكتبة #portrait:librarian_thinking

كتب العلوم عليها صور نجوم أو حيوانات أو نباتات. كتب القصص عليها صور أطفال أو أبطال خياليين.

+ [فهمت! سأرتبها الآن]
    -> start_organizing

=== start_organizing ===
*تبدأ في فحص الكتب وترتيبها*

رائع! هذا كتاب عن النجوم - إذن هو للعلوم! #speaker:أمينة المكتبة #portrait:librarian_thinking

+ [وهذا كتاب عن أميرة - للقصص!]
    -> continue_organizing
+ [أحتاج مساعدة في هذا الكتاب]
    -> ask_for_help

=== ask_for_help ===
أين المشكلة؟ #speaker:أمينة المكتبة #portrait:librarian_thinking

*تنظر أمينة المكتبة إلى الكتاب معك*

آه، هذا كتاب عن تاريخ البلدان. ضعه مع كتب العلوم!

+ [شكراً! الآن أفهم]
    -> continue_organizing

=== continue_organizing ===
*تواصل ترتيب الكتب بعناية*

ممتاز! أنت منظم جداً! #speaker:أمينة المكتبة #portrait:librarian_happy

+ [انتهيت! كيف يبدو؟]
    -> check_work
+ [آخر كتاب!]
    -> finish_task

=== check_work ===
دعني أتفحص عملك... #speaker:أمينة المكتبة #portrait:librarian_thinking

ممتاز! كل شيء في مكانه الصحيح! أحسنت جداً!

+ [شكراً!]
    -> give_reward

=== finish_task ===
رائع! انتهيت من كل شيء! #speaker:أمينة المكتبة #portrait:librarian_happy

المكتبة تبدو منظمة ونظيفة الآن. شكراً جزيلاً!

+ [كان من دواعي سروري]
    -> give_reward

=== give_reward ===
كما وعدتك، إليك حرف الـ"ب"! #speaker:أمينة المكتبة #portrait:librarian_happy

أنت تستحقه لأنك ساعدت بصدق وعمل جاد!

~ collect_letter("b")
~ talked_to_librarian = true

+ [شكراً جزيلاً!]
    -> after_helping

=== after_helping ===
عفواً! أحب الطلاب المجتهدين مثلك. #speaker:أمينة المكتبة #portrait:librarian_happy

الآن عليك إيجاد باقي الحروف لإكمال مهمتك!

+ [أين أجد الآخرين؟]
    -> suggest_others
+ [شكراً للمساعدة]
    -> librarian_goodbye_happy

=== suggest_others ===
تحدث مع الطلاب الآخرين! #speaker:أمينة المكتبة #portrait:librarian_thinking

ليلى وفاطمة وأحمد كلهم لطفاء ومفيدون!

+ [شكراً للنصيحة!]
    -> librarian_goodbye_happy

=== librarian_already_helped ===
أهلاً بك مرة أخرى! #speaker:أمينة المكتبة #portrait:librarian_happy

كيف تسير مهمة جمع الحروف؟ أتمنى أن يكون حرف الـ"ب" مفيداً!

+ [نعم، شكراً لك!]
    -> progress_discussion
+ [ما زلت أعمل على المهمة]
    -> encourage_student

=== progress_discussion ===
ممتاز! كم حرفاً جمعت حتى الآن؟ #speaker:أمينة المكتبة #portrait:librarian_thinking

{
    - has_letter_k and has_letter_t and has_letter_a:
        رائع! لديك كل الحروف! اذهب إلى المعلمة فوراً!
    - else:
        استمر في العمل الجاد! أنت على الطريق الصحيح.
}

+ [شكراً للاهتمام!]
    -> librarian_goodbye_happy

=== encourage_student ===
الصبر والمثابرة هما سر النجاح! #speaker:أمينة المكتبة #portrait:librarian_thinking

مثل قراءة الكتب، المهام تحتاج وقت وجهد.

+ [سأتذكر نصيحتك]
    -> librarian_goodbye_happy

=== librarian_after_mission ===
مبروك على إكمال المهمة بنجاح! #speaker:أمينة المكتبة #portrait:librarian_happy

أنا فخورة بك لأنك أكملت كل شيء بجد واجتهاد!

+ [شكراً لمساعدتك]
    -> final_words

=== final_words ===
عفواً! أحب رؤية الطلاب ينجحون. #speaker:أمينة المكتبة #portrait:librarian_happy

المكتبة مفتوحة لك دائماً للقراءة والتعلم!

+ [سأأتي كثيراً!]
    -> librarian_goodbye_happy

=== librarian_goodbye_casual ===
استمتع بزيارتك! مع السلامة! #speaker:أمينة المكتبة #portrait:librarian_neutral
-> END

=== librarian_goodbye_disappointed ===
لا بأس، أتمنى لك التوفيق في مهامك. #speaker:أمينة المكتبة #portrait:librarian_sad
-> END

=== librarian_goodbye_happy ===
مع السلامة! أتطلع لرؤيتك مرة أخرى! #speaker:أمينة المكتبة #portrait:librarian_happy
-> END
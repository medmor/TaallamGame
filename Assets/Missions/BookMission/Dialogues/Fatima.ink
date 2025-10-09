INCLUDE globals.ink

// Fatima - Student who gives the letter ت (Ta)
// Challenge: Simple riddle or helping with something

-> fatima_start

=== fatima_start ===
{
    - book_mission_completed:
        -> fatima_after_mission
    - talked_to_fatima and has_letter_t:
        -> fatima_already_helped
    - book_mission_started:
        -> fatima_help_with_mission
    - else:
        -> fatima_casual_chat
}

=== fatima_casual_chat ===
السلام عليكم! أنا فاطمة. #speaker:فاطمة #portrait:fatima_polite #layout:left #audio:student_girl

أحب القراءة كثيراً. هل تحب القراءة أيضاً؟

+ [نعم، أحب القراءة]
    -> bond_over_reading
+ [لا أقرأ كثيراً]
    -> encourage_reading
+ [ما كتابك المفضل؟]
    -> favorite_book

=== bond_over_reading ===
رائع! القراءة تفتح عوالم جديدة. #speaker:فاطمة #portrait:fatima_excited

أحب قصص الأطفال والحكايات الشعبية.

+ [أنا أيضاً!]
    -> shared_interest
+ [أخبريني عن قصة مفضلة]
    -> story_time

=== encourage_reading ===
لا بأس! الكتب مثل الأصدقاء. #speaker:فاطمة #portrait:fatima_encouraging

عندما تجد الكتاب المناسب، ستحب القراءة!

+ [ربما سأجرب]
    -> fatima_goodbye_casual

=== fatima_help_with_mission ===
أهلاً وسهلاً! أسمع أنك تساعد المعلمة؟ #speaker:فاطمة #portrait:fatima_curious

أنا أعرف شيئاً قد يساعدك! لكن أحتاج مساعدتك أولاً.

+ [كيف يمكنني مساعدتك؟]
    -> fatima_problem
+ [ما الشيء الذي تعرفينه؟]
    -> reveal_letter_hint
+ [أنا مستعجل]
    -> fatima_understanding

=== reveal_letter_hint ===
لدي حرف خاص جداً! #speaker:فاطمة #portrait:fatima_mysterious

لكن أولاً، ساعديني في حل هذا اللغز!

+ [ما اللغز؟]
    -> fatima_problem
+ [آسف، أنا مشغول]
    -> fatima_understanding
    -> fatima_problem

=== fatima_understanding ===
أفهم، المهام مهمة. #speaker:فاطمة #portrait:fatima_patient

لكن مساعدتي بسيطة جداً وسريعة! لن تأخذ وقتاً طويلاً.

+ [حسناً، كيف أساعدك؟]
    -> fatima_problem
+ [آسف، لا أستطيع الآن]
    -> fatima_goodbye_sad

=== fatima_problem ===
أحاول أن أتذكر شيئاً! #speaker:فاطمة #portrait:fatima_thinking

ساعدني في حل هذا اللغز البسيط:

"أنا موجود في البيت، أُستخدم للنوم، لي أربع أرجل وسطح مريح. ما أنا؟"

+ [سرير!]
    -> correct_riddle
+ [كرسي؟]
    -> wrong_riddle_chair
+ [طاولة؟]
    -> wrong_riddle_table
+ [لا أعرف...]
    -> give_riddle_hint

=== wrong_riddle_chair ===
قريب! لكن الكرسي للجلوس، ليس للنوم. #speaker:فاطمة #portrait:fatima_patient

فكر... أين ننام في البيت؟

+ [سرير!]
    -> correct_riddle
+ [أعطني تلميحاً آخر]
    -> give_riddle_hint

=== wrong_riddle_table ===
لا، الطاولة للأكل أو الكتابة. #speaker:فاطمة #portrait:fatima_helpful

أحتاج شيئاً للنوم عليه...

+ [سرير!]
    -> correct_riddle
+ [سجادة؟]
    -> close_but_not_quite

=== close_but_not_quite ===
قريب جداً! لكن السجادة ليس لها أرجل. #speaker:فاطمة #portrait:fatima_encouraging

شيء بأربع أرجل، مريح، للنوم...

+ [سرير!]
    -> correct_riddle

=== give_riddle_hint ===
لا بأس! إليك تلميح آخر: #speaker:فاطمة #portrait:fatima_helping

نحن ننام عليه كل ليلة، وأمي تضع عليه الملاءات والوسائد.

+ [سرير!]
    -> correct_riddle
+ [ما زلت لا أعرف]
    -> give_answer

=== give_answer ===
الجواب هو: سرير! #speaker:فاطمة #portrait:fatima_explaining

هل تفهم الآن؟ أرجل، مريح، للنوم... سرير!

+ [آه، أفهم الآن!]
    -> understand_riddle

=== understand_riddle ===
رائع! الألغاز تجعل عقلنا أذكى. #speaker:فاطمة #portrait:fatima_proud

شكراً لمساعدتي! إليك حرف الـ"ت"!

~ collect_letter("t")
~ talked_to_fatima = true

+ [شكراً فاطمة!]
    -> after_getting_letter

=== correct_riddle ===
ممتاز! أحسنت جداً! #speaker:فاطمة #portrait:fatima_very_happy

السرير هو الجواب الصحيح! أنت ذكي.

شكراً لمساعدتي في تذكر هذا اللغز. إليك حرف الـ"ت" كمكافأة!

~ collect_letter("t")
~ talked_to_fatima = true

+ [شكراً جزيلاً!]
    -> after_getting_letter

=== after_getting_letter ===
عفواً! أحب الألغاز والحروف. #speaker:فاطمة #portrait:fatima_warm

أتمنى أن تجد باقي الحروف بسرعة!

+ [أين أجد الآخرين؟]
    -> suggest_others
+ [شكراً للمساعدة]
    -> fatima_goodbye_happy

=== suggest_others ===
جرب التحدث مع أحمد وليلى وأمينة المكتبة! #speaker:فاطمة #portrait:fatima_helpful

كل واحد منهم لديه حرف مختلف.

+ [شكراً للنصيحة!]
    -> fatima_goodbye_happy

=== fatima_already_helped ===
أهلاً مرة أخرى! #speaker:فاطمة #portrait:fatima_friendly

كيف تسير مهمة جمع الحروف؟ هل ساعدك حرف الـ"ت"؟

+ [نعم، شكراً لك!]
    -> progress_check
+ [ما زلت أجمع الباقي]
    -> encourage_more

=== progress_check ===
رائع! كم حرفاً جمعت حتى الآن؟ #speaker:فاطمة #portrait:fatima_curious

{
    - has_letter_k and has_letter_a and has_letter_b:
        ممتاز! لديك كل الحروف! اذهب للمعلمة بسرعة!
    - else:
        استمر! أنت قريب من النهاية.
}

+ [شكراً للتشجيع!]
    -> fatima_goodbye_happy

=== encourage_more ===
لا تستسلم! أنت قادر على ذلك. #speaker:فاطمة #portrait:fatima_encouraging

الألغاز تعلمنا الصبر والتفكير!

+ [سأتذكر ذلك]
    -> fatima_goodbye_happy

=== fatima_after_mission ===
مبروك على إكمال المهمة! #speaker:فاطمة #portrait:fatima_congratulating

أنا فخورة لأنني ساعدتك بحرف الـ"ت" واللغز!

+ [أشكرك على المساعدة]
    -> final_appreciation
    -> final_appreciation

=== final_appreciation ===
عفواً! أحب مساعدة الأصدقاء في التعلم. #speaker:فاطمة #portrait:fatima_content

إذا أردت ألغازاً أكثر، أنا هنا!

+ [بالتأكيد!]
    -> fatima_goodbye_happy

=== shared_interest ===
أصدقاء القراءة الأفضل! #speaker:فاطمة #portrait:fatima_bonding
-> fatima_goodbye_casual

=== story_time ===
أحب قصة "الأرنب الذكي"! #speaker:فاطمة #portrait:fatima_storytelling
-> fatima_goodbye_casual

=== favorite_book ===
أحب كتب الألغاز والحكايات! #speaker:فاطمة #portrait:fatima_books
-> fatima_goodbye_casual

=== fatima_goodbye_casual ===
استمتع بيومك! مع السلامة! #speaker:فاطمة #portrait:fatima_wave
-> END

=== fatima_goodbye_sad ===
لا بأس، أتمنى لك التوفيق. مع السلامة. #speaker:فاطمة #portrait:fatima_disappointed
-> END

=== fatima_goodbye_happy ===
مع السلامة! حظ موفق! #speaker:فاطمة #portrait:fatima_cheerful
-> END
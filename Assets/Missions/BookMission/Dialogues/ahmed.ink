INCLUDE globals.ink

// Ahmed - Student who gives the letter ا (Alif)
// Challenge: Physical activity or helping with sports equipment

-> ahmed_start

=== ahmed_start ===
{
    - book_mission_completed:
        -> ahmed_after_mission
    - talked_to_ahmed and has_letter_a:
        -> ahmed_already_helped
    - book_mission_started:
        -> ahmed_help_with_mission
    - else:
        -> ahmed_casual_chat
}

=== ahmed_casual_chat ===
أهلاً! أنا أحمد! #speaker:أحمد #portrait:ahmed_happy #layout:right #audio:student_boy

أحب اللعب والجري! هل تريد أن نلعب معاً؟

+ [نعم، أحب اللعب!]
    -> play_sports
+ [ما ألعابك المفضلة؟]
    -> favorite_games
+ [أنا لست رياضياً]
    -> encourage_sports
+ [هل تعرف شيئاً عن الحروف؟]
    حروف؟ مثل التي نكتبها؟ لا أعرف... المعلمة أفضل! #speaker:أحمد #portrait:ahmed_thinking
    -> ahmed_goodbye_casual

=== play_sports ===
رائع! نلعب كرة القدم؟ #speaker:أحمد #portrait:ahmed_happy

أحسنت! أنت لاعب جيد!

+ [شكراً! كان ممتعاً]
    -> ahmed_goodbye_casual

=== favorite_games ===
أحب كرة القدم والجري والقفز! #speaker:أحمد #portrait:ahmed_happy

الرياضة تجعلني قوياً وسعيداً!

+ [هذا رائع!]
    -> ahmed_goodbye_casual

=== encourage_sports ===
لا مشكلة! الرياضة للجميع. #speaker:أحمد #portrait:ahmed_thinking

يمكننا البدء بأشياء بسيطة مثل المشي أو الركض البطيء.

+ [ربما أجرب ذلك]
    -> ahmed_goodbye_casual

=== ahmed_help_with_mission ===
مرحباً صديقي! سمعت أنك تساعد المعلمة! #speaker:أحمد #portrait:ahmed_thinking

أنا أيضاً أريد المساعدة! لدي شيء قد تحتاجه.

+ [ما هو؟]
    -> reveal_letter_hint
+ [كيف يمكنك المساعدة؟]
    -> ahmed_offer_help
+ [أنا مشغول الآن]
    -> ahmed_disappointed

=== reveal_letter_hint ===
لدي حرف مهم جداً! #speaker:أحمد #portrait:ahmed_happy

لكن أولاً، هل يمكنك مساعدتي؟ أحتاج مساعدة في الملعب!

+ [بالطبع! ماذا تحتاج؟]
    -> ahmed_challenge
+ [ما الحرف؟]
    -> tease_about_letter

=== tease_about_letter ===
إنه حرف جميل جداً! #speaker:أحمد #portrait:ahmed_thinking

لكن لن أخبرك إلا بعد أن تساعدني!

+ [حسناً، كيف أساعدك؟]
    -> ahmed_challenge
+ [لا أستطيع المساعدة الآن]
    -> ahmed_disappointed

=== ahmed_offer_help ===
أنا قوي ونشيط! #speaker:أحمد #portrait:ahmed_happy

يمكنني حمل أشياء ثقيلة أو الجري بسرعة. وأعرف حرفاً مفيداً!

+ [أخبرني عن الحرف]
    -> reveal_letter_hint
+ [لا أحتاج مساعدة رياضية]
    -> ahmed_other_help

=== ahmed_other_help ===
لا بأس! لكن ساعدني أنت أولاً. #speaker:أحمد #portrait:ahmed_thinking

المساعدة بسيطة، لا تحتاج قوة كبيرة!

+ [حسناً، ما هي؟]
    -> ahmed_challenge
+ [آسف، لا أستطيع]
    -> ahmed_disappointed

=== ahmed_disappointed ===
أوه... حسناً. #speaker:أحمد #portrait:ahmed_sad

إذا غيّرت رأيك، أنا في الملعب! المساعدة سريعة ومفيدة!

+ [انتظر، سأساعدك]
    -> ahmed_challenge
+ [آسف، مع السلامة]
    -> ahmed_goodbye_sad

=== ahmed_challenge ===
ممتاز! المساعدة بسيطة جداً. #speaker:أحمد #portrait:ahmed_problem

أريدك أن تعدّ الكرات الخضراء في هذا الصندوق. كم عددها؟

+ [أرى... ست كرات خضراء]
    -> correct_count
+ [أعتقد... خمس كرات خضراء]
    -> wrong_count_low
+ [سبع كرات خضراء]
    -> wrong_count_high

=== wrong_count_high ===
أوه، لا. عدّ مرة أخرى بعناية أكثر! #speaker:أحمد #portrait:ahmed_happy

حاول التركيز على الكرات الخضراء فقط.

+ [حسنًا، الرقم الصحيح هو ست كرات]
    -> correct_count

=== wrong_count_low ===
قريب! لكن عدّ مرة أخرى. #speaker:أحمد #portrait:ahmed_neutral

خذ وقتك وعدّ بإصبعك.

+ [حسنًا، إنها ست كرات]
    -> correct_count

=== correct_count ===
أحسنت! الإجابة الصحيحة هي: ست كرات خضراء. #speaker:أحمد #portrait:ahmed_happy

شكراً لمساعدتي في العد!

-> give_reward

// (المسارات القديمة لجمع الكرات أُزيلت واستُبدلت بمهمة العد)

=== give_reward ===
كما وعدتك، إليك الحرف الخاص! #speaker:أحمد #portrait:ahmed_happy

إنه حرف الـ"ا" إنه أول حرف في اسمي - أحمد!

~ collect_letter("a")
~ talked_to_ahmed = true

+ [شكراً أحمد!]
    -> after_getting_letter

=== after_getting_letter ===
عفواً! أحب مساعدة الأصدقاء. #speaker:أحمد #portrait:ahmed_happy

والآن عليك إيجاد باقي الحروف! تحدث مع الآخرين في المدرسة.

+ [من أتحدث معه؟]
    -> suggest_others
+ [شكراً للنصيحة!]
    -> ahmed_goodbye_happy

=== suggest_others ===
جرب ليلى وفاطمة وأمينة المكتبة! #speaker:أحمد #portrait:ahmed_happy

كل واحد منهم لديه حرف مختلف. وهم لطفاء مثلي!

+ [شكراً!]
    -> ahmed_goodbye_happy

=== ahmed_already_helped ===
أهلاً صديقي! #speaker:أحمد #portrait:ahmed_happy

كيف تسير مهمة الحروف؟ هل ساعدك حرف الـ"ا"؟

+ [نعم، كان مفيداً جداً!]
    -> progress_chat
+ [ما زلت أجمع الباقي]
    -> encourage_friend

=== progress_chat ===
رائع! أنا سعيد لأنني ساعدتك! #speaker:أحمد #portrait:ahmed_happy

{
    - has_letter_k and has_letter_t and has_letter_b:
        ممتاز! أراك جمعت كل الحروف! أسرع إلى المعلمة!
    - else:
        استمر في البحث! أنت قادر على ذلك!
}

+ [شكراً للتشجيع!]
    -> ahmed_goodbye_happy

=== encourage_friend ===
لا تستسلم أبداً! #speaker:أحمد #portrait:ahmed_happy

مثل الرياضة، المثابرة هي السر!

+ [سأتذكر ذلك]
    -> ahmed_goodbye_happy

=== ahmed_after_mission ===
مبروك يا بطل! #speaker:أحمد #portrait:ahmed_happy

سمعت أنك أكملت المهمة! أنا فخور بأنني ساعدتك!

+ [شكراً لمساعدتك]
    -> final_celebration

=== final_celebration ===
عفواً! الأصدقاء يساعدون بعضهم! #speaker:أحمد #portrait:ahmed_happy

إذا احتجت مساعدة رياضية، أنا هنا دائماً!

+ [بالتأكيد!]
    -> ahmed_goodbye_happy

=== ahmed_goodbye_casual ===
مع السلامة! استمتع بيومك! #speaker:أحمد #portrait:ahmed_neutral
-> END

=== ahmed_goodbye_sad ===
مع السلامة... أتمنى لك التوفيق. #speaker:أحمد #portrait:ahmed_sad
-> END

=== ahmed_goodbye_happy ===
مع السلامة صديقي! حظ موفق! #speaker:أحمد #portrait:ahmed_happy
-> END
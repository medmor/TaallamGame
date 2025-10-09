INCLUDE globals.ink

// Leila - Student who gives the letter ك (Kaf)
// Challenge: Simple counting or color recognition

-> leila_start

=== leila_start ===
{
    - book_mission_completed:
        -> leila_after_mission
    - talked_to_leila and has_letter_k:
        -> leila_already_helped
    - book_mission_started:
        -> leila_help_with_mission
    - else:
        -> leila_casual_chat
}

=== leila_casual_chat ===
مرحباً! أنا ليلى. #speaker:ليلى #portrait:leila_happy #layout:right #audio:student_girl

نلعب في الساحة، هل تريد الانضمام إلينا؟

+ [أود ذلك!]
    -> play_together
+ [ربما لاحقاً]
    -> leila_goodbye_casual
+ [هل تعرفين شيئاً عن الحروف؟]
    ما الحروف؟ لا أفهم... ربما تقصد المعلمة؟ #speaker:ليلى #portrait:leila_confused
    -> leila_goodbye_casual

=== play_together ===
رائع! نلعب لعبة العد. #speaker:ليلى #portrait:leila_excited

عدّ معي: واحد، اثنان، ثلاثة...

+ [أربعة، خمسة!]
    أحسنت! أنت ذكي. #speaker:ليلى #portrait:leila_happy
    -> leila_goodbye_casual

=== leila_help_with_mission ===
أهلاً! سمعت أنك تجمع حروفاً للمعلمة؟ #speaker:ليلى #portrait:leila_curious

أنا أعرف حرفاً واحداً! لكن أولاً، هل يمكنك مساعدتي؟

+ [بالطبع! كيف؟]
    -> leila_challenge
+ [ما الحرف الذي تعرفينه؟]
    -> reveal_letter_first
+ [أنا مشغول الآن]
    -> leila_disappointed

=== reveal_letter_first ===
إنه حرف الـ"ك"! #speaker:ليلى #portrait:leila_proud

لكن لن أعطيك إياه إلا إذا ساعدتني أولاً!

+ [حسناً، كيف أساعدك؟]
    -> leila_challenge
+ [آسف، لا أستطيع الآن]
    -> leila_disappointed

=== leila_disappointed ===
أوه... حسناً. #speaker:ليلى #portrait:leila_sad

إذا غيّرت رأيك، أنا هنا. أحتاج مساعدتك في شيء بسيط!

+ [انتظري، سأساعدك]
    -> leila_challenge
+ [آسف، وداعاً]
    -> leila_goodbye_sad

=== leila_challenge ===
ممتاز! المساعدة بسيطة جداً. #speaker:ليلى #portrait:leila_explaining

أريدك أن تعدّ الكرات الحمراء في هذه الصندوق. كم عددها؟

*تُظهر لك ليلى صندوقاً به كرات ملونة*

+ [أرى... ثلاث كرات حمراء]
    -> correct_count
+ [أعتقد... خمس كرات حمراء]
    -> wrong_count_high
+ [كرتان حمراوتان]
    -> wrong_count_low

=== wrong_count_high ===
أوه، لا. عدّ مرة أخرى بعناية أكثر! #speaker:ليلى #portrait:leila_encouraging

انظر فقط للكرات الحمراء، ليس الزرقاء أو الخضراء.

+ [حسناً... ثلاث كرات حمراء]
    -> correct_count
+ [أربع كرات حمراء]
    -> wrong_again

=== wrong_count_low ===
قريب! لكن عدّ مرة أخرى. #speaker:ليلى #portrait:leila_patient

خذ وقتك وعدّ بإصبعك.

+ [ثلاث كرات حمراء]
    -> correct_count
+ [أربع كرات حمراء]
    -> wrong_again

=== wrong_again ===
لا بأس، دعني أساعدك. #speaker:ليلى #portrait:leila_helpful

*تشير ليلى إلى كل كرة حمراء*

واحدة... اثنتان... ثلاث! صحيح؟

+ [نعم، ثلاث كرات حمراء!]
    -> correct_count

=== correct_count ===
ممتاز! أحسنت! #speaker:ليلى #portrait:leila_very_happy

شكراً لمساعدتي. إليك حرف الـ"ك" كما وعدتك!

~ collect_letter("k")
~ talked_to_leila = true

+ [شكراً ليلى!]
    -> after_getting_letter

=== after_getting_letter ===
عفواً! آمل أن تجد باقي الحروف بسرعة. #speaker:ليلى #portrait:leila_encouraging

تحدث مع فاطمة وأحمد وأمينة المكتبة!

+ [سأفعل ذلك!]
    -> leila_goodbye_happy

=== leila_already_helped ===
مرحباً مرة أخرى! #speaker:ليلى #portrait:leila_happy

هل وجدت باقي الحروف؟ أتمنى أن تُكمل المهمة قريباً!

+ [ما زلت أبحث]
    لا تستسلم! أنت قادر على ذلك. #speaker:ليلى #portrait:leila_encouraging
    -> leila_goodbye_happy
+ [نعم، وجدتها كلها!]
    رائع! اذهب للمعلمة بسرعة! #speaker:ليلى #portrait:leila_excited
    -> leila_goodbye_happy

=== leila_after_mission ===
مبروك! سمعت أنك أكملت المهمة! #speaker:ليلى #portrait:leila_proud

أنا سعيدة لأنني ساعدتك بحرف الـ"ك"!

+ [شكراً لمساعدتك!]
    -> leila_final_words

=== leila_final_words ===
عفواً! أحب مساعدة الأصدقاء. #speaker:ليلى #portrait:leila_warm

إذا احتجت مساعدة أخرى، أنا هنا دائماً!

+ [أقدر ذلك!]
    -> leila_goodbye_happy

=== leila_goodbye_casual ===
مع السلامة! استمتع بوقتك! #speaker:ليلى #portrait:leila_wave
-> END

=== leila_goodbye_sad ===
مع السلامة... #speaker:ليلى #portrait:leila_disappointed
-> END

=== leila_goodbye_happy ===
مع السلامة! حظ سعيد! #speaker:ليلى #portrait:leila_cheerful
-> END
INCLUDE globals.ink

== start ==
#speaker:عادل البائع #portrait:vendor_neutral #layout:right #audio:animal_crossing_mid
{
    - has_letter_t:
        أهلاً مرة أخرى! أتمنى أن يكون الحرف "ت" مفيداً لك.
        -> END
    - book_mission_started:
        مرحباً! أبيع الأشياء الصغيرة هنا. هل لديك تفاحة لتبادلها بحرف؟
        -> trade_offer
    - else:
        أهلاً بك! أنا عادل، أبيع الفواكه والأشياء المفيدة للأطفال.
        -> casual_browse
}

== trade_offer ==
{
    - has_apple:
        #speaker:عادل البائع #portrait:vendor_happy
        ممتاز! لديك تفاحة جميلة. هل تريد مبادلتها بحرف "ت"؟
        -> confirm_trade
    - else:
        #speaker:عادل البائع #portrait:vendor_neutral
        لا يوجد لديك تفاحة الآن. حاول أن تبحث في السوق أو اسأل الأطفال الآخرين.
        ربما يعطيك أحدهم تفاحة مقابل مساعدة صغيرة!
        -> END
}

== confirm_trade ==
+ [نعم، أبادل التفاحة بالحرف]
    { use_apple():
        ~ collect_letter("t")
        ~ talked_to_vendor = true
        #speaker:عادل البائع #portrait:vendor_happy #audio:animal_crossing_low
        اتفقنا! تفضل الحرف: «ت».
        هذا حرف مهم، استخدمه بحكمة في كلمتك!
        -> END
    - else:
        #speaker:عادل البائع #portrait:vendor_confused
        عذراً، يبدو أن التفاحة اختفت! هل أنت متأكد أن لديك واحدة؟
        -> END
    }
+ [لا، سأحتفظ بالتفاحة]
    #speaker:عادل البائع #portrait:vendor_neutral
    حسناً، كما تشاء. التفاحة لذيذة أيضاً!
    عد إذا غيرت رأيك.
    -> END

== casual_browse ==
+ [ماذا تبيع؟]
    #speaker:عادل البائع #portrait:vendor_happy
    أبيع الفواكه الطازجة والحروف التعليمية!
    إذا أحضرت لي تفاحة، يمكنني أن أعطيك حرفاً مفيداً.
    -> END
+ [أبحث عن الحروف]
    #speaker:عادل البائع #portrait:vendor_helpful
    أه! تبحث عن الحروف؟ اذهب إلى المعلم سامي أولاً.
    لديه مهمة خاصة تتعلق بجمع الحروف!
    -> END
+ [شكراً، سأعود لاحقاً]
    #speaker:عادل البائع #portrait:vendor_neutral
    على الرحب والسعة! أراك قريباً.
    -> END
INCLUDE "globals.ink"

= start =
#speaker:عادل البائع #portrait:vendor_neutral #layout:right #audio:animal_crossing_mid
مرحبًا! أبيع الأشياء الصغيرة هنا. هل لديك تفاحة لتبادلها بحرف؟
{ has_apple:
    #speaker:عادل البائع #portrait:vendor_happy
    لديك تفاحة! هل تريد مبادلتها بحرف "ت"؟
    + [نعم]
        ~ has_apple = false
        ~ has_t = true
        #speaker:عادل البائع #portrait:vendor_happy #audio:animal_crossing_low
        اتفقنا. تفضل الحرف: «ت».
        -> END
    + [لا]
        #speaker:عادل البائع
        حسنًا، احتفظ بتفاحتك.
        -> END
- else:
    #speaker:عادل البائع #portrait:vendor_neutral
    لا يوجد لديك تفاحة الآن. حاول أن تبحث في السوق أو اسأل الأطفال.
    -> END
}
INCLUDE globals.ink

== start ==
#speaker:بائعة الفواكه #portrait:fruit_seller_neutral #layout:left #audio:animal_crossing_mid
{
    - has_apple:
        أهلاً مرة أخرى! أرى أن لديك تفاحة. استخدمها بحكمة!
        -> END
    - has_letter_b:
        أهلاً! أتمنى أن يكون الحرف "ب" مفيداً لك في مهمتك.
        -> END
    - book_mission_started:
        أهلاً عزيزي! هل تحتاج تفاحة لمهمة الحروف؟
        -> offer_apple
    - else:
        أهلاً بك! أبيع الفواكه الطازجة والطيبة.
        -> casual_shopping
}

== offer_apple ==
+ [نعم، أحتاج تفاحة]
    #speaker:بائعة الفواكه #portrait:fruit_seller_happy
    ممتاز! سأعطيك تفاحة مجاناً لأنك تتعلم.
    لكن أولاً، أجب على سؤال بسيط: ما هو لون التفاح؟
    -> apple_quiz
+ [لا شكراً]
    #speaker:بائعة الفواكه #portrait:fruit_seller_neutral
    حسناً، عد إذا احتجت أي شيء!
    -> END

== apple_quiz ==
+ [أحمر]
    ~ get_apple()
    #speaker:بائعة الفواكه #portrait:fruit_seller_happy #audio:animal_crossing_low
    صحيح! تفضل هذه التفاحة الحمراء الجميلة.
    يمكنك استخدامها للمقايضة مع عادل البائع للحصول على حرف "ت".
    وكمكافأة إضافية، خذ أيضاً الحرف "ب"!
    ~ collect_letter("b")
    -> END
+ [أخضر]
    #speaker:بائعة الفواكه #portrait:fruit_seller_helpful
    التفاح يمكن أن يكون أخضر أيضاً، لكن هذه التفاحة حمراء.
    تفضل على أي حال! وخذ أيضاً الحرف "ب".
    ~ get_apple()
    ~ collect_letter("b")
    -> END
+ [أزرق]
    #speaker:بائعة الفواكه #portrait:fruit_seller_laugh
    هاها! التفاح الأزرق؟ هذا مضحك!
    التفاح عادة أحمر أو أخضر. تفضل التفاحة والحرف "ب" على أي حال!
    ~ get_apple()
    ~ collect_letter("b")
    -> END

== casual_shopping ==
+ [ماذا تبيعين؟]
    #speaker:بائعة الفواكه #portrait:fruit_seller_happy
    أبيع أفضل الفواكه في المدينة! تفاح، وبرتقال، وموز طازج.
    وأحياناً أعطي الحروف للأطفال الذين يتعلمون!
    -> END
+ [أبحث عن الحروف]
    #speaker:بائعة الفواكه #portrait:fruit_seller_helpful
    أه! تبحث عن الحروف؟ تحدث مع المعلم سامي أولاً.
    إذا كان لديك مهمة حروف، يمكنني مساعدتك!
    -> END
+ [شكراً، سأتجول]
    #speaker:بائعة الفواكه #portrait:fruit_seller_neutral
    حسناً، استمتع بوقتك في السوق!
    -> END
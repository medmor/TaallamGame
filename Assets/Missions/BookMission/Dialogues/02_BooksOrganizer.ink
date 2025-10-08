INCLUDE globals.ink
-> start
== start ==
#speaker:منظمة الكتب #portrait:librarian_helper_neutral #layout:left #audio:animal_crossing_mid
{
    - has_letter_b:
        أهلاً! أتمنى أن يكون الحرف "ب" مفيداً لك في مهمتك.
        -> END
    - book_mission_started:
        أهلاً عزيزي! أرى كتباً مبعثرة هنا. هل تساعدني في ترتيبها؟
        -> book_organizing
    - else:
        أهلاً بك! أساعد في ترتيب وتنظيم كتب المكتبة.
        -> casual_talk
}

== book_organizing ==
+ [نعم، سأساعدك في الترتيب]
    #speaker:منظمة الكتب #portrait:librarian_helper_happy
    ممتاز! هذه الكتب تحتاج ترتيب حسب الأبجدية.
    أي كتاب يبدأ بحرف "ب" يجب أن يوضع أولاً؟
    -> book_quiz
+ [لا أستطيع الآن]
    #speaker:منظمة الكتب #portrait:librarian_helper_neutral
    حسناً، لا مشكلة. سأرتبها لاحقاً!
    -> END

== book_quiz ==
+ [كتاب "بطة صغيرة"]
    #speaker:منظمة الكتب #portrait:librarian_helper_happy #audio:animal_crossing_low
    صحيح! "بطة صغيرة" يبدأ بحرف "ب".
    شكراً لمساعدتك! وجدت هذا الحرف بين الكتب، خذه: "ب"!
    ~ collect_letter("b")
    -> END
+ [كتاب "أسد الغابة"]
    #speaker:منظمة الكتب #portrait:librarian_helper_helpful
    لا، "أسد الغابة" يبدأ بحرف "أ"، لكن محاولة جيدة!
    على أي حال، خذ هذا الحرف "ب" الذي وجدته بين الكتب.
    ~ collect_letter("b")
    -> END
+ [كتاب "تمساح النيل"]
    #speaker:منظمة الكتب #portrait:librarian_helper_helpful
    لا، "تمساح النيل" يبدأ بحرف "ت". لكن أشكرك على المساعدة!
    خذ هذا الحرف "ب" الذي وجدته مخبأً بين الكتب.
    ~ collect_letter("b")
    -> END

== casual_talk ==
+ [كيف تنظمين الكتب؟]
    #speaker:منظمة الكتب #portrait:librarian_helper_happy
    أرتب الكتب حسب الأبجدية والموضوع!
    الكتب التي تبدأ بحرف "أ" تأتي قبل التي تبدأ بحرف "ب" وهكذا.
    -> END
+ [هل تحتاجين مساعدة؟]
    #speaker:منظمة الكتب #portrait:librarian_helper_excited
    نعم! إذا بدأت لعبة الحروف مع أمينة المكتبة، تعال وساعدني!
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
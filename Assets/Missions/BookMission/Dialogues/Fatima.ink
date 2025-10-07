INCLUDE globals.ink
-> start
== start ==
#speaker:فاطمة #portrait:fatima_neutral #layout:left #audio:animal_crossing_high
{
    - has_letter_a:
        أهلاً! أتمنى أن تستفيد من الحرف "ا" الذي أعطيتك إياه.
        -> END
    - book_mission_started:
        أهلاً! أقرأ قصة جميلة هنا. هل تلعب لعبة الحروف مع أمينة المكتبة؟
        -> help_with_mission
    - else:
        أهلاً! أنا فاطمة، أحب قراءة القصص في المكتبة!
        -> casual_reading
}

== help_with_mission ==
+ [نعم، أجمع الحروف]
    #speaker:فاطمة #portrait:fatima_happy
    رائع! في قصتي هذه، يحتاج الأمير لعد الكنوز. إذا ساعدتني بالعد إلى عشرة، سأعطيك حرف "ا"!
    -> counting_game
+ [لا، أقرأ شيئاً آخر]
    #speaker:فاطمة #portrait:fatima_neutral
    حسناً! استمتع بالقراءة.
    -> END

== counting_game ==
#speaker:فاطمة #portrait:fatima_excited
هيا نعد كنوز الأمير معاً! قل الأرقام من واحد إلى عشرة!
+ [واحد، اثنان، ثلاثة... عشرة!]
    ~ collect_letter("a")
    #speaker:فاطمة #portrait:fatima_happy #audio:animal_crossing_low
    ممتاز! عددت كنوز الأمير بشكل صحيح. تفضل الحرف "ا"!
    أنت ذكي جداً! هذا الحرف موجود في كل القصص الجميلة.
    -> END
+ [لا أتذكر الأرقام]
    #speaker:فاطمة #portrait:fatima_helpful
    لا مشكلة! اسأل أمينة المكتبة، ستساعدك في تعلم الأرقام.
    عد إليّ عندما تتذكرها!
    -> END

== casual_reading ==
+ [ما القصة التي تقرأين؟]
    #speaker:فاطمة #portrait:fatima_excited
    أقرأ قصة عن أمير يجمع الحروف لينقذ مملكته!
    إنها مشابهة للعبة الحروف في المكتبة.
    -> END
+ [مع السلامة]
    #speaker:فاطمة #portrait:fatima_neutral
    مع السلامة! استمتع بالقراءة في المكتبة!
    -> END
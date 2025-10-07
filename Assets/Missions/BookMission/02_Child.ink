INCLUDE globals.ink

== start ==
#speaker:طفل صغير #portrait:child_neutral #layout:left #audio:animal_crossing_high
{
    - has_letter_a:
        أهلاً! أتمنى أن تستفيد من الحرف "ا" الذي أعطيتك إياه.
        -> END
    - book_mission_started:
        أهلاً! هل تلعب لعبة الحروف مع المعلم سامي؟
        -> help_with_mission
    - else:
        أهلاً! أنا أحب اللعب في ساحة المدرسة!
        -> casual_play
}

== help_with_mission ==
+ [نعم، أجمع الحروف]
    #speaker:طفل صغير #portrait:child_happy
    رائع! أعرف لعبة ممتعة. إذا عددت إلى عشرة، سأعطيك حرف "ا"!
    -> counting_game
+ [لا، ألعب شيئاً آخر]
    #speaker:طفل صغير #portrait:child_neutral
    حسناً! استمتع باللعب.
    -> END

== counting_game ==
#speaker:طفل صغير #portrait:child_excited
هيا نعد معاً! قل الأرقام من واحد إلى عشرة!
+ [واحد، اثنان، ثلاثة... عشرة!]
    ~ collect_letter("a")
    #speaker:طفل صغير #portrait:child_happy #audio:animal_crossing_low
    ممتاز! عددت بشكل صحيح. تفضل الحرف "ا"!
    أنت ذكي جداً! هذا الحرف سيساعدك في كتابة كلمات كثيرة.
    -> END
+ [لا أتذكر الأرقام]
    #speaker:طفل صغير #portrait:child_helpful
    لا مشكلة! اذهب إلى المعلم وسيساعدك في تعلم الأرقام.
    عد إليّ عندما تتذكرها!
    -> END

== casual_play ==
+ [هل تريد أن نلعب؟]
    #speaker:طفل صغير #portrait:child_excited
    نعم! لكن أولاً، هل تعرف المعلم سامي؟
    لديه لعبة رائعة عن الحروف!
    -> END
+ [مع السلامة]
    #speaker:طفل صغير #portrait:child_neutral
    مع السلامة! استمتع في المدرسة!
    -> END
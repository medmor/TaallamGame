INCLUDE globals.ink

== start ==
#speaker:أحمد الطالب #portrait:student_neutral #layout:right #audio:animal_crossing_mid
{
    - has_letter_t:
        أهلاً مرة أخرى! أتمنى أن يكون الحرف "ت" مفيداً لك.
        -> END
    - book_mission_started:
        مرحباً! أقرأ هنا في قسم القراءة. هل يمكنك مساعدتي في إيجاد كتابي المفقود؟
        -> help_request
    - else:
        أهلاً بك! أنا أحمد، أحب القراءة في هذا القسم من المكتبة.
        -> casual_chat
}

== help_request ==
#speaker:أحمد الطالب #portrait:student_worried
كتابي عن القصص اختفى من هنا! هل يمكنك البحث عنه في الرفوف القريبة؟
+ [نعم، سأساعدك في البحث]
    #speaker:أحمد الطالب #portrait:student_happy
    شكراً! ابحث في الرف الثالث... وجدته! شكراً جزيلاً!
    -> give_letter
+ [لا أستطيع الآن]
    #speaker:أحمد الطالب #portrait:student_sad
    حسناً، لا مشكلة. سأبحث عنه بنفسي.
    -> END

== give_letter ==
~ collect_letter("t")
~ talked_to_vendor = true
#speaker:أحمد الطالب #portrait:student_happy #audio:animal_crossing_low
كشكر لك على مساعدتي، خذ هذا الحرف: «ت».
وجدته في كتابي! استخدمه في مهمة الحروف.
-> END

== casual_chat ==
+ [ماذا تقرأ؟]
    #speaker:أحمد الطالب #portrait:student_happy
    أقرأ قصصاً عن المغامرات والحروف!
    المكتبة مليئة بالكتب الرائعة.
    -> END
+ [أبحث عن الحروف]
    #speaker:أحمد الطالب #portrait:student_helpful
    أه! تبحث عن الحروف؟ اذهب إلى أمينة المكتبة أولاً.
    لديها مهمة خاصة تتعلق بجمع الحروف!
    -> END
+ [شكراً، سأعود لاحقاً]
    #speaker:عادل البائع #portrait:vendor_neutral
    على الرحب والسعة! أراك قريباً.
    -> END
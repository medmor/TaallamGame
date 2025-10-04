INCLUDE globals.ink

== start ==
#speaker:المعلم سامي #portrait:teacher_neutral #layout:left #audio:animal_crossing_mid
مرحبًا! اليوم لدينا لعبة تعليمية. هل تستطيع مساعدتي في جمع حروف كلمة "كتاب"؟
+ [نعم، أقبل المهمة]
    ~ mission_accepted = true
    #speaker:المعلم سامي #portrait:teacher_happy
    رائع! تعال إلى ساحة المدرسة وابحث عن ليلى والبائع عادل وأمينة. كل واحد يعطيك حرفًا بطريقته.
    -> END
+ [لا الآن]
    فهمت. عد إليّ عندما تكون مستعدًا.
    -> END

== check_progress ==
{ is_mission_complete():
    #speaker:المعلم سامي #portrait:teacher_proud #audio:animal_crossing_mid
    تهانينا! لقد جمعت كل الحروف.
    ~ mission_accepted = false
    -> END
- else:
    #speaker:المعلم سامي #portrait:teacher_neutral
    لم تجمع كل الحروف بعد. راجع ساحة المدرسة وتحدث مع الأطفال والبائعين.
    -> END
}
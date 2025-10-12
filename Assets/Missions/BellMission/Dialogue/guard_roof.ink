INCLUDE globals.ink

-> start

=== start ===
{
    - not got_roof_key:
        تحتاج مفتاح السطح أولاً من العمّ صالح. #portrait:guard_thinking
        -> END
    - not workshop_task_done or not music_task_done:
        أكمل مهمة الورشة والموسيقى ثم عد إلي. #portrait:guard_thinking
        -> END
    - bell_mission_completed:
        الجرس يعمل الآن. أحسنت. #portrait:guard_happy
        -> END
    - else:
        لديك كل شيء. بدّل المفاتيح بهذا النمط: تشغيل، إيقاف، تشغيل، إيقاف. #portrait:guard_thinking
        + [بدلتها كما قلت] -> solved
        + [نسيت النمط] -> hint
}

=== hint ===
تذكّر: حتى الأرقام تشغيل. 2 و4. #portrait:guard_thinking
-> start

=== solved ===
ممتاز! الجرس عاد للعمل! #portrait:guard_happy
~ bell_mission_completed = true
-> END
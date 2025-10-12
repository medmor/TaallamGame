INCLUDE globals.ink

-> start

=== start ===
{
    - bell_mission_completed:
        أحسنت! الجرس يعمل الآن. #speaker:المعلمة #portrait:teacher_happy
        -> END
    - not bell_mission_started:
        الجرس لا يعمل. نحتاج مساعدتك. #portrait:teacher_thinking
        ستحتاج مفتاح السطح من العمّ صالح، ثم مساعدة أحمد وفاطمة. #portrait:teacher_neutral
        ~ bell_mission_started = true
        -> END
    - else:
        تذكير: المفتاح من العمّ صالح، ثم أحمد في الورشة، ثم فاطمة في الموسيقى، وبعدها إلى السطح. #portrait:teacher_thinking
        -> END
}
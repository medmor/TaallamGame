INCLUDE globals.ink

-> janitor_start

=== janitor_start ===
{
    - got_roof_key:
        لديك المفتاح بالفعل. بالتوفيق! #portrait:janitor_neutral
        -> END
    - else:
        تحتاج مفتاح السطح؟ حل هذه الأحجية أولاً. #portrait:janitor_thinking
        ما الشيء الذي لديه مفاتيح كثيرة ولكنه لا يفتح الأقفال؟ #portrait:janitor_thinking
        + [البيانو] -> correct
        + [الخريطة] -> wrong
        + [الخزانة] -> wrong
}

=== correct ===
أحسنت! هذا هو المفتاح. #portrait:janitor_happy
~ got_roof_key = true
-> END

=== wrong ===
ليست الإجابة. جرّب مرة أخرى. #portrait:janitor_sad
-> janitor_start
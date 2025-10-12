INCLUDE globals.ink

-> start

=== start ===
{
    - music_task_done:
        انتهينا من العدّ هنا. #portrait:fatima_neutral
        -> END
    - else:
        عندي ورقة نوتة فيها كرات خضراء (٥) وحمراء (٣). كم المجموع؟ #portrait:fatima_thinking
        + [٨] -> correct
        + [٦] -> wrong_low
        + [١٠] -> wrong_high
}

=== correct ===
بارك الله فيك! الإجابة ٨. #portrait:fatima_happy
~ music_task_done = true
-> END

=== wrong_low ===
أقل من المطلوب. فكّر: ٥ + ٣. #portrait:fatima_thinking
-> start

=== wrong_high ===
أكثر من المطلوب. فكّر: ٥ + ٣. #portrait:fatima_thinking
-> start
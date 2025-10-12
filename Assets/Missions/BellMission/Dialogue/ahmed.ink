INCLUDE globals.ink

-> start

=== start ===
{
    - workshop_task_done:
        أنجزت المطلوب هنا. اذهب للمرحلة التالية. #portrait:ahmed_neutral
        -> END
    - else:
        نحتاج مسامير تثبيت. أي مجموعة مجموعها يساوي 8؟ #portrait:ahmed_thinking
        + [3 + 5] -> correct
        + [2 + 4] -> also_correct
        + [6 + 1] -> wrong
}

=== correct ===
تمام! إجابة صحيحة. #portrait:ahmed_happy
~ workshop_task_done = true
-> END

=== also_correct ===
صحيح أيضاً! هناك أكثر من طريقة للوصول إلى 8. #portrait:ahmed_happy
~ workshop_task_done = true
-> END

=== wrong ===
قريب! جرّب مرة أخرى. #portrait:ahmed_thinking
-> start
INCLUDE globals.ink

-> start

=== start ===
{
    - workshop_task_done:
        أنجزت المطلوب هنا. اذهب للمرحلة التالية. #portrait:ahmed_neutral
        -> END
    - else:
        السلام عليكم ياصديقي.#portrait:ahmed_thinking #speaker:العم صالح 
        أرى أنك في مهمة للإصلاح جرس المدرسة.
        لكن، لإصلاح الجرس ستحتاج، لزر جرس جديد تستبدل به الزر القديم المعطل.
        أنا أيضا احتاج للمساعدة هنا. هل ترغب بمساعدتي؟
        نحتاج مسامير تثبيت. أي مجموعة مجموعها يساوي 8؟ 
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
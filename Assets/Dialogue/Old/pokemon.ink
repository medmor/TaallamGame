INCLUDE globals.ink

#audio:animal_crossing_low
{ pokemon_name == "": -> main | -> already_chose }

=== main ===
ما هو <b>البوكيمون</b> الذي تختاره؟
    + [الاختيار الأول]
        -> chosen("الاختيار الأول")
    + [الاختيار الثاني]
        -> chosen("الاختيار الثاني")
    + [الاختيار الثالث]
        -> chosen("الاختيار الثالث")
        
=== chosen(pokemon) ===
~ pokemon_name = pokemon
لقد اخترت {pokemon}!
-> END

=== already_chose ===
لقد اخترت {pokemon_name} بالفعل!
-> END
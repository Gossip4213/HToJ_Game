-> trolley_hub

=== trolley_hub ===
#speaker: The Judge
#bgm: Dilemma
Ambrose, spójrz na tory poniżej.
Rozpędzony wagonik jedzie w stronę pięciu robotników na głównym torze. Nie zdążą zejść z toru.
Obok ciebie znajduje się dźwignia, która skieruje wagonik na boczny tor, gdzie stoi jeden robotnik.
-> trolley_choice

=== trolley_choice ===
Wagonika nie da się zatrzymać. Pociągnięcie dźwigni zmieni jego kierunek; pozostawienie jej nietkniętej zachowa obecny tor jazdy.
Co należy zrobić?

// ------------------------------------------------------------
+ #id:Lever [dźwignia]
    #speaker: Ambrose
    Mechanizm jest prosty: jeden ruch zmienia to, kto zostaje wystawiony na niebezpieczeństwo.
    Pociągnięcie jej uczyniłoby zmianę kierunku świadomym działaniem. Pozostawienie jej nietkniętej pozwoliłoby obecnemu biegowi wydarzeń trwać dalej.
    -> trolley_choice

+ #id:TheFive [pięciu robotników]
    #speaker: Ambrose
    Pięć osób jest uwięzionych na głównym torze. Każda z nich ma takie samo bezpośrednie prawo do przeżycia jak osoba na bocznym torze.
    -> trolley_choice

+ #id:TheOne [robotnik na bocznym torze]
    #speaker: Ambrose
    Jedna osoba stoi na bocznym torze, obecnie poza drogą wagonika.
    Skierowanie wagonika na ten tor naraziłoby ją na niebezpieczeństwo, aby ocalić pięciu.
    -> trolley_choice

+ #id:SilverCoin [moneta]
    #speaker: Ambrose
    Zupełnie zwyczajna.
    -> trolley_choice

* [Pociągnąć dźwignię, skierować wagonik na inny tor]
    #speaker: Ambrose
    Skieruję go na inny tor. Jedna osoba umrze, ale inaczej pięć osób zginie na obecnym torze.
    #speaker: The Judge
    Interweniujesz i zmniejszasz liczbę śmierci, czyniąc zarazem śmierć pojedynczego robotnika konsekwencją własnego działania.
    -> end_dilemma

* [Zostawić dźwignię]
    #speaker: Ambrose
    Nie skieruję wagonika na inny tor. Pięciu pozostanie w niebezpieczeństwie, ale nie postawię robotnika z bocznego toru na jego drodze.
    #speaker: The Judge
    Zachowujesz istniejący bieg wydarzeń i unikasz użycia jednej osoby jako środka do uratowania innych, dopuszczając jednocześnie pięć możliwych do uniknięcia śmierci.
    -> end_dilemma

* [Rzucić monetą]
    #speaker: Ambrose
    Orzeł: pociągnąć dźwignię. Reszka: zostawić ją nietkniętą.
    #speaker: The Judge
    Procedura jest bezstronna, ale wybór procedury nadal jest decyzją moralną.
    -> coin_result

// ------------------------------------------------------------

=== coin_result ===
#speaker: Ambrose
(Moneta obraca się w powietrzu i uderza o metalową obudowę.)
Reszka. Zgodnie z zasadą, którą wybrałem, powinienem zostawić dźwignię nietkniętą.

#speaker: The Judge
Wynik cię nie zmusza. Zwraca ci tylko decyzję w formie, za którą zgodziłeś się podążyć.

* [Podążyć za wynikiem, zostawić dźwignię nietkniętą]
    #speaker: Ambrose
    Wybrałem procedurę, zanim poznałem wynik. Podążę za nią, choć pięć osób umrze.
    #speaker: The Judge
    Widzę.
    -> end_dilemma

* [Odrzucić go, pociągnąć dźwignię]
    #speaker: Ambrose
    Użyłem przypadku, aby nie faworyzować żadnej strony, ale nie jestem gotów zaakceptować tego wyniku. Pociągnę dźwignię.
    #speaker: The Judge
    Widzę.
    -> end_dilemma

// ------------------------------------------------------------

=== end_dilemma ===
#speaker: The Judge
Decyzja została podjęta.

#speaker: Ambrose
(Dźwięk kół i metalu wraca nagle, cały naraz.)

#load_scene: Chapter1_Test
-> END

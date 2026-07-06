// Standalone third chapter. Mount Chapter2_EN.json on the Chapter2_Test scene.

-> robin_hood_intro

=== robin_hood_intro ===
#speaker: The Judge
#bgm: Dilemma
Surowa zima zniszczyła plony w całym hrabstwie.
Konwój podatkowy wiezie liczne srebrne marki do stolicy. Większość pieniędzy zebrano od wielkich właścicieli ziemskich, a resztę od kupców, z opłat dzierżawnych i targowych.
Pieniądze są prawnie przeznaczone na naprawę mostu uszkodzonego przez powódź oraz na wypłatę dla strażników dróg przez następne dwa miesiące.
Robin Hood może przejąć konwój i rozdzielić pieniądze za pośrednictwem lokalnych parafii. Rejestry parafialne wskazują, że zapewniłoby to rodzinom żywność i lekarstwa do końca zimy.
Jeśli konwój zostanie przejęty, naprawa mostu prawdopodobnie się opóźni, a strażnicy mogą nie otrzymać zapłaty. Wiele osób może zostać odciętych podczas wiosennych powodzi, a podróże po drogach hrabstwa mogą stać się mniej bezpieczne.
Jeśli konwój pojedzie dalej, roboty publiczne mogą przebiegać zgodnie z planem, ale niektóre gospodarstwa mogą głodować, zanim dotrze następna oficjalna dostawa pomocy.
Szeryf odmówił przekierowania funduszy, argumentując, że są one prawnie oznaczone na konkretny cel i że zmiana budżetu mogłaby sprowadzić kary ze strony Korony.
Czy Robin Hood powinien przejąć konwój i rozdzielić pieniądze?

#speaker: Ambrose
Jedna decyzja odpowiada na natychmiastową potrzebę, zabierając środki z ich zgodnego z prawem przeznaczenia.
Druga chroni instytucje publiczne i przyszłe bezpieczeństwo, pozostawiając obecną sytuację kryzysową niewystarczająco zaadresowaną.
-> robin_hood_hub

=== robin_hood_hub ===
#speaker: Ambrose
(Te same pieniądze nie mogą zaspokoić obu roszczeń. Powinienem sprawdzić, kto ponosi koszt każdej decyzji.)

+ #id:Convoy [Sprawdzić konwój podatkowy]
    #speaker: Ambrose
    Konwój przewozi pieniądze zebrane zgodnie z prawem hrabstwa.
    Niektórzy płatnicy mogą udźwignąć stratę. Inni to drobni kupcy i dzierżawcy, którzy już zapłacili więcej, niż mogli łatwo oddać.
    -> robin_hood_hub

+ #id:ReliefList [Przejrzeć parafialną listę pomocy]
    #speaker: Ambrose
    Lista zawiera gospodarstwa, którym zostało niewiele jedzenia. W kilku są dzieci, osoby starsze albo mieszkańcy zbyt chorzy, by pracować.
    Szacunki są wiarygodne, ale rozdanie pieniędzy teraz nie stworzyłoby trwałego systemu pomocy.
    -> robin_hood_hub

+ #id:BridgeLedger [Przejrzeć rachunki mostu i strażników]
    #speaker: Ambrose
    Most jest główną drogą dla miejscowej ludności. Inżynierowie ostrzegają, że kolejna powódź może uczynić go nieprzejezdnym bez napraw.
    Strażnicy chronią kupców i podróżnych, choć ich służba pomaga także egzekwować ten sam system podatkowy, któremu sprzeciwia się Robin Hood.
    -> robin_hood_hub

+ #id:SheriffOrder [Przeczytać pisemną odmowę szeryfa]
    #speaker: Ambrose
    Szeryf postępuje zgodnie z legalnym budżetem i mógłby zostać usunięty lub ukarany za jego przekierowanie.
    Jego odmowa chroni porządek instytucjonalny, ale nie zapewnia na czas pomocy gospodarstwom wymienionym na liście.
    -> robin_hood_hub

+ #id:SilverCoin [Obejrzeć monetę]
    #speaker: Ambrose
    Moneta mogłaby wybrać między natychmiastową pomocą a przypisanymi robotami publicznymi, nie faworyzując żadnej strony.
    Nie potrafiłaby wyjaśnić, dlaczego przegrane roszczenie ma ponieść koszt.
    -> robin_hood_hub

* [Przejąć konwój, rozdzielić pieniądze]
    #speaker: Ambrose
    Przejmę go. Zimowy kryzys jest natychmiastowy, a pieniądze mogą teraz zapobiec poważnej krzywdzie.
    #speaker: The Judge
    Zaspokajasz pilne potrzeby, unieważniając prawo, własność i przyszły cel budżetu. Gospodarstwa otrzymują pomoc, podczas gdy most, strażnicy i płatnicy ponoszą ryzyko.
    #action: meta_robin_hood_seized
    -> robin_hood_end

* [Pozwolić konwojowi jechać dalej]
    #speaker: Ambrose
    Pozwolę mu przejechać. Fundusze zebrano na wspólną infrastrukturę i bezpieczeństwo, a zabranie ich przeniosłoby kryzys na inne społeczności.
    #speaker: The Judge
    Zachowujesz legalny podział środków i przyszłe dobra publiczne, akceptując, że istniejący proces pomocy może dla niektórych gospodarstw nadejść za późno.
    #action: meta_robin_hood_allowed
    -> robin_hood_end

* [Rzucić monetą]
    #speaker: Ambrose
    Orzeł: przejąć i rozdzielić pieniądze. Reszka: pozwolić konwojowi jechać dalej.
    #speaker: The Judge
    Moneta daje obu roszczeniom równą szansę.
    -> robin_hood_coin_result

// ------------------------------------------------------------

=== robin_hood_coin_result ===
#speaker: Ambrose
(Moneta ląduje orłem do góry.)
Zgodnie z zasadą, którą wybrałem, Robin Hood powinien przejąć konwój.

#speaker: The Judge
Możesz podążyć za procedurą albo ją odrzucić. Każda decyzja ujawni, jaką rolę naprawdę odegrała moneta.

* [Podążyć za wynikiem, przejąć konwój]
    #speaker: Ambrose
    Podążę za wynikiem. Fundusze zostaną rozdzielone, a ja akceptuję, że most i system dróg mogą ucierpieć później.
    #speaker: The Judge
    Szanujesz bezstronną procedurę i dostarczasz natychmiastową pomoc.
    #action: meta_robin_hood_coin_seized
    -> robin_hood_end

* [Odrzucić go, pozwolić konwojowi jechać dalej]
    #speaker: Ambrose
    Nie pozwolę, by moneta przekierowała środki publiczne. Konwój pojedzie dalej, a ja akceptuję krzywdę, jaką może spowodować opóźniona pomoc.
    #speaker: The Judge
    Unieważniasz procedurę, aby zachować prawo i publiczne planowanie.
    #action: meta_robin_hood_coin_overridden
    -> robin_hood_end

// ------------------------------------------------------------

=== robin_hood_end ===
#speaker: The Judge
Trzecia decyzja została zapisana.

#load_scene: Test_ending
-> END

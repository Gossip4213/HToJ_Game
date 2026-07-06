// Standalone third chapter. Mount Chapter2_EN.json on the Chapter2_Test scene.

-> robin_hood_intro

=== robin_hood_intro ===
#speaker: The Judge
#bgm: Dilemma
Ein harter Winter hat die Ernte im ganzen County beschädigt.
Ein Steuerkonvoi bringt zahlreiche Silbermark in die Hauptstadt. Der größte Teil des Geldes wurde von großen Grundbesitzern eingezogen, der Rest stammt von Kaufleuten, Pächterabgaben und Marktgebühren.
Das Geld ist rechtlich dafür bestimmt, eine durch Hochwasser beschädigte Brücke zu reparieren und die Straßenwachen für die nächsten zwei Monate zu bezahlen.
Robin Hood kann den Konvoi überfallen und das Geld über die örtlichen Pfarreien verteilen. Die Pfarrregister zeigen, dass dies den Haushalten bis zum Ende des Winters Nahrung und Medizin verschaffen würde.
Wenn der Konvoi überfallen wird, wird die Brückenreparatur wahrscheinlich verzögert, und die Wachen könnten unbezahlt bleiben. Viele Menschen könnten bei den Frühjahrsfluten abgeschnitten werden, und das Reisen auf den Straßen des County könnte unsicherer werden.
Wenn der Konvoi weiterfährt, können die öffentlichen Arbeiten wie geplant fortgesetzt werden, aber einige Haushalte könnten hungern, bevor die nächste offizielle Hilfslieferung eintrifft.
Der Sheriff hat sich geweigert, die Mittel umzuleiten, mit der Begründung, dass sie rechtlich zweckgebunden seien und eine Änderung des Budgets Strafen durch die Krone nach sich ziehen könnte.
Soll Robin Hood den Konvoi überfallen und das Geld verteilen?

#speaker: Ambrose
Eine Entscheidung beantwortet die unmittelbare Not, indem sie Mittel ihrem rechtmäßigen Zweck entzieht.
Die andere schützt öffentliche Institutionen und künftige Sicherheit, während sie die gegenwärtige Not unzureichend beantwortet lässt.
-> robin_hood_hub

=== robin_hood_hub ===
#speaker: Ambrose
(Dasselbe Geld kann nicht beiden Ansprüchen genügen. Ich sollte prüfen, wer die Kosten jeder Wahl trägt.)

+ #id:Convoy [Den Steuerkonvoi prüfen]
    #speaker: Ambrose
    Der Konvoi transportiert Geld, das nach den Gesetzen des County eingezogen wurde.
    Einige Beitragszahler können den Verlust verkraften. Andere sind kleine Händler und Pächter, die bereits mehr gezahlt haben, als sie leicht entbehren konnten.
    -> robin_hood_hub

+ #id:ReliefList [Die Hilfsliste der Pfarreien prüfen]
    #speaker: Ambrose
    Die Liste enthält die Haushalte, denen nur noch wenig Nahrung bleibt. Mehrere umfassen Kinder, ältere Menschen oder Bewohner, die zu krank zum Arbeiten sind.
    Die Schätzungen sind glaubwürdig, aber das Geld jetzt zu verteilen würde kein dauerhaftes Hilfssystem schaffen.
    -> robin_hood_hub

+ #id:BridgeLedger [Die Brücken- und Wachkonten prüfen]
    #speaker: Ambrose
    Die Brücke ist der Hauptweg für die örtliche Bevölkerung. Ingenieure warnen, dass ein weiteres Hochwasser sie ohne Reparaturen unbenutzbar machen könnte.
    Die Wachen schützen Kaufleute und Reisende, auch wenn ihr Dienst zugleich hilft, dasselbe Steuersystem durchzusetzen, gegen das Robin Hood sich stellt.
    -> robin_hood_hub

+ #id:SheriffOrder [Die schriftliche Ablehnung des Sheriffs lesen]
    #speaker: Ambrose
    Der Sheriff folgt einem rechtmäßigen Budget und könnte abgesetzt oder bestraft werden, wenn er es umleitet.
    Seine Weigerung bewahrt institutionelle Ordnung, aber sie leistet den Haushalten auf der Hilfsliste keine rechtzeitige Hilfe.
    -> robin_hood_hub

+ #id:SilverCoin [Die Münze betrachten]
    #speaker: Ambrose
    Eine Münze könnte zwischen unmittelbarer Hilfe und den zugewiesenen öffentlichen Arbeiten wählen, ohne eine Seite zu bevorzugen.
    Sie könnte nicht erklären, warum der unterliegende Anspruch die Kosten tragen soll.
    -> robin_hood_hub

* [Den Konvoi überfallen, das Geld verteilen]
    #speaker: Ambrose
    Ich werde ihn überfallen. Die Winternot ist unmittelbar, und das Geld kann jetzt schweren Schaden verhindern.
    #speaker: The Judge
    Du erfüllst dringende Bedürfnisse, indem du Gesetz, Eigentum und den künftigen Zweck des Budgets übergehst. Die Haushalte erhalten Hilfe, während Brücke, Wachen und Beitragszahler das Risiko tragen.
    #action: meta_robin_hood_seized
    -> robin_hood_end

* [Den Konvoi weiterfahren lassen]
    #speaker: Ambrose
    Ich werde ihn passieren lassen. Die Mittel wurden für gemeinsame Infrastruktur und Sicherheit eingezogen, und sie zu nehmen würde die Not auf andere Gemeinschaften übertragen.
    #speaker: The Judge
    Du bewahrst die rechtmäßige Zuweisung und künftige öffentliche Güter, während du akzeptierst, dass der bestehende Hilfsprozess für manche Haushalte zu spät kommen kann.
    #action: meta_robin_hood_allowed
    -> robin_hood_end

* [Die Münze werfen]
    #speaker: Ambrose
    Kopf: das Geld beschlagnahmen und verteilen. Zahl: den Konvoi weiterfahren lassen.
    #speaker: The Judge
    Die Münze gibt beiden Ansprüchen die gleiche Chance.
    -> robin_hood_coin_result

// ------------------------------------------------------------

=== robin_hood_coin_result ===
#speaker: Ambrose
(Die Münze landet mit Kopf nach oben.)
Nach der Regel, die ich gewählt habe, sollte Robin Hood den Konvoi überfallen.

#speaker: The Judge
Du kannst dem Verfahren folgen oder es zurückweisen. Jede Wahl wird zeigen, welche Rolle die Münze tatsächlich gespielt hat.

* [Dem Ergebnis folgen, den Konvoi überfallen]
    #speaker: Ambrose
    Ich werde dem Ergebnis folgen. Die Mittel werden verteilt, und ich akzeptiere, dass die Brücke und das Straßensystem später leiden können.
    #speaker: The Judge
    Du ehrst das unparteiische Verfahren und leistest unmittelbare Hilfe.
    #action: meta_robin_hood_coin_seized
    -> robin_hood_end

* [Es zurückweisen, den Konvoi weiterfahren lassen]
    #speaker: Ambrose
    Ich werde nicht zulassen, dass die Münze öffentliche Mittel umlenkt. Der Konvoi wird weiterfahren, und ich akzeptiere den Schaden, den verzögerte Hilfe verursachen kann.
    #speaker: The Judge
    Du übergehst das Verfahren, um Gesetz und öffentliche Planung zu bewahren.
    #action: meta_robin_hood_coin_overridden
    -> robin_hood_end

// ------------------------------------------------------------

=== robin_hood_end ===
#speaker: The Judge
Die dritte Entscheidung ist aufgezeichnet.

#load_scene: Test_ending
-> END

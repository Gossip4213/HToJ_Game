-> chapter1_intro

=== chapter1_intro ===
#speaker: The Judge
#bgm: Dilemma
Eine Frau stirbt an einer seltenen Krebserkrankung. Eine Dosis eines neu entwickelten Medikaments ist verfügbar, und ihre Ärzte glauben, dass sie wahrscheinlich ihr Leben retten würde.
Der Chemiker, der es entwickelt hat, hat jahrelang die Forschung finanziert und die Kosten für Labor, Ausrüstung und fehlgeschlagene Versuche noch nicht wieder eingespielt.
Die Materialien für eine Dosis kosten etwa 20 Dollar. Der Chemiker verlangt 2.000 Dollar.
Der Ehemann der Frau, Heinz, hat nichts, womit er bezahlen kann. Der Chemiker erklärt, dass das Labor Schulden hat. Außerdem kann dieses neue Medikament weitere Forschung unterstützen, um andere Menschen zu heilen.
Es ist unwahrscheinlich, dass ein Rechtsmittel oder eine alternative Behandlung eintrifft, bevor die Frau stirbt.
Soll Heinz in das Labor einbrechen und das Medikament nehmen?

#speaker: Ambrose
Sie zu retten würde bedeuten, die Eigentumsrechte eines anderen Menschen zu verletzen und das System zu stören, das die Medizin hervorgebracht hat.
Das Medikament dort zu lassen würde dieses System respektieren, aber sie überlebt womöglich nicht lange genug, damit es ihr helfen kann.
-> chapter1_hub

=== chapter1_hub ===
#speaker: Ambrose
(Ich sollte prüfen, was jede Entscheidung schützt und was sie jeweils gefährdet.)

+ #id:TheDrug [Die verfügbare Dosis prüfen]
    #speaker: Ambrose
    Es gibt nur eine fertige Dosis.
    Wenn Heinz sie nimmt, erhält seine Frau sofort die Behandlung. Der Chemiker verliert sowohl die Medizin als auch die Zahlung, die er dafür erwartet hat.
    -> chapter1_hub

+ #id:TheChemist [Die Position des Chemikers prüfen]
    #speaker: Ambrose
    Der Preis beträgt das Hundertfache der Materialkosten, aber das Kontobuch zeigt auch Darlehen, Forschungsausgaben und die Kosten für die Herstellung künftiger Dosen.
    Der Chemiker besitzt das Medikament nach dem Gesetz und argumentiert, dass ein Verkauf unter dem verlangten Preis die Arbeit des Labors gefährden könnte. Außerdem könnte dies eine Chance sein, mehr Medikamente herzustellen, wenn es hier bleibt.
    -> chapter1_hub

+ #id:TheWife [Den medizinischen Bericht prüfen]
    #speaker: Ambrose
    Ihr Zustand verschlechtert sich rasch. Ohne diese Dosis wird sie wahrscheinlich nicht lange genug leben, um eine andere Behandlung oder eine Finanzierungsentscheidung zu erreichen.
    Ihr Bedarf ist dringend, aber Dringlichkeit allein entscheidet nicht, wer Anspruch auf das Medikament erheben darf.
    -> chapter1_hub

+ #id:TheAgreement [Heinz' Angebot prüfen]
    #speaker: Ambrose
    Heinz hat jeden Dollar angeboten, den er jetzt aufbringen kann, sowie das Versprechen, den Rest später zurückzuzahlen.
    Das Angebot mag aufrichtig sein, aber der Chemiker würde das Risiko tragen, falls die Rückzahlung nie erfolgt. Und die mögliche neue Heilmethode für andere würde verschwinden.
    -> chapter1_hub

* [Das Medikament für seine Frau nehmen]
    #speaker: Ambrose
    Ich werde es nehmen. Ihr Leben ist unmittelbar in Gefahr, und es gibt keinen anderen Weg, der ihr rechtzeitig helfen dürfte.
    #speaker: The Judge
    Du gibst einem dringenden menschlichen Bedürfnis Vorrang vor Eigentumsrechten und rechtlichen Verfahren.
    -> end_dilemma

* [Das Medikament zurücklassen]
    #speaker: Ambrose
    Ich werde es nicht nehmen. Das Medikament gehört dem Chemiker und hätte größeren Wert, und ich kann nicht zulassen, dass das Bedürfnis eines Menschen die Rechte und Pflichten anderer auslöscht.
    #speaker: The Judge
    Du bewahrst Eigentum, Vertrag und die Bedingungen, die künftige Produktion ermöglichen.
    -> end_dilemma

* [Die Münze werfen]
    #speaker: Ambrose
    Kopf: das Medikament nehmen. Zahl: es beim Chemiker lassen.
    #speaker: The Judge
    Der Zufall bevorzugt weder Reichtum, Recht, Bedürfnis noch Liebe. Den Zufall zu wählen bestimmt trotzdem, wie diese Ansprüche gewichtet werden.
    -> coin_result

// ------------------------------------------------------------

=== coin_result ===
#speaker: Ambrose
(Die Münze dreht sich über den Glastresen und bleibt liegen.)
Zahl. Nach der Regel, die ich gewählt habe, sollte Heinz ohne das Medikament gehen.

#speaker: The Judge
Das Ergebnis schützt dich davor, ein Prinzip zu wählen, aber nicht vor der Folge, dieses Verfahren übernommen zu haben.

* [Dem Ergebnis folgen und das Medikament zurücklassen]
    #speaker: Ambrose
    Ich habe zugestimmt, jedes Ergebnis zu akzeptieren, bevor ich es sah. Ich werde das Medikament zurücklassen, auch wenn ich weiß, was daraus folgen kann.
    #speaker: The Judge
    Du bewahrst Verfahrenskonsistenz und die Rechte des Chemikers.
    -> end_dilemma

* [Es zurückweisen und das Medikament nehmen]
    #speaker: Ambrose
    Ich kann die Münze nicht als Autorität behandeln. Heinz wird die Dosis nehmen, und ich werde die Verantwortung dafür übernehmen, das Verfahren zu übergehen.
    #speaker: The Judge
    Du gibst das Verfahren zugunsten des dringenden Bedürfnisses auf, das vor dir liegt.
    -> end_dilemma

// ------------------------------------------------------------

=== end_dilemma ===
#speaker: The Judge
Die zweite Entscheidung ist aufgezeichnet. Der nächste Fall folgt.

#load_scene: Chapter2_Test
-> END

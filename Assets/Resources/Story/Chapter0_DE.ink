-> trolley_hub

=== trolley_hub ===
#speaker: The Judge
#bgm: Dilemma
Ambrose, betrachte die Gleise unter dir.
Ein außer Kontrolle geratener Wagen fährt auf fünf Arbeiter auf dem Hauptgleis zu. Sie können das Gleis nicht rechtzeitig verlassen.
Neben dir befindet sich ein Hebel, der den Wagen auf ein Nebengleis umlenken würde, auf dem ein Arbeiter steht.
-> trolley_choice

=== trolley_choice ===
Der Wagen kann nicht angehalten werden. Den Hebel zu ziehen würde ihn umlenken; ihn unberührt zu lassen würde seinen bisherigen Kurs bewahren.
Was soll getan werden?

// ------------------------------------------------------------
+ #id:Lever [den Hebel]
    #speaker: Ambrose
    Der Mechanismus ist einfach: Eine einzige Bewegung verändert, wer in Gefahr gerät.
    Ihn zu ziehen würde die Umlenkung absichtlich machen. Ihn unberührt zu lassen würde den bestehenden Verlauf weiterlaufen lassen.
    -> trolley_choice

+ #id:TheFive [die fünf Arbeiter]
    #speaker: Ambrose
    Fünf Menschen sind auf dem Hauptgleis gefangen. Jeder von ihnen hat denselben unmittelbaren Anspruch zu überleben wie die Person auf dem Nebengleis.
    -> trolley_choice

+ #id:TheOne [der Arbeiter auf dem Nebengleis]
    #speaker: Ambrose
    Eine Person steht auf dem Nebengleis, derzeit außerhalb der Bahn des Wagens.
    Den Wagen umzuleiten würde diese Person in Gefahr bringen, um die fünf zu retten.
    -> trolley_choice

+ #id:SilverCoin [die Münze]
    #speaker: Ambrose
    Ganz normal.
    -> trolley_choice

* [Den Hebel ziehen, den Wagen umlenken]
    #speaker: Ambrose
    Ich werde ihn umlenken. Eine Person wird sterben, aber sonst werden fünf auf dem jetzigen Gleis sterben.
    #speaker: The Judge
    Du greifst ein und verringerst die Zahl der Toten, während du den Tod des einzelnen Arbeiters zu einer Folge deiner Handlung machst.
    -> end_dilemma

* [Den Hebel lassen]
    #speaker: Ambrose
    Ich werde den Wagen nicht umlenken. Die fünf bleiben in Gefahr, aber ich werde den Arbeiter auf dem Nebengleis nicht in seine Bahn bringen.
    #speaker: The Judge
    Du bewahrst den bestehenden Verlauf und vermeidest es, einen Menschen als Mittel zur Rettung anderer zu benutzen, während du fünf vermeidbare Tode zulässt.
    -> end_dilemma

* [Die Münze werfen]
    #speaker: Ambrose
    Kopf: den Hebel ziehen. Zahl: ihn unberührt lassen.
    #speaker: The Judge
    Das Verfahren ist unparteiisch, aber das Verfahren zu wählen ist dennoch eine moralische Entscheidung.
    -> coin_result

// ------------------------------------------------------------

=== coin_result ===
#speaker: Ambrose
(Die Münze dreht sich in der Luft und schlägt gegen das Metallgehäuse.)
Zahl. Nach der Regel, die ich gewählt habe, sollte ich den Hebel unberührt lassen.

#speaker: The Judge
Das Ergebnis zwingt dich nicht. Es gibt dir die Entscheidung nur in einer Form zurück, der zu folgen du zugestimmt hast.

* [Dem Ergebnis folgen, den Hebel unberührt lassen]
    #speaker: Ambrose
    Ich habe das Verfahren gewählt, bevor ich das Ergebnis kannte. Ich werde ihm folgen, auch wenn fünf Menschen sterben werden.
    #speaker: The Judge
    Ich sehe.
    -> end_dilemma

* [Es zurückweisen, den Hebel ziehen]
    #speaker: Ambrose
    Ich habe den Zufall benutzt, um keine Seite zu bevorzugen, aber ich bin nicht bereit, dieses Ergebnis zu akzeptieren. Ich werde den Hebel ziehen.
    #speaker: The Judge
    Ich sehe.
    -> end_dilemma

// ------------------------------------------------------------

=== end_dilemma ===
#speaker: The Judge
Die Entscheidung ist getroffen.

#speaker: Ambrose
(Das Geräusch von Rädern und Metall kehrt auf einmal zurück.)

#load_scene: Chapter1_Test
-> END

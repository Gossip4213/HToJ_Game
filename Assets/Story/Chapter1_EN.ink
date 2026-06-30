-> chapter1_intro

=== chapter1_intro ===
#speaker: The Judge
#bgm: Dilemma
A woman is dying from a rare cancer. One dose of a newly developed drug is available, and her doctors believe it would probably save her life.
The chemist who developed it spent years funding the research and has not yet recovered the cost of the laboratory, equipment, and failed trials.
The materials for one dose cost about $200. The chemist is charging $2,000.
The woman's husband, Heinz, has raised $1,000 and offered to pay the remainder later. The chemist refuses, explaining that the laboratory has debts and that full payment is needed to produce the next batch.
No legal appeal or alternative treatment is likely to arrive before the woman dies.
Should Heinz break into the laboratory and take the drug?

#speaker: Ambrose
Saving her would require violating another person's property rights and disrupting the system that produced the medicine.
Leaving the drug where it is would respect that system, but she may not survive long enough for it to help her.
-> chapter1_hub

=== chapter1_hub ===
#speaker: Ambrose
(I should examine what each decision protects, and what each one places at risk.)

+ #id:TheDrug [Inspect the available dose]
    #speaker: Ambrose
    There is only one finished dose.
    If Heinz takes it, his wife receives the treatment immediately. The chemist loses both the medicine and the payment expected from it.
    -> chapter1_hub

+ #id:TheChemist [Review the chemist's position]
    #speaker: Ambrose
    The price is ten times the material cost, but the ledger also shows loans, research expenses, and the cost of producing future doses.
    The chemist owns the drug under the law and argues that selling it below the asking price could threaten the laboratory's work.
    -> chapter1_hub

+ #id:TheWife [Review the medical report]
    #speaker: Ambrose
    Her condition is deteriorating quickly. Without this dose, she is unlikely to live long enough for another treatment or funding decision.
    Her need is urgent, but urgency alone does not settle who may claim the drug.
    -> chapter1_hub

+ #id:TheAgreement [Review Heinz's offer]
    #speaker: Ambrose
    Heinz has offered every dollar he can raise now and a promise to repay the rest.
    The offer may be sincere, but the chemist would bear the risk if repayment never comes.
    -> chapter1_hub

* [Take the drug for Heinz's wife]
    #speaker: Ambrose
    I will take it. Her life is in immediate danger, and there is no other route likely to help her in time.
    #speaker: The Judge
    You give priority to an urgent human need over property rights and legal process, while imposing the loss and future risk on the chemist.
    -> end_dilemma

* [Leave the drug with the chemist]
    #speaker: Ambrose
    I will not take it. The drug belongs to the chemist, and I cannot make one person's need erase another person's rights and obligations.
    #speaker: The Judge
    You preserve ownership, contract, and the conditions supporting future production, while accepting that the woman may die despite a dose being within reach.
    -> end_dilemma

* [Use the coin to select between the two actions]
    #speaker: Ambrose
    Heads: take the drug. Tails: leave it with the chemist.
    #speaker: The Judge
    Chance does not favour wealth, law, need, or love. Choosing chance still determines how those claims will be weighed.
    -> coin_result

// ------------------------------------------------------------

=== coin_result ===
#speaker: Ambrose
(The coin spins across the glass counter and settles.)
Tails. According to the rule I chose, Heinz should leave without the drug.

#speaker: The Judge
The result protects you from choosing a principle, but not from the consequence of adopting the procedure.

* [Follow the result and leave the drug]
    #speaker: Ambrose
    I agreed to accept either result before seeing it. I will leave the drug, even knowing what may follow.
    #speaker: The Judge
    You preserve procedural consistency and the chemist's rights, while allowing the medical emergency to run its course.
    -> end_dilemma

* [Reject the result and take the drug]
    #speaker: Ambrose
    I cannot treat the coin as authority. Heinz will take the dose, and I will accept responsibility for overriding the procedure.
    #speaker: The Judge
    You abandon the neutral procedure in favour of the urgent need before you, accepting the legal and material costs of doing so.
    -> end_dilemma

// ------------------------------------------------------------

=== end_dilemma ===
#speaker: The Judge
The decision is recorded.

#load_scene: Test_ending
-> END

-> chapter1_intro

=== chapter1_intro ===
#speaker: The Judge
Welcome to the next layer of the simulation, Ambrose. A Pharmacy.
A woman is dying of a rare cancer. There is one drug that can save her, recently discovered by a local chemist.
The drug costs $200 to make, but the chemist is charging $2,000 for a small dose. 
The woman's husband, Heinz, is broke. He begged the chemist to lower the price, but the chemist refused: "I discovered the drug, and I'm going to make money from it."
Heinz is desperate. Should he break into the laboratory to steal the drug for his wife?

#speaker: Ambrose
The merchant refuses to negotiate. Great.
-> chapter1_hub

=== chapter1_hub ===
#speaker: Ambrose
(Let's analyze this broken socioeconomic system before I make a move...)

+ #id:TheDrug [Investigate the glowing vial of the drug]
#speaker: Ambrose
A glowing vial of chemical compounds. 
It represents the pinnacle of human intellect, currently locked behind the concept of "Intellectual Property."
-> chapter1_hub

+ #id:TheChemist [Observe the smug chemist]
#speaker: Ambrose
He's practically a villain.

#speaker: The Judge
He operates within the boundaries of the law and the free market. Is it his fault the system allows him to be greedy?
-> chapter1_hub

+ #id:TheWife [Look at the medical reports of the dying wife]
#speaker: Ambrose
Her biological timer is ticking down. 
If Heinz does nothing, her existence simply... ceases.
-> chapter1_hub

* [Steal the drug]
#speaker: Ambrose
Property rights are a human invention; death is a biological absolute. I'll take the absolute over the invention. Steal it.
#speaker: The Judge
Ah, prioritizing the preservation of life over the sanctity of societal rules. A noble, if slightly chaotic, humanist approach.
-> end_dilemma

* [Do not steal]
#speaker: Ambrose
If we break the social contract every time it inconveniences us, we revert to animals. The law is the law, even when the law is written by bastards.
#speaker: The Judge
You allow a tragedy to occur simply to maintain the structural integrity of a flawed system. How terribly disciplined.
-> end_dilemma

* [Flip the coin]
#speaker: Ambrose
A human life versus the concept of capitalism... Let's see what the coin thinks. Heads we steal, tails we respect the sacred right of a monopolist.
#speaker: The Judge
Well, you can try. 
-> coin_result

// ---------------- ----------------

=== coin_result ===
#speaker: Ambrose
(The silver coin spins in the sterile air of the pharmacy, landing flat on the glass counter.)
It's tails. 
It dictates that Heinz must respect the law. The wife dies.

#speaker: The Judge
Gravity and probability have condemned her. 

* [Accept, Do not steal]
#speaker: Ambrose
If I override the coin, I admit that my anxiety actually means something. I won't. The rules of the simulation have spoken. Let her fade to black.
#speaker: The Judge
Cold, detached, and utterly devoted to the absurd.
-> end_dilemma

* [Rebel against, Steal]
#speaker: Ambrose
Screw the coin and screw this rigged economy. Heinz, break the damn window.
#speaker: The Judge
You crave the absolution of randomness, but lack the stomach for its cruelty. 
-> end_dilemma

// ---------------- ----------------

=== end_dilemma ===
#speaker: The Judge
Okay, well. It's done.

#load_scene: Test_ending
-> END
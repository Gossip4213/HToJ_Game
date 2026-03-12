-> trolley_hub

=== trolley_hub ===
#speaker: The Judge
Welcome to the intersection of morality and physics, Ambrose.
Look down at the tracks. A runaway trolley is hurtling towards five innocent workers. You can pull the lever to steer it in another direction to one unsuspecting person.
You stand by the lever. What is your justice?

// ---------------- ----------------
+ #id:Lever [Examine the heavy iron lever]
#speaker: Ambrose
It's cold and covered in rust. The physical mechanism of choice.
Pulling it makes me a murderer; ignoring it makes me a coward.
-> trolley_hub

+ #id:TheFive [Observe the five workers]
#speaker: Ambrose
They are panicking, screaming silently in this frozen anomaly.
#speaker: The Judge
Society dictates their combined utility is absolute. Do you feel the weight of their numbers?
-> trolley_hub

+ #id:TheOne [Look at the single person]
#speaker: Ambrose
He's completely unaware of the approaching doom. 
It reminds me of a serf in the Holy Roman Empire—just collateral damage for someone else's grand strategy.
-> trolley_hub

+ #id:SilverCoin [Take out the coin in your pocket]
#speaker: Ambrose
A worn silver coin.
In a universe devoid of inherent meaning, a coin toss is just as valid as a PhD dissertation on ethics.
-> trolley_hub


* [Pull the lever]
#speaker: Ambrose
The math is simple. Five lives outweigh one. It's basic utilitarianism.
#speaker: The Judge
Ah, cold, calculating, and terribly boring. 
-> end_dilemma

* [Do nothing]
#speaker: Ambrose
I refuse to be an active participant in murder. Let fate take its course.
#speaker: The Judge
Kant would applaud you, though the five people currently being crushed might disagree.
-> end_dilemma

* [Flip a coin]
#speaker: Ambrose
Heads or tails... Let chance decide the weight of human lives.
#speaker: The Judge
An existentialist cop-out, or perhaps the only true fairness in a chaotic world?
-> coin_result

// ---------------- ----------------

=== coin_result ===
#speaker: Ambrose
(The coin flips in the frozen air, landing with a sharp clink against the rusty metal...)
It's tails. 
Meaning I must do nothing. The five workers will die.

#speaker: The Judge
The oracle of chance has spoken. Will you submit to your own absurd rules, or will your human hubris force you to intervene?

* [Accept, Do nothing]
#speaker: Ambrose
I agreed to the terms. To change my mind now is to pretend my choice has inherent meaning. Let fate take them.
#speaker: The Judge
A frighteningly consistent nihilist. Let us see how you sleep tonight.
-> end_dilemma

* [Pull the lever]
#speaker: Ambrose
No... I can't just stand here and watch them get crushed. To hell with the coin! I'm pulling the lever!
#speaker: The Judge
Fascinating. You sought the comfort of randomness to escape guilt, yet buckled under its actual weight. The ultimate human hypocrisy.
-> end_dilemma


// ---------------- ----------------

=== end_dilemma ===

#speaker: The Judge
The gears of fate turn. Let time resume.

#speaker: Ambrose
(The deafening sound of metal grinding against metal fills the air...)

// 触发 Unity 的场景跳转逻辑
#load_scene: Chapter1_Test
-> END
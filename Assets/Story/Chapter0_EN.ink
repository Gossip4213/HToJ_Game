-> trolley_hub

=== trolley_hub ===
#speaker: The Judge
#bgm: Dilemma
Ambrose, consider the tracks below.
A runaway trolley is moving toward five workers on the main track. They cannot leave the track in time.
Beside you is a lever that will divert the trolley onto a side track, where one worker is standing.
The trolley cannot be stopped. Pulling the lever will redirect it; leaving the lever untouched will preserve its current course.
What should be done?

// ------------------------------------------------------------
+ #id:Lever [Inspect the lever]
    #speaker: Ambrose
    The mechanism is simple: one movement changes who is placed in danger.
    Pulling it would make the redirection deliberate. Leaving it untouched would allow the existing course to continue.
    -> trolley_hub

+ #id:TheFive [Observe the five workers]
    #speaker: Ambrose
    Five people are trapped on the main track. Each has the same immediate claim to survival as the person on the side track.
    Their number matters, but it does not make any one of them more valuable as an individual.
    -> trolley_hub

+ #id:TheOne [Observe the worker on the side track]
    #speaker: Ambrose
    One person stands on the side track, currently outside the trolley's path.
    Diverting the trolley would place that person in danger in order to spare the five.
    -> trolley_hub

+ #id:SilverCoin [Examine the coin in your pocket]
    #speaker: Ambrose
    A coin could choose without preference, but randomness would not remove my responsibility for deciding to use it.
    -> trolley_hub

* [Pull the lever and divert the trolley]
    #speaker: Ambrose
    I will redirect it. One person will die, but five will otherwise die on the current track.
    #speaker: The Judge
    You intervene and reduce the number of deaths, while making the single worker's death a consequence of your action.
    -> end_dilemma

* [Leave the lever untouched]
    #speaker: Ambrose
    I will not redirect the trolley. The five remain in danger, but I will not place the worker on the side track in its path.
    #speaker: The Judge
    You preserve the existing course and avoid using one person as the means of saving the others, while allowing five preventable deaths.
    -> end_dilemma

* [Use the coin to select between the two actions]
    #speaker: Ambrose
    Heads: pull the lever. Tails: leave it untouched.
    #speaker: The Judge
    The procedure is impartial, but choosing the procedure is still a moral decision.
    -> coin_result

// ------------------------------------------------------------

=== coin_result ===
#speaker: Ambrose
(The coin turns in the air and lands against the metal housing.)
Tails. According to the rule I chose, I should leave the lever untouched.

#speaker: The Judge
The result does not compel you. It only returns the decision in a form you agreed to follow.

* [Follow the result and leave the lever untouched]
    #speaker: Ambrose
    I chose the procedure before knowing the outcome. I will follow it, even though five people will die.
    #speaker: The Judge
    You value consistency and impartial procedure over revising the decision in response to its consequence.
    -> end_dilemma

* [Reject the result and pull the lever]
    #speaker: Ambrose
    I used chance to avoid favouring either side, but I am not willing to accept this outcome. I will pull the lever.
    #speaker: The Judge
    You revise the procedure when confronted with its result, accepting direct responsibility for the final choice.
    -> end_dilemma

// ------------------------------------------------------------

=== end_dilemma ===
#speaker: The Judge
The decision is made. Time resumes.

#speaker: Ambrose
(The sound of wheels and metal returns all at once.)

#load_scene: Chapter1_Test
-> END

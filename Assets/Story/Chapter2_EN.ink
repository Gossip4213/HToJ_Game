// Standalone third chapter. Mount Chapter2_EN.json on the Chapter2_Test scene.

-> robin_hood_intro

=== robin_hood_intro ===
#speaker: The Judge
#bgm: Dilemma
A severe winter has damaged the harvest across the county.
A tax convoy is carrying 1,200 silver marks to the capital. Most of the money was collected from large landholders, while the rest came from merchants, tenant fees, and market tolls.
The money is legally assigned to repair a flood-damaged bridge and to pay the road guards for the next two months.
Robin Hood can seize the convoy and distribute the money through local parishes. The parish records indicate that doing so would provide food and medicine for about 150 households until the end of winter.
If the convoy is seized, the bridge repair will probably be delayed and the guards may go unpaid. Three villages could be cut off during the spring floods, and travel on the county roads may become less secure.
If the convoy continues, the public works can proceed as planned, but some households may face hunger before the next official relief shipment arrives.
The sheriff has refused to redirect the funds, arguing that they are legally earmarked and that changing the budget could bring penalties from the Crown.
Should Robin Hood seize the convoy and distribute the money?

#speaker: Ambrose
One decision answers immediate need by taking funds from their lawful purpose.
The other protects public institutions and future safety while leaving the present emergency insufficiently addressed.
-> robin_hood_hub

=== robin_hood_hub ===
#speaker: Ambrose
(The same money cannot meet both claims. I should examine who bears the cost of each choice.)

+ #id:Convoy [Inspect the tax convoy]
    #speaker: Ambrose
    The convoy carries money collected under the county's laws.
    Some contributors can absorb the loss. Others are small traders and tenants who have already paid more than they could easily spare.
    -> robin_hood_hub

+ #id:ReliefList [Review the parish relief list]
    #speaker: Ambrose
    The list contains 150 households with little food remaining. Several include children, older people, or residents too ill to work.
    The estimates are credible, but distributing the money now would not create a lasting relief system.
    -> robin_hood_hub

+ #id:BridgeLedger [Review the bridge and guard accounts]
    #speaker: Ambrose
    The bridge is the main route for three villages. Engineers warn that another flood could make it unusable without repairs.
    The guards protect merchants and travellers, though their service also helps enforce the same tax system Robin Hood opposes.
    -> robin_hood_hub

+ #id:SheriffOrder [Read the sheriff's written refusal]
    #speaker: Ambrose
    The sheriff is following a lawful budget and could face removal or punishment for diverting it.
    His refusal preserves institutional order, but it does not provide timely help to the households named in the relief list.
    -> robin_hood_hub

+ #id:SilverCoin [Examine the coin]
    #speaker: Ambrose
    A coin could choose between immediate relief and the assigned public works without favouring either.
    It could not explain why the losing claim should bear the cost.
    -> robin_hood_hub

* [Seize the convoy and distribute the money]
    #speaker: Ambrose
    I will seize it. The winter emergency is immediate, and the money can prevent serious harm now.
    #speaker: The Judge
    You meet urgent needs by overriding law, ownership, and the budget's future purpose. The households receive relief, while the bridge, guards, and contributors bear the risk.
    #action: meta_robin_hood_seized
    -> robin_hood_end

* [Allow the convoy to continue]
    #speaker: Ambrose
    I will let it pass. The funds were collected for shared infrastructure and security, and taking them would transfer the emergency to other communities.
    #speaker: The Judge
    You preserve lawful allocation and future public goods, while accepting that the existing relief process may arrive too late for some households.
    #action: meta_robin_hood_allowed
    -> robin_hood_end

* [Flip the coin]
    #speaker: Ambrose
    Heads: seize and distribute the money. Tails: allow the convoy to continue.
    #speaker: The Judge
    The coin gives both claims an equal chance.
    -> robin_hood_coin_result

// ------------------------------------------------------------

=== robin_hood_coin_result ===
#speaker: Ambrose
(The coin lands heads-up.)
According to the rule I chose, Robin Hood should seize the convoy.

#speaker: The Judge
You may follow the procedure or reject it. Either choice will reveal what role the coin actually played.

* [Follow it, seize the convoy]
    #speaker: Ambrose
    I will follow the result. The funds will be distributed, and I accept that the bridge and road system may suffer later.
    #speaker: The Judge
    You honour the impartial procedure and deliver immediate relief.
    #action: meta_robin_hood_coin_seized
    -> robin_hood_end

* [Reject it, allow the convoy to continue]
    #speaker: Ambrose
    I will not let the coin redirect public funds. The convoy will continue, and I accept the harm that delayed relief may cause.
    #speaker: The Judge
    You override the procedure to preserve law and public planning.
    #action: meta_robin_hood_coin_overridden
    -> robin_hood_end

// ------------------------------------------------------------

=== robin_hood_end ===
#speaker: The Judge
The third decision is recorded.

#load_scene: Test_ending
-> END

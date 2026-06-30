// 独立的第三章。请在 Chapter2_Test 场景中使用本地化 JSON。
-> robin_hood_intro

=== robin_hood_intro ===
#speaker: The Judge
#bgm: Dilemma
严冬摧毁了郡内多地的收成。
一支税款车队正把1200银马克运往首都。大部分税款来自大地主，其余则来自商人、佃户费用与市场通行税。
依法，这笔钱已经被指定用于修复一座被洪水损坏的桥梁，并支付未来两个月的道路卫兵薪资。
罗宾汉可以截下车队，并通过当地教区把钱分发出去。教区记录显示，这笔钱足以为约150户家庭提供食物与药品，使他们撑过冬季。
若车队被截，桥梁维修很可能延误，卫兵也可能领不到薪水。春季洪水来临时，三个村庄可能因此与外界隔绝，郡内道路也会变得更不安全。
若车队继续前进，公共工程就能按计划进行，但在下一批官方救济抵达前，一些家庭可能已经陷入饥饿。
郡治安官拒绝挪用这笔资金，理由是它已有法定用途，擅自改动预算还可能招致王室处罚。
罗宾汉是否应该截下车队，把钱分给灾民？

#speaker: Ambrose
一个选择，以挪用法定资金来回应眼前的紧急需求。
另一个选择，保护制度与未来安全，却让当下的危机得不到充分救助。
-> robin_hood_hub

=== robin_hood_hub ===
#speaker: Ambrose
（同一笔钱无法同时满足两种诉求。我应该看看，每个选择会让谁承担代价。）

+ #id:Convoy [查看税款车队]
    #speaker: Ambrose
    车队运送的是依据郡内法律征收的税款。
    一些出资者能够承受损失，但另一些小商人和佃户已经缴纳了超出其轻松承受范围的金额。
    -> robin_hood_hub

+ #id:ReliefList [查看教区救济名单]
    #speaker: Ambrose
    名单上有150户家庭，所剩食物已经不多。其中不少家庭有儿童、老人或无法劳动的病人。
    这些估算可信，但现在把钱分掉，并不能建立一个长期有效的救济制度。
    -> robin_hood_hub

+ #id:BridgeLedger [查看桥梁与卫兵账目]
    #speaker: Ambrose
    这座桥是三个村庄的主要通道。工程师警告说，若不及时维修，下一次洪水可能让它彻底无法通行。
    卫兵保护商人与旅客，但他们的工作也在维护罗宾汉所反对的税收秩序。
    -> robin_hood_hub

+ #id:SheriffOrder [阅读治安官的书面拒绝]
    #speaker: Ambrose
    治安官正在遵守合法预算。若擅自挪用，他可能被撤职或处罚。
    他的拒绝维护了制度秩序，却无法及时帮助救济名单上的家庭。
    -> robin_hood_hub

+ #id:SilverCoin [查看硬币]
    #speaker: Ambrose
    一枚硬币可以在眼前救济与既定公共工程之间作出选择，而不偏袒任何一方。
    但它无法解释，为什么落败的一方理应承担代价。
    -> robin_hood_hub

* [截下车队，分发税款]
    #speaker: Ambrose
    我会截下它。冬季危机就在眼前，这笔钱现在就能避免严重伤害。
    #speaker: The Judge
    你以凌驾法律、所有权和未来预算用途的方式满足紧急需求。灾民获得救济，而桥梁、卫兵与纳税者承担风险。
    #action: meta_robin_hood_seized
    -> robin_hood_end

* [让车队继续前进]
    #speaker: Ambrose
    我会让它通过。这笔钱原本就用于共同的基础设施与安全，夺走它只会把危机转移给其他社区。
    #speaker: The Judge
    你维护了合法分配与未来公共利益，也接受现有救济程序可能对一些家庭来得太迟。
    #action: meta_robin_hood_allowed
    -> robin_hood_end

* [掷硬币决定]
    #speaker: Ambrose
    正面：截下车队并分发税款。反面：让车队继续前进。
    #speaker: The Judge
    硬币给予双方同等机会。
    -> robin_hood_coin_result

=== robin_hood_coin_result ===
#speaker: Ambrose
（硬币落下，正面朝上。）
按照我选择的规则，罗宾汉应该截下车队。

#speaker: The Judge
你可以遵守程序，也可以拒绝它。无论如何，你的选择都会显露硬币真正扮演的角色。

* [遵守结果，截下车队]
    #speaker: Ambrose
    我会遵守结果。税款将被分发，我也接受桥梁与道路体系日后可能因此受损。
    #speaker: The Judge
    你遵守了不偏不倚的程序，并提供了眼前的救济。
    #action: meta_robin_hood_coin_seized
    -> robin_hood_end

* [拒绝结果，让车队继续]
    #speaker: Ambrose
    我不会让硬币改变公共资金的用途。车队会继续前进，而我接受救济延误可能造成的伤害。
    #speaker: The Judge
    你推翻了程序，以维护法律与公共规划。
    #action: meta_robin_hood_coin_overridden
    -> robin_hood_end

=== robin_hood_end ===
#speaker: The Judge
第三项决定已被记录。

#load_scene: Test_ending
-> END

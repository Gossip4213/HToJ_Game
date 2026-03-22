// ============================================================
// HToJ - 第一章：慈悲的毒药
// Chapter 1: The Merciful Poison
//
// 标签约定（Unity端按此解析）：
//   # speaker: [角色名]         → 切换发言人名牌
//   # sprite: [立绘key]         → 切换立绘/表情
//   # bg: [背景key]             → 切换背景
//   # bgm: [音乐key]            → 切换BGM
//   # sfx: [音效key]            → 单次音效
//   # anim: [动画key]           → 触发Unity动画事件
//   # book_text: [文本]         → 渲染《启迪之书》上的文字
// ============================================================


// ------------------------------------------------------------
// 跨章节全局变量（在主ink文件声明，此处仅为参考注释）
// VAR truth_score = 0          // 倾向真相/后果的累计次数
// VAR care_score  = 0          // 倾向保护/效用的累计次数
// VAR adams_affinity  = 0
// VAR kate_affinity   = 0
// VAR rumins_affinity = 0
// VAR miniel_affinity = 0
// ------------------------------------------------------------

// 本章局部变量
VAR ch1_choice = 0    // 1 = A（揭露真相），2 = B（隐瞒谎言）
VAR ch1_coin   = 1    // 硬币结果写死：1 = A（剧情锚点）


// ============================================================
=== chapter_1 ===
// ============================================================

-> ch1_arrival


// ------------------------------------------------------------
// 第一节：抵达维隆
// ------------------------------------------------------------
= ch1_arrival

# bg: overworld_road_dusk
# bgm: bgm_travel_somber

# speaker: Sera
# sprite: sera_neutral
神使Sera的声音从虚空传来，不带任何情绪。

"第一道裁决。矿镇维隆。"

# speaker: narrator
路上你们已经听说了这件事：一座本该死去的矿镇，突然降临了一个神迹。

# bg: mining_town_entrance
# bgm: bgm_town_eerie_calm

# speaker: narrator
走进维隆的第一感觉不是欢欣，而是……不对劲。

街道太安静了。安静得像一幅画。

# speaker: Adams
# sprite: adams_observing
"这地方不正常。"

# speaker: Kate
# sprite: kate_wary
"怎么了？街上的人在微笑，没有打架，没有哭声——难道不好吗？"

# speaker: Adams
"我见过幸福的人。幸福的人会吵架，会急着赶路，会因为小事骂骂咧咧。"

他环顾四周，压低声音。

# speaker: Adams
"这里的人是被什么东西麻痹了。"

-> ch1_church_exterior


// ------------------------------------------------------------
// 第二节：圣像与圣水
// ------------------------------------------------------------
= ch1_church_exterior

# bg: church_exterior_dusk
# bgm: bgm_hymn_distant_eerie

教堂前聚集着一群人，神情安详，眼神漫散，像是永远没睡醒的孩子。

圣母像就在门口——石雕的眼眶里，缓缓渗出暗红色的液体。

# speaker: Miniel
# sprite: miniel_uneasy
"……这不是圣血的颜色。圣血是金色的。"

# speaker: Rumins
# sprite: rumins_examining
"有机物渗出。水源性污染，大概率是某种真菌。"

他蹲下来，用手指蘸了一点干涸的红迹，凑近眼镜看了看，表情平静得令人不舒服。

# speaker: Rumins
"……有趣。我需要去地下看看水源。"

-> ch1_investigation


// ------------------------------------------------------------
// 第三节：地下调查——真相浮出水面
// ------------------------------------------------------------
= ch1_investigation

# bg: church_underground
# bgm: bgm_investigation_tense
# sfx: sfx_dripping_water

# speaker: narrator
调查花了整整两天。

烛光摇曳的地下水道里，陆明斯把最终的报告摆在了所有人面前，声音比地下室的温度还要低。

# speaker: Rumins
# sprite: rumins_neutral
"结论如下。"

"这种真菌极其罕见，我在文献里只看过理论描述。它会麻痹宿主的痛觉神经，并持续刺激多巴胺分泌。效果上，感染者会感到一切病痛消失，持续处于类似极度愉悦的状态。"

"代价是——"

他摘下眼镜，擦了擦镜片。

# speaker: Rumins
"彻底透支生命力。已经饮用圣水超过三周的人，最多还能再活三年，然后在睡眠中脑死亡。无征兆，无痛苦。"

# speaker: Miniel
# sprite: miniel_pale
"三……三年。"

# speaker: Kate
# sprite: kate_stricken
"那如果我们净化水源、解毒——他们接下来的三十年，是什么？"

沉默。

# speaker: Adams
# sprite: adams_grim
"是这里。"

# speaker: narrator
他举起蜡烛，烛光照亮了渗水的石墙、锈蚀的铁架、从顶缝压下来的永久黑暗。

# speaker: Adams
"病痛、饥饿、在矿道里弯着腰，每天和同伴争一块煤渣。清醒地，再活三十年。"

# speaker: Kate
# sprite: kate_conflicted
"……那我们到底在救他们，还是在害他们？"

没有人回答这个问题。

-> ch1_roundtable_start


// ------------------------------------------------------------
// 第四节：圆桌——辩论开始
// ------------------------------------------------------------
= ch1_roundtable_start

# bg: roundtable_room
# bgm: bgm_roundtable_debate
# sfx: sfx_candles_ambience

神使Sera将那枚旧金币放在桌子中央，没有立刻去碰它。

# speaker: Sera
# sprite: sera_neutral
"两个选项。请各位先表态，再由硬币裁决。"

"选项A：净化水源，烧毁圣像，告知镇民真相。"
"选项B：封锁消息，保留水源，让他们继续活在幸福中。"

她在椅子上坐直，如同法庭上的书记官。

# speaker: Sera
"亚当斯，你先说。"

-> ch1_adams_speaks


// ------------------------------------------------------------
// 四位NPC各自发言（顺序：Adams → Kate → Rumins → Miniel）
// 设计意图：玩家只旁观，不打断，充分吸收四种道德视角
// 最后形成2:2局面，再抛硬币，再交玩家决定
// ------------------------------------------------------------

= ch1_adams_speaks

# speaker: Adams
# sprite: adams_resolute
亚当斯点上一根烟，烟雾在烛光里慢慢散开。

"我支持A。告诉他们真相。"

他顿了一下，像是在整理措辞，但其实并不需要。

# speaker: Adams
"我知道这听起来很残忍。那就残忍吧。"

"我见过太多'善意的谎言'——从来没有一个有好结局的。谎言腐烂的时候，会把所有人一起炸掉。三年后这些人大批死去，毫无准备，他们的家人怎么办？"

# speaker: Adams
"更何况，让一个人活在无知的幸福里，本质上是把他当猪养。我不认为这是保护，我认为这是侮辱。"

他把烟灰弹进烛台旁的小碟子，态度已经结束。

# speaker: Sera
"凯特。"

-> ch1_kate_speaks

= ch1_kate_speaks

# speaker: Kate
# sprite: kate_conflicted
凯特没有立刻开口。

她的手放在桌面上，手指慢慢收紧，又松开。

# speaker: Kate
"我的信条是保护弱者。但我现在不知道，什么才是保护。"

"昨天我走过那条街——有个老矿工，第一次不用弯着腰走路。有个小孩，第一次不用哭着入睡。"

她的声音压低了，像是在对抗某种很重的东西。

# speaker: Kate
"我凭什么把那个从他们手里夺走？"

"……我选B。罪孽由我来担。我来保守这个秘密，同时去寻找可能的解药。我不相信这个世界上没有解药。"

# speaker: Sera
"陆明斯。"

-> ch1_rumins_speaks

= ch1_rumins_speaks

# speaker: Rumins
# sprite: rumins_detached
陆明斯把报告叠整齐，重新戴上眼镜，语气如同在讨论一道数学题。

# speaker: Rumins
"我也倾向于B。但理由和凯特不同。"

"首先，这个镇子的历史死亡率本就畸高。就算没有毒菌，这群人中的大多数，也未必能活过三年。这是客观数字，不是冷血。"

# speaker: Rumins
"其次，他们目前的'真相'——神迹治愈了他们——在主观层面对他们是真实的。打破这个认知框架，是粗暴的外部干涉。谁给了我们这个权利？"

他推了推眼镜。

# speaker: Rumins
"况且，若处理不当，这件事会把我们全部牵扯进去。为了一个本就命不久矣的地方，没有必要。"

# speaker: Sera
"明伊尔。"

-> ch1_miniel_speaks

= ch1_miniel_speaks

# speaker: Miniel
# sprite: miniel_torn
明伊尔盯着桌上的硬币看了很久。

# speaker: Miniel
"作为教会的人……告诉他们，那个圣像只是一块石头，里面没有神——"

她闭上眼睛。

# speaker: Miniel
"对我而言，这不比判他们死刑容易。"

一段沉默。

# speaker: Miniel
"但我想起了一句话，是我刚入教时背诵的：'神明喜爱诚实，更胜于狂热。'"

她睁开眼，神情已经稳定下来。

# speaker: Miniel
"……我支持A。告诉他们真相。哪怕这会摧毁他们的信仰。至少，那是属于他们自己的、真实的悲伤。"

-> ch1_coin_flip


// ------------------------------------------------------------
// 第五节：掷硬币（结果写死为A，但这只是参考，不是命令）
// ------------------------------------------------------------
= ch1_coin_flip

# speaker: Sera
# sprite: sera_neutral
"两票A，两票B。"

她拿起硬币，放在拇指指甲上。

"按照惯例。"

# sfx: sfx_coin_flip
# anim: coin_toss_ui

金属在烛光里旋转，两面交替闪过——

# sfx: sfx_coin_land
# anim: coin_result_A

# speaker: Sera
"A。"

硬币正面朝上，静止在桌面。

// NPC对硬币结果的反应——各自的性格在此体现
# speaker: Adams
# sprite: adams_neutral
"意料之中。"

# speaker: Kate
# sprite: kate_reluctant
"Sera。我必须声明——我不认为这个结果是正确的。"

# speaker: Rumins
# sprite: rumins_shrug
"嗯。当作参考意见就好。"

# speaker: Miniel
# sprite: miniel_resolute
"天意如此。"

神使Sera将目光从硬币移向你，没有催促，也没有表情。

# speaker: Sera
# sprite: sera_watching
"硬币只是惯例，不是命令。裁决权，在你。"

"维隆镇的人，你将如何处置？"

-> ch1_player_choice


// ------------------------------------------------------------
// 第六节：玩家决策——2:2破局
// ------------------------------------------------------------
= ch1_player_choice

* [A：净化水源，告知真相。让他们用清醒的眼睛，面对剩下的时间。]
    -> ch1_choose_A

* [B：封锁消息，保留水源。让他们在幸福中，走完这三年。]
    -> ch1_choose_B


// ------------------------------------------------------------
// 选择A：揭露真相
// ------------------------------------------------------------
= ch1_choose_A

~ ch1_choice = 1
// ~ truth_score++   // 主文件中追踪

# speaker: narrator
你做出了决定。

# speaker: Adams
# sprite: adams_nod
"好。"

他掐灭烟蒂，站起来。

# speaker: Adams
"我来主导现场。这种话，需要一个不会心软的人来说。"

# speaker: Kate
# sprite: kate_pained
"……我会在场。"

她停顿了一秒。

# speaker: Kate
"但那些话，不会从我嘴里出来。"

# speaker: Rumins
# sprite: rumins_indifferent
"我留在地下实验室，整理毒菌的样本数据。万一将来有用。"

-> ch1_consequence_A

// ------------------------------------------------------------
// 选择B：隐瞒真相
// ------------------------------------------------------------
= ch1_choose_B

~ ch1_choice = 2
// ~ care_score++    // 主文件中追踪

# speaker: narrator
你摇了摇头。

# speaker: Kate
# sprite: kate_quietly_relieved
"……谢谢你。"

她说得很轻，像是怕被什么东西听见。

# speaker: Adams
# sprite: adams_smoking
亚当斯沉默地点上另一根烟，没有说话。

-> ch1_consequence_B


// ------------------------------------------------------------
// 第七节A：揭露真相后
// ------------------------------------------------------------
= ch1_consequence_A

# bg: mining_town_plaza
# bgm: bgm_aftermath_heavy
# sfx: sfx_crowd_murmur

你们在教堂广场召集了全镇的人。

阳光很好。他们站在那里，脸上还带着那种松弛的、蒙昧的幸福。

亚当斯开了口，说完，广场沉默了大约三秒钟。

然后是第一声哭泣。

# sfx: sfx_crowd_breaking

// 这里可以插入一段无台词的立绘动态展示
# anim: crowd_reaction_sequence

# speaker: Miniel
# sprite: miniel_praying
她低着头，用几乎没人能听见的声音，在心里为那些哭泣的人祈祷。

# speaker: Kate
# sprite: kate_watching_crowd
凯特站在人群边缘，手按在剑柄上，始终没有拔出来。

# speaker: narrator
不是所有人都立刻相信。有人骂你们是骗子，有人跪倒在地，有人什么都没说，转身走进了矿道。

// 叙述者的最后一段话——章节的道德余韵
# speaker: narrator
三年后，这里的人会开始死去。

但他们知道了。

他们知道，在这三年里，还有什么值得他们放下幻觉，去亲眼看见。

-> ch1_chapter_end


// ------------------------------------------------------------
// 第七节B：隐瞒真相后
// ------------------------------------------------------------
= ch1_consequence_B

# bg: mining_town_entrance_night
# bgm: bgm_aftermath_quiet

你们在夜里封锁了地下通道。净化方案被搁置。

神使Sera在离开前，在记录本上写了一行字，合上，没有给任何人看。

# speaker: Rumins
# sprite: rumins_packing
"理性的选择。他们本来就活不久，至少现在活得开心。"

他收起手稿，语气里没有讽刺，也没有安慰，只是一个中性的结论。

# speaker: Adams
# sprite: adams_smoking_night
亚当斯坐在镇子入口的石阶上，点上了今天的第四根烟。

远处的街道上，有人在笑。

他没有动，也没有说话，就那样坐着。

# speaker: Kate
# sprite: kate_resolute_night
"我会找到解药的。"

她说。声音里没有疑问。

# speaker: narrator
你不知道她是否找得到。

毒菌依然在地下生长着。圣像依然在黑暗里流下红色的泪水。

镇子里的人依然幸福，幸福得不知道自己在死去。

-> ch1_chapter_end


// ------------------------------------------------------------
// 章节结尾：回到圆桌，《启迪之书》显示
// ------------------------------------------------------------
= ch1_chapter_end

# bg: roundtable_room
# bgm: bgm_roundtable_end_sting

Sera收回硬币，放进那个旧皮革小袋里。

# speaker: Sera
# sprite: sera_neutral
"第一道裁决，已记录。"

// 《启迪之书》根据玩家选择显示不同文字
// Unity端读取 ch1_choice 变量后渲染对应文本
{ ch1_choice == 1:
    # book_text: "真实是人之所以为人的底线。虚假的幸福，是猪圈。"
- else:
    # book_text: "若真相只带来纯粹的痛苦，它还剩什么价值？"
}

# anim: book_page_turn

-> END

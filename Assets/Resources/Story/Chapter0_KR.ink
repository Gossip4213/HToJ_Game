-> trolley_hub

=== trolley_hub ===
#speaker: The Judge
#bgm: Dilemma
앰브로즈, 아래의 선로를 보세요.
제어를 잃은 전차가 본선 위의 작업자 다섯 명을 향해 달리고 있습니다. 그들은 제때 선로를 벗어날 수 없습니다.
당신 곁에는 전차를 지선으로 돌리는 레버가 있습니다. 하지만 그 지선에는 작업자 한 명이 서 있습니다.
전차를 멈출 수는 없습니다. 레버를 당기면 진로가 바뀌고, 그대로 두면 현재 경로를 유지합니다.
어떻게 해야 할까요?

+ #id:Lever [레버를 살펴본다]
    #speaker: Ambrose
    구조는 단순하다. 한 번 움직이면 누가 위험을 떠안는지가 달라진다.
    당기면 내가 의도적으로 진로를 바꾸는 셈이고, 당기지 않으면 이미 진행 중인 흐름을 그대로 두는 셈이다.
    -> trolley_hub

+ #id:TheFive [다섯 명의 작업자를 본다]
    #speaker: Ambrose
    다섯 명은 본선에 갇혀 있다. 지금 이 순간, 그들 각자가 살고자 하는 권리는 지선의 한 사람과 같다.
    -> trolley_hub

+ #id:TheOne [지선의 작업자를 본다]
    #speaker: Ambrose
    한 사람이 지선에 서 있다. 현재로서는 전차의 경로 밖에 있다.
    진로를 바꾸면 다섯 명을 살리기 위해 내가 그 사람을 위험 속에 놓는 것이다.
    -> trolley_hub

+ #id:SilverCoin [동전을 살펴본다]
    #speaker: Ambrose
    아주 평범한 동전처럼 보인다.
    -> trolley_hub

* [레버를 당겨 전차를 돌린다]
    #speaker: Ambrose
    진로를 바꾸겠다. 한 명은 죽겠지만, 그대로 두면 다섯 명이 죽는다.
    #speaker: The Judge
    당신은 개입하여 사망자 수를 줄였고, 한 사람의 죽음을 자신의 행동이 낳은 결과로 만들었습니다.
    -> end_dilemma

* [레버를 당기지 않는다]
    #speaker: Ambrose
    진로를 바꾸지 않겠다. 다섯 명은 계속 위험하지만, 지선의 한 사람을 내 손으로 전차 앞에 놓지는 않겠다.
    #speaker: The Judge
    당신은 기존 경로를 유지하고 한 사람을 다른 이들을 구하기 위한 수단으로 삼지 않았습니다. 그러나 막을 수도 있었던 다섯 명의 죽음을 허용하게 됩니다.
    -> end_dilemma

* [동전으로 결정한다]
    #speaker: Ambrose
    앞면이면 레버를 당긴다. 뒷면이면 그대로 둔다.
    #speaker: The Judge
    그 절차는 어느 쪽도 편들지 않습니다. 하지만 그 절차를 택하는 것 자체가 도덕적 결정입니다.
    -> coin_result

=== coin_result ===
#speaker: Ambrose
(동전이 공중에서 돌다가 금속 덮개에 부딪혀 멈춘다.)
뒷면이다. 내가 정한 규칙대로라면 레버를 당기지 않아야 한다.

#speaker: The Judge
결과가 당신을 강제하는 것은 아닙니다. 그것은 당신이 따르기로 한 형식으로 판단을 되돌려 줄 뿐입니다.

* [결과를 따르고 레버를 당기지 않는다]
    #speaker: Ambrose
    결과를 알기 전에 이 절차를 택했다. 다섯 명이 죽더라도 따르겠다.
    #speaker: The Judge
    알겠습니다.
    -> end_dilemma

* [결과를 거부하고 레버를 당긴다]
    #speaker: Ambrose
    어느 쪽도 편들지 않으려고 우연에 맡겼지만, 이 결과는 받아들일 수 없다. 레버를 당기겠다.
    #speaker: The Judge
    알겠습니다.
    -> end_dilemma

=== end_dilemma ===
#speaker: The Judge
결정이 내려졌습니다.

#speaker: Ambrose
(바퀴와 금속이 마찰하는 소리가 한꺼번에 되돌아온다.)

#load_scene: Chapter1_Test
-> END

-> trolley_hub

=== trolley_hub ===
#speaker: The Judge
#bgm: Dilemma
アンブローズ、下の線路を見てください。
制御を失ったトロッコが、本線上の五人の作業員へ向かっています。彼らはもう避難できません。
あなたのそばには分岐器のレバーがあります。引けばトロッコは側線へ移りますが、そこには一人の作業員が立っています。
トロッコを止めることはできません。レバーを引けば進路が変わり、何もしなければ現在の進路を保ちます。
どうするべきでしょうか。

+ #id:Lever [レバーを調べる]
    #speaker: Ambrose
    仕組みは単純だ。一度動かすだけで、誰が危険にさらされるかが変わる。
    引けば、私が意図的に進路を変えたことになる。引かなければ、既に始まっている流れをそのまま進ませることになる。
    -> trolley_hub

+ #id:TheFive [五人の作業員を見る]
    #speaker: Ambrose
    五人は本線上に閉じ込められている。今この瞬間、彼ら一人ひとりが生きることを求める権利は、側線の一人と同じだ。
    -> trolley_hub

+ #id:TheOne [側線の作業員を見る]
    #speaker: Ambrose
    一人の作業員が側線に立っている。今のままなら、彼はトロッコの進路上にはいない。
    進路を変えれば、五人を救うために、私が彼を危険へ置くことになる。
    -> trolley_hub

+ #id:SilverCoin [コインを見る]
    #speaker: Ambrose
    ごく普通のコインに見える。
    -> trolley_hub

* [レバーを引き、トロッコを側線へ移す]
    #speaker: Ambrose
    進路を変える。一人は死ぬが、このままでは五人が死ぬ。
    #speaker: The Judge
    あなたは介入によって死者を減らし、その一人の死を自らの行為の結果としました。
    -> end_dilemma

* [レバーを引かない]
    #speaker: Ambrose
    進路は変えない。五人は危険なままだが、側線の一人を私の手でトロッコの前へ置くことはしない。
    #speaker: The Judge
    あなたは既存の進路を保ち、一人を他者を救うための手段にはしませんでした。しかし、防げたかもしれない五人の死を許すことになります。
    -> end_dilemma

* [コインで決める]
    #speaker: Ambrose
    表ならレバーを引く。裏なら何もしない。
    #speaker: The Judge
    その手続きはどちらにも肩入れしません。しかし、その手続きを選ぶこと自体が道徳的な決定です。
    -> coin_result

=== coin_result ===
#speaker: Ambrose
（コインは空中で回転し、金属の覆いに当たって止まった。）
裏だ。自分で定めた規則に従うなら、レバーは引かない。

#speaker: The Judge
結果はあなたを強制しません。それは、あなたが従うと決めた形式で、判断を返しているだけです。

* [結果に従い、レバーを引かない]
    #speaker: Ambrose
    結果を知る前に、この手続きを選んだ。五人が死ぬとしても、私は従う。
    #speaker: The Judge
    分かりました。
    -> end_dilemma

* [結果を拒み、レバーを引く]
    #speaker: Ambrose
    どちらもひいきしないために偶然へ委ねた。だが、この結果は受け入れられない。レバーを引く。
    #speaker: The Judge
    分かりました。
    -> end_dilemma

=== end_dilemma ===
#speaker: The Judge
決定は下されました。

#speaker: Ambrose
（車輪と金属の音が、一度に戻ってくる。）

#load_scene: Chapter1_Test
-> END

# フォス
## 概要
主人公であるキノコが敵キャラクターに寄生し、その能力を利用することでゴールまで進むゲームです。
敵は石と草の二種類が存在し、どちらも他の同種を自分の身体の上に積み上げることで攻撃を行います。
そのため、主人公も寄生した後は同様に仲間を集めてギミックを解くことになります。
石は5体まで積み重なって高くなり上にキャラクターを投げる能力、草は2体でペアを作りツタを伸ばして水平に空中を移動する能力を持ちます。
## 使用技術
C#
Unity
## 担当箇所
メインプログラマーとして、UIやロード処理、スコア処理を除いた処理のほとんどを作成しました。
### Assets/Scripts内
C#スクリプトです。大体は自作ですが、数が多いため特に工夫したもののみについて紹介します。
#### AnimeStarter
ギミックが作動した際にアニメーションの再生速度を1.0にするスクリプトです。
扉やリフトに使い、この際カメラを動かす演出を行うためアニメーションの再生タイミングを変えられるようになっています。
#### GoalUnit
ギミックを作動させる鐘のスクリプトです。
Ring()によって鐘を鳴らす処理を行います。1フレーム中に複数回行われた場合アニメーションや音がおかしくなるので、そのようにはならないようにしました。
#### LightMover
収集アイテムに対しプレイヤーが接近したらプレイヤーに吸い込まれるように動くスクリプトです。
元々は触れると取れるようにしていたのですが、思っていたよりも取りにくかったので作成しました。
#### Monster
モンスターの行動を処理するスクリプトです。
NavmeshAgentによる移動とRigidBodyによる移動を使い分けることで石は石らしく、草は草らしく動くようにしました。
#### MoveGround
移動するオブジェクトに付け、プレイヤーを足場の子にすることで足場に追従するようにします。
キャラクターの形がおかしくなるバグが発生してしまった際、その原因が足場から離れた際にScaleが足場のlocalScaleのままになってしまっていたからだということが分かったためそれを解消する処理を追加しました。
#### PlayerController
プレイヤーの行動を処理するスクリプトです。
チーム内の他プログラマーが作成した、入力を受け取るためのPlayerMoverというクラスを継承しています。
寄生したキャラクターとの対応に関する部分を頑張りました。
#### StepSE
キャラクターの足音を鳴らすスクリプトです。
一定時間おきにランダムなピッチで足音を鳴らすことで機械的な印象を与えないようにしました。
isMovingを切り替えることで空中などでは足音を鳴らさないようにできます。
#### ThreeGoalUnit
鐘を鳴らしたときの判定を行うスクリプトです。
rings[]に含まれる一つあるいは複数の鐘が一定時間内に全て鳴らされた場合、goal[]に含まれるギミックの有効化を切り替えます。
開発初期に作ったもので、アニメーションが設定されてない扉などを消すために作ったもので、AnimeStarterはこれに合わせてStartで初期化してUpdateで戻すという処理を行う形になりました。

### Assets/Prefabs内
Prefabです。モデルとアニメーション、エフェクトは全て同チームの他メンバーが作成したものです。
#### Player
プレイヤーのPrefabです。
敵キャラクターを自分の上に乗せる際、本当に敵のオブジェクトを乗せるのではなくあらかじめ用意してある見かけ上のキャラクターの表示を切り替えるという処理をしているため、このPrefabの中にCube0~5、Grs0~1という敵と同じ外見のゲームオブジェクトを作成しました。
石の投げる能力や草の伸びる能力を使用する際に表示される予測線やツタもこの中に入っています。
#### StoneとGrass
どちらも敵キャラクターのPrefabです。
Stoneは石の敵、Grassは草の敵です。
### Assets/Prefabs/EnvironmentPrefabs/Gimmick内
#### DestroyableWall
破壊できる壁のPrefabです。
アニメーションをそのまま再生するとガレキがその場に残って邪魔だし、急に消えるのも違和感があるのでBoxColliderのisTriggerをtrueに、RigidbodyのisKinematicをfalseにすることで地面を貫通して落下していくようにしました。
#### elevatorVariant
プレイヤーが乗ることのできるリフトのPrefabです。
動作時に
#### SideViewCube
範囲内に入るとカメラの視点が切り替えるギミックです。
Priorityの高いCinemachineVirtualCameraと見えない壁のPlaneを有効化することでカメラが切り替わりキャラクターが前後にある程度以上移動しないようにしました。
#### swamp
沼です。NavigationのNavigationAreaをSwampという専用の物にすることで、石が沼に嵌まっているかの判定を行っています。
沼にプレイヤーが嵌まっているときに音がくぐもっているようにするためにAudioReverbZoneを追加しました。
#### Zhong(2)Variant
ギミックを作動させるための鐘のPrefabです。
[Tank](https://github.com/sny0/Tank)のリメイク。  
2Dの戦車ゲーム。  
作り直すにあたり、ゲームAIやグラフィックスなどに拘って制作した。

# 遊び方

[ここから、遊べます！](https://unityroom.com/games/tank_sny)

## 操作方法
| アクション | 入力 |
| --- | --- |
| 砲台の回転 | カーソル |
| 移動 | AWSD / 矢印キー |
| 弾の発射 | 左クリック |

# 拘ったポイント
[ここを参照されたし](https://sny0.github.io/my-docusaurus/docs/tank2)

# 作成環境
- ゲームエンジン：Unity(ver. 2021.3.16f1)
- 言語：C#, ShaderLab

# 参考にしたもの
- ゲームAI技術入門 広大な人工知能の世界を体形的に学ぶ, 三宅 陽一郎, 技術評論社  
ゲームAIについて

- [楽しい！Unityシェーダーお絵描き入門！, Seita Matsushita](https://docs.google.com/presentation/d/1NMhx4HWuNZsjNRRlaFOu2ysjo04NgcpFlEhzodE8Rlg/edit#slide=id.g35dce8d1c5_0_28)  
ShaderLabやshaderを書くテクニックについて

# ファイルの簡単な説明
## C#スクリプト
/Assets/Scripts にある。


- GameManager.cs  
メインゲームの進行/管理。

- TittleManager.cs  
タイトル画面の管理。  
staticなゲーム全体の情報を管理する「GameData」クラスも含む。

- UIManager.cs  
メインゲームのゲームスタート時のカウントダウンUIの管理。

- MapManager.cs  
メインゲームでのフィールドの情報（障害物や危険度）の管理。  
※フィールドを格子状に区切り、それぞれのマスの情報を持っている。この情報を利用して敵機が移動を行う。

- AudioManager.cs  
音の管理。

- ResultManager.cs  
リザルト画面の管理。

- Player.cs  
自機。

- Enemy.cs  
敵機。  
※現在は利用していない。リファクタリングや機能の追加を行い現在は以下の、Tank.cs～MobileEnemyBrain.csを使用している。

- Tank.cs  
機体。TankBrainインスタンスとTankBodyインスタンスを保持し、TankBrainインスタンスで次の行動を考えさせ、TankBodyインスタンスで動作させる。  
抽象クラスであり、Enemy2クラスがこれを継承している。

- TankBrain.cs  
機体の脳に当たる。現在の状況から次の行動（移動方向、砲台の回転角度、弾を発射するか）を考える。  
抽象クラスであり、EnemyBrainがこれを継承している。

- TakBody.cs  
機体の体に当たる。Tankから指示を受け取り、動作の実行（移動、砲台の回転、弾の発射）を行う。

- StealthTankBody.cs  
一定時間透明になる。TankBodyクラスを継承する。白色の敵はこれを保持する。

- Enemy2.cs  
敵機。Tankクラスを継承する。

- EnemyBrain.cs  
敵機の脳。TankBrainクラスを継承する。  
ステイトマシンによるゲームAIの実装を行っている。基本的な実装やステイト毎の関数を保持。  
抽象クラスであり、ImmobileEnemyBrainクラスやMobileEnemyクラスがこれを継承している。

- ImmobileEnemyBrain.cs  
動かない敵機の脳。EnemyBrainクラスを継承する。
EnemyBrainクラスの関数をオーバーライドすることで具体的な思考を実装。緑色の敵がこれを保持する。


- MobileEnemyBrain.cs  
動くの敵機の脳。EnemyBrainクラスを継承する。
EnemyBrainクラスの関数をオーバーライドすることで具体的な思考を実装。茶色の敵、白色の敵、紫色の敵がこれを保持する。

- Bullet.cs  
機体が発射する弾。

- Cursol.cs  
自機から出ている照準。

- Bikkuri.cs  
敵機が自機を発見したときに出る！マーク

- Explosion.cs  
機体が破壊されたときに出る爆発。

- ImageSwitcher.cs  
タイトル画面の「How to Play」における操作の画像の制御。

- TitlePostEffect.cs  
タイトル画面でのポストエフェクトを行う。

- ResultPostEffect.cs  
リザルト画面でのポストエフェクトを行う。

- PostEffect.cs  
メインゲームでのポストエフェクトを行う。

- BulletPostEffect  
「Bullet」のみを撮るカメラにアタッチし、ダブルバッファリングにより弾の軌道を保持するRenderTextureを描く。

## ShaderLabスクリプト
/Assets/Shader にある。


- AfterImageBuffer.shader  
弾の残像を描く。  
メインゲームで使用。

- AwakedEnemy.shader  
覚醒した敵のスプライトに使用する。  
メインゲームで使用。

- BrownTubeShader.shader  
テクスチャに樽型歪曲収差を加え、縁に黒色を追加する。  
タイトル画面、メインゲーム、リザルト画面で使用。

- BulletAfterImageMix.shader  
プレイ画面と弾の残像テクスチャをミックスする。  
メインゲームで使用。

- Distort.shader  
テクスチャを歪ませる。  
エクストラモードのタイトル画面、リザルト画面で使用。

- Initialize.shader  
テクスチャの画素値をすべて(R, G, B, A) = (0, 0, 0, 0)にする。
弾の残像テクスチャを初期化するときに使用。

- Mix.shader  
2つのテクスチャを混ぜる。
エクストラモードのタイトル画面で使用。

- ScanlinesAndStripes.shader  
ブラウン管テレビの縞や走査線を表示。  
タイトル画面、メインゲーム、リザルト画面で使用。

- Sonar.shader  
敵が探索中のときに探索している様子を表現する波を表示。  
メインゲームで使用。

- TVNoise.shader  
テレビの砂嵐を描く。  
エクストラモードのタイトル画面で使用。
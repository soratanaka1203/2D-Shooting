using Cysharp.Threading.Tasks;
using MyNameSpace;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public int maxHealth = 400;                 // ボスの最大体力
    private int currentHealth;                  // ボスの現在の体力
    public float moveSpeed = 2.0f;              // ボスの移動速度

    [SerializeField] private EnemyBulletPool enemyBulletPool;  // 弾のプール
    public Transform firePoint;                 // 弾を発射する位置
    public float bulletSpeed = 20f;             // 弾の速度
    public float phaseChangeHealthThreshold = 0.5f;  // フェーズ変更の体力割合 (50%)

    [SerializeField] private BulletPool playerBulletPool;     // プレイヤー弾のプール
    [SerializeField] private EffectPool effectPool;           // エフェクトプール

    [SerializeField] private TextMeshProUGUI scoreText;       // スコア表示用テキスト
    [SerializeField] private GameObject gameClearUI;          // ゲームクリアのUI
    [SerializeField] private TextMeshProUGUI resultGameClearText;//リザルトテキスト
    [SerializeField] private Slider healthBar;                //ボスの体力バー
    [SerializeField] private GameObject enemySpawner;         // 敵スポナーの参照

    [SerializeField] private AudioPlayer audioPlayer;
    [SerializeField] private AudioClip audioClip;
    public float volume;

    private bool isDead = false;                // ボスが死亡しているかのフラグ
    private bool isPhaseChanged = false;        // フェーズが変更されたかのフラグ
    private int currentPhase = 1;               // 現在のフェーズ

    void Start()
    {
        // 各オブジェクトやコンポーネントの取得
        if (enemyBulletPool == null) enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<EnemyBulletPool>();
        if (effectPool == null) effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();
        if (playerBulletPool == null) playerBulletPool = GameObject.Find("PlayerBulletPool").GetComponentInChildren<BulletPool>();
        if (scoreText == null) scoreText = GameObject.Find("scoreText").GetComponentInChildren<TextMeshProUGUI>();
        if (gameClearUI == null) gameClearUI = GameObject.Find("GameClearUI");
        if (enemySpawner == null) enemySpawner = GameObject.Find("EnemySpawner");

        enemySpawner.SetActive(false); // 敵スポナーを非アクティブ化
        healthBar.gameObject.SetActive(true);//体力バーを表示
        healthBar.value = maxHealth;  // 現在の体力を初期化
        currentHealth = maxHealth;     

        InvokeRepeating("ExecuteAttackPattern", 1f, 2.3f); // 2.3秒ごとに攻撃パターンを実行
    }

    void Update()
    {
        if (!isDead) // ボスが生存している場合のみ更新
        {
            Move();  // ボスの移動処理

            // 体力が閾値を下回り、まだフェーズ変更していない場合
            if (currentHealth <= maxHealth * phaseChangeHealthThreshold && !isPhaseChanged)
            {
                ChangePhase(2);           // フェーズ変更
                enemySpawner?.SetActive(true);  // 敵スポナーをアクティブ化
            }
        }
    }

    // プレイヤーの弾との衝突を検知し、ダメージ処理を行う
    private async void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet") // プレイヤー弾との衝突判定
        {
            TakeDamage(1, collision.gameObject);  // ダメージを受ける
            UpdateScore(200); // スコアを更新
        }
    }

    // ボスの移動パターン (例: 左右にスムーズに移動)
    void Move()
    {
        float moveX = Mathf.Sin(Time.time * moveSpeed) * 30.0f; // 左右にスムーズに移動
        transform.position = new Vector3(moveX, transform.position.y, transform.position.z);
    }

    // 攻撃パターンをランダムに実行する
    void ExecuteAttackPattern()
    {
        switch (UnityEngine.Random.Range(1,4))
        {
            case 1:
                FireStraight(); // 直線攻撃
                Debug.Log("直接攻撃");
                break;
            case 2:
                FireFanPattern(); // 扇状の攻撃
                Debug.Log("扇状の攻撃");
                break;
            case 3:
                StartFireCircularPattern(); //乱射
                Debug.Log("乱射攻撃");
                break;
            default:
                FireStraight();
                break;
        }
    }

    // 直線に弾を発射するパターン
    async void FireStraight()
    {
        int bulletCount = 4;
        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = enemyBulletPool.GetEnemyBullet(); // 弾をプールから取得
            bullet.transform.position = firePoint.position;
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -bulletSpeed); // 下向きに発射
            enemyBulletPool.ReleaseEnemyBullet(bullet, 3f); // 一定時間後にプールに戻す
            await UniTask.Delay(TimeSpan.FromSeconds(0.1));  // 0.1秒間待機
        }
    }

    // 扇状に複数の弾を発射するパターン
    void FireFanPattern()
    {
        int bulletsCount = 6;                // 発射する弾数
        float angleStep = 45f / (bulletsCount - 1); // 弾の角度ステップ
        float startAngle = -18f;            // 開始角度

        for (int i = 0; i < bulletsCount; i++)
        {
            GameObject bullet = enemyBulletPool.GetEnemyBullet();
            bullet.transform.position = firePoint.position;

            // 弾の発射方向を設定
            float angle = startAngle + angleStep * i;
            Vector2 direction = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), -Mathf.Cos(angle * Mathf.Deg2Rad));
            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed / 2;

            enemyBulletPool.ReleaseEnemyBullet(bullet, 3f);
        }
    }

    //間隔をあける
    void StartFireCircularPattern()
    {
        int bulletCount = 20;
        float angleStep = -180f / bulletCount;

        // 発射するタイミングをずらして弾を出す
        for (int i = 0; i < bulletCount; i++)
        {
            float delay = i * 0.1f; // 0.1秒ずつ遅らせて発射
            Invoke("FireBullet", delay); // FireBulletメソッドを遅延させて呼び出す
        }
    }
    //乱射する
    void FireBullet()
    {
        // 弾をプールから取得
        GameObject bullet = enemyBulletPool.GetEnemyBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;

            // 円周上の位置を計算
            float angle = UnityEngine.Random.Range(0f, -180f);
            float angleInRadians = Mathf.Deg2Rad * angle;

            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = direction * bulletSpeed / 2;

            // 弾をプールに戻すタイミング（例：3秒後）
            enemyBulletPool.ReleaseEnemyBullet(bullet, 3f);
        }
    }

    // ボスのフェーズが変更されたときの処理
    void ChangePhase(int newPhase)
    {
        currentPhase = newPhase;  // フェーズを更新
        isPhaseChanged = true;    // フェーズ変更フラグを設定
        Debug.Log("ボスのフェーズが変更されました！");

        // フェーズ変更時の強化処理
        moveSpeed += 1.0f;  // 移動速度を速くする
        bulletSpeed += 5f;  // 弾の速度を速くする

        // 発射間隔を更新
        CancelInvoke(); // 既存のInvokeをキャンセル
        InvokeRepeating("ExecuteAttackPattern", 1f, Mathf.Max(0.5f, 3f - currentPhase)); // 新しい間隔で発射
    }

    // ボスがダメージを受けた際の処理
    public void TakeDamage(int damageAmount, GameObject gameObject)
    {
        currentHealth -= damageAmount; // 現在の体力を減少
        PlayEffect(gameObject.transform, 0.2f).Forget(); // ヒットエフェクトを再生
        UpdateHealthBar();//体力バーを更新
        audioPlayer.PlayAudio(audioClip, volume);

        if (currentHealth <= 0 && !isDead) // 体力が0以下になったら死亡処理
        {
            Die();
        }
    }

    // ボスが死亡したときの処理
    void Die()
    {
        isDead = true;
        // 攻撃パターンを停止
        CancelInvoke("ExecuteAttackPattern");
        for (int i = 0; i < 15; i++)
        {
            // エフェクトをランダム位置に表示
            Transform tr = new GameObject().transform;
            tr.position = gameObject.transform.position + new Vector3(UnityEngine.Random.Range(-10, 11), UnityEngine.Random.Range(-10, 11), 0);
            PlayEffect(tr, 2f).Forget();
            audioPlayer.PlayAudio(audioClip, volume);
        }
        enemySpawner.SetActive(false);
        gameObject.SetActive(false); // ボスを非アクティブ化
        UpdateScore(100000);         // スコアを加算
        ScoreManager.Instance.SetDisplayScore(resultGameClearText);//結果を表示
        gameClearUI.SetActive(true); // ゲームクリアのUIを表示

    }

    // ヒットエフェクトを表示
    async UniTask PlayEffect(Transform effectTransform, float delay)
    {
        GameObject effect = effectPool.GetEffect(); // エフェクトを取得
        effect.transform.position = effectTransform.position; // エフェクト位置設定

        // 一定時間後にエフェクトをプールに戻す
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        effectPool.ReleaseEffect(effect);
    }

    // スコアの更新
    private void UpdateScore(int amount)
    {
        ScoreManager.Instance.AddScore(amount);         // スコアを加算
        ScoreManager.Instance.SetDisplayScore(scoreText); // スコアテキストを更新
    }

    //体力バーの更新
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
}

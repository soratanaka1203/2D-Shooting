using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using MyNameSpace;

public class BossController : MonoBehaviour
{
    // ボスの最大体力
    public int maxHealth = 400;
    private int currentHealth;          // 現在の体力

    // ボスの移動速度
    public float moveSpeed = 2.0f;

    // ボスが発射する弾のプレハブ
    [SerializeField] private BulletPool enemyBulletPool;
    public Transform firePoint;         // 弾を発射する位置
    public float fireInterval = 1.5f;   // 弾を発射する間隔
    public float bulletSpeed = 20f;     // 弾の速度
    public float phaseChangeHealthThreshold = 0.5f;  // フェーズ変更の体力割合 (50%)

    //プレイヤーの弾のプレハブ
    [SerializeField] private BulletPool playerBulletPool;

    // エフェクトプール
    [SerializeField] EffectPool effectPool;

    // UI要素
    [SerializeField] TextMeshProUGUI scoreText;   // スコア表示用テキスト
    [SerializeField] GameObject gameClearUI;       // ゲームクリアのUI

    [SerializeField] GameObject EnemySpawner;       // 敵スポナーの参照

    private bool isDead = false;                    // ボスが死亡しているかのフラグ
    private bool isPhaseChanged = false;            // フェーズが変更されたかのフラグ

    void Start()
    {
        // 各コンポーネントの初期設定
        if (enemyBulletPool == null)
        {
            enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<BulletPool>();
        }
        if (effectPool == null)
        {
            effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();
        }
        if (playerBulletPool == null)
        {
            playerBulletPool = GameObject.Find("PlayerBulletPool").GetComponentInChildren<BulletPool>();
        }
        if (scoreText == null)
        {
            scoreText = GameObject.Find("scoreText").GetComponentInChildren<TextMeshProUGUI>();
        }
        if (gameClearUI == null)
        {
            gameClearUI = GameObject.Find("GameClearUI");
        }
        if (EnemySpawner == null)
        {
            EnemySpawner = GameObject.Find("EnemySpawner");
        }

        // 敵スポナーを非アクティブにする
        EnemySpawner.SetActive(false);
        currentHealth = maxHealth; // 現在の体力を最大体力で初期化
        InvokeRepeating("Fire", fireInterval, fireInterval); // 定期的に弾を発射
    }

    void Update()
    {
        // ボスが死亡していない場合の更新処理
        if (!isDead)
        {
            Move(); // ボスの移動を更新

            // 体力が50%以下になったらフェーズを変更
            if (currentHealth <= maxHealth * phaseChangeHealthThreshold && !isPhaseChanged)
            {
                ChangePhase(); // フェーズを変更
                EnemySpawner?.SetActive(true); // 敵スポナーをアクティブにする
            }
        }
    }

    // 弾が当たったらダメージを与える処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet")) // プレイヤーの弾と衝突した場合
        {
            TakeDamage(1, collision.gameObject); // ダメージを受ける
            //playerBulletPool.ReleaseBullet(collision.gameObject);
            UpdateScore(200); // スコアを加算
        }
    }

    // ボスの移動パターン (例: 左右にスムーズに移動)
    void Move()
    {
        float moveX = Mathf.Sin(Time.time * moveSpeed) * 30.0f;  // 左右にスムーズに移動
        transform.position = new Vector3(moveX, transform.position.y, transform.position.z);
    }

    // ボスが弾を発射する処理
    void Fire()
    {
        GameObject enemyBullet = enemyBulletPool.GetBullet(); // 弾をプールから取得
        enemyBullet.transform.position = firePoint.position; // 弾の位置を設定
        enemyBullet.transform.rotation = Quaternion.identity; // 弾の回転を初期化

        Rigidbody2D eBulletRb = enemyBullet.GetComponent<Rigidbody2D>(); // リジットボディを取得
        eBulletRb.velocity = Vector2.zero; // 前回の動きをリセット
        eBulletRb.velocity = new Vector2(0f, bulletSpeed); // 弾の速度を設定

        // 弾を一定時間後にプールに戻す
        enemyBulletPool.ReleaseBullet(enemyBullet, 3f);
    }

    // ボスのダメージ処理
    public void TakeDamage(int damageAmount, GameObject gameObject)
    {
        currentHealth -= damageAmount; // 現在の体力を減少
        PlayEffect(gameObject.transform, 0.2f).Forget(); // ヒットエフェクトを再生

        Debug.Log($"ボスが{damageAmount}のダメージを受けました。現在のヘルス: {currentHealth}");

        // 体力が0以下になったら死亡処理を行う
        if (currentHealth <= 0 && !isDead)
        {
            Die(); // 死亡処理を実行
        }
    }

    // ボスが倒れたときの処理
    void Die()
    {
        isDead = true; // 死亡フラグを立てる
        // ボスを倒した時のエフェクトを再生
        for (int i = 0; i < 15; i++)
        {
            Transform tr = new GameObject().transform; // 新しい一時的なオブジェクトのTransformを作成
            tr.position = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0); // ランダムな位置に設定
            PlayEffect(tr, 2f).Forget(); // エフェクトを再生
        }
        gameObject.SetActive(false); // ボスを非アクティブ化
        UpdateScore(100000); // スコアを加算
        gameClearUI.SetActive(true); // ゲームクリアのUIを表示
    }

    // ボスのフェーズが変更されたときの処理
    void ChangePhase()
    {
        isPhaseChanged = true; // フェーズ変更フラグを立てる
        Debug.Log("ボスのフェーズが変更されました！");

        // フェーズ変更時にボスの挙動を変化させる
        moveSpeed += 1.0f;  // 移動速度を速くする
        fireInterval -= 0.5f;  // 弾を発射する間隔を短くする

        // 発射間隔を更新
        CancelInvoke(); // 既存のInvokeをキャンセル
        InvokeRepeating("Fire", fireInterval, fireInterval); // 新しい間隔で弾を発射
    }

    // ヒットエフェクトを表示
    async UniTaskVoid PlayEffect(Transform effectTransform, float delay)
    {
        GameObject effect = effectPool.GetEffect(); // エフェクトを取得
        effect.transform.position = effectTransform.position; // エフェクトの位置を設定

        // 一定時間待ってからエフェクトをプールに戻す
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        effectPool.ReleaseEffect(effect); // エフェクトをプールに戻す
    }

    // スコアを更新するメソッド
    private void UpdateScore(int amount)
    {
        ScoreManager.Instance.AddScore(amount); // スコアを加算
        ScoreManager.Instance.SetDisplayScore(scoreText); // スコアをUIに表示
    }
}

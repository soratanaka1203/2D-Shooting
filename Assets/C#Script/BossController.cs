using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using MyNameSpace;

public class BossController : MonoBehaviour
{
    public int maxHealth = 400;        // ボスの最大体力
    private int currentHealth;          // 現在の体力

    public float moveSpeed = 2.0f;      // ボスの移動速度
    [SerializeField] private BulletPool enemyBulletPool;// ボスが発射する弾のプレハブ
    public Transform firePoint;         // 弾を発射する位置
    public float fireInterval = 1.5f;   // 弾を発射する間隔
    public float bulletSpeed = 20f;
    public float phaseChangeHealthThreshold = 0.5f;  // フェーズ変更の体力割合 (50%)

    //エフェクト
    [SerializeField] EffectPool effectPool;

    //UI
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject gameClearUI;

    [SerializeField] GameObject EnemySpawner;

    private bool isDead = false;
    private bool isPhaseChanged = false;

    void Start()
    {
        //アタッチされていなかったら探してとってくる
        if (enemyBulletPool == null)
        {
            enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<BulletPool>();
        }
        if (effectPool == null)
        {
            effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();
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
        
        EnemySpawner.SetActive(false);
        currentHealth = maxHealth;
        InvokeRepeating("Fire", fireInterval, fireInterval);
    }

    void Update()
    {
        if (!isDead)
        {
            Move();

            // 体力が50%以下になったらフェーズを変更
            if (currentHealth <= maxHealth * phaseChangeHealthThreshold && !isPhaseChanged)
            {
                ChangePhase();
                EnemySpawner?.SetActive(true);
            }
        }
    }

    //弾が当たったらダメージを与える
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            TakeDamage(1, collision.gameObject);
            //スコアを加算
            ScoreManager.Instance.AddScore(200);
            ScoreManager.Instance.SetDisplayScore(scoreText);
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
        GameObject enemyBullet = enemyBulletPool.GetBullet();//弾を取得
        enemyBullet.transform.position = firePoint.position;
        enemyBullet.transform.rotation = Quaternion.identity;

        Rigidbody2D eBulletRb = enemyBullet.GetComponent<Rigidbody2D>();//リジットボディを取得
        eBulletRb.velocity = Vector2.zero; // 前回の動きをリセット
        eBulletRb.velocity = new Vector2(0f, bulletSpeed); // 弾の速度を設定
        enemyBulletPool.ReleaseBullet(enemyBullet, 3f);

    }

    // ボスのダメージ処理
    public void TakeDamage(int damageAmount ,GameObject gameObject)
    {
        currentHealth -= damageAmount;
        PlayEffect(gameObject.transform, 0.2f);
        
        Debug.Log("Boss took damage, current health: " + currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // ボスが倒れたときの処理
    void Die()
    {
        isDead = true;
        // ボスを倒した時のエフェクトを再生
        for (int i = 0; i < 15; i++)
        {
            Transform tr = new GameObject().transform; // 新しい一時的なオブジェクトのTransformを作成
            tr.position = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0);
            PlayEffect(tr, 2f);
        }
        // ボスを非アクティブ化
        gameObject.SetActive(false);
        //スコアを加算
        ScoreManager.Instance.AddScore(100000);
        ScoreManager.Instance.SetDisplayScore(scoreText);
        // ゲームクリアのUIを表示
        gameClearUI.SetActive(true);
    }

    // ボスのフェーズが変更されたときの処理
    void ChangePhase()
    {
        isPhaseChanged = true;
        Debug.Log("Boss phase changed!");

        // フェーズ変更時にボスの挙動を変化させる
        moveSpeed += 1.0f;  // 移動速度を速くする
        fireInterval -= 0.5f;  // 弾を発射する間隔を短くする

        // 発射間隔を更新
        CancelInvoke();
        InvokeRepeating("Fire", fireInterval, fireInterval);
    }

    // ヒットエフェクトを表示
    async UniTaskVoid PlayEffect(Transform effectTransform, float delay)
    {
        GameObject effect = effectPool.GetEffect();
        effect.transform.position = effectTransform.position;
       //エフェクトをプールに戻す
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        effectPool.ReleaseEffect(effect);
    }
}

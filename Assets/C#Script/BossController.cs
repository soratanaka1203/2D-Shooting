using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int maxHealth = 1000;        // ボスの最大体力
    private int currentHealth;          // 現在の体力

    public float moveSpeed = 2.0f;      // ボスの移動速度
    [SerializeField] private BulletPool enemyBulletPool;// ボスが発射する弾のプレハブ
    public Transform firePoint;         // 弾を発射する位置
    public float fireInterval = 2.0f;   // 弾を発射する間隔
    public float bulletSpeed = 5f;
    public float phaseChangeHealthThreshold = 0.5f;  // フェーズ変更の体力割合 (50%)

    [SerializeField] TextMeshProUGUI scoreText;

    private bool isDead = false;
    private bool isPhaseChanged = false;

    void Start()
    {
        scoreText = GameObject.Find("scoreText").GetComponent<TextMeshProUGUI>();
        enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<BulletPool>();

        currentHealth = maxHealth;
        // 弾の発射を開始
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
            }
        }
    }

    // ボスの移動パターン (例: 左右にスムーズに移動)
    void Move()
    {
        float moveX = Mathf.Sin(Time.time * moveSpeed) * 3.0f;  // 左右にスムーズに移動
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

    }

    // ボスのダメージ処理
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
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
        // ボスを倒した時のエフェクトや音を再生
        Debug.Log("Boss defeated!");
        // ボスを非アクティブ化
        gameObject.SetActive(false);
        // 必要であれば、クリア画面や次のステージへの遷移などを追加
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
}

using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool; // 敵のプールを参照
    [SerializeField] private GameObject boss; // ボスのオブジェクト
    public float spawnInterval = 2.5f; // 初期スポーン間隔
    public float minSpawnInterval = 0.3f; // スポーン間隔の最小値
    public float spawnIntervalDecreaseRate = 0.02f; // スポーン間隔の減少量

    private bool bossIs = false; // ボスが登場したかどうかのフラグ
    private float startTime; // スポーン開始時の時刻を保持

    void Start()
    {
        // enemyPoolが未設定の場合、シーン内から自動取得
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();
        }

        InitializeBoss(); // ボスの初期化
        StartSpawner(); // 敵スポーンの開始
    }

    // オブジェクトがアクティブ化されたときの処理
    void OnEnable()
    {
        if (bossIs)
        {
            StartSpawner();
        }
    }

    // ボスの初期設定
    public void InitializeBoss()
    {
        // bossが未設定の場合、シーン内から自動取得
        if (boss == null)
        {
            boss = GameObject.Find("Boss");
        }

        boss.SetActive(false); // 初期状態で非アクティブに設定
        bossIs = false; // ボスフラグをリセット
    }

    // スポーンのリセットメソッド
    public void ResetSpawner()
    {
        InitializeBoss(); // ボスの初期化
        spawnInterval = 2.5f; // スポーン間隔を初期値にリセット
        StartSpawner(); // スポーンを再起動
    }

    // スポーンを開始するメソッド
    private void StartSpawner()
    {
        startTime = Time.time; // 現在の時刻を記録し、経過時間をリセット
        StartCoroutine(SpawnEnemies()); // コルーチン開始
    }

    // 敵を定期的にスポーンするコルーチン
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // スポーン開始から10秒経過したらボスを登場させる
            if (Time.time - startTime >= 3)
            {
                if (!bossIs) // まだボスが登場していない場合
                {
                    boss.transform.position = new Vector3(0, 40, 0); // ボスの初期位置を設定
                    boss.SetActive(true); // ボスをアクティブ化
                    bossIs = true; // フラグを更新
                    Debug.Log("Boss Active: " + boss.activeSelf); // デバッグログでボスの状態を確認
                    Debug.Log("Boss Position: " + boss.transform.position); // ボスの位置を確認
                }
            }

            // プールから敵を取得
            var enemyObject = enemyPool.GetEnemy();
            if (enemyObject != null)
            {
                // 敵のスポーン位置をランダムに設定
                enemyObject.transform.position = new Vector3(Random.Range(-50f, 50f), 55f, 0.0f);
                enemyObject.transform.rotation = Quaternion.identity;

                // 敵の動作とスコア設定
                var enemyControl = enemyObject.GetComponent<EnemyControl>();
                var enemyBH = enemyObject.GetComponent<BulletHit>();
                SpriteRenderer spriteRenderer = enemyObject.GetComponent<SpriteRenderer>();

                // 各コンポーネントが存在するか確認
                if (enemyControl != null && enemyBH != null)
                {
                    // ランダムに敵の移動パターンを選択
                    int randomMovement = Random.Range(0, 3);
                    switch (randomMovement)
                    {
                        case 0:
                            enemyControl.SetMovement(new StraightMovement()); // 直進移動
                            spriteRenderer.color = Color.yellow;
                            enemyBH.scorePoint = 500;
                            enemyBH.enemyHp = 5;
                            break;
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement()); // ジグザグ移動
                            spriteRenderer.color = Color.red;
                            enemyBH.scorePoint = 350;
                            enemyBH.enemyHp = 4;
                            break;
                        case 2:
                            enemyControl.SetMovement(new CircularMovement()); // 円運動
                            spriteRenderer.color = Color.magenta;
                            enemyBH.scorePoint = 200;
                            enemyBH.enemyHp = 4;
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"EnemyControl or BulletHit component not found on the enemy object: {enemyObject.name}");
                }
            }
            else
            {
                Debug.LogError("Failed to get enemy from pool.");
            }

            // スポーン間隔を減少させ、最小値以下にならないようにする
            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecreaseRate, minSpawnInterval);

            // 次のスポーンまで待機
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

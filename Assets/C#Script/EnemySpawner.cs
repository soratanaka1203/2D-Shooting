using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] GameObject boss;
    public float spawnInterval = 2.5f;
    public float minSpawnInterval = 0.3f;  // スポーン間隔の最小値
    public float spawnIntervalDecreaseRate = 0.02f;  // スポーン間隔を減少させる量

    private bool bossIs = false;

    void Start()
    {
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();
        }

        InitializeBoss();
        // 敵を定期的にスポーンするコルーチンを開始
        StartCoroutine(SpawnEnemies());
    }

    private void InitializeBoss()
    {
        if (boss == null)
        {
            boss = GameObject.Find("Boss");
        }

        boss.SetActive(false); // 初期状態で非アクティブ
        bossIs = false; // ボスフラグを初期化
    }

    // リセットメソッド
    public void ResetSpawner()
    {
        InitializeBoss(); // ボスの初期化
        spawnInterval = 2.5f; // スポーン間隔をリセット
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // ゲームを開始して10秒経過したらボスを登場させる
            if (Time.time >= 10)
            {
                if (!bossIs)
                {
                    boss.transform.position = new Vector3(0, 40, 0);
                    boss.SetActive(true);
                    bossIs = true; // フラグを更新
                    Debug.Log("Boss Active: " + boss.activeSelf);
                    Debug.Log("Boss Position: " + boss.transform.position);
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

                if (enemyControl != null && enemyBH != null)
                {
                    // ランダムに敵の移動パターンを選択
                    int randomMovement = Random.Range(0, 3);
                    switch (randomMovement)
                    {
                        case 0:
                            enemyControl.SetMovement(new StraightMovement());
                            spriteRenderer.color = Color.yellow;
                            enemyBH.scorePoint = 500;
                            enemyBH.enemyHp = 5;
                            break;
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement());
                            spriteRenderer.color = Color.red;
                            enemyBH.scorePoint = 350;
                            enemyBH.enemyHp = 4;
                            break;
                        case 2:
                            enemyControl.SetMovement(new CircularMovement());
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

            // スポーン間隔を減少させる（最小値以下にならないようにする）
            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecreaseRate, minSpawnInterval);

            // 次のスポーンまで待機
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

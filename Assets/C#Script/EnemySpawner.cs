using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool;
    public float spawnInterval = 2.5f;
    public float minSpawnInterval = 0.3f;  // スポーン間隔の最小値
    public float spawnIntervalDecreaseRate = 0.02f;  // スポーン間隔を減少させる量

    void Start()
    {
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool")?.GetComponent<EnemyPool>();
        }

        // 敵を定期的にスポーンするコルーチンを開始
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // プールから敵を取得
            var enemyObject = enemyPool.GetEnemy();

            if (enemyObject != null)
            {
                // 敵のスポーン位置をランダムに設定
                enemyObject.transform.position = new Vector3(Random.Range(-5.0f, 5.0f), 5.5f, 0.0f);
                enemyObject.transform.rotation = Quaternion.identity;

                // 敵の動作とスコア設定
                var enemyControl = enemyObject.GetComponent<EnemyControl>();
                var enemyBH = enemyObject.GetComponent<BulletHit>();

                if (enemyControl != null && enemyBH != null)
                {
                    // ランダムに敵の移動パターンを選択
                    int randomMovement = Random.Range(0, 3);
                    switch (randomMovement)
                    {
                        case 0:
                            enemyControl.SetMovement(new StraightMovement());
                            enemyBH.scorePoint = 100;
                            break;
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement());
                            enemyBH.scorePoint = 300;
                            break;
                        case 2:
                            enemyControl.SetMovement(new CircularMovement());
                            enemyBH.scorePoint = 450;
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

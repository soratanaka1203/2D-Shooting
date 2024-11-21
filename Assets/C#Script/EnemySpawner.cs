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
    private float timeElapsed = 0; // 経過時間を追跡

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
        // コルーチンのループ。永遠に繰り返される
        while (true)
        {
            // 経過時間を更新。timeElapsedはスポーン間隔を考慮した経過時間
            timeElapsed += spawnInterval;

            // ボスがまだ登場していないかつスポーン開始から90秒が経過した場合、ボスを登場させる
            if (Time.time - startTime >= 90 && !bossIs)
            {
                // ボスの初期位置を設定（画面外に配置）
                boss.transform.position = new Vector3(0, 40, 0);
                boss.SetActive(true); // ボスをアクティブ化
                bossIs = true; // ボスが登場したことを示すフラグ
                Debug.Log("Boss Active: " + boss.activeSelf); // ボスがアクティブになったことをデバッグ出力
            }

            // プールから敵を取得
            var enemyObject = enemyPool.GetEnemy();
            if (enemyObject != null)
            {
                // 敵のスポーン位置をランダムに設定（画面の上部）
                enemyObject.transform.position = new Vector3(Random.Range(-50f, 50f), 55f, 0.0f);
                enemyObject.transform.rotation = Quaternion.identity; // 回転をリセット

                // 敵の制御用コンポーネントとスコア、体力管理用コンポーネントを取得
                var enemyControl = enemyObject.GetComponent<EnemyControl>();
                var enemyBH = enemyObject.GetComponent<BulletHit>();
                SpriteRenderer spriteRenderer = enemyObject.GetComponent<SpriteRenderer>();

                // もし必要なコンポーネントが全て揃っていたら
                if (enemyControl != null && enemyBH != null)
                {
                    // 敵の移動パターンをランダムで選択
                    int randomMovement = Random.Range(0, 4); // 0~3のランダムな整数を生成

                    // 難易度に応じた倍率を設定
                    float difficultyMultiplier = 1 + (timeElapsed / 120f); // timeElapsedの経過時間に基づき難易度を増加

                    // 移動パターンに応じて処理を分岐
                    switch (randomMovement)
                    {
                        // 直進移動
                        case 0:
                            enemyControl.SetMovement(new StraightMovement());
                            spriteRenderer.color = Color.yellow; // 敵の色を設定
                            enemyBH.scorePoint = Mathf.RoundToInt(500 * difficultyMultiplier); // スコア設定（難易度に応じて増加）
                            enemyBH.enemyHp = Mathf.CeilToInt(5 * difficultyMultiplier); // 体力設定（難易度に応じて増加）
                            break;

                        // ジグザグ移動
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement());
                            spriteRenderer.color = Color.red; // 敵の色を設定
                            enemyBH.scorePoint = Mathf.RoundToInt(350 * difficultyMultiplier);
                            enemyBH.enemyHp = Mathf.CeilToInt(4 * difficultyMultiplier);
                            break;

                        // 円運動移動
                        case 2:
                            enemyControl.SetMovement(new CircularMovement());
                            spriteRenderer.color = Color.magenta; // 敵の色を設定
                            enemyBH.scorePoint = Mathf.RoundToInt(200 * difficultyMultiplier);
                            enemyBH.enemyHp = Mathf.CeilToInt(4 * difficultyMultiplier);
                            break;

                        // 波状移動（新しく追加した移動パターン）
                        case 3:
                            enemyControl.SetMovement(new WaveMovement());
                            spriteRenderer.color = Color.cyan; // 敵の色を設定
                            enemyBH.scorePoint = Mathf.RoundToInt(600 * difficultyMultiplier);
                            enemyBH.enemyHp = Mathf.CeilToInt(6 * difficultyMultiplier);
                            break;
                    }
                }
            }

            // スポーン間隔を減少させ、最小値以下にならないように制限
            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecreaseRate, minSpawnInterval);

            // 次のスポーンまで待機
            yield return new WaitForSeconds(spawnInterval);
        }
    }

}

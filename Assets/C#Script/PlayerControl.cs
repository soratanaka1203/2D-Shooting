using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using MyNameSpace;
using Unity.VisualScripting;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    // プレイヤーの移動速度
    public float speed = 120f;
    Rigidbody2D rb; // Rigidbody2Dコンポーネントを参照
    [SerializeField] GameObject shotPoint; // 弾を発射する位置
    [SerializeField] BulletPool bulletPool; // 弾のプール
    public float bulletSpeed = 300f; // 弾の速度
    public int bulletCount = 1; // 発射する弾の数
    float fireRate = 0.1f; // 発射間隔
    float nextFireTime = 0f; // 次の発射時間

    // カメラの境界を定義するための変数
    private Vector3 minBounds; // カメラの左下の境界
    private Vector3 maxBounds; // カメラの右上の境界
    private float objectWidth; // プレイヤーの幅
    private float objectHeight; // プレイヤーの高さ

    [SerializeField] AudioClip shotSe; // 弾発射時の音
    [SerializeField] AudioClip dethSe; // プレイヤー死亡時の音
    [SerializeField] AudioPlayer audioPlayer; // オーディオ管理クラス
    float volum = 5f; // 音量

    [SerializeField] GameObject gameOverUi; // ゲームオーバーUI
    [SerializeField] GameObject enemySpawner; // 敵のスポナー

    [SerializeField] TextMeshProUGUI ResultGameOver;

    private bool isShield = false;
    public GameObject shieldObject; // シールドのプレハブまたは子オブジェクト

    void Start()
    {
        // ゲームオーバーUIを非表示に設定
        gameOverUi.SetActive(false);
        // 敵のスポナーをアクティブに設定
        enemySpawner.SetActive(true);
        shieldObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dを取得

        // 弾のプールが設定されていなければ探して取得
        if (bulletPool == null)
        {
            bulletPool = GameObject.Find("PlayerBulletPool").GetComponent<BulletPool>();
        }

        // カメラのビューポート境界を計算
        Camera cam = Camera.main;
        minBounds = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); // カメラ左下
        maxBounds = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); // カメラ右上

        // プレイヤーのサイズを取得（Colliderが必要）
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x; // オブジェクトの幅の半分
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y; // オブジェクトの高さの半分
    }

    void Update()
    {
        // プレイヤーの移動
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // X軸の移動
        float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime; // Y軸の移動

        // 新しい位置を計算
        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

        // カメラの境界内にプレイヤーを制限
        float clampedX = Mathf.Clamp(newPosition.x, minBounds.x + objectWidth, maxBounds.x - objectWidth);
        float clampedY = Mathf.Clamp(newPosition.y, minBounds.y + objectHeight, maxBounds.y - objectHeight);

        // プレイヤーの新しい位置を設定
        transform.position = new Vector3(clampedX, clampedY, newPosition.z);

        // スペースキーが押されていて、次の発射時間を超えたら弾を発射
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate; // 次の発射時間を設定
            Shot().Forget(); // UniTaskを使用して非同期処理
        }
    }

    // 弾や敵が当たった時の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isShield)
            {
                SetShiled(false);
            }
            else
            {
                
                ScoreManager.Instance.SetDisplayScore(ResultGameOver);//結果を表示
                ScoreManager.Instance.SetRanking(ScoreManager.Instance.score); // スコアをランキングにセット
                ScoreManager.Instance.ResetScore(); // スコアをリセット

                // ゲームオーバーUIを表示
                gameOverUi.SetActive(true);
                enemySpawner.SetActive(false); // 敵のスポナーを非アクティブ化
                audioPlayer.PlayAudio(dethSe, volum); // 死亡時の音を再生
                Destroy(gameObject); // プレイヤーを破棄
            }
        }
    }

    // 弾を打つ
    private async UniTaskVoid Shot()
    {
        // 弾のプールと発射点が設定されていない場合は終了
        if (bulletPool == null || shotPoint == null)
        {
            Debug.LogError("弾のプールまたは発射点が設定されていません。");
            return;
        }

        float spreadAngle = 15f; // 弾の拡散角度

        for (int i = 0; i < bulletCount; i++)
        {
            // プールから弾丸を取得
            var bulletGB = bulletPool.GetBullet();
            if (bulletGB == null)
            {
                Debug.LogError("弾を取得できませんでした。");
                continue; // 次の弾を試す
            }

            // 発射位置と初期設定
            bulletGB.transform.position = shotPoint.transform.position;

            // 弾の角度を調整
            float angle = -spreadAngle * (bulletCount - 1) / 2 + spreadAngle * i;
            bulletGB.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            var bulletRB = bulletGB.GetComponent<Rigidbody2D>();
            bulletRB.velocity = Vector2.zero; // 前回の動きをリセット

            // 発射方向を計算
            Vector2 shotDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
            bulletRB.velocity = shotDirection * bulletSpeed;

            // 弾発射音を再生
            audioPlayer.PlayAudio(shotSe, volum);
        }
    }


    //アイテムのシールド
    public void SetShiled(bool value)
    {
        isShield = value;
        if (isShield)
        {
            shieldObject.SetActive(true);
        }
        else
        {
            shieldObject.SetActive(false);
        }
    }
}

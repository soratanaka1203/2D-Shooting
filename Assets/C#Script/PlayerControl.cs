using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using MyNameSpace;

public class PlayerControl : MonoBehaviour
{
    public float speed = 120f;
    Rigidbody2D rb;
    [SerializeField] GameObject shotPoint;
    [SerializeField] BulletPool bulletPool;
    public float bulletSpeed = 300f;
    float fireRate = 0.1f;
    float nextFireTime = 0f;

    private Vector3 minBounds; // カメラの左下の境界
    private Vector3 maxBounds; // カメラの右上の境界
    private float objectWidth; // プレイヤーの幅
    private float objectHeight; // プレイヤーの高さ

    [SerializeField] AudioClip shotSe;
    [SerializeField] AudioClip dethSe;
    [SerializeField] AudioPlayer audioPlayer;
    float volum = 5f;

    [SerializeField] GameObject gameOverUi;
    [SerializeField] GameObject enemySpawner;

    void Start()
    {
        gameOverUi.SetActive(false);
        enemySpawner.SetActive(true);
        rb = GetComponent<Rigidbody2D>();
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
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

        // カメラの境界内にプレイヤーを制限
        float clampedX = Mathf.Clamp(newPosition.x, minBounds.x + objectWidth, maxBounds.x - objectWidth);
        float clampedY = Mathf.Clamp(newPosition.y, minBounds.y + objectHeight, maxBounds.y - objectHeight);

        // プレイヤーの新しい位置を設定
        transform.position = new Vector3(clampedX, clampedY, newPosition.z);

        //弾を打つ
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shot().Forget(); // UniTask を使用して非同期処理
        }

    }

    //弾が当たった時の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(gameObject);
            ScoreManager.Instance.SetRanking(ScoreManager.Instance.score);
            ScoreManager.Instance.ResetScore();
            //ここから
            //enemySpawner.G
            gameOverUi.SetActive(true);
            enemySpawner.SetActive(false);
            audioPlayer.PlayAudio(dethSe, volum);
        }
    }

    //弾を打つ
    private async UniTaskVoid Shot()
    {
        if (bulletPool == null || shotPoint == null)
        {
            return;
        }

        // プールから弾丸を取得
        var bulletGB = bulletPool.GetBullet();
        bulletGB.transform.position = shotPoint.transform.position;
        bulletGB.transform.rotation = Quaternion.identity;

        var bulletRB = bulletGB.GetComponent<Rigidbody2D>();
        bulletRB.velocity = Vector2.zero; // 前回の動きをリセット
        bulletRB.velocity = new Vector2(0f, bulletSpeed); // 弾の速度を設定

        audioPlayer.PlayAudio(shotSe, volum);
        
        //弾がアクティブだったら
        if (bulletGB)
        {
            //2秒後に弾をプールに戻す
            bulletPool.ReleaseBullet(bulletGB, 2f);
        }
        
    }
}

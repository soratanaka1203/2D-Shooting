using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using MyNameSpace;

public class PlayerControl : MonoBehaviour
{
    public float speed = 8f;
    Rigidbody2D rb;
    [SerializeField] GameObject shotPoint;
    [SerializeField] BulletPool bulletPool;
    public float bulletSpeed = 30f;

    float fireRate = 0.1f;
    float nextFireTime = 0f;

    [SerializeField] AudioClip shotSe;
    [SerializeField] AudioClip dethSe;
    [SerializeField] AudioPlayer audioPlayer;
    float volum = 5f;

    [SerializeField] GameObject gameOverUi;

    void Start()
    {
        gameOverUi.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        if (bulletPool == null)
        {
            bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();
        }
    }

    void Update()
    {
        //プレイヤーのWASD移動
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(x, y, 0) * speed * Time.deltaTime;

        //弾を打つ
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shot().Forget(); // UniTask を使用して非同期処理
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(gameObject);
            ScoreManager.Instance.SetRanking(ScoreManager.Instance.score);
            ScoreManager.Instance.ResetScore();
            gameOverUi.SetActive(true);
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
    }
}

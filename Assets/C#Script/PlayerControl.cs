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
        //�v���C���[��WASD�ړ�
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(x, y, 0) * speed * Time.deltaTime;

        //�e��ł�
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shot().Forget(); // UniTask ���g�p���Ĕ񓯊�����
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

    //�e��ł�
    private async UniTaskVoid Shot()
    {
        if (bulletPool == null || shotPoint == null)
        {
            return;
        }

        // �v�[������e�ۂ��擾
        var bulletGB = bulletPool.GetBullet();
        bulletGB.transform.position = shotPoint.transform.position;
        bulletGB.transform.rotation = Quaternion.identity;

        var bulletRB = bulletGB.GetComponent<Rigidbody2D>();
        bulletRB.velocity = Vector2.zero; // �O��̓��������Z�b�g
        bulletRB.velocity = new Vector2(0f, bulletSpeed); // �e�̑��x��ݒ�

        audioPlayer.PlayAudio(shotSe, volum);
    }
}

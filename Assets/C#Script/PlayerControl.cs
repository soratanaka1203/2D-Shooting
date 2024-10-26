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

    private Vector3 minBounds; // �J�����̍����̋��E
    private Vector3 maxBounds; // �J�����̉E��̋��E
    private float objectWidth; // �v���C���[�̕�
    private float objectHeight; // �v���C���[�̍���

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

        // �J�����̃r���[�|�[�g���E���v�Z
        Camera cam = Camera.main;
        minBounds = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); // �J��������
        maxBounds = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); // �J�����E��

        // �v���C���[�̃T�C�Y���擾�iCollider���K�v�j
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x; // �I�u�W�F�N�g�̕��̔���
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y; // �I�u�W�F�N�g�̍����̔���
    }

    void Update()
    {
        // �v���C���[�̈ړ�
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

        // �J�����̋��E���Ƀv���C���[�𐧌�
        float clampedX = Mathf.Clamp(newPosition.x, minBounds.x + objectWidth, maxBounds.x - objectWidth);
        float clampedY = Mathf.Clamp(newPosition.y, minBounds.y + objectHeight, maxBounds.y - objectHeight);

        // �v���C���[�̐V�����ʒu��ݒ�
        transform.position = new Vector3(clampedX, clampedY, newPosition.z);

        //�e��ł�
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shot().Forget(); // UniTask ���g�p���Ĕ񓯊�����
        }

    }

    //�e�������������̏���
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(gameObject);
            ScoreManager.Instance.SetRanking(ScoreManager.Instance.score);
            ScoreManager.Instance.ResetScore();
            //��������
            //enemySpawner.G
            gameOverUi.SetActive(true);
            enemySpawner.SetActive(false);
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
        
        //�e���A�N�e�B�u��������
        if (bulletGB)
        {
            //2�b��ɒe���v�[���ɖ߂�
            bulletPool.ReleaseBullet(bulletGB, 2f);
        }
        
    }
}

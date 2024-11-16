using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using MyNameSpace;

public class PlayerControl : MonoBehaviour
{
    // �v���C���[�̈ړ����x
    public float speed = 120f;
    Rigidbody2D rb; // Rigidbody2D�R���|�[�l���g���Q��
    [SerializeField] GameObject shotPoint; // �e�𔭎˂���ʒu
    [SerializeField] BulletPool bulletPool; // �e�̃v�[��
    public float bulletSpeed = 300f; // �e�̑��x
    float fireRate = 0.1f; // ���ˊԊu
    float nextFireTime = 0f; // ���̔��ˎ���

    // �J�����̋��E���`���邽�߂̕ϐ�
    private Vector3 minBounds; // �J�����̍����̋��E
    private Vector3 maxBounds; // �J�����̉E��̋��E
    private float objectWidth; // �v���C���[�̕�
    private float objectHeight; // �v���C���[�̍���

    [SerializeField] AudioClip shotSe; // �e���ˎ��̉�
    [SerializeField] AudioClip dethSe; // �v���C���[���S���̉�
    [SerializeField] AudioPlayer audioPlayer; // �I�[�f�B�I�Ǘ��N���X
    float volum = 5f; // ����

    [SerializeField] GameObject gameOverUi; // �Q�[���I�[�o�[UI
    [SerializeField] GameObject enemySpawner; // �G�̃X�|�i�[

    void Start()
    {
        // �Q�[���I�[�o�[UI���\���ɐݒ�
        gameOverUi.SetActive(false);
        // �G�̃X�|�i�[���A�N�e�B�u�ɐݒ�
        enemySpawner.SetActive(true);
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D���擾

        // �e�̃v�[�����ݒ肳��Ă��Ȃ���ΒT���Ď擾
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
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // X���̈ړ�
        float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime; // Y���̈ړ�

        // �V�����ʒu���v�Z
        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

        // �J�����̋��E���Ƀv���C���[�𐧌�
        float clampedX = Mathf.Clamp(newPosition.x, minBounds.x + objectWidth, maxBounds.x - objectWidth);
        float clampedY = Mathf.Clamp(newPosition.y, minBounds.y + objectHeight, maxBounds.y - objectHeight);

        // �v���C���[�̐V�����ʒu��ݒ�
        transform.position = new Vector3(clampedX, clampedY, newPosition.z);

        // �X�y�[�X�L�[��������Ă��āA���̔��ˎ��Ԃ𒴂�����e�𔭎�
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate; // ���̔��ˎ��Ԃ�ݒ�
            Shot().Forget(); // UniTask���g�p���Ĕ񓯊�����
        }
    }

    // �e�������������̏���
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(gameObject); // �v���C���[��j��
            ScoreManager.Instance.SetRanking(ScoreManager.Instance.score); // �X�R�A�������L���O�ɃZ�b�g
            ScoreManager.Instance.ResetScore(); // �X�R�A�����Z�b�g

            // �Q�[���I�[�o�[UI��\��
            gameOverUi.SetActive(true);
            enemySpawner.SetActive(false); // �G�̃X�|�i�[���A�N�e�B�u��
            audioPlayer.PlayAudio(dethSe, volum); // ���S���̉����Đ�
        }
    }

    // �e��ł�
    private async UniTaskVoid Shot()
    {
        // �e�̃v�[���Ɣ��˓_���ݒ肳��Ă��Ȃ��ꍇ�͏I��
        if (bulletPool == null || shotPoint == null)
        {
            Debug.LogError("�e�̃v�[���܂��͔��˓_���ݒ肳��Ă��܂���B");
            return;
        }

        // �v�[������e�ۂ��擾
        var bulletGB = bulletPool.GetBullet();
        if (bulletGB == null)
        {
            Debug.LogError("�e���擾�ł��܂���ł����B");
            return; // �e���擾�ł��Ȃ��ꍇ�͏I��
        }

        bulletGB.transform.position = shotPoint.transform.position; // ���ˈʒu��ݒ�
        bulletGB.transform.rotation = Quaternion.identity; // �����̉�]��ݒ�

        var bulletRB = bulletGB.GetComponent<Rigidbody2D>(); // Rigidbody2D���擾
        bulletRB.velocity = Vector2.zero; // �O��̓��������Z�b�g
        bulletRB.velocity = new Vector2(0f, bulletSpeed); // �e�̑��x��ݒ�

        audioPlayer.PlayAudio(shotSe, volum); // �e���ˉ����Đ�

        // �e��Active��������
        if (bulletGB)
        {
            // 2�b��ɒe���v�[���ɖ߂�
            bulletPool.ReleaseBullet(bulletGB, 1.5f);
        }
    }
}

using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using MyNameSpace;

public class BossController : MonoBehaviour
{
    // �{�X�̍ő�̗�
    public int maxHealth = 400;
    private int currentHealth;          // ���݂̗̑�

    // �{�X�̈ړ����x
    public float moveSpeed = 2.0f;

    // �{�X�����˂���e�̃v���n�u
    [SerializeField] private BulletPool enemyBulletPool;
    public Transform firePoint;         // �e�𔭎˂���ʒu
    public float fireInterval = 1.5f;   // �e�𔭎˂���Ԋu
    public float bulletSpeed = 20f;     // �e�̑��x
    public float phaseChangeHealthThreshold = 0.5f;  // �t�F�[�Y�ύX�̗̑͊��� (50%)

    //�v���C���[�̒e�̃v���n�u
    [SerializeField] private BulletPool playerBulletPool;

    // �G�t�F�N�g�v�[��
    [SerializeField] EffectPool effectPool;

    // UI�v�f
    [SerializeField] TextMeshProUGUI scoreText;   // �X�R�A�\���p�e�L�X�g
    [SerializeField] GameObject gameClearUI;       // �Q�[���N���A��UI

    [SerializeField] GameObject EnemySpawner;       // �G�X�|�i�[�̎Q��

    private bool isDead = false;                    // �{�X�����S���Ă��邩�̃t���O
    private bool isPhaseChanged = false;            // �t�F�[�Y���ύX���ꂽ���̃t���O

    void Start()
    {
        // �e�R���|�[�l���g�̏����ݒ�
        if (enemyBulletPool == null)
        {
            enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<BulletPool>();
        }
        if (effectPool == null)
        {
            effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();
        }
        if (playerBulletPool == null)
        {
            playerBulletPool = GameObject.Find("PlayerBulletPool").GetComponentInChildren<BulletPool>();
        }
        if (scoreText == null)
        {
            scoreText = GameObject.Find("scoreText").GetComponentInChildren<TextMeshProUGUI>();
        }
        if (gameClearUI == null)
        {
            gameClearUI = GameObject.Find("GameClearUI");
        }
        if (EnemySpawner == null)
        {
            EnemySpawner = GameObject.Find("EnemySpawner");
        }

        // �G�X�|�i�[���A�N�e�B�u�ɂ���
        EnemySpawner.SetActive(false);
        currentHealth = maxHealth; // ���݂̗̑͂��ő�̗͂ŏ�����
        InvokeRepeating("Fire", fireInterval, fireInterval); // ����I�ɒe�𔭎�
    }

    void Update()
    {
        // �{�X�����S���Ă��Ȃ��ꍇ�̍X�V����
        if (!isDead)
        {
            Move(); // �{�X�̈ړ����X�V

            // �̗͂�50%�ȉ��ɂȂ�����t�F�[�Y��ύX
            if (currentHealth <= maxHealth * phaseChangeHealthThreshold && !isPhaseChanged)
            {
                ChangePhase(); // �t�F�[�Y��ύX
                EnemySpawner?.SetActive(true); // �G�X�|�i�[���A�N�e�B�u�ɂ���
            }
        }
    }

    // �e������������_���[�W��^���鏈��
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet")) // �v���C���[�̒e�ƏՓ˂����ꍇ
        {
            TakeDamage(1, collision.gameObject); // �_���[�W���󂯂�
            //playerBulletPool.ReleaseBullet(collision.gameObject);
            UpdateScore(200); // �X�R�A�����Z
        }
    }

    // �{�X�̈ړ��p�^�[�� (��: ���E�ɃX���[�Y�Ɉړ�)
    void Move()
    {
        float moveX = Mathf.Sin(Time.time * moveSpeed) * 30.0f;  // ���E�ɃX���[�Y�Ɉړ�
        transform.position = new Vector3(moveX, transform.position.y, transform.position.z);
    }

    // �{�X���e�𔭎˂��鏈��
    void Fire()
    {
        GameObject enemyBullet = enemyBulletPool.GetBullet(); // �e���v�[������擾
        enemyBullet.transform.position = firePoint.position; // �e�̈ʒu��ݒ�
        enemyBullet.transform.rotation = Quaternion.identity; // �e�̉�]��������

        Rigidbody2D eBulletRb = enemyBullet.GetComponent<Rigidbody2D>(); // ���W�b�g�{�f�B���擾
        eBulletRb.velocity = Vector2.zero; // �O��̓��������Z�b�g
        eBulletRb.velocity = new Vector2(0f, bulletSpeed); // �e�̑��x��ݒ�

        // �e����莞�Ԍ�Ƀv�[���ɖ߂�
        enemyBulletPool.ReleaseBullet(enemyBullet, 3f);
    }

    // �{�X�̃_���[�W����
    public void TakeDamage(int damageAmount, GameObject gameObject)
    {
        currentHealth -= damageAmount; // ���݂̗̑͂�����
        PlayEffect(gameObject.transform, 0.2f).Forget(); // �q�b�g�G�t�F�N�g���Đ�

        Debug.Log($"�{�X��{damageAmount}�̃_���[�W���󂯂܂����B���݂̃w���X: {currentHealth}");

        // �̗͂�0�ȉ��ɂȂ����玀�S�������s��
        if (currentHealth <= 0 && !isDead)
        {
            Die(); // ���S���������s
        }
    }

    // �{�X���|�ꂽ�Ƃ��̏���
    void Die()
    {
        isDead = true; // ���S�t���O�𗧂Ă�
        // �{�X��|�������̃G�t�F�N�g���Đ�
        for (int i = 0; i < 15; i++)
        {
            Transform tr = new GameObject().transform; // �V�����ꎞ�I�ȃI�u�W�F�N�g��Transform���쐬
            tr.position = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0); // �����_���Ȉʒu�ɐݒ�
            PlayEffect(tr, 2f).Forget(); // �G�t�F�N�g���Đ�
        }
        gameObject.SetActive(false); // �{�X���A�N�e�B�u��
        UpdateScore(100000); // �X�R�A�����Z
        gameClearUI.SetActive(true); // �Q�[���N���A��UI��\��
    }

    // �{�X�̃t�F�[�Y���ύX���ꂽ�Ƃ��̏���
    void ChangePhase()
    {
        isPhaseChanged = true; // �t�F�[�Y�ύX�t���O�𗧂Ă�
        Debug.Log("�{�X�̃t�F�[�Y���ύX����܂����I");

        // �t�F�[�Y�ύX���Ƀ{�X�̋�����ω�������
        moveSpeed += 1.0f;  // �ړ����x�𑬂�����
        fireInterval -= 0.5f;  // �e�𔭎˂���Ԋu��Z������

        // ���ˊԊu���X�V
        CancelInvoke(); // ������Invoke���L�����Z��
        InvokeRepeating("Fire", fireInterval, fireInterval); // �V�����Ԋu�Œe�𔭎�
    }

    // �q�b�g�G�t�F�N�g��\��
    async UniTaskVoid PlayEffect(Transform effectTransform, float delay)
    {
        GameObject effect = effectPool.GetEffect(); // �G�t�F�N�g���擾
        effect.transform.position = effectTransform.position; // �G�t�F�N�g�̈ʒu��ݒ�

        // ��莞�ԑ҂��Ă���G�t�F�N�g���v�[���ɖ߂�
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        effectPool.ReleaseEffect(effect); // �G�t�F�N�g���v�[���ɖ߂�
    }

    // �X�R�A���X�V���郁�\�b�h
    private void UpdateScore(int amount)
    {
        ScoreManager.Instance.AddScore(amount); // �X�R�A�����Z
        ScoreManager.Instance.SetDisplayScore(scoreText); // �X�R�A��UI�ɕ\��
    }
}

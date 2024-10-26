using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using MyNameSpace;

public class BossController : MonoBehaviour
{
    public int maxHealth = 400;        // �{�X�̍ő�̗�
    private int currentHealth;          // ���݂̗̑�

    public float moveSpeed = 2.0f;      // �{�X�̈ړ����x
    [SerializeField] private BulletPool enemyBulletPool;// �{�X�����˂���e�̃v���n�u
    public Transform firePoint;         // �e�𔭎˂���ʒu
    public float fireInterval = 1.5f;   // �e�𔭎˂���Ԋu
    public float bulletSpeed = 20f;
    public float phaseChangeHealthThreshold = 0.5f;  // �t�F�[�Y�ύX�̗̑͊��� (50%)

    //�G�t�F�N�g
    [SerializeField] EffectPool effectPool;

    //UI
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject gameClearUI;

    [SerializeField] GameObject EnemySpawner;

    private bool isDead = false;
    private bool isPhaseChanged = false;

    void Start()
    {
        //�A�^�b�`����Ă��Ȃ�������T���ĂƂ��Ă���
        if (enemyBulletPool == null)
        {
            enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<BulletPool>();
        }
        if (effectPool == null)
        {
            effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();
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
        
        EnemySpawner.SetActive(false);
        currentHealth = maxHealth;
        InvokeRepeating("Fire", fireInterval, fireInterval);
    }

    void Update()
    {
        if (!isDead)
        {
            Move();

            // �̗͂�50%�ȉ��ɂȂ�����t�F�[�Y��ύX
            if (currentHealth <= maxHealth * phaseChangeHealthThreshold && !isPhaseChanged)
            {
                ChangePhase();
                EnemySpawner?.SetActive(true);
            }
        }
    }

    //�e������������_���[�W��^����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            TakeDamage(1, collision.gameObject);
            //�X�R�A�����Z
            ScoreManager.Instance.AddScore(200);
            ScoreManager.Instance.SetDisplayScore(scoreText);
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
        GameObject enemyBullet = enemyBulletPool.GetBullet();//�e���擾
        enemyBullet.transform.position = firePoint.position;
        enemyBullet.transform.rotation = Quaternion.identity;

        Rigidbody2D eBulletRb = enemyBullet.GetComponent<Rigidbody2D>();//���W�b�g�{�f�B���擾
        eBulletRb.velocity = Vector2.zero; // �O��̓��������Z�b�g
        eBulletRb.velocity = new Vector2(0f, bulletSpeed); // �e�̑��x��ݒ�
        enemyBulletPool.ReleaseBullet(enemyBullet, 3f);

    }

    // �{�X�̃_���[�W����
    public void TakeDamage(int damageAmount ,GameObject gameObject)
    {
        currentHealth -= damageAmount;
        PlayEffect(gameObject.transform, 0.2f);
        
        Debug.Log("Boss took damage, current health: " + currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // �{�X���|�ꂽ�Ƃ��̏���
    void Die()
    {
        isDead = true;
        // �{�X��|�������̃G�t�F�N�g���Đ�
        for (int i = 0; i < 15; i++)
        {
            Transform tr = new GameObject().transform; // �V�����ꎞ�I�ȃI�u�W�F�N�g��Transform���쐬
            tr.position = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0);
            PlayEffect(tr, 2f);
        }
        // �{�X���A�N�e�B�u��
        gameObject.SetActive(false);
        //�X�R�A�����Z
        ScoreManager.Instance.AddScore(100000);
        ScoreManager.Instance.SetDisplayScore(scoreText);
        // �Q�[���N���A��UI��\��
        gameClearUI.SetActive(true);
    }

    // �{�X�̃t�F�[�Y���ύX���ꂽ�Ƃ��̏���
    void ChangePhase()
    {
        isPhaseChanged = true;
        Debug.Log("Boss phase changed!");

        // �t�F�[�Y�ύX���Ƀ{�X�̋�����ω�������
        moveSpeed += 1.0f;  // �ړ����x�𑬂�����
        fireInterval -= 0.5f;  // �e�𔭎˂���Ԋu��Z������

        // ���ˊԊu���X�V
        CancelInvoke();
        InvokeRepeating("Fire", fireInterval, fireInterval);
    }

    // �q�b�g�G�t�F�N�g��\��
    async UniTaskVoid PlayEffect(Transform effectTransform, float delay)
    {
        GameObject effect = effectPool.GetEffect();
        effect.transform.position = effectTransform.position;
       //�G�t�F�N�g���v�[���ɖ߂�
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        effectPool.ReleaseEffect(effect);
    }
}

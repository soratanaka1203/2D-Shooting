using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int maxHealth = 1000;        // �{�X�̍ő�̗�
    private int currentHealth;          // ���݂̗̑�

    public float moveSpeed = 2.0f;      // �{�X�̈ړ����x
    [SerializeField] private BulletPool enemyBulletPool;// �{�X�����˂���e�̃v���n�u
    public Transform firePoint;         // �e�𔭎˂���ʒu
    public float fireInterval = 2.0f;   // �e�𔭎˂���Ԋu
    public float bulletSpeed = 5f;
    public float phaseChangeHealthThreshold = 0.5f;  // �t�F�[�Y�ύX�̗̑͊��� (50%)

    [SerializeField] TextMeshProUGUI scoreText;

    private bool isDead = false;
    private bool isPhaseChanged = false;

    void Start()
    {
        scoreText = GameObject.Find("scoreText").GetComponent<TextMeshProUGUI>();
        enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<BulletPool>();

        currentHealth = maxHealth;
        // �e�̔��˂��J�n
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
            }
        }
    }

    // �{�X�̈ړ��p�^�[�� (��: ���E�ɃX���[�Y�Ɉړ�)
    void Move()
    {
        float moveX = Mathf.Sin(Time.time * moveSpeed) * 3.0f;  // ���E�ɃX���[�Y�Ɉړ�
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

    }

    // �{�X�̃_���[�W����
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
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
        // �{�X��|�������̃G�t�F�N�g�≹���Đ�
        Debug.Log("Boss defeated!");
        // �{�X���A�N�e�B�u��
        gameObject.SetActive(false);
        // �K�v�ł���΁A�N���A��ʂ⎟�̃X�e�[�W�ւ̑J�ڂȂǂ�ǉ�
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
}

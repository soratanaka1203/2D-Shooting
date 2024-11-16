using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool; // �G�̃v�[�����Q��
    [SerializeField] private GameObject boss; // �{�X�̃I�u�W�F�N�g
    public float spawnInterval = 2.5f; // �����X�|�[���Ԋu
    public float minSpawnInterval = 0.3f; // �X�|�[���Ԋu�̍ŏ��l
    public float spawnIntervalDecreaseRate = 0.02f; // �X�|�[���Ԋu�̌�����

    private bool bossIs = false; // �{�X���o�ꂵ�����ǂ����̃t���O
    private float startTime; // �X�|�[���J�n���̎�����ێ�

    void Start()
    {
        // enemyPool�����ݒ�̏ꍇ�A�V�[�������玩���擾
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();
        }

        InitializeBoss(); // �{�X�̏�����
        StartSpawner(); // �G�X�|�[���̊J�n
    }

    // �I�u�W�F�N�g���A�N�e�B�u�����ꂽ�Ƃ��̏���
    void OnEnable()
    {
        if (bossIs)
        {
            StartSpawner();
        }
    }

    // �{�X�̏����ݒ�
    public void InitializeBoss()
    {
        // boss�����ݒ�̏ꍇ�A�V�[�������玩���擾
        if (boss == null)
        {
            boss = GameObject.Find("Boss");
        }

        boss.SetActive(false); // ������ԂŔ�A�N�e�B�u�ɐݒ�
        bossIs = false; // �{�X�t���O�����Z�b�g
    }

    // �X�|�[���̃��Z�b�g���\�b�h
    public void ResetSpawner()
    {
        InitializeBoss(); // �{�X�̏�����
        spawnInterval = 2.5f; // �X�|�[���Ԋu�������l�Ƀ��Z�b�g
        StartSpawner(); // �X�|�[�����ċN��
    }

    // �X�|�[�����J�n���郁�\�b�h
    private void StartSpawner()
    {
        startTime = Time.time; // ���݂̎������L�^���A�o�ߎ��Ԃ����Z�b�g
        StartCoroutine(SpawnEnemies()); // �R���[�`���J�n
    }

    // �G�����I�ɃX�|�[������R���[�`��
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // �X�|�[���J�n����10�b�o�߂�����{�X��o�ꂳ����
            if (Time.time - startTime >= 3)
            {
                if (!bossIs) // �܂��{�X���o�ꂵ�Ă��Ȃ��ꍇ
                {
                    boss.transform.position = new Vector3(0, 40, 0); // �{�X�̏����ʒu��ݒ�
                    boss.SetActive(true); // �{�X���A�N�e�B�u��
                    bossIs = true; // �t���O���X�V
                    Debug.Log("Boss Active: " + boss.activeSelf); // �f�o�b�O���O�Ń{�X�̏�Ԃ��m�F
                    Debug.Log("Boss Position: " + boss.transform.position); // �{�X�̈ʒu���m�F
                }
            }

            // �v�[������G���擾
            var enemyObject = enemyPool.GetEnemy();
            if (enemyObject != null)
            {
                // �G�̃X�|�[���ʒu�������_���ɐݒ�
                enemyObject.transform.position = new Vector3(Random.Range(-50f, 50f), 55f, 0.0f);
                enemyObject.transform.rotation = Quaternion.identity;

                // �G�̓���ƃX�R�A�ݒ�
                var enemyControl = enemyObject.GetComponent<EnemyControl>();
                var enemyBH = enemyObject.GetComponent<BulletHit>();
                SpriteRenderer spriteRenderer = enemyObject.GetComponent<SpriteRenderer>();

                // �e�R���|�[�l���g�����݂��邩�m�F
                if (enemyControl != null && enemyBH != null)
                {
                    // �����_���ɓG�̈ړ��p�^�[����I��
                    int randomMovement = Random.Range(0, 3);
                    switch (randomMovement)
                    {
                        case 0:
                            enemyControl.SetMovement(new StraightMovement()); // ���i�ړ�
                            spriteRenderer.color = Color.yellow;
                            enemyBH.scorePoint = 500;
                            enemyBH.enemyHp = 5;
                            break;
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement()); // �W�O�U�O�ړ�
                            spriteRenderer.color = Color.red;
                            enemyBH.scorePoint = 350;
                            enemyBH.enemyHp = 4;
                            break;
                        case 2:
                            enemyControl.SetMovement(new CircularMovement()); // �~�^��
                            spriteRenderer.color = Color.magenta;
                            enemyBH.scorePoint = 200;
                            enemyBH.enemyHp = 4;
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"EnemyControl or BulletHit component not found on the enemy object: {enemyObject.name}");
                }
            }
            else
            {
                Debug.LogError("Failed to get enemy from pool.");
            }

            // �X�|�[���Ԋu�����������A�ŏ��l�ȉ��ɂȂ�Ȃ��悤�ɂ���
            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecreaseRate, minSpawnInterval);

            // ���̃X�|�[���܂őҋ@
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] GameObject boss;
    public float spawnInterval = 2.5f;
    public float minSpawnInterval = 0.3f;  // �X�|�[���Ԋu�̍ŏ��l
    public float spawnIntervalDecreaseRate = 0.02f;  // �X�|�[���Ԋu�������������

    private bool bossIs = false;

    void Start()
    {
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();
        }

        InitializeBoss();
        // �G�����I�ɃX�|�[������R���[�`�����J�n
        StartCoroutine(SpawnEnemies());
    }

    private void InitializeBoss()
    {
        if (boss == null)
        {
            boss = GameObject.Find("Boss");
        }

        boss.SetActive(false); // ������ԂŔ�A�N�e�B�u
        bossIs = false; // �{�X�t���O��������
    }

    // ���Z�b�g���\�b�h
    public void ResetSpawner()
    {
        InitializeBoss(); // �{�X�̏�����
        spawnInterval = 2.5f; // �X�|�[���Ԋu�����Z�b�g
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // �Q�[�����J�n����10�b�o�߂�����{�X��o�ꂳ����
            if (Time.time >= 10)
            {
                if (!bossIs)
                {
                    boss.transform.position = new Vector3(0, 40, 0);
                    boss.SetActive(true);
                    bossIs = true; // �t���O���X�V
                    Debug.Log("Boss Active: " + boss.activeSelf);
                    Debug.Log("Boss Position: " + boss.transform.position);
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

                if (enemyControl != null && enemyBH != null)
                {
                    // �����_���ɓG�̈ړ��p�^�[����I��
                    int randomMovement = Random.Range(0, 3);
                    switch (randomMovement)
                    {
                        case 0:
                            enemyControl.SetMovement(new StraightMovement());
                            spriteRenderer.color = Color.yellow;
                            enemyBH.scorePoint = 500;
                            enemyBH.enemyHp = 5;
                            break;
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement());
                            spriteRenderer.color = Color.red;
                            enemyBH.scorePoint = 350;
                            enemyBH.enemyHp = 4;
                            break;
                        case 2:
                            enemyControl.SetMovement(new CircularMovement());
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

            // �X�|�[���Ԋu������������i�ŏ��l�ȉ��ɂȂ�Ȃ��悤�ɂ���j
            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecreaseRate, minSpawnInterval);

            // ���̃X�|�[���܂őҋ@
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

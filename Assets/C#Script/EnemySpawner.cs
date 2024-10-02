using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool;
    public float spawnInterval = 2.5f;
    public float minSpawnInterval = 0.3f;  // �X�|�[���Ԋu�̍ŏ��l
    public float spawnIntervalDecreaseRate = 0.02f;  // �X�|�[���Ԋu�������������

    void Start()
    {
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool")?.GetComponent<EnemyPool>();
        }

        // �G�����I�ɃX�|�[������R���[�`�����J�n
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // �v�[������G���擾
            var enemyObject = enemyPool.GetEnemy();

            if (enemyObject != null)
            {
                // �G�̃X�|�[���ʒu�������_���ɐݒ�
                enemyObject.transform.position = new Vector3(Random.Range(-5.0f, 5.0f), 5.5f, 0.0f);
                enemyObject.transform.rotation = Quaternion.identity;

                // �G�̓���ƃX�R�A�ݒ�
                var enemyControl = enemyObject.GetComponent<EnemyControl>();
                var enemyBH = enemyObject.GetComponent<BulletHit>();

                if (enemyControl != null && enemyBH != null)
                {
                    // �����_���ɓG�̈ړ��p�^�[����I��
                    int randomMovement = Random.Range(0, 3);
                    switch (randomMovement)
                    {
                        case 0:
                            enemyControl.SetMovement(new StraightMovement());
                            enemyBH.scorePoint = 100;
                            break;
                        case 1:
                            enemyControl.SetMovement(new ZigzagMovement());
                            enemyBH.scorePoint = 300;
                            break;
                        case 2:
                            enemyControl.SetMovement(new CircularMovement());
                            enemyBH.scorePoint = 450;
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

using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private ObjectPool<GameObject> enemyPool;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned.");
            return;
        }

        enemyPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(enemyPrefab),
            actionOnGet: enemy => enemy.SetActive(true),
            actionOnRelease: enemy => enemy.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );


        if (enemyPool == null)
        {
            Debug.LogError("Failed to initialize enemy pool.");
        }
        else
        {
            Debug.Log("Enemy pool initialized successfully.");
        }
    }


    public GameObject GetEnemy()
    {
        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool is not initialized.");
            return null;
        }

        var enemy = enemyPool.Get();
        if (enemy == null)
        {
            Debug.LogError("Failed to get enemy from pool.");
        }
        return enemy;
    }


    public void ReleaseEnemy(GameObject enemy)
    {
        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool is not initialized.");
            return;
        }

        enemyPool.Release(enemy);
    }
}

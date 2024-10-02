using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private ObjectPool<GameObject> bulletPool;

    void Start()
    {
        bulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: bullet =>
            {
                if (bullet != null)
                {
                    bullet.SetActive(true);
                }
            },
            actionOnRelease: bullet =>
            {
                if (bullet != null)
                {
                    bullet.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("破棄済みの弾をプールに戻そうとしました。");
                }
            },
            actionOnDestroy: bullet =>
            {
                if (bullet != null)
                {
                    Destroy(bullet);
                }
                else
                {
                    Debug.LogWarning("破棄済みの弾を破壊しようとしました。");
                }
            },
            collectionCheck: false,
            defaultCapacity: 25,
            maxSize: 35
        );
    }


    //プールから弾を取り出す
    public GameObject GetBullet()
    {
        return bulletPool.Get();
    }


    //プールに弾を戻す
    public void ReleaseBullet(GameObject bullet)
    {
        bulletPool.Release(bullet);
    }
}

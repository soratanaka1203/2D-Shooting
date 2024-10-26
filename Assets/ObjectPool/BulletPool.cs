using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System;


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
            defaultCapacity: 30,
            maxSize: 50
        );
    }


    //プールから弾を取り出す
    public GameObject GetBullet()
    {
        return bulletPool.Get();
    }


    // プールに弾を戻す
    public async UniTaskVoid ReleaseBullet(GameObject bullet, float delay = 0)
    {
        if (bullet == null)//nullチェック
        {
            Debug.LogWarning("破壊済みの弾を戻そうとした");
            return;
        }
        // もしディレイを適用したい場合は await で待機します
        if (delay > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }

        // プールに弾を戻す
        bulletPool.Release(bullet);
    }

}

using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // 弾のプレハブ
    private ObjectPool<GameObject> bulletPool; // 弾のプール

    void Start()
    {
        // 弾のオブジェクトプールを初期化
        bulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab), // 新しい弾を生成する関数
            actionOnGet: bullet =>
            {
                if (bullet != null)
                {
                    bullet.SetActive(true); // 弾をアクティブにする
                }
            },
            actionOnRelease: bullet =>
            {
                if (bullet != null)
                {
                    bullet.SetActive(false); // 弾を非アクティブにする
                    Debug.Log("弾を非アクティブにして戻す");
                }
            },
            actionOnDestroy: bullet =>
            {
                if (bullet != null)
                {
                    Destroy(bullet); // 弾を破壊
                }
            },
            collectionCheck: false, // コレクションチェックをオフ
            defaultCapacity: 50, // 初期容量
            maxSize: 250 // 最大サイズ
        );
    }

    // プールから弾を取り出す
    public GameObject GetBullet()
    {
        var bullet = bulletPool.Get();
        if (bullet == null)
        {
            Debug.LogError("弾を取得できませんでした。プールの状態を確認してください。");
            return null;
        }
        return bullet;
    }



    //プールに返却する
    public async UniTask ReleaseBullet(GameObject bullet, float delay = 0)
    {
        if (bullet == null)
        {
            Debug.LogWarning("破壊済みの弾を戻そうとした");
            return;
        }

        if (delay > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay)); // 弾が待機時間後にプールに戻る
        }

        bullet.SetActive(false);
        bulletPool.Release(bullet); // プールに戻す
    }


}

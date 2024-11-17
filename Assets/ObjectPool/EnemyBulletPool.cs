using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // 弾のプレハブ
    private ObjectPool<GameObject> enemyBulletPool; // 弾のプール

    void Start()
    {
        // 弾のオブジェクトプールを初期化
        enemyBulletPool = new ObjectPool<GameObject>(
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
                }
                else
                {
                    Debug.LogWarning("破棄済みの弾をプールに戻そうとしました。"); // 警告メッセージ
                }
            },
            actionOnDestroy: bullet =>
            {
                if (bullet != null)
                {
                    Destroy(bullet); // 弾を破壊
                }
                else
                {
                    Debug.LogWarning("破棄済みの弾を破壊しようとしました。"); // 警告メッセージ
                }
            },
            collectionCheck: false, // コレクションチェックをオフ
            defaultCapacity: 50, // 初期容量
            maxSize: 100 // 最大サイズ
        );
    }

    // プールから弾を取り出す
    public GameObject GetEnemyBullet()
    {
        var bullet = enemyBulletPool.Get();
        if (bullet == null)
        {
            Debug.LogError("弾を取得できませんでした。プールの状態を確認してください。");
        }
        return bullet;
    }




    // プールに弾を戻す
    public async UniTask ReleaseEnemyBullet(GameObject bullet, float delay = 0)
    {
        if (bullet == null) // nullチェック
        {
            Debug.LogWarning("破壊済みの弾を戻そうとした");
            return; // 処理を終了
        }

        // ディレイが指定されている場合、待機
        if (delay > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay)); // 指定された秒数待機
        }

        // プールに弾を戻す
        enemyBulletPool.Release(bullet); 
    }
}

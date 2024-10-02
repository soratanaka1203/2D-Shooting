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
                    Debug.LogWarning("�j���ς݂̒e���v�[���ɖ߂����Ƃ��܂����B");
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
                    Debug.LogWarning("�j���ς݂̒e��j�󂵂悤�Ƃ��܂����B");
                }
            },
            collectionCheck: false,
            defaultCapacity: 25,
            maxSize: 35
        );
    }


    //�v�[������e�����o��
    public GameObject GetBullet()
    {
        return bulletPool.Get();
    }


    //�v�[���ɒe��߂�
    public void ReleaseBullet(GameObject bullet)
    {
        bulletPool.Release(bullet);
    }
}

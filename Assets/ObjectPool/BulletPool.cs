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
            defaultCapacity: 30,
            maxSize: 50
        );
    }


    //�v�[������e�����o��
    public GameObject GetBullet()
    {
        return bulletPool.Get();
    }


    // �v�[���ɒe��߂�
    public async UniTaskVoid ReleaseBullet(GameObject bullet, float delay = 0)
    {
        if (bullet == null)//null�`�F�b�N
        {
            Debug.LogWarning("�j��ς݂̒e��߂����Ƃ���");
            return;
        }
        // �����f�B���C��K�p�������ꍇ�� await �őҋ@���܂�
        if (delay > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }

        // �v�[���ɒe��߂�
        bulletPool.Release(bullet);
    }

}

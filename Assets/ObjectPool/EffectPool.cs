using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    private ObjectPool<GameObject> effectPool;

    void Start()
    {
        effectPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(effectPrefab),
            actionOnGet: effect => effect.SetActive(true),
            actionOnRelease: effect => effect.SetActive(false),
            actionOnDestroy: effect => Destroy(effect),
            collectionCheck: false,
            defaultCapacity: 15,
            maxSize: 30
        );
    }

    public GameObject GetEffect()
    {
        return effectPool.Get();
    }

    public void ReleaseEffect(GameObject effect)
    {
        effectPool.Release(effect);
    }
}

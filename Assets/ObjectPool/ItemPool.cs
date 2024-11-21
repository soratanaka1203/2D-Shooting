using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms.Impl;

public class ItemPool : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // アイテムのプレハブ
    //アイテムのスプライト
    [SerializeField] private Sprite shiledSprite;
    [SerializeField] private Sprite scoreSprite;
    [SerializeField] private Sprite plusBulletSprite;
    private ObjectPool<GameObject> itemPool; // アイテムのプール

    // Start is called before the first frame update
    void Start()
    {
        // アイテムのオブジェクトプールを初期化
        itemPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(itemPrefab), // 新しいアイテムを生成する関数
            actionOnGet: item =>
            {
                if (item != null)
                {
                    item.SetActive(true); // アイテムをアクティブにする
                }
            },
            actionOnRelease: item =>
            {
                if (item != null)
                {
                    item.SetActive(false); // アイテムを非アクティブにする
                }
            },
            actionOnDestroy: item =>
            {
                if (item != null)
                {
                    Destroy(item); // アイテムを破壊
                }
            },
            collectionCheck: false, // コレクションチェックをオフ
            defaultCapacity: 10, // 初期容量
            maxSize: 20 // 最大サイズ
        );
    }

    public GameObject GetItem(string itemName)
    {
        var item = itemPool.Get();
        if (item == null)
        {
            Debug.LogError("アイテムを取得できませんでした。プールの状態を確認してください。");
            return null;
        }
        switch (itemName)
        {
            case "Score":
                item.GetComponent<SpriteRenderer>().sprite = scoreSprite;
                item.GetComponent<Item>().itemType = Item.ItemType.Score;
                return item;
            case "PlusBullet":
                item.GetComponent<SpriteRenderer>().sprite = plusBulletSprite;
                item.GetComponent<Item>().itemType = Item.ItemType.PlusBullet;
                return item;
            case "Shield":
                item.GetComponent<SpriteRenderer>().sprite = shiledSprite;
                item.GetComponent<Item>().itemType = Item.ItemType.Shield;
                return item;
        }
        return item;
    }
    
    public void ReleaseItem(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("アイテムを取得できませんでした。");
        }
        else
        {
            itemPool.Release(item);
        }
    }
}

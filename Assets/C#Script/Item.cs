using DG.Tweening.Core.Easing;
using UnityEngine;
using MyNameSpace;
using TMPro;

public class Item : MonoBehaviour
{
    private Camera mainCamera;
    public enum ItemType { Score, PlusBullet, Shield }
    public ItemType itemType;

    public float speed = 2f;//落下スピード

    public ItemPool pool;

    [SerializeField] TextMeshProUGUI scoreText;

    private void Start()
    {
        mainCamera = Camera.main;
        if (pool == null)
        {
            pool = GameObject.Find("ItemPool").GetComponent<ItemPool>();
        }
    }

    private void Update()
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            // 弾をプールに戻す
            pool.ReleaseItem(gameObject);
        }

        transform.position += new Vector3(0, -0.4f * speed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ApplyEffect(collision.gameObject);
            pool.ReleaseItem(gameObject); //オブジェクトプールに戻す
            Debug.Log("アイテムを獲得");
        }
    }

    //アイテムの処理
    private void ApplyEffect(GameObject player)
    {
        switch (itemType)
        {
            case ItemType.Score:
                ScoreManager.Instance.AddScore(500);
                ScoreManager.Instance.SetDisplayScore(scoreText);
                break;
            case ItemType.PlusBullet:
                int bulletCount = player.GetComponent<PlayerControl>().bulletCount++;
                //発射できる玉の数を制限
                if (bulletCount > 10)
                {
                    player.GetComponent<PlayerControl>().bulletCount--;
                }
                break;
            case ItemType.Shield:
                //すでにシールドを持っていたらスコアを加算
                if (player.GetComponent<PlayerControl>().isShield)
                {
                    ScoreManager.Instance.AddScore(300);
                    ScoreManager.Instance.SetDisplayScore(scoreText);
                    break;
                }
                player.GetComponent<PlayerControl>().SetShiled(true);
                break;
        }
    }
}

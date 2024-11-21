using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyNameSpace;
using TMPro;

public class BulletHit : MonoBehaviour
{
    [SerializeField] private AudioPlayer audioPlayer;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] public float volume;
    public BulletPool bulletPool;
    public EffectPool effectPool;
    public EnemyPool enemyPool;
    public ItemPool itemPool; // アイテムプールの参照を追加
    public int enemyHp = 5;

    public int scorePoint = 100; // 敵を倒したときに得られるスコアのデフォルト値
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float dropChance = 0.4f; // アイテムが出現する確率 (0.3 = 30%)

    private void Start()
    {
        // 各プールやUIコンポーネントへの参照を取得
        if (bulletPool == null)
        {
            bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();
        }
        if (effectPool == null)
        {
            effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();
        }
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();
        }
        if (itemPool == null)
        {
            itemPool = GameObject.Find("ItemPool").GetComponent<ItemPool>();
        }
        if (scoreText == null)
        {
            scoreText = GameObject.Find("scoreText").GetComponent<TextMeshProUGUI>();
        }
    }

    private async void OnCollisionEnter2D(Collision2D collision)
    {
        // 当たったオブジェクトのタグがPlayerBulletだったら
        if (collision.gameObject.tag == "PlayerBullet" && bulletPool != null && effectPool != null && enemyPool != null)
        {
            // ヒットエフェクトを表示
            GameObject effect = effectPool.GetEffect();
            effect.transform.position = collision.gameObject.transform.position;

            // 一定時間後にエフェクトをプールに戻す
            ReturnEffectToPool(effect, 0.1f);

            // オーディオ再生
            audioPlayer.PlayAudio(audioClip, volume);

            enemyHp--; // 体力を減らす

            if (enemyHp <= 0)
            {
                // アイテムをドロップするか判定
                if (itemPool != null && UnityEngine.Random.value < dropChance)
                {
                    string randomItemType = GetRandomItemType();
                    GameObject item = itemPool.GetItem(randomItemType);
                    if (item != null)
                    {
                        item.transform.position = transform.position; // 敵の位置にアイテムを配置
                    }
                }

                // 敵をプールに戻す
                enemyPool.ReleaseEnemy(gameObject);
                enemyHp = 0; // 再度同じ敵が処理されないように
            }

            // スコアの追加と表示更新
            ScoreManager.Instance.AddScore(scorePoint);
            ScoreManager.Instance.SetDisplayScore(scoreText);
        }
    }

    private async UniTaskVoid ReturnEffectToPool(GameObject effect, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        effectPool.ReleaseEffect(effect);
    }

    private string GetRandomItemType()
    {
        // ランダムにアイテムタイプを選択
        string[] itemTypes = { "Score", "PlusBullet", "Shield" };
        int randomIndex = UnityEngine.Random.Range(0, itemTypes.Length);
        return itemTypes[randomIndex];
    }
}

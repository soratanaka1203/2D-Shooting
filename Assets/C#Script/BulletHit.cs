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
    public int enemyHp = 5;

    public int scorePoint = 100; //敵を倒した時に得られるスコアのデフォルト値

    [SerializeField] TextMeshProUGUI scoreText;

    private void Start()
    {
        if (bulletPool == null)
        {
            bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();// BulletPoolへの参照
            Debug.Log("バレット");
        }
        if (effectPool == null)
        {
            effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();// EffectPoolへの参照
            Debug.Log("エフェクト");
        }
        if (enemyPool == null)
        {
            enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();//EnemyPoolへの参照
            Debug.Log("エネミープール");
        }
        if (scoreText == null)
        {
            scoreText = GameObject.Find("scoreText").GetComponent<TextMeshProUGUI>();
            Debug.Log("スコアテキスト");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
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

            // 弾オブジェクトをプールに戻す
            bulletPool.ReleaseBullet(collision.gameObject);

            enemyHp--; // 体力を減らす

            if (enemyHp <= 0)
            {
                // 敵をプールに戻す
                enemyPool.ReleaseEnemy(gameObject);
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

}



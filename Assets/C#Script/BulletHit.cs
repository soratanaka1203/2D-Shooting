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
    private BulletPool bulletPool;
    private EffectPool effectPool;
    private EnemyPool enemyPool;
    int enemyHp = 6;

    public int scorePoint = 100; //敵を倒した時に得られるスコアのデフォルト値

    [SerializeField] TextMeshProUGUI scoreText;

    private void Start()
    {
        bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();// BulletPoolへの参照
        effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();// EffectPoolへの参照
        enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();//EnemyPoolへの参照
        scoreText = GameObject.Find("scoreText").GetComponent<TextMeshProUGUI>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 当たったオブジェクトのタグがPlayerBulletだったら
        if (collision.gameObject.tag =="PlayerBullet")
        {
            // ヒットエフェクトを表示
            GameObject effect = effectPool.GetEffect();
            effect.transform.position = collision.gameObject.transform.position;

            // 一定時間後にエフェクトをプールに戻す
            ReturnEffectToPool(effect, 0.1f);

            // オーディオ再生
            audioPlayer.PlayAudio(audioClip, volume);

            // 弾オブジェクトをプールに戻すか、適宜処理
            bulletPool.ReleaseBullet(collision.gameObject);

            enemyHp--;//体力を減らす

            if (enemyHp <= 0)
            {
                // 敵自身を破壊するか、プールに戻す
                enemyPool.ReleaseEnemy(gameObject);
                enemyHp = 6;
            }

            //スコアの追加
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



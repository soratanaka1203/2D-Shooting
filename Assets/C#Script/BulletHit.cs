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

    public int scorePoint = 100; //�G��|�������ɓ�����X�R�A�̃f�t�H���g�l

    [SerializeField] TextMeshProUGUI scoreText;

    private void Start()
    {
        bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();// BulletPool�ւ̎Q��
        effectPool = GameObject.Find("EffectPool").GetComponent<EffectPool>();// EffectPool�ւ̎Q��
        enemyPool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();//EnemyPool�ւ̎Q��
        scoreText = GameObject.Find("scoreText").GetComponent<TextMeshProUGUI>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������I�u�W�F�N�g�̃^�O��PlayerBullet��������
        if (collision.gameObject.tag =="PlayerBullet")
        {
            // �q�b�g�G�t�F�N�g��\��
            GameObject effect = effectPool.GetEffect();
            effect.transform.position = collision.gameObject.transform.position;

            // ��莞�Ԍ�ɃG�t�F�N�g���v�[���ɖ߂�
            ReturnEffectToPool(effect, 0.1f);

            // �I�[�f�B�I�Đ�
            audioPlayer.PlayAudio(audioClip, volume);

            // �e�I�u�W�F�N�g���v�[���ɖ߂����A�K�X����
            bulletPool.ReleaseBullet(collision.gameObject);

            enemyHp--;//�̗͂����炷

            if (enemyHp <= 0)
            {
                // �G���g��j�󂷂邩�A�v�[���ɖ߂�
                enemyPool.ReleaseEnemy(gameObject);
                enemyHp = 6;
            }

            //�X�R�A�̒ǉ�
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



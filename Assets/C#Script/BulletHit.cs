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
    public ItemPool itemPool; // �A�C�e���v�[���̎Q�Ƃ�ǉ�
    public int enemyHp = 5;

    public int scorePoint = 100; // �G��|�����Ƃ��ɓ�����X�R�A�̃f�t�H���g�l
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float dropChance = 0.4f; // �A�C�e�����o������m�� (0.3 = 30%)

    private void Start()
    {
        // �e�v�[����UI�R���|�[�l���g�ւ̎Q�Ƃ��擾
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
        // ���������I�u�W�F�N�g�̃^�O��PlayerBullet��������
        if (collision.gameObject.tag == "PlayerBullet" && bulletPool != null && effectPool != null && enemyPool != null)
        {
            // �q�b�g�G�t�F�N�g��\��
            GameObject effect = effectPool.GetEffect();
            effect.transform.position = collision.gameObject.transform.position;

            // ��莞�Ԍ�ɃG�t�F�N�g���v�[���ɖ߂�
            ReturnEffectToPool(effect, 0.1f);

            // �I�[�f�B�I�Đ�
            audioPlayer.PlayAudio(audioClip, volume);

            enemyHp--; // �̗͂����炷

            if (enemyHp <= 0)
            {
                // �A�C�e�����h���b�v���邩����
                if (itemPool != null && UnityEngine.Random.value < dropChance)
                {
                    string randomItemType = GetRandomItemType();
                    GameObject item = itemPool.GetItem(randomItemType);
                    if (item != null)
                    {
                        item.transform.position = transform.position; // �G�̈ʒu�ɃA�C�e����z�u
                    }
                }

                // �G���v�[���ɖ߂�
                enemyPool.ReleaseEnemy(gameObject);
                enemyHp = 0; // �ēx�����G����������Ȃ��悤��
            }

            // �X�R�A�̒ǉ��ƕ\���X�V
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
        // �����_���ɃA�C�e���^�C�v��I��
        string[] itemTypes = { "Score", "PlusBullet", "Shield" };
        int randomIndex = UnityEngine.Random.Range(0, itemTypes.Length);
        return itemTypes[randomIndex];
    }
}

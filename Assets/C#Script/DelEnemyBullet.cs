using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelEnemyBullet : MonoBehaviour
{
    private Camera mainCamera;
    public EnemyBulletPool enemyBulletPool;

    private void Start()
    {
        mainCamera = Camera.main;
        if (enemyBulletPool == null)
        {
            enemyBulletPool = GameObject.Find("EnemyBulletPool").GetComponent<EnemyBulletPool>();
        }
    }

    private void Update()
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            // 弾をプールに戻す
            enemyBulletPool.ReleaseEnemyBullet(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enemyBulletPool.ReleaseEnemyBullet(gameObject);//プールに戻す
        }
    }
}

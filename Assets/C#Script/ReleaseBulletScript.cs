using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseBulletScript : MonoBehaviour
{
    [SerializeField] BulletPool bulletPool;

    private void Start()
    {
        if (bulletPool == null)
        {
            bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();// BulletPoolÇ÷ÇÃéQè∆
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet" || collision.gameObject.tag == "EnemyBullet")
        {
            bulletPool.ReleaseBullet(collision.gameObject);
        }
    }
}

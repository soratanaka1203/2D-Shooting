using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelBullet : MonoBehaviour
{
    private Camera mainCamera;
    public BulletPool bulletPool;

    private void Start()
    {
        mainCamera = Camera.main;
        if (bulletPool == null)
        {
            bulletPool = GameObject.Find("PlayerBulletPool").GetComponent<BulletPool>();
        }
    }

    private void Update()
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            // 弾をプールに戻す
            bulletPool.ReleaseBullet(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            bulletPool.ReleaseBullet(gameObject);//プールに戻す
        }
    }
}


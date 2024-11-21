using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyControl : MonoBehaviour
{
    public Transform shotPoint; // 弾の発射位置
    public IMovement movement; // 移動パターンを管理
    public EnemyBulletPool enemyBulletPool; // 弾のプール（別のスクリプトで管理）
    public bool isShot=false;//弾を打つかどうか

    public float fireRate = 3.5f; // 弾の発射間隔
    private float fireCooldown = 0f; // 発射のクールダウンタイマー

    public Transform player; // プレイヤーの位置

    void Update()
    {
        movement.Move(transform); // 敵の移動

        fireCooldown -= Time.deltaTime; // クールダウンを減少

        if (isShot)//trueだったら弾を打つ
        {
            // 発射間隔を過ぎたら弾を発射
            if (fireCooldown <= 0f)
            {
                FireBullet(); // 弾を発射
                fireCooldown = fireRate; // クールダウンをリセット
            }
        }
    }

    // 移動パターンの設定
    public void SetMovement(IMovement movement)
    {
        this.movement = movement;
    }

    // 弾を発射するメソッド
    private void FireBullet()
    {
        if (enemyBulletPool != null && shotPoint != null && player != null)
        {
            GameObject bullet = enemyBulletPool.GetEnemyBullet(); // 弾をプールから取得

            if (bullet != null)
            {
                bullet.transform.position = shotPoint.position; // 発射位置に設定
                bullet.SetActive(true); // 弾をアクティブにする

                // プレイヤーへの方向を計算
                Vector2 direction = (player.position - shotPoint.position).normalized;

                // 弾の速度を設定
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.velocity = direction * 30f; // プレイヤー方向に速度を設定
            }
        }
    }
}

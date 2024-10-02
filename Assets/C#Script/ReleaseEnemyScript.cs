using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseEnemyScript : MonoBehaviour
{
    [SerializeField] EnemyPool pool;

    private void Start()
    {
        pool = GameObject.Find("EnemyPool").GetComponent<EnemyPool>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            pool.ReleaseEnemy(collision.gameObject);
        }
    }
}

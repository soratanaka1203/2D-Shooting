using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //ÉvÉåÉCÉÑÅ[ÇÃWASDà⁄ìÆ
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x, y).normalized * speed * Time.deltaTime;
    }
}

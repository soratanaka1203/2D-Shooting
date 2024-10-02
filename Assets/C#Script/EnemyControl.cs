using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyControl : MonoBehaviour
{

    public IMovement movement;

    // Update is called once per frame
    void Update()
    {
        movement.Move(transform);
    }


    public void SetMovement(IMovement movement)
    {
        this.movement = movement;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class IEnemy : MonoBehaviour
{
    public interface IMovement
    {
        void Move(Transform transform);
    }
}

public class StraightMovement : IMovement//下に直線運動
{
    public float speed = 6.0f;// 移動速度

    public void Move(Transform transform)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);//下に直線移動
    }
}

public class ZigzagMovement : IMovement//ジグザグ運動
{
    public float speed = 0.8f;// 移動速度
    public float frequency = 8.0f; // ジグザグの周波数
    public float magnitude = 0.07f;  // ジグザグの幅

    public void Move(Transform transform)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);//下に直線移動
        transform.Translate(Vector3.right * Mathf.Sin(Time.time * frequency) * magnitude);//左右に移動
    }
}
public class CircularMovement : IMovement//円運動
{
    public float speed = 3f;//移動速度
    public float radius = 0.03f;//半径

    private float angle = 0.0f;

    public void Move(Transform transform)
    {       
        angle += speed * Time.deltaTime;// 時間経過に応じて角度を増加
        float x = Mathf.Cos(angle) * -radius;// X座標の計算
        float y = Mathf.Sin(angle) * -radius;// Y座標の計算
        transform.position += new Vector3(x, y, transform.position.z);// 新しい位置に移動
    }
}

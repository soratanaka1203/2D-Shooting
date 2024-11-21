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
    public float speed = 80f;// 移動速度

    public void Move(Transform transform)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);//下に直線移動
    }
}

public class ZigzagMovement : IMovement//ジグザグ運動
{
    public float speed = 40f;// 移動速度
    public float frequency = 5.0f; // ジグザグの周波数
    public float magnitude = 2.5f;  // ジグザグの幅

    public void Move(Transform transform)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);//下に直線移動
        transform.Translate(Vector3.right * Mathf.Sin(Time.time * frequency) * magnitude);//左右に移動
    }
}
public class CircularMovement : IMovement//円運動
{
    public float speed = 4f;//移動速度
    public float radius = 2f;//半径

    private float angle = 0.0f;

    public void Move(Transform transform)
    {       
        angle += speed * Time.deltaTime;// 時間経過に応じて角度を増加
        float x = Mathf.Cos(angle) * -radius;// X座標の計算
        float y = Mathf.Sin(angle) * -radius;// Y座標の計算
        transform.position += new Vector3(x, y, transform.position.z);// 新しい位置に移動
    }
}

public class WaveMovement : IMovement
{
    private float speed = 3f; // 基本の移動速度
    private float waveFrequency = 2f; // 波の周波数
    private float waveAmplitude = 2f; // 波の振幅
    private float elapsedTime = 0f; // 経過時間

    public void Move(Transform transform)
    {
        elapsedTime += Time.deltaTime;

        // 垂直方向に前進しつつ、波状に横方向へ移動
        float waveOffset = Mathf.Sin(elapsedTime * waveFrequency) * waveAmplitude;
        Vector3 movement = new Vector3(waveOffset, -speed * Time.deltaTime, 0);

        transform.Translate(movement, Space.World);
    }
}

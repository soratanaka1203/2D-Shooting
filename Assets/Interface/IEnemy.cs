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

public class StraightMovement : IMovement//���ɒ����^��
{
    public float speed = 80f;// �ړ����x

    public void Move(Transform transform)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);//���ɒ����ړ�
    }
}

public class ZigzagMovement : IMovement//�W�O�U�O�^��
{
    public float speed = 40f;// �ړ����x
    public float frequency = 5.0f; // �W�O�U�O�̎��g��
    public float magnitude = 2.5f;  // �W�O�U�O�̕�

    public void Move(Transform transform)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);//���ɒ����ړ�
        transform.Translate(Vector3.right * Mathf.Sin(Time.time * frequency) * magnitude);//���E�Ɉړ�
    }
}
public class CircularMovement : IMovement//�~�^��
{
    public float speed = 4f;//�ړ����x
    public float radius = 2f;//���a

    private float angle = 0.0f;

    public void Move(Transform transform)
    {       
        angle += speed * Time.deltaTime;// ���Ԍo�߂ɉ����Ċp�x�𑝉�
        float x = Mathf.Cos(angle) * -radius;// X���W�̌v�Z
        float y = Mathf.Sin(angle) * -radius;// Y���W�̌v�Z
        transform.position += new Vector3(x, y, transform.position.z);// �V�����ʒu�Ɉړ�
    }
}

public class WaveMovement : IMovement
{
    private float speed = 3f; // ��{�̈ړ����x
    private float waveFrequency = 2f; // �g�̎��g��
    private float waveAmplitude = 2f; // �g�̐U��
    private float elapsedTime = 0f; // �o�ߎ���

    // �����ʒu�iX���j��ݒ�
    private float centerX = 0f;

    public void Move(Transform transform)
    {
        elapsedTime += Time.deltaTime;

        // �������ɔg��̈ړ����s���AX���͒����Ɍ������Đi��
        float waveOffset = Mathf.Sin(elapsedTime * waveFrequency) * waveAmplitude;
        Vector3 movement = new Vector3(waveOffset, -speed * Time.deltaTime, 0);

        // Y���͈��̈ړ��AX���͒����Ɍ������Ĉړ�
        float targetX = centerX;  // ������X���W
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), speed * Time.deltaTime);

        // �ŏI�I�Ȕg�̓���
        transform.Translate(movement, Space.World);
    }
}


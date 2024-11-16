using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar; // 体力バーのRectTransform
    [SerializeField] private Transform boss;          // ボスのTransform
    [SerializeField] private Camera mainCamera;       // メインカメラ
    [SerializeField] private Vector3 offset;          // ワールド空間でのオフセット

    private void Update()
    {
        // ボスのワールド座標をスクリーン座標に変換
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(boss.position + (offset + new Vector3(0,5,0)));

        // ボスが画面内にいる場合のみ体力バーを表示
        if (screenPosition.z > 0)
        {
            healthBar.position = screenPosition;
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}

